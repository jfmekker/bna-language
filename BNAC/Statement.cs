using System;
using System.Collections.Generic;

namespace BNAC
{
	/// <summary>
	/// A collection of tokens that make up a whole valid statement or instruction.
	/// Each statement maps to a "line of code".
	/// </summary>
	class Statement
	{
		/// <summary>
		/// Valid statements or instructions
		/// </summary>
		public enum StatementType
		{
			// default to unknown
			UNKNOWN,

			// non-operations
			LABEL,

			// variable operations
			OP_SET,
			OP_ADD,
			OP_SUB,
			OP_MUL,
			OP_DIV,
			OP_RAND,

			// non-variable operations
			OP_PRINT,
			OP_SLEEP,
		}

		/// <summary>
		/// The StatementType of this Statement
		/// </summary>
		public StatementType Type
		{
			get; set;
		}

		/// <summary>
		/// All Tokens that make up this Statement
		/// </summary>
		private List<Token> _tokens = new List<Token>();

		/// <summary>
		/// First operand of this Statement, the storage variable for operations that store a value
		/// </summary>
		public Token Operand1
		{
			get; private set;
		}

		/// <summary>
		/// Second operand of this Statement
		/// </summary>
		public Token Operand2
		{
			get; private set;
		}

		/// <summary>
		/// Parse valid Statements from a stream of Tokens.
		/// </summary>
		/// <param name="tokenStream"></param>
		/// <returns></returns>
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
					{
						// SET
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_SET;

						// VARIABLE
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.VARIABLE );
						candidate._tokens.Add( token );
						candidate.Operand1 = token;

						// TO
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.TO );
						candidate._tokens.Add( token );

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) { Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand2 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// Add operation
					/// "ADD [VARIABLE/LITERAL] TO [VARIABLE]"
					case Token.TokenType.ADD:
					{
						// ADD
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_ADD;

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) { Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand2 = token;

						// TO
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.TO );
						candidate._tokens.Add( token );

						// VARIABLE
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.VARIABLE );
						candidate._tokens.Add( token );
						candidate.Operand1 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// Subtract operation
					/// "Subtract [VARIABLE/LITERAL] FROM [VARIABLE]"
					case Token.TokenType.SUBTRACT:
					{
						// ADD
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_RAND;

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) { Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand2 = token;

						// TO
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.TO );
						candidate._tokens.Add( token );

						// VARIABLE
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.VARIABLE );
						candidate._tokens.Add( token );
						candidate.Operand1 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// Multiply operation
					/// "Multiply [VARIABLE] BY [VARIABLE/LITERAL]"
					case Token.TokenType.MULTIPLY:
					{
						// MULTIPLY
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_MUL;

						// VARIABLE
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.VARIABLE );
						candidate._tokens.Add( token );
						candidate.Operand1 = token;

						// BY
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.BY );
						candidate._tokens.Add( token );

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) { Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand2 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// Set operation
					/// "DIVIDE [VARIABLE] BY [VARIABLE/LITERAL]"
					case Token.TokenType.DIVIDE:
					{
						// SET
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_DIV;

						// VARIABLE
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.VARIABLE );
						candidate._tokens.Add( token );
						candidate.Operand1 = token;

						// BY
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.BY );
						candidate._tokens.Add( token );

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) { Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand2 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// Random operation
					/// "RANDOM [VARIABLE] MAX [VARIABLE/LITERAL]"
					case Token.TokenType.RANDOM:
					{
						// RANDOM
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_RAND;

						// VARIABLE
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.VARIABLE );
						candidate._tokens.Add( token );
						candidate.Operand1 = token;

						// MAX
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.MAX );
						candidate._tokens.Add( token );

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) { Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand2 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// Print operation
					/// "PRINT [VARIABLE/LITERAL]"
					case Token.TokenType.PRINT:
					{
						// PRINT
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_PRINT;

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) { Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand1 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// Sleep operation
					/// "WAIT [VARIABLE/LITERAL]"
					case Token.TokenType.WAIT:
					{
						// PRINT
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_SLEEP;

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) { Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand1 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// Label
					/// "[VARIABLE]LABEL_END"
					case Token.TokenType.VARIABLE:
					{
						// VARIABLE
						candidate._tokens.Add( token );
						candidate.Operand1 = token;
						candidate.Type = StatementType.LABEL;

						// LABEL_END
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.LABEL_END );
						candidate._tokens.Add( token );

						statements.Enqueue( candidate );
						break;
					}

					// default case
					default:
						throw new Exception( "Unknown type token in statement: " + token.ToString( ) );
				}
			}

			return statements;
		}

		/// <summary>
		/// Stringify the Statement with type information.
		/// </summary>
		/// <returns>String description of this Statement</returns>
		public override string ToString( )
		{
			var str = "[" + Type + "]   { ";
			foreach ( Token t in _tokens )
				str += t.ToString( ) + " ";
			return str + "}";
		}
	}
}
