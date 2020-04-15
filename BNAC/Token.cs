using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BNAC
{
	
	class Token
	{
		const char LABEL_END = ':';

		public enum TokenType
		{
			UNKNOWN,

			KEYWORD,
			LITERAL,
			VARIABLE,

			LABEL_END,

			// Operation start keywords
			SET,
			ADD,

			// Operation mid keywords
			TO,

			PRINT,
		}

		public TokenType Type
		{
			get; set;
		}

		public string Value
		{
			get; set;
		}

		private Token( string value, TokenType type = TokenType.UNKNOWN )
		{
			Value = value;
			Type = type;

			// Try to identify type
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

				// Keywords
				else if ( Value.ToUpper( ).Equals( "SET" ) ) {
					Type = TokenType.SET;
				}
				else if ( Value.ToUpper( ).Equals( "ADD" ) ) {
					Type = TokenType.ADD;
				}
				else if ( Value.ToUpper( ).Equals( "TO" ) ) {
					Type = TokenType.TO;
				}

				// Variable
				else {
					Type = TokenType.VARIABLE;
				}
			}
		}

		public static Queue<Token> TokenizeLine( string line )
		{
			var tokens = new Queue<Token>( );

			// First, check if this is a comment
			if ( line[0] == '#' )
				return tokens;

			// Parse character by character
			string candidate = "";
			foreach ( char c in line ) {
				switch ( c ) {
					case LABEL_END:
						tokens.Enqueue( new Token( candidate ) );
						candidate = "";
						tokens.Enqueue( new Token( c.ToString(), TokenType.LABEL_END ) );
						break;

					case ' ':
					case '\t':
						if ( candidate.Length > 0 ) 
							tokens.Enqueue( new Token( candidate ) );
						candidate = "";
						break;

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

		public override string ToString( )
		{
			return "'" + Value + "' (" + Type + ")";
		}

		public static void ThrowIfNotType( Token token , TokenType type )
		{
			if ( token == null ) {
				throw new Exception( "Expected " + type.ToString() + " token, instead got null." );
			}
			else if ( token.Type != type ) {
				throw new Exception( "Unexpected token: '" + token.ToString( ) + "', expected " + type.ToString() + ".");
			}
		}

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
