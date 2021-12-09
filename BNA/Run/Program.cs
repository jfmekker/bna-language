using System;
using System.Collections.Generic;
using BNA.Common;
using BNA.Compile;
using BNA.Exceptions;
using BNA.Values;

namespace BNA.Run
{
	/// <summary>
	/// Object class representing a BNA program.
	/// </summary>
	public partial class Program
	{
		/// <summary>
		/// The statements that make up what the program does.
		/// </summary>
		private Statement[] Statements { get; set; }

		/// <summary>
		/// Running list of variables held by the program while running.
		/// Starts with label values and special variable default values.
		/// </summary>
		private Dictionary<Token , Value> Variables
		{
			get => ( this.Scopes is null || this.Scopes.Count <= 0 ) ? throw new Exception( "No scopes found." ) : this.Scopes.Peek( );

			set
			{
				this.Scopes.Push( value );
				this.CompileLabels( );
				this.SetValue( SpecialVariables.TEST_RESULT , SpecialVariables.TEST_RESULT_DEFAULT , true );
				this.SetValue( SpecialVariables.ARGUMENT , SpecialVariables.ARGUMENT_DEFAULT , true );
				this.SetValue( SpecialVariables.RETURN , SpecialVariables.RETURN_DEFAULT , true );
				this.SetValue( SpecialVariables.NULL , SpecialVariables.NULL_DEFAULT , true );
			}
		}

		/// <summary>
		/// Stack of scopes, collections of variables.
		/// </summary>
		private Stack<Dictionary<Token , Value>> Scopes { get; set; }

		/// <summary>
		/// Shortcut to the statement pointed to by <see cref="IP"/>.
		/// </summary>
		public Statement Current
		{
			get
			{
				if ( this.IP < 0 || this.IP >= this.Statements.Length )
				{
					throw new Exception( "Bad instruction pointer value ( " + this.IP + " )" );
				}
				else if ( this.Statements[this.IP] is null )
				{
					throw new Exception( "No statement at instruction pointer ( " + this.IP + " )" );
				}
				return this.Statements[this.IP];
			}
		}

		/// <summary>
		/// Instruction pointer; what statement is the program currently on.
		/// </summary>
		public int IP { get; set; }

		/// <summary>
		/// Whether the program is currently running.
		/// </summary>
		public bool Running
		{
			get; set;
		}

		/// <summary>
		/// Create a new <see cref="Program"/> instance.
		/// </summary>
		/// <param name="statements">Statements that make up the program</param>
		public Program( Statement[] statements )
		{
			this.Statements = statements;
			this.IP = 0;

			this.Scopes = new Stack<Dictionary<Token , Value>>( );
			this.Variables = new Dictionary<Token , Value>( );
		}

		/// <summary>
		/// Run a program statement by statement.
		/// </summary>
		public void Run( )
		{
			this.Running = true;
			while ( this.Running )
			{
				try
				{
					Instruction instruction = new( this.Current , this );

					instruction.Execute( );
				}
				catch ( Exception e )
				{
					if ( e is UndefinedOperationException
						   or IncorrectOperandTypeException
						   or InvalidIndexValueException
						   or NonIndexableValueException
						   or NonExistantVariableException
						   or ErrorStatementException )
					{
						throw new RuntimeException( this.IP , this.Current , e );
					}
					else
					{
						throw;
					}
				}

				if ( this.Running && ++this.IP >= this.Statements.Length )
				{
					this.Running = false;
				}
			}

			CloseAllFiles( new List<Value>( this.Variables.Values ) );
		}

		/// <summary>
		/// Get the value of an operand based on a Token.
		/// </summary>
		/// <param name="token">Token to identify/convert to a value</param>
		/// <returns>Value of the operand</returns>
		public Value GetValue( Token token )
		{
			switch ( token.Type )
			{
				// Parse the literal value from the token
				case TokenType.LITERAL:
				{
					if ( long.TryParse( token.Value , out long lval ) )
					{
						return new IntegerValue( lval );
					}
					else if ( double.TryParse( token.Value , out double dval ) )
					{
						return new FloatValue( dval );
					}

					// Should have caught this in compiler?
					throw new Exception( $"Could not parse value from literal: {token}" );
					// throw new RuntimeException( this.IP , this.Statements[this.IP] , $"Could not parse value from literal: {token}" );
				}

				// Capture the string contents
				case TokenType.STRING:
				{
					return token.Value.Length >= 2 ? new StringValue( token.Value[1..^1] ) : throw new Exception( $"String token too short to be valid: {token}" );
				}

				// Get the Value of a variable
				case TokenType.VARIABLE:
				{
					// Get list element
					if ( token.Value.Contains( "" + (char)Symbol.ACCESSOR ) )
					{
						// Get last accessor first (so multi-lists are properly chained)
						int accessor = token.Value.LastIndexOf( (char)Symbol.ACCESSOR );
						if ( accessor == 0 || accessor == token.Value.Length )
						{
							// Should have been detected at compiletime
							throw new Exception( $"Accessor at start or end of token: {token}" );
						}

						// Get value of index part
						Token indexTok = Lexer.ReadSingleToken( token.Value[( accessor + 1 )..] );
						Value indexVal = this.GetValue( indexTok );
						if ( indexVal is not IntegerValue index )
						{
							throw new InvalidIndexValueException( indexTok , indexVal );
						}

						// Get the value
						Token accessedTok = Lexer.ReadSingleToken( token.Value.Substring( 0 , accessor ) );
						Value accessedVal = this.GetValue( accessedTok );
						return accessedVal is ListValue listVal
								? index.Get >= 0 && index.Get < listVal.Get.Count ? listVal.Get[index.Get]
								: throw new ValueOutOfRangeException( indexVal , $"List {listVal}" )
							: accessedVal is StringValue strVal
								? index.Get >= 0 && index.Get < strVal.Get.Length ? new StringValue( "" + strVal.Get[index.Get] )
								: throw new ValueOutOfRangeException( indexVal , $"String \"{strVal}\"" )
							: throw new NonIndexableValueException( accessedTok , accessedVal );
					}

					// Find variable
					if ( this.Variables.TryGetValue( token , out Value? v ) )
					{
						return v;
					}

					// No value yet
					//throw new NonExistantVariableException( token );
					return Value.NULL;
				}

				// Tokenize and evaluate the contents of a list literal
				case TokenType.LIST:
				{
					Lexer lexer = new( token.Value[1..^1] );
					List<Token> listTokens = lexer.ReadTokens( );
					List<Value> listValues = new( );

					foreach ( Token t in listTokens )
					{
						if ( t.AsSymbol() is not Symbol.LIST_SEPERATOR  )
						{
							listValues.Add( this.GetValue( t ) );
						}
					}

					return new ListValue( listValues );
				}

				// Null operands for single operand statements
				case TokenType.NULL:
				{
					return Value.NULL;
				}

				default:
					throw new Exception( "Unexpected token type in GetValue: " + token.ToString( ) );
			}
		}

		/// <summary>
		/// Set the value of a variable based on a token identifier.
		/// </summary>
		/// <param name="token">Token to locate the value to change</param>
		/// <param name="newValue">New value to insert or change</param>
		/// <param name="add">Whether to add the variable if it does not exist</param>
		public void SetValue( Token token , Value newValue , bool add = false )
		{
			// Unexpected token type
			if ( token.Type != TokenType.VARIABLE )
			{
				throw new Exception( "Unexpected token type in SetValue: " + token );
			}

			// Handle list element
			if ( token.Value.Contains( "" + (char)Symbol.ACCESSOR ) )
			{
				// Get last accessor first (so multi-lists are properly chained)
				int accessor = token.Value.LastIndexOf( (char)Symbol.ACCESSOR );
				if ( accessor == 0 || accessor == token.Value.Length )
				{
					throw new Exception( $"Accessor at start or end of token: {token}" );
				}

				// Get value of index part
				Token indexTok = Lexer.ReadSingleToken( token.Value[( accessor + 1 )..] );
				Value indexVal = this.GetValue( indexTok );
				if ( indexVal is not IntegerValue index )
				{
					throw new InvalidIndexValueException( indexTok , indexVal );
				}

				// Set the value
				Token accessedTok = Lexer.ReadSingleToken( token.Value.Substring( 0 , accessor ) );
				Value accessedVal = this.GetValue( accessedTok );
				if ( accessedVal is ListValue listVal )
				{
					if ( index.Get < 0 || index.Get >= listVal.Get.Count )
					{
						throw new ValueOutOfRangeException( indexVal , $"List {listVal}" );
					}

					listVal.Get[index.Get] = newValue;
					return;
				}
				else if ( accessedVal is StringValue )
				{
					throw new NotImplementedException( );
				}
				else
				{
					throw new NonIndexableValueException( accessedTok , accessedVal );
				}
			}

			// Set or add value
			if ( this.Variables.ContainsKey( token ) )
			{
				this.Variables[token] = newValue;
			}
			else if ( add )
			{
				this.Variables.Add( token , newValue );
			}
			else
			{
				throw new NonExistantVariableException( token );
			}
		}

		/// <summary>
		/// Open a new scope of variables while passing in <see cref="SpecialVariables.ARGUMENT"/>.
		/// </summary>
		public void OpenScope( )
		{
			Value argument_val = this.GetValue( SpecialVariables.ARGUMENT );
			this.Variables = new Dictionary<Token , Value>( );
			this.SetValue( SpecialVariables.ARGUMENT , argument_val );
		}

		/// <summary>
		/// Close the current scope of variables while passing out <see cref="SpecialVariables.RETURN"/>.
		/// </summary>
		public void CloseScope( )
		{
			Value return_val = this.GetValue( SpecialVariables.RETURN );
			if ( this.Scopes.Count < 2 )
			{
				throw new CloseFinalScopeException( );
			}
			_ = this.Scopes.Pop( );
			this.SetValue( SpecialVariables.RETURN , return_val );
		}

		/// <summary>
		/// Close all file-type values in a list
		/// </summary>
		/// <param name="values">Values to check and close</param>
		private static void CloseAllFiles( List<Value> values )
		{
			foreach ( Value v in values )
			{
				if ( v is FileValue file )
				{
					file.Close( );
				}
				else if ( v is ListValue list )
				{
					CloseAllFiles( list.Get );
				}
			}
		}

		/// <summary>
		/// Read through all statements and set labels equal to their line number.
		/// </summary>
		private void CompileLabels( )
		{
			for ( int i = 0 ; i < this.Statements.Length ; i += 1 )
			{
				if ( this.Statements[i].Type == Operation.LABEL )
				{
					this.SetValue( this.Statements[i].Operand1 , new IntegerValue( i ) , true );
				}
			}
		}
	}
}
