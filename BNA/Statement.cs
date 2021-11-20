﻿using BNA.Exceptions;
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
		SET,
		ADD,
		SUBTRACT,
		MULTIPLY,
		DIVIDE,
		RANDOM,
		BITWISE_OR,
		BITWISE_AND,
		BITWISE_XOR,
		BITWISE_NEGATE,
		POWER,
		MODULUS,
		LOGARITHM,
		ROUND,

		// list operations
		LIST,
		APPEND,
		SIZE,

		// io operations
		OPEN_READ,
		OPEN_WRITE,
		CLOSE,
		READ,
		WRITE,
		INPUT,
		PRINT,

		// test operations
		TEST_GTR,
		TEST_LSS,
		TEST_EQU,
		TEST_NEQ,

		// scope operations
		SCOPE_OPEN,
		SCOPE_CLOSE,

		// misc operations
		WAIT,
		GOTO,
		TYPE,
		EXIT,
		ERROR,
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
		private readonly List<Token> tokens = new( );

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
			if ( tokenList.Count == 0 )
			{
				var s = new Statement( );
				s.tokens.Add( new Token( "" ) );
				s.Type = StatementType.NULL;
				return s;
			}
			else if ( tokenList[0].Type == TokenType.COMMENT )
			{
				var s = new Statement( );
				s.tokens.Add( tokenList[0] );
				s.Type = StatementType.COMMENT;
				return s;
			}
			else if ( tokenList[0].Type == TokenType.KEYWORD )
			{
				try
				{
					return ParseKeywordStatement( tokenList );
				}
				catch ( InvalidOperationException )
				{
					throw new CompiletimeException( "Statement ended too early" );
				}
			}
			else if ( tokenList[0].Type == TokenType.SYMBOL )
			{
				try
				{
					return ParseSymbolStatement( tokenList );
				}
				catch ( InvalidOperationException )
				{
					throw new CompiletimeException( "Statement ended too early" );
				}
			}
			else
			{
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
			candidate.tokens.Add( startToken );

			// Based on starting token, try to parse the appropriate statement
			var start = (Keyword)Enum.Parse( typeof( Keyword ) , startToken.Value , true );
			switch ( start )
			{

				// SET var TO var|lit|string|list
				case Keyword.SET:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.TO } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL , TokenType.STRING , TokenType.LIST } , operand: 2 );
					candidate.Type = StatementType.SET;
					break;
				}

				// ADD var|lit TO var
				case Keyword.ADD:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.TO } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.Type = StatementType.ADD;
					break;
				}

				// SUBTRACT var|lit FROM var
				case Keyword.SUBTRACT:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.FROM } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.Type = StatementType.SUBTRACT;
					break;
				}

				// MULTIPLY|DIVIDE var BY var|lit
				case Keyword.MULTIPLY:
				case Keyword.DIVIDE:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.BY } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.Type = ( start == Keyword.MULTIPLY ) ? StatementType.MULTIPLY : StatementType.DIVIDE;
					break;
				}

				// AND|OR|XOR var WITH var|lit
				case Keyword.AND:
				case Keyword.OR:
				case Keyword.XOR:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.WITH } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.Type = ( start == Keyword.AND ) ? StatementType.BITWISE_AND : ( ( start == Keyword.OR ) ? StatementType.BITWISE_OR : StatementType.BITWISE_XOR );
					break;
				}

				// MOD|LOG var|lit OF var
				case Keyword.MOD:
				case Keyword.LOG:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.OF } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.Type = ( start == Keyword.MOD ) ? StatementType.MODULUS : StatementType.LOGARITHM;
					break;
				}

				// RAISE var TO var|lit
				case Keyword.RAISE:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.TO } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.Type = StatementType.POWER;
					break;
				}

				// NEGATE|ROUND var
				case Keyword.NEGATE:
				case Keyword.ROUND:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.Type = ( start == Keyword.NEGATE ) ? StatementType.BITWISE_NEGATE : StatementType.ROUND;
					break;
				}

				// RANDOM var MAX var|lit
				case Keyword.RANDOM:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.MAX } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.Type = StatementType.RANDOM;
					break;
				}

				// WAIT var|lit
				case Keyword.WAIT:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.Type = StatementType.WAIT;
					break;
				}

				// TEST var GT|LT var|lit
				// TEST var EQ var|lit|string
				case Keyword.TEST:
				{
					// Have to do TEST a little manually because I don't want to allow string value comparison (only equals)
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					Token next = tokens.Dequeue( );

					candidate.AddTokenOfSymbols( next , new List<Symbol> { Symbol.GREATER_THAN , Symbol.LESS_THAN , Symbol.EQUAL , Symbol.NOT } );
					var symbol = (Symbol)next.Value[0];

					if ( symbol is Symbol.GREATER_THAN or Symbol.LESS_THAN )
					{
						candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
						candidate.Type = symbol == Symbol.GREATER_THAN ? StatementType.TEST_GTR : StatementType.TEST_LSS;
					}
					else if ( symbol is Symbol.EQUAL or Symbol.NOT )
					{
						candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL , TokenType.STRING } , operand: 2 );
						candidate.Type = symbol == Symbol.EQUAL ? StatementType.TEST_EQU : StatementType.TEST_NEQ;
					}
					else
					{
						throw new CompiletimeException( "Unexpected symbol: " + next.ToString( ) );
					}

					break;
				}

				// GOTO var IF var|lit
				case Keyword.GOTO:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.IF } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.Type = StatementType.GOTO;
					break;
				}

				// LIST var SIZE var|lit
				case Keyword.LIST:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.SIZE } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL } , operand: 2 );
					candidate.Type = StatementType.LIST;
					break;
				}

				// APPEND var|lit|string|list TO var
				case Keyword.APPEND:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL , TokenType.STRING , TokenType.LIST } , operand: 2 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.TO } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.Type = StatementType.APPEND;
					break;
				}

				// SIZE var OF var|string|list
				case Keyword.SIZE:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.OF } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.STRING , TokenType.LIST } , operand: 2 );
					candidate.Type = StatementType.SIZE;
					break;
				}

				// OPEN var|string AS READ|WRITE var
				case Keyword.OPEN:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.STRING } , operand: 2 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.AS } );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.READ , Keyword.WRITE } );
					candidate.Type = (Keyword)Enum.Parse( typeof( Keyword ) , candidate.tokens[3].Value ) == Keyword.READ
						? StatementType.READ : StatementType.WRITE;
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					break;
				}

				// CLOSE var
				case Keyword.CLOSE:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.Type = StatementType.CLOSE;
					break;
				}

				// WRITE var|lit|string|list TO var
				case Keyword.WRITE:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL , TokenType.STRING , TokenType.LIST } , operand: 2 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.TO } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.Type = StatementType.WRITE;
					break;
				}

				// READ var FROM var
				case Keyword.READ:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.FROM } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 2 );
					candidate.Type = StatementType.READ;
					break;
				}

				// INPUT var WITH var|string
				case Keyword.INPUT:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.WITH } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.STRING } , operand: 2 );
					candidate.Type = StatementType.INPUT;
					break;
				}

				// PRINT var|lit|string|list
				case Keyword.PRINT:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL , TokenType.STRING , TokenType.LIST } , operand: 2 );
					candidate.Type = StatementType.PRINT;
					break;
				}

				// TYPE var OF var|lit|string|list
				case Keyword.TYPE:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE } , operand: 1 );
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.OF } );
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.LITERAL , TokenType.STRING , TokenType.LIST } , operand: 2 );
					candidate.Type = StatementType.TYPE;
					break;
				}

				// SCOPE OPEN|CLOSE
				case Keyword.SCOPE:
				{
					candidate.AddTokenOfKeywords( tokens.Dequeue( ) , new List<Keyword> { Keyword.OPEN , Keyword.CLOSE } );
					candidate.Type = (Keyword)Enum.Parse( typeof( Keyword ) , candidate.tokens[1].Value ) == Keyword.OPEN
						? StatementType.SCOPE_OPEN : StatementType.SCOPE_CLOSE;
					break;
				}

				// EXIT
				case Keyword.EXIT:
				{
					candidate.Type = StatementType.EXIT;
					break;
				}

				// ERROR var|string
				case Keyword.ERROR:
				{
					candidate.AddTokenOfTypes( tokens.Dequeue( ) , new List<TokenType> { TokenType.VARIABLE , TokenType.STRING } , operand: 2 );
					candidate.Type = StatementType.ERROR;
					break;
				}

				default:
					throw new CompiletimeException( "Invalid start of statement: " + startToken.ToString( ) );

			}

			return tokens.Count <= 0 || tokens.Peek( ).IdentifyType( ) is TokenType.COMMENT ? candidate : throw new CompiletimeException( "Line did not end with statement" );
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
			this.tokens.Add( token );
			if ( operand == 1 )
			{
				this.Operand1 = token;
			}
			else if ( operand == 2 )
			{
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
			this.tokens.Add( token );
		}

		/// <summary>
		/// Add a Token if it is a specified Symbol, else throw an exception.
		/// </summary>
		/// <param name="token">Token to add</param>
		/// <param name="symbols">List of valid symbols</param>
		public void AddTokenOfSymbols( Token token , List<Symbol> symbols )
		{
			token.ThrowIfNotSymbols( symbols );
			this.tokens.Add( token );
		}

		/// <summary>
		/// Stringify the Statement with type information.
		/// </summary>
		/// <returns>String description of this Statement</returns>
		public override string ToString( )
		{
			string str = "[" + this.Type + "] \t";

			str += $"op1={this.Operand1,-24} op2={this.Operand2,-24}";

			return str;
		}

		/// <summary>
		/// Gives the raw string of the line that created this Statement.
		/// </summary>
		/// <returns>String of the raw input that made this Statement.</returns>
		public string RawString( )
		{
			string str = "";
			for ( int i = 0 ; i < this.tokens.Count ; i += 1 )
			{
				str += this.tokens[i].Value + " ";
			}
			return str;
		}

		/// <summary>
		/// Get the primary and secondary tokens of a statement.
		/// This is determined by the statement sematics, not syntactic order.
		/// </summary>
		/// <returns>A tuple with the primary and secondary tokens in order.</returns>
		public (Token, Token) GetPrimaryAndSecondaryTokens( )
		{
			switch ( this.Type )
			{
				case StatementType.NULL:
				case StatementType.COMMENT:
				case StatementType.LABEL:
				case StatementType.SCOPE_OPEN:
				case StatementType.SCOPE_CLOSE:
				case StatementType.EXIT:
					return (default, default);

				case StatementType.ADD:
				case StatementType.SUBTRACT:
				case StatementType.MODULUS:
				case StatementType.LOGARITHM:
				case StatementType.APPEND:
				case StatementType.OPEN_READ:
				case StatementType.OPEN_WRITE:
				case StatementType.READ:
				case StatementType.WRITE:
					return (this.Operand2, this.Operand1);

				case StatementType.SET:
				case StatementType.MULTIPLY:
				case StatementType.DIVIDE:
				case StatementType.RANDOM:
				case StatementType.POWER:
				case StatementType.LIST:
				case StatementType.SIZE:
				case StatementType.TEST_GTR:
				case StatementType.TEST_LSS:
				case StatementType.TEST_EQU:
				case StatementType.TEST_NEQ:
				case StatementType.TYPE:
					return (this.Operand1, this.Operand2);

				case StatementType.ROUND:
				case StatementType.CLOSE:
				case StatementType.INPUT:
				case StatementType.PRINT:
					return (this.Operand1, default);

				case StatementType.WAIT:
				case StatementType.GOTO:
				case StatementType.ERROR:
					return (this.Operand2, default);

				case StatementType.BITWISE_OR:
				case StatementType.BITWISE_AND:
				case StatementType.BITWISE_XOR:
				case StatementType.BITWISE_NEGATE:
					throw new NotImplementedException( );

				case StatementType.UNKNOWN:
				default:
					throw new Exception( $"Unexpected statement type in token sorting: {this.Type}" );
			}
		}
	}
}
