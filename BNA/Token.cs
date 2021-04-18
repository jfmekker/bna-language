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
		OPEN,
		CLOSE,
		READ,   // TODO change keywords for IO operations
		WRITE,  //
		INPUT,  //
		PRINT,  //

		// Operation mid keywords
		TO,
		BY,
		FROM,
		MAX,
		IF,
		WITH,
		OF,
		SIZE,
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
		LABEL_START = '^',
		LABEL_END = ':',
		LINE_END = '\n',
		ACCESSOR = '@',
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
			if ( this.Type == TokenType.UNKNOWN ) {
				// Null
				if ( this.Value.Length <= 0 ) {
					return TokenType.NULL;
				}

				// Literal
				if ( char.IsDigit( this.Value[0] ) || this.Value[0] == '-' || this.Value[0] == '.' ) {
					if ( int.TryParse( this.Value , out int i_val ) ) {
						return TokenType.LITERAL;
					}
					else if ( double.TryParse( this.Value , out double d_val ) ) {
						return TokenType.LITERAL;
					}
					else {
						return TokenType.INVALID;
					}
				}

				// String
				if ( this.Value[0] == '"' ) {
					if ( this.Value.Length >= 2 && this.Value[this.Value.Length - 1] == '"' ) {
						return TokenType.STRING;
					}
					else {
						return TokenType.INVALID;
					}
				}


				// Keyword
				if ( TryParseKeyword( this.Value.ToUpper( ) , out Keyword keyword ) ) {
					return TokenType.KEYWORD;
				}

				// Symbol
				if ( this.Value.Length == 1 && TryParseSymbol( this.Value[0] , out Symbol symbol ) ) {
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
			if ( line.Equals( "" ) ) {
				return tokens;
			}

			// Parse character by character
			string candidate = "";
			bool inString = false;
			for ( int i = 0 ; i < line.Length ; i += 1 ) {
				char c = line[i];
				// Letters, numbers, underscores, accessor, or anything in a string passes
				if ( char.IsLetterOrDigit( c )
					|| c == '_'
					|| c == (char)Symbol.ACCESSOR
					|| inString ) {

					candidate += c;

					if ( c == '"' ) {
						inString = false;
						tokens.Add( new Token( candidate , TokenType.STRING ) );
						candidate = "";
					}
				}
				// Comments end the line
				else if ( c == (char)Symbol.COMMENT ) {
					if ( candidate.Length > 0 ) {
						throw new CompiletimeException( "Comment must be separated by whitespace" );
					}

					while ( i < line.Length ) {
						candidate += line[i];
						i += 1;
					}

					tokens.Add( new Token( candidate , TokenType.COMMENT ) );
					candidate = "";
					break;
				}
				// Other special characters
				else {
					switch ( c ) {
						// LABEL_START
						case (char)Symbol.LABEL_START:
						// LABEL_END
						case (char)Symbol.LABEL_END:
						// GREATER_THAN
						case (char)Symbol.GREATER_THAN:
						// LESS_THAN
						case (char)Symbol.LESS_THAN:
						// EQUAL
						case (char)Symbol.EQUAL:
							if ( candidate.Length > 0 ) {
								var t = new Token( candidate );
								if ( t.Type == TokenType.INVALID ) {
									throw new CompiletimeException( "Invalid token: '" + candidate + "'" );
								}
								tokens.Add( t );
							}

							tokens.Add( new Token( c.ToString( ) , TokenType.SYMBOL ) );
							break;

						// Whitespace
						case ' ':
						case '\t':
							if ( candidate.Length > 0 ) {
								var t = new Token( candidate );
								if ( t.Type == TokenType.INVALID ) {
									throw new CompiletimeException( "Invalid token: '" + candidate + "'" );
								}
								tokens.Add( t );
							}

							candidate = "";
							break;

						// Negative sign
						case '-':
							if ( candidate.Length > 0 ) {
								throw new CompiletimeException( "Unexpected symbol in middle of token: '" + c + "' (" + ( (uint)c ).ToString( ) + ")." );
							}

							candidate += c;
							break;

						// Decimal point
						case '.':
							foreach ( char ch in candidate ) {
								if ( !char.IsDigit( ch ) || ch == '-' ) {
									throw new CompiletimeException( "Unexpected symbol in middle of token: '" + c + "' (" + ( (uint)c ).ToString( ) + ")." );
								}
							}

							candidate += c;
							break;

						// String start
						case '"':
							inString = true;
							candidate += c;
							break;

						default:
							throw new CompiletimeException( "Illegal symbol: '" + c + "' (" + ( (uint)c ).ToString( ) + ")." );
					}
				}
			}

			// Add last candidate
			if ( inString ) {
				throw new CompiletimeException( "Line ended before string: '" + candidate + "'." );
			}

			if ( candidate.Length > 0 ) {
				var t = new Token( candidate );
				if ( t.Type == TokenType.INVALID ) {
					throw new CompiletimeException( "Invalid token: '" + candidate + "'" );
				}
				tokens.Add( t );
			}

			return tokens;
		}

		/// <summary>
		/// Throw an exception if the given Token is not of the given TokenType.
		/// </summary>
		/// <param name="token">Token to check type of (can be null)</param>
		/// <param name="type">The TokenType to check for</param>
		public static void ThrowIfNotType( Token token , TokenType type )
		{
			if ( token.Type != type ) {
				throw new CompiletimeException( "Unexpected token: '" + token.ToString( ) + "', expected " + type.ToString( ) + "." );
			}
		}

		/// <summary>
		/// Throw an exception if the given Token is not one of the given TokenTypes.
		/// </summary>
		/// <param name="token">Token to check type of (can be null)</param>
		/// <param name="types">The TokenTypes to check for</param>
		public static void ThrowIfNotTypes( Token token , ICollection<TokenType> types )
		{
			if ( !types.Contains( token.Type ) ) {
				throw new CompiletimeException( "Unexpected token: '" + token.ToString( ) + "', expected " + types.ToString( ) + "." );
			}
		}

		/// <summary>
		/// Throw an exception if the given Token is not a given Keyword.
		/// </summary>
		/// <param name="keyword">The Keyword to check for</param>
		public void ThrowIfNotKeyword( Keyword keyword )
		{
			ThrowIfNotType( this , TokenType.KEYWORD );
			if ( Enum.TryParse( this.Value , out Keyword result ) && result != keyword ) {
				throw new CompiletimeException( "Token not expected keyword " + keyword.ToString( ) + ": " + this.ToString( ) );
			}
		}

		/// <summary>
		/// Throw an exception if the given Token is not one of the given Kymbols.
		/// </summary>
		/// <param name="symbols">List of valid keywords</param>
		public void ThrowIfNotKeywords( IEnumerable<Keyword> keywords )
		{
			bool success = false;
			foreach ( Keyword keyword in keywords ) {
				try {
					this.ThrowIfNotKeyword( keyword );
					success = true;
				}
				catch ( Exception ) {
					// Do nothing
				}
			}
			if ( !success ) {
				string message = "Token not of given keywords [ ";
				foreach ( Keyword s in keywords ) {
					message += s.ToString( ) + " ";
				}

				message += "]: " + this.ToString( );
				throw new CompiletimeException( message );
			}
		}

		/// <summary>
		/// Throw an exception if the given Token is not a given Symbol.
		/// </summary>
		/// <param name="symbol">The Symbol to check for</param>
		public void ThrowIfNotSymbol( Symbol symbol )
		{
			ThrowIfNotType( this , TokenType.SYMBOL );
			if ( Enum.TryParse( this.Value , out Symbol result ) && result != symbol ) {
				throw new CompiletimeException( "Token not expected keyword " + symbol.ToString( ) + ": " + this.ToString( ) );
			}
		}

		/// <summary>
		/// Throw an exception if the given Token is not one of the given Symbols.
		/// </summary>
		/// <param name="symbols">List of valid symbols</param>
		public void ThrowIfNotSymbols( IEnumerable<Symbol> symbols )
		{
			bool success = false;
			foreach ( Symbol symbol in symbols ) {
				try {
					this.ThrowIfNotSymbol( symbol );
					success = true;
				}
				catch ( Exception ) {
					// Do nothing
				}
			}
			if ( !success ) {
				string message = "Token not of given symbols [ ";
				foreach ( Symbol s in symbols ) {
					message += s.ToString( ) + " ";
				}

				message += ": " + this.ToString( );
				throw new CompiletimeException( message );
			}
		}

		/// <summary>
		/// Try to parse a Keyword from a string ignoring case
		/// </summary>
		/// <param name="word">The string to parse</param>
		/// <param name="value">Output value</param>
		/// <returns>True if the word was parsed as a Keyword</returns>
		public static bool TryParseKeyword( string word , out Keyword value )
		{
			try {
				value = (Keyword)Enum.Parse( typeof( Keyword ) , word , true );
			}
			catch {
				value = Keyword._;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Check if a character is a defined Symbol
		/// </summary>
		/// <param name="c">The character to check</param>
		/// <param name="value">Output value</param>
		/// <returns>True if the character was a defined Symbol</returns>
		public static bool TryParseSymbol( char c , out Symbol value )
		{
			if ( Enum.IsDefined( typeof( Symbol ) , (int)c ) ) {
				value = (Symbol)c;
				return true;
			}
			value = Symbol.NULL;
			return false;
		}

		/// <summary>
		/// Test equality to another Token.
		/// </summary>
		/// <param name="obj">Object to test equality with.</param>
		/// <returns>True if the object is a Token and shares TokenType and value with this.</returns>
		public override bool Equals( object obj )
		{
			if ( obj is Token ) {
				return ( this.Type == ( (Token)obj ).Type ) && ( this.Value == ( (Token)obj ).Value );
			}
			return false;
		}

		/// <summary>
		/// Get a hash code for the Token object
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode( )
		{
			int hashCode = 1265339359;
			hashCode = hashCode * -1521134295 + this.Type.GetHashCode( );
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode( this.Value );
			return hashCode;
		}

		/// <summary>
		/// Stringify the Token with type information.
		/// </summary>
		/// <returns>String description of the Token</returns>
		public override string ToString( )
		{
			string str = "<(" + this.Type;
			if ( this.Type == TokenType.KEYWORD ) {
				str += ":" + ( (Keyword)Enum.Parse( typeof( Keyword ) , this.Value , true ) ).ToString( );
			}
			else if ( this.Type == TokenType.SYMBOL && Enum.IsDefined( typeof( Symbol ) , (int)this.Value[0] ) ) {
				str += ":" + Enum.GetName( typeof( Symbol ) , (int)this.Value[0] );
			}
			str += ( this.Type == TokenType.NULL ) ? ")>" : ( ") " + this.Value + ">" );

			return str;
		}
	}
}
