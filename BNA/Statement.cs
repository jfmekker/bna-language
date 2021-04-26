using System;
using System.Collections.Generic;

namespace BNA
{
	/// <summary>
	/// Valid statements or instructions
	/// </summary>
	public enum StatementType
	{
		UNKNOWN = -1,

		// non-operations
		NULL = 0,
		COMMENT,
		LABEL,

		// numeric operations
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
		OP_SIZE,

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
	public class Statement
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
		/// Parse a valid statement from a <see cref="Token"/> list.
		/// </summary>
		/// <param name="tokenList">List of tokens to parse</param>
		/// <returns>A valid statement</returns>
		/// <exception cref="CompiletimeException">Excepts for invalid statements</exception>
		public static Statement ParseStatement( List<Token> tokenList )
		{
			if ( tokenList.Count == 0 ) {
				var s = new Statement( );
				s._tokens.Add( new Token( "" ) );
				s.Type = StatementType.NULL;
				return s;
			}
			else if ( tokenList[0].Type == TokenType.COMMENT ) {
				var s = new Statement( );
				s._tokens.Add( tokenList[0] );
				s.Type = StatementType.COMMENT;
				return s;
			}
			else if ( tokenList[0].Type == TokenType.KEYWORD ) {
				try {
					return ParseKeywordStatement( tokenList );
				}
				catch (InvalidOperationException e) {
					throw new CompiletimeException( "Statement ended too early" );
				}
			}
			else if ( tokenList[0].Type == TokenType.SYMBOL ) {
				try {
					return ParseSymbolStatement( tokenList );
				}
				catch ( InvalidOperationException e ) {
					throw new CompiletimeException( "Statement ended too early" );
				}
			}
			else {
				throw new CompiletimeException( "Invalid start of statement: " + tokenList[0].ToString( ) );
			}
		}

		/// <summary>
		/// Parse a valid statement that starts with a <see cref="Symbol"/>.
		/// </summary>
		/// <param name="tokenList">List of tokens to parse</param>
		/// <returns>A valid statement</returns>
		/// <exception cref="CompiletimeException">Excepts for invalid statements</exception>
		private static Statement ParseSymbolStatement( List<Token> tokenList )
		{
			var tokens = new Queue<Token>( tokenList );
			var candidate = new Statement( );

			// LABEL_START var LABEL_END:
			candidate.AddTokenOfSymbols( tokens.Dequeue( ) , new List<Symbol> { Symbol.LABEL_START } );
			candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
			candidate.AddTokenOfSymbols( tokens.Dequeue( ) , new List<Symbol> { Symbol.LABEL_END } );
			candidate.Type = StatementType.LABEL;
			return candidate;
		}

		/// <summary>
		/// Parse a valid statement that starts with a <see cref="Keyword"/>.
		/// </summary>
		/// <param name="tokenList">List of tokens to parse</param>
		/// <returns>A valid statement</returns>
		/// <exception cref="CompiletimeException">Excepts for invalid statements</exception>
		private static Statement ParseKeywordStatement( List<Token> tokenList )
		{
			var tokens = new Queue<Token>( tokenList );
			Token startToken = tokens.Dequeue( );
			var candidate = new Statement( );
			candidate._tokens.Add( startToken );

			// Based on starting token, try to parse the appropriate statement
			var start = (Keyword)Enum.Parse( typeof( Keyword ) , startToken.Value , true );
			switch ( start ) {

				// SET var TO var|lit|string|list
				case Keyword.SET: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.TO } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL , TokenType.STRING , TokenType.LIST } , operand: 2 );
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
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.Type = StatementType.OP_WAIT;
					break;
				}

				// TEST var GT|LT var|lit
				// TEST var EQ var|lit|string
				case Keyword.TEST: {
					// Have to do TEST a little manually because I don't want to allow string value comparison (only equals)
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					Token next = tokens.Dequeue( );

					candidate.AddTokenOfSymbols( next , new List<Symbol> { Symbol.GREATER_THAN , Symbol.LESS_THAN , Symbol.EQUAL } );
					var symbol = (Symbol)next.Value[0];

					if ( symbol == Symbol.GREATER_THAN || symbol == Symbol.LESS_THAN ) {
						candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
						candidate.Type = symbol == Symbol.GREATER_THAN ? StatementType.OP_TEST_GT : StatementType.OP_TEST_LT;
					}
					else if ( symbol == Symbol.EQUAL ) {
						candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL , TokenType.STRING } , operand: 2 );
						candidate.Type = StatementType.OP_TEST_EQ;
					}
					else {
						throw new CompiletimeException( "Unexpected symbol: " + next.ToString( ) );
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

				// APPEND var|lit|string TO var
				case Keyword.APPEND: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL , TokenType.STRING } , operand: 2 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.TO } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.Type = StatementType.OP_APPEND;
					break;
				}

				// SIZE var OF var|string|list
				case Keyword.SIZE: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.OF } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.STRING , TokenType.LIST } , operand: 2 );
					candidate.Type = StatementType.OP_SIZE;
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

				// PRINT var|lit|string|list
				case Keyword.PRINT: {
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL , TokenType.STRING , TokenType.LIST } , operand: 2 );
					candidate.Type = StatementType.OP_PRINT;
					break;
				}

				default:
					throw new CompiletimeException( "Invalid start of statement: " + startToken.ToString( ) );

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
			string str = "[" + this.Type + "] \t";

			str += $"op1={this.Operand1.ToString( ),-24} op2={this.Operand2.ToString( ),-24}";

			return str;
		}

		/// <summary>
		/// Gives the raw string of the line that created this Statement.
		/// </summary>
		/// <returns>String of the raw input that made this Statement.</returns>
		public string RawString( )
		{
			string str = "";
			for ( int i = 0 ; i < this._tokens.Count ; i += 1 ) {
				str += this._tokens[i].Value + " ";
			}
			return str;
		}
	}
}
