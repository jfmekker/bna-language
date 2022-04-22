using System;
using System.Collections.Generic;
using System.Text;
using BNA.Common;
using BNA.Exceptions;

namespace BNA.Compile
{
	public class Lexer
	{
		public string Line { get; private init; }

		public int Index { get; private set; }

		public char? Current => this.Index < this.Line.Length ? this.Line[this.Index] : null;

		public char? Previous => this.Index + 1 < this.Line.Length ? this.Line[this.Index + 1] : null;

		public char? Next => this.Index - 1 > 0 ? this.Line[this.Index - 1] : null;

		private char ConsumeCurrent => this.Line[this.Index++];

		public static Token ReadSingleToken( string value )
		{
			Lexer lexer = new( value );
			List<Token> tokens = lexer.ReadTokens( );

			return tokens.Count == 1 ? tokens[0]
				: throw new IllegalTokenException( $"One token expected but {tokens.Count} were parsed." );
		}

		public Lexer( string line )
		{
			this.Line = line;
			this.Index = 0;
		}

		public List<Token> ReadTokens( )
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

		public Token? NextToken( )
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
						: this.Current.IsLetter( ) || this.Current is '_' ? this.NextVariableOrKeyword( )
						: this.Current is (char)Symbol.STRING_MARKER ? this.NextString( )
						: this.Current is (char)Symbol.LIST_START ? this.NextList( )
						: this.Current is (char)Symbol.COMMENT ? this.NextComment( )
						: this.NextSymbol( );
				}
			}

			return null;
		}

		private Token NextLiteral( )
		{
			StringBuilder builder = new( $"{this.ConsumeCurrent}" );

			while ( this.Current is not null )
			{
				if ( this.Current.IsLetterOrDigit( ) || this.Current is '.' or '+' or '-' )
				{
					_ = builder.Append( this.ConsumeCurrent );
				}
				else
				{
					break;
				}
			}

			string str = builder.ToString( );
			return long.TryParse( str , out long _ ) || double.TryParse( str , out double _ )
				? new Token( str , TokenType.NUMBER )
				: throw new InvalidTokenException( "Literal is not parsable as number." );
		}

		private Token NextVariableOrKeyword( )
		{
			StringBuilder builder = new( $"{this.ConsumeCurrent}" );

			while ( this.Current is not null )
			{
				if ( !this.Current.IsLetterOrDigit( ) && this.Current is not '_' and not (char)Symbol.ACCESSOR )
				{
					break;
				}

				_ = builder.Append( this.ConsumeCurrent );
			}

			string str = builder.ToString( );
			for ( int i = 0 ; i < str.Length ; i += 1 )
			{
				if ( str[i] == (char)Symbol.ACCESSOR &&
					( i + 1 >= str.Length || i - 1 < 0
					|| !( char.IsLetterOrDigit( str[i + 1] ) || str[i + 1] == '_' )
					|| !( char.IsLetterOrDigit( str[i - 1] ) || str[i - 1] == '_' ) ) )
				{
					throw new InvalidTokenException( $"Accessors must have a letter, digit, or underscore on either side: {str}" );
				}
			}

			return new Token( str , Enum.TryParse( str , out Keyword _ ) ? TokenType.KEYWORD : TokenType.VARIABLE );
		}

		private Token NextString( )
		{
			StringBuilder builder = new( $"{this.ConsumeCurrent}" );

			bool string_ended = false;
			while ( this.Current is not null )
			{
				_ = builder.Append( this.ConsumeCurrent );

				if ( builder.ToString( )[^1] == (char)Symbol.STRING_MARKER &&
					builder.ToString( )[^2] != (char)Symbol.ESCAPE )
				{
					string_ended = true;
					break;
				}
			}

			return string_ended ? new Token( builder.ToString( ) , TokenType.STRING )
				: throw new MissingTerminatorException( "String" , (char)Symbol.STRING_MARKER );
		}

		private Token NextList( )
		{
			int start_index = this.Index;
			List<Token> list = new( ) { new Token( $"{this.ConsumeCurrent}" , TokenType.SYMBOL ) };

			while ( this.Current is not null )
			{
				if ( this.NextToken( ) is Token token )
				{
					if ( token.AsSymbol( ) is Symbol.LIST_END )
					{
						list.Add( token );
						break;
					}
					else if ( token.AsSymbol( ) is Symbol.LIST_SEPARATOR )
					{
						list.Add( token );
					}
					else if ( token.Type is TokenType.NUMBER or TokenType.VARIABLE or TokenType.STRING or TokenType.LIST )
					{
						list.Add( token ); // TODO
						//if ( list[^1].AsSymbol( ) is Symbol.LIST_SEPARATOR or Symbol.LIST_START )
						//{
						//	list.Add( token );
						//}
						//else
						//{
						//	throw new IllegalTokenException( $"Tokens in lists must be separated by '{Symbol.LIST_SEPARATOR}'." );
						//}
					}
					else
					{
						throw new IllegalTokenException( $"Tokens of type '{token.Type}' not allowed in lists." );
					}
				}
			}

			return this.Line[this.Index - 1] == (char)Symbol.LIST_END
				? new Token( this.Line[start_index..this.Index] , TokenType.LIST )
				: throw new MissingTerminatorException( "List" , (char)Symbol.LIST_END );
		}

		private Token NextComment( )
		{
			StringBuilder builder = new( );

			while ( this.Current is not null )
			{
				_ = builder.Append( this.ConsumeCurrent );
			}

			return new Token( builder.ToString( ) , TokenType.COMMENT );
		}

		private Token NextSymbol( )
		{
			return this.Current is (char)Symbol.LESS_THAN
								or (char)Symbol.GREATER_THAN
								or (char)Symbol.EQUAL
								or (char)Symbol.NOT
								or (char)Symbol.LIST_SEPARATOR
								or (char)Symbol.LIST_END
								or (char)Symbol.LABEL_START
								or (char)Symbol.LABEL_END
				? new Token( $"{this.ConsumeCurrent}" , TokenType.SYMBOL )
				: throw new UnexpectedSymbolException( this.Current );
		}
	}
}
