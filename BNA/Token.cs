using System;
using System.Collections.Generic;

namespace BNA
{
	/// <summary>
	/// The recognized and reserved keywords of BNA.
	/// </summary>
	public enum Keyword
	{
		// Empty keyword to represent unknown
		_ = 0,

		// Operation start keywords
		SET,
		ADD,
		SUBTRACT,
		MULTIPLY,
		DIVIDE,
		WAIT,
		RANDOM,
		TEST,
		GOTO,
		OR,
		AND,
		XOR,
		NEGATE,
		RAISE,
		MOD,
		LOG,
		ROUND,
		LIST,
		APPEND,
		SIZE,
		OPEN,
		CLOSE,
		READ,
		WRITE,
		INPUT,
		PRINT,
		TYPE,
		EXIT,
		ERROR,
		SCOPE,

		// Operation mid keywords
		TO,
		BY,
		FROM,
		MAX,
		IF,
		WITH,
		OF,
		AS,
	}

	/// <summary>
	/// Special symbols within the BNA language.
	/// </summary>
	public enum Symbol
	{
		// Default to 'null' value
		NULL = '\0',
		COMMENT = '#',
		GREATER_THAN = '>',
		LESS_THAN = '<',
		EQUAL = '=',
		NOT = '!',
		LABEL_START = '^',
		LABEL_END = ':',
		LINE_END = '\n',
		STRING_MARKER = '"',
		ACCESSOR = '@',
		LIST_START = '(',
		LIST_END = ')',
		LIST_SEPERATOR = ',',
	}

	/// <summary>
	/// What a given Token is identified as.
	/// </summary>
	public enum TokenType
	{
		INVALID = -2,
		UNKNOWN = -1,
		NULL = 0,
		LITERAL,
		VARIABLE,
		STRING,
		LIST,
		KEYWORD,
		SYMBOL,
		COMMENT
	}

	/// <summary>
	/// Group of related characters that form keywords/variables/etc.
	/// </summary>
	public struct Token
	{
		/// <summary>
		/// The TokenType of this Token.
		/// </summary>
		public TokenType Type
		{
			get; set;
		}

		/// <summary>
		/// The actual string input for this Token.
		/// </summary>
		public string Value
		{
			get; set;
		}

		/// <summary>
		/// Create a new Token and try to identify it.
		/// </summary>
		/// <param name="value">The string input of the Token</param>
		/// <param name="type">The Token's type; if unknown, will attempt identifying</param>
		public Token( string value , TokenType type = TokenType.UNKNOWN )
		{
			this.Value = value;
			this.Type = type;
			this.Type = this.IdentifyType( );
		}

		/// <summary>
		/// Try to identify this Token's type if unknown.
		/// </summary>
		/// <returns>The potentially identified type</returns>
		public TokenType IdentifyType( )
		{
			// Don't re-identify if we know it
			if ( this.Type == TokenType.UNKNOWN )
			{
				// Null
				if ( this.Value.Length <= 0 )
				{
					return TokenType.NULL;
				}

				// Literal
				if ( char.IsDigit( this.Value[0] ) || this.Value[0] == '-' || this.Value[0] == '.' )
				{
					if ( int.TryParse( this.Value , out _ ) )
					{
						return TokenType.LITERAL;
					}
					else if ( double.TryParse( this.Value , out _ ) )
					{
						return TokenType.LITERAL;
					}
					else
					{
						return TokenType.INVALID;
					}
				}

				// String
				if ( this.Value[0] == (char)Symbol.STRING_MARKER )
				{
					return this.Value.Length >= 2
						&& this.Value[0] == (char)Symbol.STRING_MARKER
						&& this.Value[^1] == (char)Symbol.STRING_MARKER
						? TokenType.STRING : TokenType.INVALID;
				}

				// List
				if ( this.Value[0] == (char)Symbol.LIST_START )
				{
					if ( this.Value.Length >= 2 && this.Value[^1] == (char)Symbol.LIST_END )
					{
						// Tokenize contents to catch other compile errors
						_ = TokenizeLine( this.Value[1..^1] );

						return TokenType.LIST;
					}
					else
					{
						return TokenType.INVALID;
					}
				}

				// Keyword
				if ( Enum.TryParse( this.Value.ToUpper( ) , out Keyword _ ) )
				{
					return TokenType.KEYWORD;
				}

				// Symbol
				if ( this.Value.Length == 1 && Enum.IsDefined( typeof( Symbol ) , (int)this.Value[0] ) )
				{
					return TokenType.SYMBOL;
				}

				// Variable
				return TokenType.VARIABLE;
			}

			return this.Type;
		}

		/// <summary>
		/// Break a line of characters into a list of Tokens.
		/// </summary>
		/// <param name="line">The string of the line to be tokenized</param>
		/// <returns>List of Tokens in order that they appear</returns>
		public static List<Token> TokenizeLine( string line )
		{
			var tokens = new List<Token>( );

			// Ignore if the line empty
			if ( line.Equals( "" ) )
			{
				return tokens;
			}

			// Parse character by character
			string candidate = "";
			int list_nest_level = 0;
			for ( int i = 0 ; i < line.Length ; i += 1 )
			{
				char c = line[i];
				// Letters, numbers, underscores, accessor, or we are in a list
				if ( char.IsLetterOrDigit( c )
					|| c == '_'
					|| c == (char)Symbol.ACCESSOR
					|| ( list_nest_level > 0
						&& c != (char)Symbol.LIST_END
						&& c != (char)Symbol.LIST_START )
					)
				{

					candidate += c;
				}
				// Other special characters
				else
				{
					switch ( c )
					{
						// Comment
						case (char)Symbol.COMMENT:
						{
							if ( candidate.Length > 0 )
							{
								throw new CompiletimeException( "Comment must be separated by whitespace" );
							}

							while ( i < line.Length )
							{
								candidate += line[i];
								i += 1;
							}

							tokens.Add( new Token( candidate , TokenType.COMMENT ) );
							candidate = "";
							break;
						}

						// Standalone symbol tokens
						case (char)Symbol.LABEL_START:
						case (char)Symbol.LABEL_END:
						case (char)Symbol.GREATER_THAN:
						case (char)Symbol.LESS_THAN:
						case (char)Symbol.EQUAL:
						case (char)Symbol.NOT:
						case (char)Symbol.LIST_SEPERATOR:
						{
							if ( candidate.Length > 0 )
							{
								var t = new Token( candidate );
								if ( t.Type == TokenType.INVALID )
								{
									throw new CompiletimeException( "Invalid token: '" + candidate + "'" );
								}
								tokens.Add( t );
								candidate = "";
							}

							tokens.Add( new Token( c.ToString( ) , TokenType.SYMBOL ) );
							break;
						}

						// Whitespace
						case ' ':
						case '\t':
						{
							if ( candidate.Length > 0 )
							{
								var t = new Token( candidate );
								if ( t.Type == TokenType.INVALID )
								{
									throw new CompiletimeException( "Invalid token: '" + candidate + "'" );
								}
								tokens.Add( t );
							}

							candidate = "";
							break;
						}

						// Negative sign
						case '-':
						{
							if ( candidate.Length > 0 )
							{
								throw new CompiletimeException( "Unexpected symbol in middle of token: '" + c + "' (" + ( (uint)c ).ToString( ) + ")." );
							}

							candidate += c;
							break;
						}

						// Decimal point
						case '.':
						{
							for ( int j = 0 ; j < candidate.Length ; j += 1 )
							{
								// Candidate must be all digits except for potential negative sign at the start
								if ( !char.IsDigit( candidate[j] ) && candidate[j] != '-' && j == 0 )
								{
									throw new CompiletimeException( "Unexpected symbol in middle of token: '" + c + "' (" + ( (uint)c ).ToString( ) + ")." );
								}
							}

							candidate += c;
							break;
						}

						// String
						case (char)Symbol.STRING_MARKER:
						{
							if ( candidate.Length > 0 && candidate[0] != (char)Symbol.STRING_MARKER )
							{
								throw new CompiletimeException( "String must be separated by whitespace" );
							}

							candidate += line[i];
							i += 1;

							while ( i < line.Length )
							{
								candidate += line[i];
								i += 1;

								if ( line[i - 1] == (char)Symbol.STRING_MARKER )
								{
									break;
								}
							}

							tokens.Add( new Token( candidate , TokenType.STRING ) );
							candidate = "";
							break;
						}

						// List
						case (char)Symbol.LIST_START:
						{
							candidate += c;

							if ( list_nest_level == 0 && candidate.Length > 1 )
							{
								throw new CompiletimeException( "List start must be start of new token: '" + candidate + "'" );
							}

							list_nest_level += 1;
							break;
						}
						case (char)Symbol.LIST_END:
						{
							candidate += c;

							if ( list_nest_level == 0 )
							{
								throw new CompiletimeException( "List ending with no matching start: '" + candidate + "'" );
							}

							list_nest_level -= 1;
							break;
						}

						// Unexpected
						default:
							throw new CompiletimeException( "Illegal symbol: '" + c + "' (" + ( (uint)c ).ToString( ) + ")." );
					}
				}
			}

			// Add last candidate
			if ( candidate.Length > 0 )
			{
				if ( list_nest_level > 0 )
				{
					throw new CompiletimeException( "Line ended before list: '" + candidate + "'" );
				}

				var t = new Token( candidate );
				if ( t.Type == TokenType.INVALID )
				{
					throw new CompiletimeException( "Invalid token: '" + candidate + "'" );
				}
				tokens.Add( t );
			}

			return tokens;
		}

		/// <summary>
		/// Throw an exception if the given Token is not one of the given TokenTypes.
		/// </summary>
		/// <param name="token">Token to check type of (can be null)</param>
		/// <param name="types">The TokenTypes to check for</param>
		public static void ThrowIfNotTypes( Token token , ICollection<TokenType> types )
		{
			if ( !types.Contains( token.Type ) )
			{
				throw new CompiletimeException( "Unexpected token: '" + token.ToString( ) + "', expected {" + string.Join( ", " , types ) + "}." );
			}
		}

		/// <summary>
		/// Throw an exception if the given Token is not one of the given Kymbols.
		/// </summary>
		/// <param name="symbols">List of valid keywords</param>
		public void ThrowIfNotKeywords( ICollection<Keyword> keywords )
		{
			ThrowIfNotTypes( this , new List<TokenType> { TokenType.KEYWORD } );
			if ( Enum.TryParse( this.Value , out Keyword result ) && !keywords.Contains( result ) )
			{
				throw new CompiletimeException( "Unexpected keyword '" + this.ToString( ) + "', expected {" + string.Join( ", " , keywords ) + "}." );
			}
		}

		/// <summary>
		/// Throw an exception if the given Token is not one of the given Symbols.
		/// </summary>
		/// <param name="symbols">List of valid symbols</param>
		public void ThrowIfNotSymbols( ICollection<Symbol> symbols )
		{
			ThrowIfNotTypes( this , new List<TokenType> { TokenType.SYMBOL } );
			if ( Enum.TryParse( this.Value , out Symbol result ) && !symbols.Contains( result ) )
			{
				throw new CompiletimeException( "Unexpected symbol '" + this.ToString( ) + "', expected {" + string.Join( ", " , symbols ) + "}." );
			}
		}

		/// <summary>
		/// Check if a character is a defined Symbol
		/// </summary>
		/// <param name="c">The character to check</param>
		/// <param name="value">Output value</param>
		/// <returns>True if the character was a defined Symbol</returns>
		public static bool TryParseSymbol( char c , out Symbol value )
		{
			if ( Enum.IsDefined( typeof( Symbol ) , (int)c ) )
			{
				value = (Symbol)c;
				return true;
			}
			value = Symbol.NULL;
			return false;
		}

		/// <summary>
		/// Get a hash code for the Token object.
		/// </summary>
		/// <returns>Hash code.</returns>
		public override int GetHashCode( )
		{
			return HashCode.Combine( this.Type , this.Value );
		}

		/// <summary>
		/// Stringify the Token with type information.
		/// </summary>
		/// <returns>String description of the Token</returns>
		public override string ToString( )
		{
			string str = "<(" + this.Type;
			if ( this.Type == TokenType.KEYWORD )
			{
				str += ":" + ( (Keyword)Enum.Parse( typeof( Keyword ) , this.Value , true ) ).ToString( );
			}
			else if ( this.Type == TokenType.SYMBOL && Enum.IsDefined( typeof( Symbol ) , (int)this.Value[0] ) )
			{
				str += ":" + Enum.GetName( typeof( Symbol ) , (int)this.Value[0] );
			}
			str += ( this.Type == TokenType.NULL ) ? ")>" : ( ") " + this.Value + ">" );

			return str;
		}

		/// <summary>
		/// Compare equality of two Token with the equality operator.
		/// </summary>
		/// <param name="left">Left hand operand.</param>
		/// <param name="right">Right hand operand.</param>
		/// <returns>True if the Tokens have the same type and value.</returns>
		public static bool operator ==( Token left , Token right ) => ( left.Type == right.Type ) && ( left.Value == right.Value );

		/// <summary>
		/// Compare non-equality of two Token with the equality operator.
		/// </summary>
		/// <param name="left">Left hand operand.</param>
		/// <param name="right">Right hand operand.</param>
		/// <returns>True if the Tokens have different types or values.</returns>
		public static bool operator !=( Token left , Token right ) => !( left == right );

		/// <summary>
		/// Compare a Token to an object.
		/// </summary>
		/// <param name="obj">Object to compare against.</param>
		/// <returns>True if the object is an equal Token.</returns>
		public override bool Equals( object? obj ) => obj is Token other && this == other;
	}
}
