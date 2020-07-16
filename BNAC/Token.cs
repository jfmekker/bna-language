using System;
using System.Collections.Generic;

namespace BNAC
{
	internal enum Keyword
	{
		// Empty keyword to represent unknown
		_,

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

	internal enum Symbol
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

	internal enum TokenType
	{
		UNKNOWN,
		LITERAL,
		VARIABLE,
		STRING,
		KEYWORD,
		SYMBOL,
	}

	/// <summary>
	/// Group of related characters that form keywords/variables/etc.
	/// </summary>
	internal class Token
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
		private Token( string value , TokenType type = TokenType.UNKNOWN )
		{
			this.Value = value;
			this.Type = type;
			this.Type = this.IdentifyType( );
		}

		/// <summary>
		/// Try to identify this Token's type if unknown.
		/// </summary>
		/// <returns>The potentially identified type</returns>
		private TokenType IdentifyType( )
		{
			// Don't re-identify if we know it
			if ( this.Type == TokenType.UNKNOWN ) {
				// Sanity check the length
				if ( this.Value.Length <= 0 ) {
					throw new Exception( "Can not identify Token with empty value." );
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
						throw new Exception( "Failed to parse literal value: '" + this.Value + "'." );
					}
				}

				// String
				if ( this.Value.Length >= 2 && this.Value[0] == '"' && this.Value[this.Value.Length - 1] == '"' ) {
					return TokenType.STRING;
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
		/// Break a line of characters into a queue of Tokens.
		/// </summary>
		/// <param name="line">The string of the line to be tokenized</param>
		/// <returns>Queue of Tokens in order that they appear</returns>
		public static Queue<Token> TokenizeLine( string line )
		{
			var tokens = new Queue<Token>( );

			// Ignore if the line is a comment or empty
			if ( line.Equals( "" ) || line[0] == (char)Symbol.COMMENT ) {
				return tokens;
			}

			// Parse character by character
			string candidate = "";
			bool inString = false;
			foreach ( char c in line ) {
				// Letters, numbers, underscores, accessor, or anything in a string passes
				if ( char.IsLetterOrDigit( c )
					|| c == '_'
					|| c == (char)Symbol.ACCESSOR
					|| inString ) {

					candidate += c;

					if ( c == '"' ) {
						inString = false;
						tokens.Enqueue( new Token( candidate , TokenType.STRING ) );
						candidate = "";
					}
				}
				// Comments end the line early
				else if ( c == (char)Symbol.COMMENT ) {
					break;
				}
				// Other special characters
				else {
					switch ( c ) {
						// ACCESSOR
						case (char)Symbol.ACCESSOR:
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
								tokens.Enqueue( new Token( candidate ) );
							}

							candidate = "";
							tokens.Enqueue( new Token( c.ToString( ) , TokenType.SYMBOL ) );
							break;

						// Whitespace
						case ' ':
						case '\t':
							if ( candidate.Length > 0 ) {
								tokens.Enqueue( new Token( candidate ) );
							}

							candidate = "";
							break;

						// Negative sign
						case '-':
							if ( candidate.Length > 0 ) {
								throw new Exception( "Unexpected symbol in middle of token: '" + c + "' (" + ( (uint)c ).ToString( ) + ")." );
							}

							candidate += c;
							break;

						// Decimal point
						case '.':
							foreach ( char ch in candidate ) {
								if ( !char.IsDigit( ch ) || ch == '-' ) {
									throw new Exception( "Unexpected symbol in middle of token: '" + c + "' (" + ( (uint)c ).ToString( ) + ")." );
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
							throw new Exception( "Illegal symbol: '" + c + "' (" + ( (uint)c ).ToString( ) + ")." );
					}
				}
			}

			// Add last candidate
			if ( inString ) {
				throw new Exception( "Line ended before string: '" + candidate + "'." );
			}

			if ( candidate.Length > 0 ) {
				tokens.Enqueue( new Token( candidate ) );
			}

			return tokens;
		}

		/// <summary>
		/// Tokenize a whole Queue of lines.
		/// </summary>
		/// <param name="lines">The program to be tokenized, broken into lines</param>
		/// <returns>Queue of Tokens in order that they appear</returns>
		public static Queue<Token> TokenizeProgram( Queue<string> lines )
		{
			var tokens = new Queue<Token>( );

			while ( lines.Count > 0 ) {
				Queue<Token> line_tokens = TokenizeLine( lines.Dequeue( ) );
				while ( line_tokens.Count > 0 ) {
					tokens.Enqueue( line_tokens.Dequeue( ) );
				}
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
			if ( token == null ) {
				throw new Exception( "Expected " + type.ToString( ) + " token, instead got null." );
			}
			else if ( token.Type != type ) {
				throw new Exception( "Unexpected token: '" + token.ToString( ) + "', expected " + type.ToString( ) + "." );
			}
		}

		/// <summary>
		/// Throw an exception if the given Token is not one of the given TokenTypes.
		/// </summary>
		/// <param name="token">Token to check type of (can be null)</param>
		/// <param name="types">The TokenTypes to check for</param>
		public static void ThrowIfNotTypes( Token token , ICollection<TokenType> types )
		{
			if ( token == null ) {
				throw new Exception( "Expected " + types.ToString( ) + " token, instead got null." );
			}
			else if ( !types.Contains( token.Type ) ) {
				throw new Exception( "Unexpected token: '" + token.ToString( ) + "', expected " + types.ToString( ) + "." );
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
				throw new Exception( "Token not expected keyword " + keyword.ToString( ) + ": " + this.ToString( ) );
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
				throw new Exception( message );
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
				throw new Exception( "Token not expected keyword " + symbol.ToString( ) + ": " + this.ToString( ) );
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
				throw new Exception( message );
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
				return ( this.Type == ( obj as Token ).Type ) && ( this.Value == ( obj as Token ).Value );
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
			string str = "'" + this.Value + "' (" + this.Type;
			if ( this.Type == TokenType.KEYWORD ) {
				str += ":" + ( (Keyword)Enum.Parse( typeof( Keyword ) , this.Value , true ) ).ToString( );
			}
			else if ( this.Type == TokenType.SYMBOL && Enum.IsDefined( typeof( Symbol ) , (int)this.Value[0] ) ) {
				str += ":" + Enum.GetName( typeof( Symbol ) , (int)this.Value[0] );
			}

			return str + ")";
		}
	}
}
