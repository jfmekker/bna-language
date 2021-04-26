using System;
using System.Collections.Generic;

namespace BNA
{

	/// <summary>
	/// Main program of the BNA compiler. Compiles files or terminal input.
	/// </summary>
	public class Program
	{
		/// <summary>
		/// The statements that make up what the program does.
		/// </summary>
		private Statement[] Statements;

		/// <summary>
		/// Instruction pointer; what statement is the program currently on.
		/// </summary>
		private int IP;

		/// <summary>
		/// Running list of variables held by the program while running, starts empty.
		/// </summary>
		private Dictionary<Token , Value> Variables;

		/// <summary>
		/// Create a new <see cref="Program"/> instance.
		/// </summary>
		/// <param name="statements">Statements that make up the program</param>
		public Program( Statement[] statements )
		{
			this.Statements = statements;
			this.IP = 0;
			this.Variables = new Dictionary<Token , Value>( );
		}

		/// <summary>
		/// Run a program statement by statement.
		/// </summary>
		public void Run( )
		{
			if ( this.Statements == null ) {
				throw new Exception( "Program told to run but Statements is null" );
			}

			bool running = true;
			while ( running ) {
				// Instruction pointer
				if ( this.IP < 0 || this.IP >= this.Statements.Length ) {
					throw new Exception( "Bad instruction pointer value ( " + this.IP + " )" );
				}

				// Get current statement
				Statement curr = this.Statements[this.IP];
				if ( curr == null ) {
					throw new Exception( "No statement at instruction pointer ( " + this.IP + " )" );
				}

				Value op1 = this.GetValue( curr.Operand1 );
				Value op2 = this.GetValue( curr.Operand2 );

				// Execute statement
				switch ( curr.Type ) {
					// Set operation
					case StatementType.OP_SET: {
						if ( op2.Type == ValueType.LIST ) {
							var l = Value.DeepCopy( op2 );
							this.SetValue( curr.Operand1 , l , true );
						}
						else {
							this.SetValue( curr.Operand1 , op2 , true );
						}
						break;
					}


					// Numeric two-operand operations
					case StatementType.OP_ADD:
					case StatementType.OP_SUB:
					case StatementType.OP_MUL:
					case StatementType.OP_DIV:
					case StatementType.OP_LOG:
					case StatementType.OP_POW: {
						if ( op1 == Value.NULL ) {
							throw new RuntimeException( this.IP , curr , "Cannot use variable that has not been set: " + curr.Operand1.ToString( ) );
						}

						if ( op1.Type != ValueType.INTEGER && op1.Type != ValueType.FLOAT ) {
							throw new RuntimeException( this.IP , curr , "Operand 1 of incorrect type (" + op1.Type.ToString( ) + ") for numeric operation" );
						}

						if ( op2.Type != ValueType.INTEGER && op2.Type != ValueType.FLOAT ) {
							throw new RuntimeException( this.IP , curr , "Operand 2 of incorrect type (" + op2.Type.ToString( ) + ") for numeric operation" );
						}

						var newValue = Value.DoNumericOperation( op1 , op2 , curr.Type );
						this.SetValue( curr.Operand1 , newValue );
						break;
					}


					// Bitwise operations
					case StatementType.OP_MOD:
					case StatementType.OP_AND:
					case StatementType.OP_OR:
					case StatementType.OP_XOR: {
						if ( op1.Type != ValueType.INTEGER ) {
							throw new RuntimeException( this.IP , curr , "Operand 1 of incorrect type (" + op1.Type.ToString( ) + ") for bitwise operation" );
						}

						if ( op2.Type != ValueType.INTEGER ) {
							throw new RuntimeException( this.IP , curr , "Operand 2 of incorrect type (" + op2.Type.ToString( ) + ") for bitwise operation" );
						}

						var newValue = Value.DoBitwiseOperation( op1 , op2 , curr.Type );
						this.SetValue( curr.Operand1 , newValue );
						break;
					}


					// Numeric one-operand operations
					case StatementType.OP_RAND: {
						if ( op2.Type == ValueType.INTEGER ) {
							var newValue = new Value( ValueType.INTEGER , BNA.RNG.Next( (int)(long)op2.Val ) );
							this.SetValue( curr.Operand1 , newValue , true );
						}
						else if ( op2.Type == ValueType.FLOAT ) {
							var newValue = new Value( ValueType.INTEGER , BNA.RNG.NextDouble( ) * (double)op2.Val );
							this.SetValue( curr.Operand1 , newValue , true );
						}
						else {
							throw new RuntimeException( this.IP , curr , "Operand 2 of incorrect type (" + op2.Type.ToString( ) + ") for numeric operation" );
						}
						break;
					}

					case StatementType.OP_NEG: {
						if ( op1.Type == ValueType.INTEGER ) {
							var newValue = new Value( ValueType.INTEGER , ( (ulong)(long)op1.Val ) ^ ulong.MaxValue );
							this.SetValue( curr.Operand1 , newValue );
						}
						else {
							throw new RuntimeException( this.IP , curr , "Operand 2 of incorrect type (" + op2.Type.ToString( ) + ") for numeric operation" );
						}
						break;
					}

					case StatementType.OP_ROUND: {
						if ( op1.Type == ValueType.FLOAT ) {
							var newValue = new Value( ValueType.INTEGER , (long)Math.Round( (double)op1.Val ) );
							this.SetValue( curr.Operand1 , newValue );
						}
						else if ( op1.Type != ValueType.INTEGER ) {
							throw new RuntimeException( this.IP , curr , "Operand 2 of incorrect type (" + op2.Type.ToString( ) + ") for numeric operation" );
						}
						break;
					}


					// List operations
					case StatementType.OP_LIST: {
						// Check operands
						if ( op2.Type != ValueType.INTEGER ) {
							throw new RuntimeException( this.IP , curr , "List size must be integer" );
						}

						// Fill list
						var list = new List<Value>( );
						for ( int i = 0 ; i < (long)op2.Val ; i += 1 ) {
							list.Add( new Value( ) );
						}

						this.SetValue( curr.Operand1 , new Value( ValueType.LIST , list ) , true );
						break;
					}

					case StatementType.OP_APPEND: {
						if ( op1.Type == ValueType.LIST ) {
							if ( op2.Type == ValueType.LIST ) {
								var l = Value.DeepCopy( op2 );
								( (List<Value>)op1.Val ).Add( l );
							}
							else {
								( (List<Value>)op1.Val ).Add( op2 );
							}
						}
						else if ( op1.Type == ValueType.STRING ) {
							// TODO ?
							( (string)op1.Val ).Insert( ( (string)op1.Val ).Length , op2.Val.ToString( ) );
						}
						else {
							throw new RuntimeException( this.IP , this.Statements[this.IP] , "Can not append to non-list-like type: " + op1.Type );
						}
						break;
					}

					case StatementType.OP_SIZE: {
						if ( op2.Type == ValueType.LIST ) {
							this.SetValue( curr.Operand1 , new Value( ValueType.INTEGER , (long)( (List<Value>)op2.Val ).Count ) , true );
						}
						else if ( op2.Type == ValueType.STRING ) {
							this.SetValue( curr.Operand1 , new Value( ValueType.INTEGER , (long)( (string)op2.Val ).Length ) , true );
						}
						else {
							throw new RuntimeException( this.IP , curr , "Can not size a non-list-like value: '" + op2.ToString( ) + "'" );
						}
						break;
					}


					// I/O operations
					case StatementType.OP_PRINT: {
						Console.WriteLine( op2.ToString( ) );
						break;
					}

					case StatementType.OP_INPUT: {
						Console.Write( op2.ToString( ) );
						var token = new Token( Console.ReadLine( ) );
						switch ( token.Type ) {
							case TokenType.LITERAL:
								if ( long.TryParse( token.Value , out long lval ) ) {
									this.SetValue( curr.Operand1 , new Value( ValueType.INTEGER , lval ) , true );
								}
								else if ( double.TryParse( token.Value , out double dval ) ) {
									this.SetValue( curr.Operand1 , new Value( ValueType.FLOAT , dval ) , true );
								}
								else {
									throw new Exception( "Tokenizer identified literal that could not be parsed: " + token.ToString( ) );
								}
								break;

							case TokenType.STRING:
								if ( token.Value.Length < 2 ) {
									throw new Exception( "Tokenizer identified string that is too short: " + token.ToString( ) );
								}

								this.SetValue( curr.Operand1 , new Value( ValueType.STRING , token.Value.Substring( 1 , token.Value.Length - 2 ) ) , true );
								break;

							default:
								throw new RuntimeException( this.IP , curr , "Invalid input: " + token.ToString( ) );
						}
						break;
					}

					case StatementType.OP_OPEN_R:
					case StatementType.OP_OPEN_W:
					case StatementType.OP_WRITE:
					case StatementType.OP_READ:
					case StatementType.OP_CLOSE:
						throw new NotImplementedException( );


					// Test operations
					case StatementType.OP_TEST_EQ:
					case StatementType.OP_TEST_NE:
					case StatementType.OP_TEST_GT:
					case StatementType.OP_TEST_LT: {
						if ( op1.Type == ValueType.INVALID || op1.Type == ValueType.NULL
							|| op2.Type == ValueType.INVALID || op2.Type == ValueType.NULL ) {
							throw new RuntimeException( this.IP , curr , "Both operands must have valid values to compare" );
						}

						var result = Value.DoComparisonOperation( op1 , op2 , curr.Type );
						if ( result.Equals( Value.NAN ) ) {
							throw new RuntimeException( this.IP , curr , "Could not compare operands: "
								+ "op1=" + curr.Operand1.ToString( ) + " op2=" + curr.Operand2.ToString( ) );
						}

						this.SetValue( SpecialVariables.TEST_RESULT , result , true );
						break;
					}


					// Misc operaions
					case StatementType.OP_WAIT: {
						op2 = this.GetValue( curr.Operand2 );
						if ( op2.Type != ValueType.INTEGER && op2.Type != ValueType.FLOAT ) {
							throw new RuntimeException( this.IP , curr , "Argument to WAIT must be numeric: " + op2.ToString( ) );
						}

						int ms = (int)( 1000 * ( op2.Type == ValueType.INTEGER ? (long)op2.Val : (double)op2.Val ) );
						try {
							System.Threading.Thread.Sleep( ms );
						}
						catch ( Exception e ) {
							throw new RuntimeException( this.IP , curr , "Exception caught while waiting:\n" + e.Message );
						}
						break;
					}

					case StatementType.OP_GOTO: {
						// Find line
						Token label = curr.Operand1;
						int line = -1;
						for ( int i = 0 ; i < this.Statements.Length ; i += 1 ) {
							if ( this.Statements[i].Type == StatementType.LABEL ) {
								if ( this.Statements[i].Operand1.Equals( label ) ) {
									line = i;
									break;
								}
							}
						}
						if ( line < 0 ) {
							throw new RuntimeException( this.IP , curr , "Found no label with token " + label.ToString( ) );
						}

						// Test condition
						if ( op2 != Value.FALSE ) {
							this.IP = line;
						}

						break;
					}

					case StatementType.OP_TYPE: {
						this.SetValue( curr.Operand1 , new Value( ValueType.STRING , op2.Type.ToString( ) ) , true );
						break;
					}

					case StatementType.OP_EXIT: {
						running = false;
						break;
					}

					case StatementType.OP_ERROR: {
						throw new RuntimeException( op2.ToString( ) );
					}

					// Non-operations
					case StatementType.NULL:
					case StatementType.COMMENT:
					case StatementType.LABEL: {
						// Do nothing
						break;
					}


					default:
						throw new RuntimeException( this.IP , curr , "Unexpected statement type ( " + curr.Type.ToString( ) + " )" );
				}

				// Next statement or end
				if ( running && ++this.IP == this.Statements.Length ) {
					running = false;
				}
			}
		}

		/// <summary>
		/// Get the value of an operand based on a Token.
		/// </summary>
		/// <param name="token">Token to identify/convert to a value</param>
		/// <returns>Value of the operand</returns>
		private Value GetValue( Token token )
		{
			switch ( token.Type ) {
				// Parse the literal value from the token
				case TokenType.LITERAL: {
					if ( long.TryParse( token.Value , out long lval ) ) {
						return new Value( ValueType.INTEGER , lval );
					}
					else if ( double.TryParse( token.Value , out double dval ) ) {
						return new Value( ValueType.FLOAT , dval );
					}
					else {
						throw new RuntimeException( this.IP , this.Statements[this.IP] , "Could not parse value from literal: " + token );
					}
				}

				// Capture the string contents
				case TokenType.STRING: {
					if ( token.Value.Length < 2 ) {
						throw new RuntimeException( this.IP , this.Statements[this.IP] , "String token too short to be valid: " + token );
					}

					return new Value( ValueType.STRING , token.Value.Substring( 1 , token.Value.Length - 2 ) );
				}

				// Get the Value of a variable
				case TokenType.VARIABLE: {
					// Get list element
					if ( token.Value.Contains( "" + (char)Symbol.ACCESSOR ) ) {
						// Get last accessor first (so multi-lists are properly chained)
						int accessor = token.Value.LastIndexOf( (char)Symbol.ACCESSOR );
						if ( accessor == 0 || accessor == token.Value.Length ) {
							throw new RuntimeException( this.IP , this.Statements[this.IP] , "Accessor at start or end of token: " + token );
						}

						// Get value of list part
						string listPart = token.Value.Substring( 0 , accessor );
						Value list = this.GetValue( new Token( listPart ) );
						if ( list.Type != ValueType.LIST && list.Type != ValueType.STRING ) {
							throw new RuntimeException( this.IP , this.Statements[this.IP] , "Accessed variable not a list or string: " + token );
						}

						// Get value of index part
						string indxPart = token.Value.Substring( accessor + 1 );
						Value index = this.GetValue( new Token( indxPart ) );
						if ( index.Type != ValueType.INTEGER ) {
							throw new RuntimeException( this.IP , this.Statements[this.IP] , "Index is not an integer: " + token );
						}
						int i = (int)(long)index.Val;

						// Get the value
						if ( list.Type == ValueType.LIST ) {
							var l = (List<Value>)list.Val;

							if ( i < 0 || i >= l.Count ) {
								throw new RuntimeException( this.IP , this.Statements[this.IP] , "Invalid index (" + i + ") for list of size " + l.Count );
							}

							return l[i];
						}
						else {
							string s = (string)list.Val;
							return new Value( ValueType.STRING , "" + s[i] );
						}
					}

					// Find variable
					if ( this.Variables.TryGetValue( token , out Value v ) ) {
						return v;
					}

					// No value yet
					return Value.NULL;
				}

				// Tokenize and evaluate the contents of a list literal
				case TokenType.LIST: {
					List<Token> listTokens = Token.TokenizeLine( token.Value.Substring( 1 , token.Value.Length - 2 ) );
					var listValues = new List<Value>( );

					foreach ( Token t in listTokens ) {
						if ( t.Type != TokenType.SYMBOL || t.Value[0] != (char)Symbol.LIST_SEPERATOR ) {
							listValues.Add( this.GetValue( t ) );
						}
					}

					return new Value( ValueType.LIST , listValues );
				}

				// Null operands for single operand statements
				case TokenType.NULL: {
					return Value.NULL;
				}

				// Invalid token
				case TokenType.INVALID:
				case TokenType.UNKNOWN: {
					throw new RuntimeException( this.IP , this.Statements[this.IP] , "Can not get value from invalid or unknown token: " + token.ToString( ) );
				}

				default:
					throw new RuntimeException( this.IP , this.Statements[this.IP] , "Unexpected token type for operand: " + token.ToString( ) );
			}
		}

		/// <summary>
		/// Set the value of a variable based on a token identifier.
		/// </summary>
		/// <param name="token">Token to locate the value to change</param>
		/// <param name="newValue">New value to insert or change</param>
		/// <param name="add">Whether to add the variable if it does not exist</param>
		private void SetValue( Token token , Value newValue , bool add = false )
		{
			// Unexpected token type
			if ( token.Type != TokenType.VARIABLE ) {
				throw new Exception( "Unexpected token type in SetValue: " + token );
			}

			// Handle list element
			if ( token.Value.Contains( "" + (char)Symbol.ACCESSOR ) ) {
				// Get last accessor first (so multi-lists are properly chained)
				int accessor = token.Value.LastIndexOf( (char)Symbol.ACCESSOR );
				if ( accessor == 0 || accessor == token.Value.Length ) {
					throw new RuntimeException( this.IP , this.Statements[this.IP] , "Accessor at start or end of token: " + token );
				}

				// Get value of list part
				string listPart = token.Value.Substring( 0 , accessor );
				Value list = this.GetValue( new Token( listPart ) );
				if ( list.Type != ValueType.LIST && list.Type != ValueType.STRING ) {
					throw new RuntimeException( this.IP , this.Statements[this.IP] , "Accessed variable not a list or string: " + token );
				}

				// Get value of index part
				string indxPart = token.Value.Substring( accessor + 1 );
				Value index = this.GetValue( new Token( indxPart ) );
				if ( index.Type != ValueType.INTEGER ) {
					throw new RuntimeException( this.IP , this.Statements[this.IP] , "Index is not an integer: " + token );
				}
				int i = (int)(long)index.Val;

				// Set the value
				if ( list.Type == ValueType.LIST ) {
					var l = (List<Value>)list.Val;

					if ( i < 0 || i >= l.Count ) {
						throw new RuntimeException( this.IP , this.Statements[this.IP] , "Invalid index for list: " + i );
					}

					l[i] = newValue;
					return;
				}
				else {
					throw new RuntimeException( this.IP , this.Statements[this.IP] , "Can not set specific index of string." );
				}
			}

			// Set or add value
			if ( this.Variables.ContainsKey( token ) ) {
				this.Variables[token] = newValue;
			}
			else if ( add ) {
				this.Variables.Add( token , newValue );
			}
			else {
				throw new RuntimeException( this.IP , this.Statements[this.IP] , "Could not find variable to set." );
			}
		}
	}
}
