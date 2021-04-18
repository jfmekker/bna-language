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
		private List<Variable> Variables;

		/// <summary>
		/// Create a new <see cref="Program"/> instance.
		/// </summary>
		/// <param name="statements">Statements that make up the program</param>
		public Program( Statement[] statements )
		{
			this.Statements = statements;
			this.IP = 0;
			this.Variables = new List<Variable>( );
		}

		/// <summary>
		/// Run a program statement by statement.
		/// </summary>
		public void Run( )
		{
			if ( this.Statements == null ) {
				throw new RuntimeException( "Program told to run but Statements is null" );
			}

			bool running = true;
			while ( running ) {
				// Instruction pointer
				if ( this.IP < 0 || this.IP >= this.Statements.Length ) {
					throw new RuntimeException( "Bad instruction pointer value ( " + this.IP + " )" );
				}

				// Get current statement
				Statement curr = this.Statements[this.IP];
				if ( curr == null ) {
					throw new RuntimeException( "No statement at instruction pointer ( " + this.IP + " )" );
				}

				Variable op1 = this.GetVariable( curr.Operand1 );
				Value op2 = this.GetValue( curr.Operand2 );

				// Execute statement
				switch ( curr.Type ) {
					// Set operation
					case StatementType.OP_SET: {
						if ( op1 == null ) {
							op1 = new Variable( curr.Operand1 );
							this.Variables.Add( op1 );
						}
						op1.Value = op2;
						break;
					}


					// Numeric two-operand operations
					case StatementType.OP_ADD:
					case StatementType.OP_SUB:
					case StatementType.OP_MUL:
					case StatementType.OP_DIV:
					case StatementType.OP_LOG:
					case StatementType.OP_POW: {
						if ( op1 == null ) {
							throw new RuntimeException( this.IP , curr , "Cannot use variable that has not been set: " + curr.Operand1.ToString( ) );
						}

						if ( op1.Value.Type != ValueType.INTEGER && op1.Value.Type != ValueType.FLOAT ) {
							throw new RuntimeException( this.IP , curr , "Operand 1 of incorrect type (" + op1.Value.Type.ToString( ) + ") for numeric operation" );
						}

						if ( op2.Type != ValueType.INTEGER && op2.Type != ValueType.FLOAT ) {
							throw new RuntimeException( this.IP , curr , "Operand 2 of incorrect type (" + op2.Type.ToString( ) + ") for numeric operation" );
						}

						op1.Value = Value.DoNumericOperation( op1.Value , op2 , curr.Type );
						break;
					}


					// Bitwise operations
					case StatementType.OP_MOD:
					case StatementType.OP_AND:
					case StatementType.OP_OR:
					case StatementType.OP_XOR: {
						if ( op1.Value.Type != ValueType.INTEGER ) {
							throw new RuntimeException( this.IP , curr , "Operand 1 of incorrect type (" + op1.Value.Type.ToString( ) + ") for bitwise operation" );
						}
						else if ( op2.Type != ValueType.INTEGER ) {
							throw new RuntimeException( this.IP , curr , "Operand 2 of incorrect type (" + op2.Type.ToString( ) + ") for bitwise operation" );
						}
						op1.Value = Value.DoBitwiseOperation( op1.Value , op2 , curr.Type );
						break;
					}


					// Numeric one-operand operations
					case StatementType.OP_RAND: {
						if ( op2.Type == ValueType.INTEGER ) {
							if ( op1 == null ) {
								op1 = new Variable( curr.Operand1 );
								this.Variables.Add( op1 );
							}
							op1.Value = new Value( ValueType.INTEGER , BNA.RNG.Next( (int)(long)op2.Val ) );
						}
						else if ( op2.Type == ValueType.FLOAT ) {
							if ( op1 == null ) {
								op1 = new Variable( curr.Operand1 );
								this.Variables.Add( op1 );
							}
							op1.Value = new Value( ValueType.INTEGER , BNA.RNG.NextDouble( ) * (double)op2.Val );
						}
						else {
							throw new RuntimeException( this.IP , curr , "Operand 2 of incorrect type (" + op2.Type.ToString( ) + ") for numeric operation" );
						}
						break;
					}

					case StatementType.OP_NEG: {
						if ( op1.Value.Type == ValueType.INTEGER ) {
							op1.Value = new Value( ValueType.INTEGER , ( (ulong)(long)op1.Value.Val ) ^ ulong.MaxValue );
						}
						else {
							throw new RuntimeException( this.IP , curr , "Operand 2 of incorrect type (" + op2.Type.ToString( ) + ") for numeric operation" );
						}
						break;
					}

					case StatementType.OP_ROUND: {
						if ( op1.Value.Type == ValueType.FLOAT ) {
							op1.Value = new Value( ValueType.INTEGER , (long)Math.Round( (double)op1.Value.Val ) );
						}
						else if ( op1.Value.Type != ValueType.INTEGER ) {
							throw new RuntimeException( this.IP , curr , "Operand 2 of incorrect type (" + op2.Type.ToString( ) + ") for numeric operation" );
						}
						break;
					}


					// List operations
					case StatementType.OP_LIST:
					case StatementType.OP_APPEND:
						throw new NotImplementedException( );


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
									if ( op1.Value.Type != ValueType.INTEGER ) {
										throw new RuntimeException( this.IP , curr , "Tried to input integer value to variable of type " + op1.Value.Type.ToString( ) );
									}

									op1.Value = new Value( ValueType.INTEGER , lval );
								}
								else if ( double.TryParse( token.Value , out double dval ) ) {
									if ( op1.Value.Type != ValueType.FLOAT ) {
										throw new RuntimeException( this.IP , curr , "Tried to input float value to variable of type " + op1.Value.Type.ToString( ) );
									}

									op1.Value = new Value( ValueType.FLOAT , dval );
								}
								else {
									throw new Exception( "Tokenized identified literal that could not be parsed: " + token.ToString( ) );
								}
								break;

							case TokenType.STRING:
								if ( op1.Value.Type != ValueType.STRING ) {
									throw new RuntimeException( this.IP , curr , "Tried to input string value to variable of type " + op1.Value.Type.ToString( ) );
								}

								if ( token.Value.Length < 2 ) {
									throw new Exception( "Tokenized identified string that is too short: " + token.ToString( ) );
								}

								op1.Value = new Value( ValueType.STRING , token.Value.Substring( 1 , token.Value.Length - 2 ) );
								break;

							default:
								throw new RuntimeException( this.IP , curr , "Unexpected token input: " + token.ToString( ) );
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
					case StatementType.OP_TEST_GT:
					case StatementType.OP_TEST_LT: {
						Variable test = this.GetVariable( SpecialVariables.TEST_RESULT );
						if ( test == null ) {
							test = new Variable( SpecialVariables.TEST_RESULT );
							this.Variables.Add( test );
						}

						var v = Value.DoComparisonOperation( op1.Value , op2 , curr.Type );
						if ( v.Equals( Value.NAN ) ) {
							throw new RuntimeException( IP , curr , "Could not compare operands: "
								+ "op1=" + curr.Operand1.ToString( ) + " op2=" + curr.Operand2.ToString( ) );
						}

						test.Value = v;
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

					case StatementType.OP_GOTO:
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
						this.IP = line;
						break;


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
				if ( ++this.IP == this.Statements.Length ) {
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
				case TokenType.LITERAL:
					if ( long.TryParse( token.Value , out long lval ) ) {
						return new Value( ValueType.INTEGER , lval );
					}
					else if ( double.TryParse( token.Value , out double dval ) ) {
						return new Value( ValueType.FLOAT , dval );
					}
					else {
						throw new RuntimeException( this.IP , this.Statements[this.IP] , "Could not parse value from literal: " + token );
					}

				// Capture the string contents
				case TokenType.STRING:
					if ( token.Value.Length < 2 ) {
						throw new RuntimeException( this.IP , this.Statements[this.IP] , "String token too short to be valid: " + token );
					}

					return new Value( ValueType.STRING , token.Value.Substring( 1 , token.Value.Length - 2 ) );

				// Get the Value of a variable
				case TokenType.VARIABLE:
					// TODO
					if ( token.Value.Contains( Symbol.ACCESSOR.ToString( ) ) ) {
						return default; // TODO is element? (change token to x@# if so)
					}

					// Find variable
					foreach ( Variable v in this.Variables ) {
						if ( v.Identifier.Equals( token ) ) {
							return v.Value;
						}
					}

					throw new RuntimeException( this.IP , this.Statements[this.IP] , "No value yet for variable operand: " + token.ToString( ) );

				// Null operands for single operand statements
				case TokenType.NULL:
					return default;

				default:
					throw new RuntimeException( this.IP , this.Statements[this.IP] , "Unexpected token type for operand: " + token.ToString( ) );
			}

			throw new RuntimeException( this.IP , this.Statements[this.IP] , "Failed to get value for operand: " + token.ToString( ) );
		}

		/// <summary>
		/// Get a reference to the <see cref="Variable"/> object identified by a token.
		/// </summary>
		/// <param name="id">Token to ID the variable with</param>
		/// <returns>Reference object to the variable</returns>
		private Variable GetVariable( Token id )
		{
			// TODO
			//if ( id.Value.Contains( Symbol.ACCESSOR.ToString( ) ) )
			//	return null; // TODO is element? (change token to x@# if so)

			// Find variable
			foreach ( Variable v in this.Variables ) {
				if ( v.Identifier.Equals( id ) ) {
					return v;
				}
			}

			return null;
		}
	}
}
