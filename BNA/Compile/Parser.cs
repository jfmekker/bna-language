using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BNA.Common;
using BNA.Exceptions;

namespace BNA.Compile
{
	public class Parser
	{
		public string RawLine { get; init; }

		public IReadOnlyList<Token> Tokens { get; init; }

		public int Index { get; private set; }

		public Token? Current => this.Index < this.Tokens.Count( ) ? this.Tokens[this.Index] : null;

		private Operation operation;

		private Token? operand1;

		private Token? operand2;

		private Statement? Statement { get; set; }

		public Parser( string line , ICollection<Token> tokens )
		{
			this.RawLine = line;
			this.Tokens = tokens.ToList( );
			this.Index = 0;
		}

		public Statement ParseStatement( )
		{
			if ( this.Current?.AsKeyword( ) is Keyword keyword )
			{
				this.Index += 1;
				this.ParseKeywordStatement( keyword );
			}
			else if ( this.Current?.AsSymbol( ) is Symbol symbol )
			{
				this.Index += 1;
				this.ParseSymbolStatement( symbol );
			}
			else
			{
				this.End( );
			}

			return new Statement( this.operation , this.operand1 , this.operand2 );
		}

		private void ParseKeywordStatement( Keyword start )
		{
			switch ( start )
			{
				case Keyword.SET:
				{ this.SetOperation( Operation.SET ).Operand1( ).Next( Keyword.TO ).Operand2( AllOperandTypes( ) ).End( ); break; }

				case Keyword.ADD:
				{ this.SetOperation( Operation.ADD ).Operand1( ).Next( Keyword.TO ).Operand2( NumericOperandTypes( ) ).End( ); break; }

				case Keyword.SUBTRACT:
				{ this.SetOperation( Operation.SUBTRACT ).Operand2( NumericOperandTypes( ) ).Next( Keyword.FROM ).Operand1( ).End( ); break; }

				case Keyword.MULTIPLY:
				{ this.SetOperation( Operation.MULTIPLY ).Operand1( ).Next( Keyword.BY ).Operand2( NumericOperandTypes( ) ).End( ); break; }

				case Keyword.DIVIDE:
				{ this.SetOperation( Operation.DIVIDE ).Operand1( ).Next( Keyword.BY ).Operand2( NumericOperandTypes( ) ).End( ); break; }

				case Keyword.MOD:
				{ this.SetOperation( Operation.MODULUS ).Operand2( NumericOperandTypes( ) ).Next( Keyword.OF ).Operand1( ).End( ); break; }

				case Keyword.LOG:
				{ this.SetOperation( Operation.LOGARITHM ).Operand2( NumericOperandTypes( ) ).Next( Keyword.OF ).Operand1( ).End( ); break; }

				case Keyword.RAISE:
				{ this.SetOperation( Operation.POWER ).Operand1( ).Next( Keyword.TO ).Operand2( NumericOperandTypes( ) ).End( ); break; }

				case Keyword.ROUND:
				{ this.SetOperation( Operation.ROUND ).Operand1( ).End( ); break; }

				case Keyword.RANDOM:
				{ this.SetOperation( Operation.RANDOM ).Operand1( ).Next( Keyword.MAX ).Operand2( NumericOperandTypes( ) ).End( ); break; }

				case Keyword.WAIT:
				{ this.SetOperation( Operation.WAIT ).Operand2( NumericOperandTypes( ) ).End( ); break; }

				case Keyword.TEST:
				{ throw new NotImplementedException( ); }

				case Keyword.GOTO:
				{ this.SetOperation( Operation.GOTO ).Operand1( ).Optional( Keyword.IF )?.Operand2( NumericOperandTypes( ) ).End( ); break; }

				case Keyword.LIST:
				{ this.SetOperation( Operation.LIST ).Operand1( ).Optional( Keyword.SIZE )?.Operand2( NumericOperandTypes( ) ).End( ); break; }

				case Keyword.APPEND:
				{ this.SetOperation( Operation.APPEND ).Operand2( AllOperandTypes( ) ).Next( Keyword.TO ).Operand1( ).End( ); break; }

				case Keyword.SIZE:
				{ this.SetOperation( Operation.SIZE ).Operand1( ).Next( Keyword.OF ).Operand2( AllOperandTypes( ) ).End( ); break; }

				case Keyword.OPEN:
				{ throw new NotImplementedException( ); }

				case Keyword.CLOSE:
				{ this.SetOperation( Operation.CLOSE ).Operand1( ).End( ); break; }

				case Keyword.WRITE:
				{ this.SetOperation( Operation.WRITE ).Operand2( AllOperandTypes( ) ).Next( Keyword.TO ).Operand1( ).End( ); break; }

				case Keyword.READ:
				{ this.SetOperation( Operation.READ ).Operand2( TokenType.VARIABLE ).Next( Keyword.FROM ).Operand1( ).End( ); break; }

				case Keyword.INPUT:
				{ this.SetOperation( Operation.INPUT ).Operand1( ).Next( Keyword.WITH ).Operand2( StringOperandTypes( ) ).End( ); break; }

				case Keyword.PRINT:
				{ this.SetOperation( Operation.PRINT ).Operand2( AllOperandTypes( ) ).End( ); break; }

				case Keyword.TYPE:
				{ this.SetOperation( Operation.TYPE ).Operand1( ).Next( Keyword.OF ).Operand2( AllOperandTypes( ) ).End( ); break; }

				case Keyword.SCOPE:
				{ throw new NotImplementedException( ); }

				case Keyword.EXIT:
				{ this.SetOperation( Operation.EXIT ).End( ); break; }

				case Keyword.ERROR:
				{ this.SetOperation( Operation.ERROR ).Operand2( StringOperandTypes( ) ).End( ); break; }

				default:
				{
					this.Index -= 1;
					throw new IllegalTokenException( $"Illegal symbol to start statement: {this.Current}." );
				}
			}
		}

		private void ParseSymbolStatement( Symbol start )
		{
			switch ( start )
			{
				case Symbol.LABEL_START:
				{ this.Operand1( ).Next( Symbol.LABEL_END ).End( ); break; }

				default:
					this.Index -= 1;
					throw new IllegalTokenException( $"Illegal symbol to start statement: {this.Current}." );
			}
		}

		// TODO guard against setting operation twice
		private Parser SetOperation( Operation operation )
		{
			this.operation = operation;
			return this;
		}

		private Parser Next( Keyword keyword )
		{
			if ( this.Current is Token token && token.AsKeyword( ) == keyword )
			{
				if ( token.AsKeyword( ) == keyword )
				{
					this.Index += 1;
					return this;
				}
				else
				{
					throw new IllegalTokenException( $"Expected keyword '{keyword}', got token {token}." );
				}
			}
			else
			{
				throw new MissingTokenException( keyword.ToString( ) );
			}
		}

		private Parser Next( Symbol symbol )
		{
			if ( this.Current is Token token && token.AsSymbol( ) == symbol )
			{
				if ( token.AsSymbol( ) == symbol )
				{
					this.Index += 1;
					return this;
				}
				else
				{
					throw new IllegalTokenException( $"Expected keyword '{symbol}', got token {token}." );
				}
			}
			else
			{
				throw new MissingTokenException( symbol.ToString( ) );
			}
		}

		private Parser Next( params TokenType[] types )
		{
			if ( this.Current is Token token )
			{
				foreach ( TokenType type in types )
				{
					if ( token.Type == type )
					{
						this.Index += 1;
						return this;
					}
				}

				throw new IllegalTokenException( $"Expected operand of type {types.PrintElements( )}, got token {token}." );
			}
			else
			{
				throw new MissingTokenException( types );
			}
		}

		// TODO guard against setting operand twice
		private Parser Operand2( params TokenType[] types )
		{
			if ( this.Current is Token token )
			{
				foreach ( TokenType type in types )
				{
					if ( token.Type == type )
					{
						this.operand2 = token;
						this.Index += 1;
						return this;
					}
				}

				throw new IllegalTokenException( $"Expected operand of type {types.PrintElements( )}, got token {token}." );
			}
			else
			{
				throw new MissingTokenException( types );
			}
		}

		private Parser Operand1( )
		{
			if ( this.Current is Token token )
			{
				if ( token.Type == TokenType.VARIABLE )
				{
					this.operand1 = token;
					this.Index += 1;
					return this;
				}

				throw new IllegalTokenException( $"Expected operand of type {TokenType.VARIABLE}, got token {token}." );
			}
			else
			{
				throw new MissingTokenException( TokenType.VARIABLE );
			}
		}

		// TODO allow optional to ignore comments
		private Parser? Optional( Keyword keyword )
		{
			try
			{
				return this.Next( keyword );
			}
			catch ( MissingTokenException )
			{
				return null;
			}
		}

		private Parser? Optional( Symbol symbol )
		{
			try
			{
				return this.Next( symbol );
			}
			catch ( MissingTokenException )
			{
				return null;
			}
		}

		private Parser? Optional( params TokenType[] types )
		{
			try
			{
				return this.Next( types );
			}
			catch ( MissingTokenException )
			{
				return null;
			}
		}

		private void End( bool allow_comment = true )
		{
			if ( this.Current is not null && ( !allow_comment || this.Current.Value.Type != TokenType.COMMENT || ++this.Index != this.Tokens.Count ) )
			{
				throw new IllegalTokenException( $"Statement ended with token still remaining: {this.Current}." );
			}
		}

		private static TokenType[] AllOperandTypes( )
		{
			return new TokenType[] { TokenType.VARIABLE , TokenType.LITERAL , TokenType.STRING , TokenType.LIST };
		}

		private static TokenType[] StringOperandTypes( )
		{
			return new TokenType[] { TokenType.VARIABLE , TokenType.STRING };
		}

		private static TokenType[] NumericOperandTypes( )
		{
			return new TokenType[] { TokenType.VARIABLE , TokenType.LITERAL };
		}
	}
}
