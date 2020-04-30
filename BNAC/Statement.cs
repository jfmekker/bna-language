using System;
using System.Collections.Generic;

namespace BNAC
{
	/// <summary>
	/// A collection of tokens that make up a whole valid statement or instruction.
	/// Each statement maps to a "line of code".
	/// </summary>
	internal class Statement
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
			OP_LOG,
			OP_ROUND,

			// test operations
			OP_TEST_GT,
			OP_TEST_LT,
			OP_TEST_EQ,

			// non-variable modifying operations
			OP_PRINT,
			OP_WAIT,
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
		private List<Token> _tokens = new List<Token>( );

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
				Token token = tokenStream.Dequeue( );
				var candidate = new Statement( );

				switch ( token.Type ) {

					case TokenType.KEYWORD:
						statements.Enqueue( ParseKeywordStatement( token , tokenStream ) );
						break;

					case TokenType.SYMBOL:
						// Only symbol started statement is a label
						// LABEL_START var LABEL_END:
						candidate._tokens.Add( token );
						candidate.AddTokenOfSymbols( tokenStream.Dequeue( ) , new List<Symbol> { Symbol.LABEL_START } );
						candidate.AddTokenOfTypes( tokenStream.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } );
						candidate.AddTokenOfSymbols( tokenStream.Dequeue( ) , new List<Symbol> { Symbol.LABEL_END } );
						candidate.Type = StatementType.LABEL;
						statements.Enqueue( candidate );
						break;

					default:
						throw new Exception( "Invalid start of statement: " + token );

				}

			}

			return statements;
		}

		/// <summary>
		/// Parse a Statement from a queue, starting with a keyword.
		/// </summary>
		/// <param name="token">The starting keyword Token</param>
		/// <param name="tokens">The token queue to parse from</param>
		/// <returns></returns>
		private static Statement ParseKeywordStatement( Token token , Queue<Token> tokens )
		{
			var candidate = new Statement( );
			candidate._tokens.Add( token );

			var start = (Keyword)Enum.Parse( typeof(Keyword) , token.Value );
			switch ( start ) {

				// SET var TO var|lit|string
				case Keyword.SET: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.TO } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL , TokenType.STRING } , operand: 2 );
					candidate.Type = StatementType.OP_SET;
					break;
				}

				// ADD var|lit TO var
				case Keyword.ADD: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.TO } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.Type = StatementType.OP_ADD;
					break;
				}

				// SUBTRACT var|lit FROM var
				case Keyword.SUBTRACT: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.FROM } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.Type = StatementType.OP_SUB;
					break;
				}

				// MULTIPLY|DIVIDE var BY var|lit
				case Keyword.MULTIPLY:
				case Keyword.DIVIDE: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.BY } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.Type = ( start == Keyword.MULTIPLY ) ? StatementType.OP_MUL : StatementType.OP_DIV;
					break;
				}

				// AND|OR|XOR var WITH var|lit
				case Keyword.AND:
				case Keyword.OR:
				case Keyword.XOR: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.BY } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.Type = ( start == Keyword.AND ) ? StatementType.OP_AND : ( ( start == Keyword.OR ) ? StatementType.OP_OR : StatementType.OP_XOR );
					break;
				}

				// MOD|LOG var|lit OF var
				case Keyword.MOD:
				case Keyword.LOG: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.FROM } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.Type = ( start == Keyword.MOD ) ? StatementType.OP_MOD : StatementType.OP_LOG;
					break;
				}

				// RAISE var TO var|lit
				case Keyword.RAISE: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.TO } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.Type = StatementType.OP_POW;
					break;
				}

				// NEGATE|ROUND var
				case Keyword.NEGATE:
				case Keyword.ROUND: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.Type = ( start == Keyword.NEGATE ) ? StatementType.OP_NEG : StatementType.OP_ROUND;
					break;
				}

				// RANDOM var MAX var|lit
				case Keyword.RANDOM: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.MAX } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.Type = StatementType.OP_RAND;
					break;
				}

				// PRINT var|lit|string
				case Keyword.PRINT: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL , TokenType.STRING } , operand: 1 );
					candidate.Type = StatementType.OP_PRINT;
					break;
				}

				// WAIT var|lit
				case Keyword.WAIT: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 1 );
					candidate.Type = StatementType.OP_WAIT;
					break;
				}

				// TEST var GT|LT var|lit
				// TEST var EQ var|lit|string
				case Keyword.TEST: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					Token next = tokens.Dequeue( );
					if ( Token.TryParseSymbol( next.Value[0] , out Symbol symbol ) ) {
						if ( symbol == Symbol.GREATER_THAN || symbol == Symbol.LESS_THAN ) {
							candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
							candidate.Type = symbol == Symbol.GREATER_THAN ? StatementType.OP_TEST_GT : StatementType.OP_TEST_LT;
						}
						else if ( symbol == Symbol.EQUAL ) {
							candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL , TokenType.STRING } , operand: 2 );
							candidate.Type = StatementType.OP_TEST_EQ;
						}
						else {
							throw new Exception( "Unexpected symbol: " + next.ToString( ) );
						}
					}
					else {
						throw new Exception( "Expected symbol, but got other token: " + next.ToString( ) );
					}
					break;
				}

				// GOTO var IF var|lit
				case Keyword.GOTO: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.IF } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.Type = StatementType.OP_GOTO;
					break;
				}

				default:
					throw new Exception( "Invalid start of statement: " + token.ToString( ) );

			}

			return candidate;
		}

		public void AddTokenOfTypes( Token token , List<TokenType> tokenTypes , int operand = 0 )
		{
			Token.ThrowIfNotTypes( token , tokenTypes );
			this._tokens.Add( token );
			if ( operand == 1 ) {
				this.Operand1 = token;
			}
			else if ( operand == 2 ) {
				this.Operand2 = token;
			}
		}

		public void AddTokenOfKeywords( Token token , List<Keyword> keywords )
		{
			token.ThrowIfNotKeywords( keywords );
			this._tokens.Add( token );
		}

		public void AddTokenOfSymbols( Token token , List<Symbol> symbols )
		{
			token.ThrowIfNotSymbols( symbols );
			this._tokens.Add( token );
		}

		/// <summary>
		/// Stringify the Statement with type information.
		/// </summary>
		/// <returns>String description of this Statement</returns>
		public override string ToString( )
		{
			string str = "[" + this.Type + "]   { ";
			foreach ( Token t in this._tokens ) {
				str += t.ToString( ) + " ";
			}

			return str + "}";
		}
	}
}
