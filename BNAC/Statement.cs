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
			OP_LOG,
			OP_ROUND,

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
		private static Statement ParseKeywordStatement( Token token, Queue<Token> tokens )
		{
			var candidate = new Statement( );
			candidate._tokens.Add( token );

			switch ( (Keyword)Enum.Parse(System.Type.GetType("Keyword"), token.Value) ) {

				// SET var TO var|lit|string
				case Keyword.SET:
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.TO } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE, TokenType.LITERAL, TokenType.STRING } , operand: 2 );
					candidate.Type = StatementType.OP_SET;
					break;

				// ADD var|lit TO var
				case Keyword.ADD:
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE, TokenType.LITERAL } , operand: 2 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.TO } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.Type = StatementType.OP_ADD;
					break;

				// SUBTRACT var|lit FROM var
				case Keyword.SUBTRACT:
					break;

				// MULTIPLY|DIVIDE var BY var|lit
				case Keyword.MULTIPLY:
				case Keyword.DIVIDE:
					break;

				// AND|OR|XOR var WITH var|lit
				case Keyword.AND:
				case Keyword.OR:
				case Keyword.XOR:
					break;

				// MOD|LOG var|lit OF var
				case Keyword.MOD:
				case Keyword.LOG:
					break;

				// RAISE var TO var|lit
				case Keyword.RAISE:
					break;

				// NEGATE|ROUND var
				case Keyword.NEGATE:
				case Keyword.ROUND:
					break;

				// RANDOM var MAX var|lit
				case Keyword.RANDOM:
					break;

				// PRINT var|lit|string
				case Keyword.PRINT:
					break;

				// WAIT var|lit
				case Keyword.WAIT:
					break;

				// TEST var GT|LT var|lit
				// TEST var EQ var|lit|string
				case Keyword.TEST:
					break;

				// GOTO var IF var|lit
				case Keyword.GOTO:
					break;

				default:
					throw new Exception( "Invalid start of statement: " + token.ToString( ) );

			}

			return candidate;
		}

		public void AddTokenOfTypes( Token token, List<TokenType> tokenTypes, int operand = 0 )
		{
			Token.ThrowIfNotTypes( token , tokenTypes );
			_tokens.Add( token );
			if ( operand == 1 )
				Operand1 = token;
			else if ( operand == 2 )
				Operand2 = token;
		}

		public void AddTokenOfKeywords( Token token , List<Keyword> keywords )
		{
			token.ThrowIfNotKeywords( keywords );
			_tokens.Add( token );
		}

		public void AddTokenOfSymbols( Token token , List<Symbol> symbols )
		{
			token.ThrowIfNotSymbols( symbols );
			_tokens.Add( token );
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

	class StatementValidatorNode
	{
		public static StatementValidatorNode START = new StatementValidatorNode();

		private IList<StatementValidatorNode> followers = new List<StatementValidatorNode>( );

		private Token predecessor;

		/// <summary>
		/// Add a new valid statement to a StatementValidator tree;
		/// </summary>
		/// <param name="tokens"></param>
		public void AddStatement( IList<Token> tokens, int index = 0 )
		{
			bool added = false;
			foreach ( StatementValidatorNode s in followers ) {
				if ( s.predecessor.Equals( tokens[index] ) ) {
					s.AddStatement( tokens , index + 1 );
					added = true;
					break;
				}
			}
			if ( !added ) {
				var s = new StatementValidatorNode( );
				s.predecessor = tokens[index];
			}
		}

		/// <summary>
		/// Validate a set of Tokens as a Statement and build said Statement.
		/// </summary>
		/// <param name="tokens">The ordered set of Tokens to validate.</param>
		/// <returns>Statement parse from the Tokens, null if invalid.</returns>
		public Statement Validate( IList<Token> tokens )
		{
			return null;
		}
	}
}
