using System;
using System.Collections.Generic;

namespace BNAC
{
	/// <summary>
	/// Group of related characters that form keywords/variables/etc.
	/// </summary>
	class Token
	{
		/// <summary>
		/// Recognized token types and keywords.
		/// </summary>
		public enum TokenType
		{
			// Default to unknown token
			UNKNOWN,

			// Non-keywords
			LITERAL,
			VARIABLE,

			// Special characters
			LABEL_END,

			// Operation start keywords
			SET,
			ADD,
			SUBTRACT,
			MULTIPLY,
			DIVIDE,
			PRINT,
			WAIT,
			RANDOM,

			// Operation mid keywords
			TO,
			BY,
			FROM,
			MAX,

			// Special words
		}

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
		private Token( string value, TokenType type = TokenType.UNKNOWN )
		{
			Value = value;
			Type = type;

			IdentifyType( );
		}

		/// <summary>
		/// Try to identify this Token's type if unknown.
		/// </summary>
		/// <returns>The potentially identified type</returns>
		private TokenType IdentifyType( )
		{
			// Don't re-identify if we know it
			if ( Type == TokenType.UNKNOWN ) {
				// Number Literal
				if ( char.IsDigit( Value[0] ) ) {
					if ( int.TryParse( Value , out int val ) ) {
						Type = TokenType.LITERAL;
					}
					else {
						throw new Exception( "Failed to parse literal value: '" + Value + "'." );
					}
				}

				// Main keywords
				else if ( Value.ToUpper( ).Equals( "SET" ) ) {
					Type = TokenType.SET;
				}
				else if ( Value.ToUpper( ).Equals( "ADD" ) ) {
					Type = TokenType.ADD;
				}
				else if ( Value.ToUpper( ).Equals( "SUBTRACT" ) ) {
					Type = TokenType.SUBTRACT;
				}
				else if ( Value.ToUpper( ).Equals( "MULTIPLY" ) ) {
					Type = TokenType.MULTIPLY;
				}
				else if ( Value.ToUpper( ).Equals( "DIVIDE" ) ) {
					Type = TokenType.DIVIDE;
				}
				else if ( Value.ToUpper( ).Equals( "PRINT" ) ) {
					Type = TokenType.PRINT;
				}
				else if ( Value.ToUpper( ).Equals( "WAIT" ) ) {
					Type = TokenType.WAIT;
				}
				else if ( Value.ToUpper( ).Equals( "RANDOM" ) ) {
					Type = TokenType.RANDOM;
				}

				// Mid-operation keywords
				else if ( Value.ToUpper( ).Equals( "TO" ) ) {
					Type = TokenType.TO;
				}
				else if ( Value.ToUpper( ).Equals( "BY" ) ) {
					Type = TokenType.BY;
				}
				else if ( Value.ToUpper( ).Equals( "FROM" ) ) {
					Type = TokenType.FROM;
				}
				else if ( Value.ToUpper( ).Equals( "MAX" ) ) {
					Type = TokenType.MAX;
				}

				// Variable
				else {
					Type = TokenType.VARIABLE;
				}
			}

			return Type;
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
			if ( line.Equals("") || line[0] == '#' )
				return tokens;

			// Parse character by character
			string candidate = "";
			foreach ( char c in line ) {
				switch ( c ) {
					// LABEL_END
					case ':':
						tokens.Enqueue( new Token( candidate ) );
						candidate = "";
						tokens.Enqueue( new Token( c.ToString(), TokenType.LABEL_END ) );
						break;

					// Whitespace
					case ' ':
					case '\t':
						if ( candidate.Length > 0 ) 
							tokens.Enqueue( new Token( candidate ) );
						candidate = "";
						break;

					// Check if alphanumeric
					default:
						if ( char.IsLetterOrDigit( c ) ) {
							candidate += c;
							break;
						}
						throw new Exception( "Illegal symbol: '" + c + "' (" + ( (uint)c ).ToString( ) + ").");
				}
			}

			// Add last candidate
			if ( candidate.Length > 0 )
				tokens.Enqueue( new Token( candidate ) );

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
				var line_tokens = TokenizeLine( lines.Dequeue() );
				while ( line_tokens.Count > 0 ) {
					tokens.Enqueue( line_tokens.Dequeue( ) );
				}
			}

			return tokens;
		}

		/// <summary>
		/// Stringify the Token with type information.
		/// </summary>
		/// <returns>String description of the Token</returns>
		public override string ToString( )
		{
			return "'" + Value + "' (" + Type + ")";
		}

		/// <summary>
		/// Throw an exception if the given Token is not of the given TokenType.
		/// </summary>
		/// <param name="token">Token to check type of (can be null)</param>
		/// <param name="type">The TokenType to check for</param>
		public static void ThrowIfNotType( Token token , TokenType type )
		{
			if ( token == null ) {
				throw new Exception( "Expected " + type.ToString() + " token, instead got null." );
			}
			else if ( token.Type != type ) {
				throw new Exception( "Unexpected token: '" + token.ToString( ) + "', expected " + type.ToString() + ".");
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
	}
}
