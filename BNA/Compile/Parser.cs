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

		public char Current => this.Line[this.Index];

		public Parser( string line )
		{
			this.Line = line;
			this.Index = 0;
		}

		public List<Token> ParseTokens( )
		{
			List<Token> tokens = new( );
			while ( this.Index < this.Line.Length )
			{
				if ( this.Next( ) is Token token )
				{
					tokens.Add( token );
				}
			}
			return tokens;
		}

		private Token? Next( )
		{
			while ( this.Index < this.Line.Length )
			{
				if ( this.Current is ' ' or '\t' )
				{
					// Ignore whitespace
					this.Index += 1;
				}
				else
				{
					return char.IsDigit( this.Current ) || this.Current is '-' or '.' ? this.NextLiteral( )
						: char.IsLetter( this.Current ) ? this.NextVariableOrKeyword( )
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

		private Token? NextVariableOrKeyword( ) { throw new NotImplementedException( ); }

		private Token? NextSymbol( ) { throw new NotImplementedException( ); }

		private Token? NextLiteral( ) { throw new NotImplementedException( ); }

		private Token NextString( )
		{
			StringBuilder builder = new( $"{this.Current}" );
			while ( this.Index < this.Line.Length )
			{
				if ( this.Current is (char)Symbol.ESCAPE )
				{
					_ = builder.Append( this.Current );

					this.Index += 1;
					if ( this.Index >= this.Line.Length || this.Current is (char)Symbol.STRING_MARKER )
					{
						throw new Exception( ); // TODO
					}

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

			if ( builder.ToString( )[^1] is not (char)Symbol.STRING_MARKER )
			{
				throw new Exception( ); // TODO
			}

			return new Token( builder.ToString( ) , TokenType.STRING );
		}

		private Token? NextList( ) { throw new NotImplementedException( ); }

		private Token NextLabel( )
		{
			StringBuilder builder = new( $"{this.Current}" );
			while ( this.Index < this.Line.Length )
			{
				if ( char.IsLetter( this.Current ) )
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

			// TODO check it's not a keyword

			return new Token( builder.ToString( ) , TokenType.LABEL );
		}

		private Token NextComment( )
		{
			StringBuilder builder = new( );
			while ( this.Index < this.Line.Length )
			{
				_ = builder.Append( this.Current );
				this.Index += 1;
			}
			return new Token( builder.ToString( ) , TokenType.COMMENT );
		}
	}
}
