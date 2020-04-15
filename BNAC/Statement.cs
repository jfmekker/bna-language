using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNAC
{
	class Statement
	{
		public enum StatementType
		{
			UNKNOWN,

			LABEL,

			OP_SET,
			OP_ADD,
		}

		public StatementType Type
		{
			get; set;
		}

		private List<Token> _tokens = new List<Token>();

		private Token _operand1;
		private Token _operand2;

		public static Queue<Statement> ParseStatements( Queue<Token> tokenStream )
		{
			var statements = new Queue<Statement>( );

			// Go through each token
			while ( tokenStream.Count > 0 ) {
				var token = tokenStream.Dequeue( );
				var candidate = new Statement( );

				// Assume we start on a valid start token
				switch ( token.Type ) {
					
					/// Set operation
					/// "SET [VARIABLE] TO [VARIABLE/LITERAL]"
					case Token.TokenType.SET:
						// SET
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_SET;

						// VARIABLE
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.VARIABLE );
						candidate._tokens.Add( token );
						candidate._operand1 = token;

						// TO
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.TO );
						candidate._tokens.Add( token );

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) { Token.TokenType.VARIABLE, Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate._operand2 = token;

						statements.Enqueue( candidate );
						break;

					/// Add operation
					/// "ADD [VARIABLE/LITERAL] TO [VARIABLE]"
					case Token.TokenType.ADD:
						// ADD
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_ADD;

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) { Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate._operand2 = token;

						// TO
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.TO );
						candidate._tokens.Add( token );

						// VARIABLE
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.VARIABLE );
						candidate._tokens.Add( token );
						candidate._operand1 = token;

						statements.Enqueue( candidate );
						break;

					/// Label
					/// "[VARIABLE]LABEL_END"
					case Token.TokenType.VARIABLE:
						// VARIABLE
						candidate._tokens.Add( token );
						candidate.Type = StatementType.LABEL;
						
						// LABEL_END
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.LABEL_END );
						candidate._tokens.Add( token );

						statements.Enqueue( candidate );
						break;

					// default case
					default:
						throw new Exception( "Unknown type token in statement: " + token.ToString( ) );
				}
			}

			return statements;
		}

		public override string ToString( )
		{
			var str = "[" + Type + "]   { ";
			foreach ( Token t in _tokens )
				str += t.ToString( ) + " ";
			return str + "}";
		}
	}
}
