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

			// variable modifying operations
			OP_SET,
			OP_ADD,
			OP_SUB,
			OP_MUL,
			OP_DIV,
			OP_RAND,
			OP_OR,
			OP_AND,
			OP_XOR,
			OP_NEG,
			OP_POW,
			OP_MOD,

			// test operations
			OP_TEST_GT,
			OP_TEST_LT,
			OP_TEST_EQ,

			// non-variable modifying operations
			OP_PRINT,
			OP_SLEEP,
			OP_GOTO,
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
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) {
							Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand2 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// "ADD [VARIABLE/LITERAL] TO [VARIABLE]"
					case Token.TokenType.ADD:
					{
						// ADD
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_ADD;

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) {
							Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
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

					/// "SUBTRACT [VARIABLE/LITERAL] FROM [VARIABLE]"
					case Token.TokenType.SUBTRACT:
					{
						// SUBTRACT
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_SUB;

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) {
							Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand2 = token;

						// FROM
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.FROM );
						candidate._tokens.Add( token );

						// VARIABLE
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.VARIABLE );
						candidate._tokens.Add( token );
						candidate.Operand1 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// "MULTIPLY [VARIABLE] BY [VARIABLE/LITERAL]"
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
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) {
							Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand2 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// "DIVIDE [VARIABLE] BY [VARIABLE/LITERAL]"
					case Token.TokenType.DIVIDE:
					{
						// DIVIDE
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
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) {
							Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand2 = token;

						statements.Enqueue( candidate );
						break;
					}

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
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) {
							Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand2 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// "TEST [VARIABLE/LITERAL] [>/</=] [VARIABLE/LITERAL]"
					case Token.TokenType.TEST:
					{
						// TEST
						candidate._tokens.Add( token );

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.VARIABLE );
						candidate._tokens.Add( token );
						candidate.Operand1 = token;

						// GREATER_THAN or LESS_THAN or EQUAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) {
							Token.TokenType.GREATER_THAN, Token.TokenType.LESS_THAN, Token.TokenType.EQUAL } );
						candidate._tokens.Add( token );
						switch ( token.Type ) {
							case Token.TokenType.GREATER_THAN:
								candidate.Type = StatementType.OP_TEST_GT;
								break;
							case Token.TokenType.LESS_THAN:
								candidate.Type = StatementType.OP_TEST_LT;
								break;
							case Token.TokenType.EQUAL:
								candidate.Type = StatementType.OP_TEST_EQ;
								break;
						}

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) {
							Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand2 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// "OR [VARIABLE] WITH [VARIABLE/LITERAL]"
					case Token.TokenType.OR:
					{
						// OR
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_OR;

						// VARIABLE
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.VARIABLE );
						candidate._tokens.Add( token );
						candidate.Operand1 = token;

						// WITH
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.WITH );
						candidate._tokens.Add( token );

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) {
							Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand2 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// "AND [VARIABLE] WITH [VARIABLE/LITERAL]"
					case Token.TokenType.AND:
					{
						// AND
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_AND;

						// VARIABLE
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.VARIABLE );
						candidate._tokens.Add( token );
						candidate.Operand1 = token;

						// WITH
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.WITH );
						candidate._tokens.Add( token );

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) {
							Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand2 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// "XOR [VARIABLE] WITH [VARIABLE/LITERAL]"
					case Token.TokenType.XOR:
					{
						// XOR
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_XOR;

						// VARIABLE
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.VARIABLE );
						candidate._tokens.Add( token );
						candidate.Operand1 = token;

						// WITH
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.WITH );
						candidate._tokens.Add( token );

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) {
							Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand2 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// "NEGATE [VARIABLE]"
					case Token.TokenType.NEGATE:
					{
						// NEGATE
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_NEG;

						// VARIABLE
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.VARIABLE );
						candidate._tokens.Add( token );
						candidate.Operand1 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// "RAISE [VARIABLE] TO [VARIABLE/LITERAL]"
					case Token.TokenType.RAISE:
					{
						// RAISE
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_POW;

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
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) {
							Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand2 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// "MOD [VARIABLE/LITERAL] OF [VARIABLE]"
					case Token.TokenType.MOD:
					{
						// MOD
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_MOD;

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) {
							Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand2 = token;

						// OF
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.OF );
						candidate._tokens.Add( token );

						// VARIABLE
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.VARIABLE );
						candidate._tokens.Add( token );
						candidate.Operand1 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// "PRINT [VARIABLE/LITERAL]"
					case Token.TokenType.PRINT:
					{
						// PRINT
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_PRINT;

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) {
							Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand1 = token;

						statements.Enqueue( candidate );
						break;
					}

					/// "WAIT [VARIABLE/LITERAL]"
					case Token.TokenType.WAIT:
					{
						// WAIT
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_SLEEP;

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) {
							Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand1 = token;

						statements.Enqueue( candidate );
						break;
					}

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

					/// "GOTO [VARIABLE] IF [VARIABLE/LITERAL]"
					case Token.TokenType.GOTO:
					{
						// GOTO
						candidate._tokens.Add( token );
						candidate.Type = StatementType.OP_GOTO;

						// VARIABLE
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.VARIABLE );
						candidate._tokens.Add( token );
						candidate.Operand1 = token;

						// IF
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotType( token , Token.TokenType.IF );
						candidate._tokens.Add( token );

						// VARIABLE or LITERAL
						token = tokenStream.Dequeue( );
						Token.ThrowIfNotTypes( token , new List<Token.TokenType>( ) {
							Token.TokenType.VARIABLE , Token.TokenType.LITERAL } );
						candidate._tokens.Add( token );
						candidate.Operand2 = token;

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
