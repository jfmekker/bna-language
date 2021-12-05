using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BNA.Common;
using BNA.Exceptions;

namespace BNA.Compile
{
	public class Parser
	{
		public string Line { get; private init; }

		public int Index { get; private set; }

		public char? Current => this.Index < this.Line.Length ? this.Line[this.Index] : null;

		public char? Previous => this.Index + 1 < this.Line.Length ? this.Line[this.Index + 1] : null;

		public char? Next => this.Index - 1 > 0 ? this.Line[this.Index - 1] : null;

		public Parser( string line )
		{
			this.Line = line;
			this.Index = 0;
		}

		public List<Token> ParseTokens( )
		{
			List<Token> tokens = new( );
			while ( this.Current is not null )
			{
				if ( this.NextToken( ) is Token token )
				{
					tokens.Add( token );
				}
			}
			return tokens;
		}

		private Token? NextToken( )
		{
			while ( this.Current is not null )
			{
				if ( this.Current is ' ' or '\t' )
				{
					// Ignore whitespace
					this.Index += 1;
				}
				else
				{
					return this.Current.IsDigit( ) || this.Current is '-' or '.' ? this.NextLiteral( )
						: this.Current.IsLetter( ) ? this.NextVariableOrKeyword( )
						: this.Current is (char)Symbol.LABEL_START ? this.NextLabel( )
						: this.Current is (char)Symbol.STRING_MARKER ? this.NextString( )
						: this.Current is (char)Symbol.LIST_START ? this.NextList( )
						: this.Current is (char)Symbol.LABEL_END ? throw new UnmatchedTerminatorException( ) // TODO
						: this.Current is (char)Symbol.COMMENT ? this.NextComment( )
						: this.NextSymbol( );
				}
			}

			return null;
		}

		private Token NextVariableOrKeyword( )
		{
			StringBuilder builder = new( );

			while ( this.Current is not null )
			{
				if ( this.Current.IsLetter( ) || this.Current is '_' )
				{
					_ = builder.Append( this.Current );
				}
				else if ( this.Current is ' ' or '\t' or (char)Symbol.LIST_SEPERATOR or (char)Symbol.LIST_END )
				{
					break;
				}
				else
				{
					throw new Exception( ); // TODO
				}

				this.Index += 1;
			}

			string str = builder.ToString( );
			return new Token( str , Enum.TryParse( str , out Keyword _ ) ? TokenType.KEYWORD : TokenType.VARIABLE );
		}

		private Token NextSymbol( )
		{
			return this.Current is '<' or '>' or '=' or '!'
				? new Token( $"{this.Line[this.Index++]}" , TokenType.SYMBOL )
				: throw new Exception( ); // TODO
		}

		private Token NextLiteral( )
		{
			StringBuilder builder = new( $"{this.Current}" );
			this.Index += 1;

			while ( this.Current is not null )
			{
				if ( this.Current.IsDigit( ) )
				{
					_ = builder.Append( this.Current );
				}
				else if ( this.Current is '.' )
				{
					if ( builder.ToString( ).Contains( '.' ) )
					{
						throw new Exception( ); // TODO
					}

					_ = builder.Append( this.Current );
				}
				else if ( this.Current is ' ' or '\t' or (char)Symbol.LIST_SEPERATOR or (char)Symbol.LIST_END )
				{
					break;
				}
				else
				{
					throw new Exception( ); // TODO
				}

				this.Index += 1;
			}

			return new Token( builder.ToString( ) , TokenType.LITERAL );
		}

		private Token NextString( )
		{
			StringBuilder builder = new( $"{this.Current}" );
			this.Index += 1;

			while ( this.Current is not null )
			{
				if ( this.Current is (char)Symbol.ESCAPE )
				{
					_ = builder.Append( this.Current );

					if ( this.Next is null )
					{
						throw new Exception( ); // TODO
					}

					this.Index += 1;
					_ = builder.Append( this.Current );
				}
				else if ( this.Current is (char)Symbol.STRING_MARKER )
				{
					_ = builder.Append( this.Current );
					break;
				}

				_ = builder.Append( this.Current );
				this.Index += 1;
			}

			if ( builder.ToString( )[^1] is not (char)Symbol.STRING_MARKER
				|| builder.ToString( )[^2] is (char)Symbol.ESCAPE )
			{
				throw new Exception( ); // TODO
			}

			return new Token( builder.ToString( ) , TokenType.STRING );
		}

		private Token NextList( )
		{
			StringBuilder builder = new( $"{this.Current}" );
			this.Index += 1;

			while ( this.Current is not null and not (char)Symbol.LIST_END )
			{
				if ( this.Current is (char)Symbol.LIST_SEPERATOR )
				{
					_ = builder.Append( this.Current );
					this.Index += 1;
				}
				else
				{
					_ = builder.Append( this.NextToken( )?.Value );
				}
			}

			if ( this.Current is (char)Symbol.LIST_END )
			{
				_ = builder.Append( this.Current );
				this.Index += 1;
				return new Token( builder.ToString( ) , TokenType.LIST );
			}
			else
			{
				throw new Exception( ); // TODO
			}
		}

		private Token NextLabel( )
		{
			StringBuilder builder = new( $"{this.Current}" );
			this.Index += 1;

			while ( this.Current is not null )
			{
				if ( this.Current.IsLetter( ) )
				{
					_ = builder.Append( this.Current );
				}
				else if ( this.Current is (char)Symbol.LABEL_END )
				{
					if ( builder.Length > 1 )
					{
						_ = builder.Append( this.Current );
					}
					else
					{
						throw new Exception( ); // TODO
					}
					break;
				}
				else
				{
					throw new Exception( ); // TODO
				}

				this.Index += 1;
			}

			if ( Enum.TryParse( builder.ToString( ) , out Keyword _ ) )
			{
				throw new Exception( ); // TODO
			}

			return new Token( builder.ToString( ) , TokenType.LABEL );
		}

		private Token NextComment( )
		{
			StringBuilder builder = new( );

			while ( this.Current is not null )
			{
				_ = builder.Append( this.Current );
				this.Index += 1;
			}

			return new Token( builder.ToString( ) , TokenType.COMMENT );
		}
	}
}
