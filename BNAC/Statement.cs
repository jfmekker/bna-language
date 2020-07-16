using System;
using System.Collections.Generic;

namespace BNAC
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

		// arithmetic operations
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

		// list operations
		OP_LIST,
		OP_APPEND,

		// io operations
		OP_OPEN_R,
		OP_OPEN_W,
		OP_CLOSE,
		OP_READ,
		OP_WRITE,
		OP_INPUT,
		OP_PRINT,

		// test operations
		OP_TEST_GT,
		OP_TEST_LT,
		OP_TEST_EQ,

		// misc operations
		OP_WAIT,
		OP_GOTO,
	}

	/// <summary>
	/// A collection of tokens that make up a whole valid statement or instruction.
	/// Each statement maps to a "line of code".
	/// </summary>
	internal class Statement
	{
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
		/// <param name="tokenStream">A stream of tokens that represent a set of statements</param>
		/// <returns>Queue of parsed Statements</returns>
		public static Queue<Statement> ParseStatements( Queue<Token> tokenStream )
		{
			var statements = new Queue<Statement>( );

			// Go through each token, assume we start on the start of a statement
			while ( tokenStream.Count > 0 ) {
				Token token = tokenStream.Dequeue( );
				var candidate = new Statement( );

				switch ( token.Type ) {

					case TokenType.KEYWORD:
						statements.Enqueue( ParseKeywordStatement( token , tokenStream ) );
						break;

					case TokenType.SYMBOL:
						// LABEL_START var LABEL_END:
						candidate.AddTokenOfSymbols( token , new List<Symbol> { Symbol.LABEL_START } );
						candidate.AddTokenOfTypes( tokenStream.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
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
		/// Parse a Statement from a queue, starting with a keyword. Removes items from the queue.
		/// </summary>
		/// <param name="token">Starting keyword Token</param>
		/// <param name="tokens">Token queue to parse from</param>
		/// <returns>Single parsed Statement</returns>
		private static Statement ParseKeywordStatement( Token token , Queue<Token> tokens )
		{
			var candidate = new Statement( );
			candidate._tokens.Add( token );

			// Based on starting token, try to parse the appropriate statement
			var start = (Keyword)Enum.Parse( typeof( Keyword ) , token.Value , true );
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
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.WITH } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.Type = ( start == Keyword.AND ) ? StatementType.OP_AND : ( ( start == Keyword.OR ) ? StatementType.OP_OR : StatementType.OP_XOR );
					break;
				}

				// MOD|LOG var|lit OF var
				case Keyword.MOD:
				case Keyword.LOG: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.OF } );
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

				// WAIT var|lit
				case Keyword.WAIT: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 1 );
					candidate.Type = StatementType.OP_WAIT;
					break;
				}

				// TEST var GT|LT var|lit
				// TEST var EQ var|lit|string
				case Keyword.TEST: {
					// Have to do TEST a little manually because I don't want to allow string value comparison (only equals)
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

				// LIST var SIZE var|lit
				case Keyword.LIST: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.SIZE } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.Type = StatementType.OP_LIST;
					break;
				}

				// APPEND var|lit TO var
				case Keyword.APPEND: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.TO } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.Type = StatementType.OP_APPEND;
					break;
				}

				// OPEN var|string AS READ|WRITE var
				case Keyword.OPEN: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.STRING } , operand: 2 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.AS } );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.READ , Keyword.WRITE } );
					candidate.Type = candidate._tokens[candidate._tokens.Count - 1].Value.Equals( Keyword.READ.ToString( ) , StringComparison.CurrentCultureIgnoreCase ) ?
						StatementType.OP_OPEN_R : StatementType.OP_OPEN_W;
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					break;
				}

				// CLOSE var
				case Keyword.CLOSE: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.Type = StatementType.OP_CLOSE;
					break;
				}

				// WRITE var|lit|string TO var
				case Keyword.WRITE: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL , TokenType.STRING } , operand: 2 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.TO } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.Type = StatementType.OP_WRITE;
					break;
				}

				// READ var FROM var
				case Keyword.READ: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.FROM } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 2 );
					candidate.Type = StatementType.OP_READ;
					break;
				}

				// INPUT var WITH var|string
				case Keyword.INPUT: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.WITH } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.STRING } , operand: 2 );
					candidate.Type = StatementType.OP_INPUT;
					break;
				}

				// PRINT var|lit|string
				case Keyword.PRINT: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL , TokenType.STRING } , operand: 1 );
					candidate.Type = StatementType.OP_PRINT;
					break;
				}

				default:
					throw new Exception( "Invalid start of statement: " + token.ToString( ) );

			}

			return candidate;
		}

		/// <summary>
		/// Add a Token if it is a specified type, else throw an exception.
		/// </summary>
		/// <param name="token">Token to add</param>
		/// <param name="tokenTypes">List of valid types</param>
		/// <param name="operand">Which operand to set (defaults to none)</param>
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

		/// <summary>
		/// Add a Token if it is a specified Keyword, else throw an exception.
		/// </summary>
		/// <param name="token">Token to add</param>
		/// <param name="keywords">List of valid keywords</param>
		public void AddTokenOfKeywords( Token token , List<Keyword> keywords )
		{
			token.ThrowIfNotKeywords( keywords );
			this._tokens.Add( token );
		}

		/// <summary>
		/// Add a Token if it is a specified Symbol, else throw an exception.
		/// </summary>
		/// <param name="token">Token to add</param>
		/// <param name="symbols">List of valid symbols</param>
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
