using System;
using System.Collections.Generic;
using System.IO;
using BNA.Exceptions;
using BNA.Values;
using ValueType = BNA.Values.ValueType;

namespace BNA
{
	/// <summary>
	/// Object class representing a BNA program.
	/// </summary>
	public partial class Program
	{
		/// <summary>
		/// Execute the current statement pointed to by IP.
		/// </summary>
		private void ExecuteCurrentStatement( )
		{
			switch ( this.Current.Type )
			{
				case StatementType.OP_SET:
					this.ExecuteSetStatement( );
					break;

				case StatementType.OP_ADD:
				case StatementType.OP_SUB:
				case StatementType.OP_MUL:
				case StatementType.OP_DIV:
				case StatementType.OP_LOG:
				case StatementType.OP_POW:
					this.ExecuteNumericTwoOpStatement( );
					break;

				case StatementType.OP_MOD:
				case StatementType.OP_AND:
				case StatementType.OP_OR:
				case StatementType.OP_XOR:
					this.ExecuteBitwiseStatement( );
					break;

				case StatementType.OP_RAND:
				case StatementType.OP_NEG:
				case StatementType.OP_ROUND:
					this.ExecuteNumericOneOpStatement( );
					break;

				case StatementType.OP_LIST:
				case StatementType.OP_APPEND:
				case StatementType.OP_SIZE:
					this.ExecuteListOperationStatement( );
					break;

				case StatementType.OP_PRINT:
					Console.WriteLine( this.GetValue( this.Current.Operand2 ).ToString( ) );
					break;

				case StatementType.OP_INPUT:
					this.ExecuteInputStatement( );
					break;

				case StatementType.OP_OPEN_R:
				case StatementType.OP_OPEN_W:
				case StatementType.OP_WRITE:
				case StatementType.OP_READ:
					this.ExecuteFileStatement( );
					break;

				case StatementType.OP_TEST_EQ:
				case StatementType.OP_TEST_NE:
				case StatementType.OP_TEST_GT:
				case StatementType.OP_TEST_LT:
					this.ExecuteTestStatement( );
					break;

				case StatementType.OP_SCOPE_OPEN:
				case StatementType.OP_SCOPE_CLOSE:
					this.ExecuteScopeStatement( );
					break;

				case StatementType.OP_WAIT:
					this.ExecuteWaitStatement( );
					break;

				case StatementType.OP_GOTO:
					this.ExecuteGotoStatement( );
					break;

				case StatementType.OP_TYPE:
					this.SetValue( this.Current.Operand1 , new Value( ValueType.STRING , this.GetValue( this.Current.Operand2 ).Type.ToString( ) ) , true );
					break;

				case StatementType.OP_EXIT:
					this.Running = false;
					break;

				case StatementType.OP_ERROR:
					throw new RuntimeException( this.GetValue( this.Current.Operand2 ).ToString( ) , true );

				case StatementType.NULL:
				case StatementType.COMMENT:
				case StatementType.LABEL:
					// do nothing
					break;

				default:
					throw new RuntimeException( "Unexpected statement type ( " + this.Current.Type.ToString( ) + " )" );
			}
		}

		/// <summary>
		/// Execute the current statement as a SET statement.
		/// </summary>
		private void ExecuteSetStatement( )
		{
			Value op2 = this.GetValue( this.Current.Operand2 );

			if ( op2.Type == ValueType.LIST )
			{
				var l = Value.DeepCopy( op2 );
				this.SetValue( this.Current.Operand1 , l , true );
			}
			else
			{
				this.SetValue( this.Current.Operand1 , op2 , true );
			}
		}

		/// <summary>
		/// Execute the current statement as a two-operand numeric statement.
		/// </summary>
		private void ExecuteNumericTwoOpStatement( )
		{
			Value op1 = this.GetValue( this.Current.Operand1 );
			Value op2 = this.GetValue( this.Current.Operand2 );

			if ( op1 == Value.NULL )
			{
				throw new RuntimeException( "Cannot use variable that has not been set: " + this.Current.Operand1.ToString( ) );
			}

			if ( op1.Type is not ValueType.INTEGER and not ValueType.FLOAT )
			{
				throw new RuntimeException( "Operand 1 of incorrect type (" + op1.Type.ToString( ) + ") for numeric operation" );
			}

			if ( op2.Type is not ValueType.INTEGER and not ValueType.FLOAT )
			{
				throw new RuntimeException( "Operand 2 of incorrect type (" + op2.Type.ToString( ) + ") for numeric operation" );
			}

			var newValue = Value.DoNumericOperation( op1 , op2 , this.Current.Type );
			this.SetValue( this.Current.Operand1 , newValue );
		}

		/// <summary>
		/// Execute the current statement as a bitwise statement.
		/// </summary>
		private void ExecuteBitwiseStatement( )
		{
			Value op1 = this.GetValue( this.Current.Operand1 );
			Value op2 = this.GetValue( this.Current.Operand2 );

			if ( op1.Type != ValueType.INTEGER )
			{
				throw new RuntimeException( "Operand 1 of incorrect type (" + op1.Type.ToString( ) + ") for bitwise operation" );
			}

			if ( op2.Type != ValueType.INTEGER )
			{
				throw new RuntimeException( "Operand 2 of incorrect type (" + op2.Type.ToString( ) + ") for bitwise operation" );
			}

			var newValue = Value.DoBitwiseOperation( op1 , op2 , this.Current.Type );
			this.SetValue( this.Current.Operand1 , newValue );
		}

		/// <summary>
		/// Execute the current statement as a one-operand numeric statement.
		/// </summary>
		private void ExecuteNumericOneOpStatement( )
		{
			Value op1 = this.GetValue( this.Current.Operand1 );
			Value op2 = this.GetValue( this.Current.Operand2 );

			// RANDOM
			if ( this.Current.Type == StatementType.OP_RAND )
			{
				if ( op2.Type == ValueType.INTEGER )
				{
					var newValue = new Value( ValueType.INTEGER , BNA.RNG.Next( (int)(long)op2.Val ) );
					this.SetValue( this.Current.Operand1 , newValue , true );
				}
				else if ( op2.Type == ValueType.FLOAT )
				{
					var newValue = new Value( ValueType.INTEGER , BNA.RNG.NextDouble( ) * (double)op2.Val );
					this.SetValue( this.Current.Operand1 , newValue , true );
				}
				else
				{
					throw new RuntimeException( "Operand 2 of incorrect type (" + op2.Type.ToString( ) + ") for numeric operation" );
				}
			}
			// NEGATE
			else if ( this.Current.Type == StatementType.OP_NEG )
			{
				if ( op1.Type == ValueType.INTEGER )
				{
					var newValue = new Value( ValueType.INTEGER , ( (ulong)(long)op1.Val ) ^ ulong.MaxValue );
					this.SetValue( this.Current.Operand1 , newValue );
				}
				else
				{
					throw new RuntimeException( "Operand 2 of incorrect type (" + op2.Type.ToString( ) + ") for numeric operation" );
				}
			}
			// ROUND
			else if ( this.Current.Type == StatementType.OP_ROUND )
			{
				if ( op1.Type == ValueType.FLOAT )
				{
					var newValue = new Value( ValueType.INTEGER , (long)Math.Round( (double)op1.Val ) );
					this.SetValue( this.Current.Operand1 , newValue );
				}
				else if ( op1.Type != ValueType.INTEGER )
				{
					throw new RuntimeException( "Operand 2 of incorrect type (" + op2.Type.ToString( ) + ") for numeric operation" );
				}
			}
			// error
			else
			{
				throw new Exception( "Program called wrong execute function." );
			}
		}

		/// <summary>
		/// Execute the current statement as a list operation statement.
		/// </summary>
		private void ExecuteListOperationStatement( )
		{
			Value op1 = this.GetValue( this.Current.Operand1 );
			Value op2 = this.GetValue( this.Current.Operand2 );

			// LIST
			switch ( this.Current.Type )
			{
				case StatementType.OP_LIST:
				{
					// Check operands
					if ( op2.Type != ValueType.INTEGER || (long)op2.Val < 0 )
						throw new RuntimeException( "List size must be positive or zero integer" );

					// Fill list
					var list = new List<Value>( );
					for ( int i = 0 ; i < (long)op2.Val ; i += 1 )
					{
						list.Add( Value.NULL );
					}

					this.SetValue( this.Current.Operand1 , new Value( ValueType.LIST , list ) , true );
					break;
				}

				case StatementType.OP_APPEND:
				{
					if ( op1.Type == ValueType.LIST )
					{
						if ( op2.Type == ValueType.LIST )
						{
							var l = Value.DeepCopy( op2 );
							( (List<Value>)op1.Val ).Add( l );
						}
						else
						{
							( (List<Value>)op1.Val ).Add( op2 );
						}
					}
					else if ( op1.Type == ValueType.STRING )
					{
						string new_str = ( (string)op1.Val ).Insert( ( (string)op1.Val ).Length , op2.Val.ToString( ) ?? string.Empty );
						this.SetValue( this.Current.Operand1 , new Value( ValueType.STRING , new_str ) );
					}
					else
					{
						throw new RuntimeException( "Can not append to non-list-like type: " + op1.Type );
					}

					break;
				}

				case StatementType.OP_SIZE:
				{
					switch ( op2.Type )
					{
						case ValueType.LIST:
							this.SetValue( this.Current.Operand1 , new Value( ValueType.INTEGER , (long)( (List<Value>)op2.Val ).Count ) , true );
							break;
						case ValueType.STRING:
							this.SetValue( this.Current.Operand1 , new Value( ValueType.INTEGER , (long)( (string)op2.Val ).Length ) , true );
							break;
						default:
							throw new RuntimeException( "Can not size a non-list-like value: '" + op2.ToString( ) + "'" );
					}
					break;
				}

				default:
					throw new Exception( "Program called wrong execute function." );
			}
		}

		/// <summary>
		/// Execute the current statement as an INPUT statement.
		/// </summary>
		private void ExecuteInputStatement( )
		{
			Console.Write( this.GetValue( this.Current.Operand2 ).ToString( ) );
			var token = new Token( Console.ReadLine( ) ?? string.Empty );
			switch ( token.Type )
			{
				case TokenType.LITERAL:
				case TokenType.STRING:
				case TokenType.LIST:
				case TokenType.NULL:
					this.SetValue( this.Current.Operand1 , this.GetValue( token ) , true );
					break;

				default:
					throw new RuntimeException( "Invalid input: " + token.ToString( ) );
			}
		}

		/// <summary>
		/// Execute the current statement as a file operation statement.
		/// </summary>
		private void ExecuteFileStatement( )
		{
			Value op1 = this.GetValue( this.Current.Operand1 );
			Value op2 = this.GetValue( this.Current.Operand2 );

			switch ( this.Current.Type )
			{
				case StatementType.OP_OPEN_R:
				{
					if ( op2.Type != ValueType.STRING )
					{
						throw new RuntimeException( "Filename must be string '" + op2.ToString( ) + "'" );
					}
					string filename = (string)op2.Val;

					var stream_r = new StreamReader( filename );

					this.SetValue( this.Current.Operand1 , new Value( ValueType.READ_FILE , stream_r ) , true );
					break;
				}

				case StatementType.OP_OPEN_W:
				{
					if ( op2.Type != ValueType.STRING )
					{
						throw new RuntimeException( "Filename must be string '" + op2.ToString( ) + "'" );
					}
					string filename = (string)op2.Val;

					var stream_w = new StreamWriter( filename , true )
					{
						AutoFlush = true
					};

					this.SetValue( this.Current.Operand1 , new Value( ValueType.WRITE_FILE , stream_w ) , true );
					break;
				}

				case StatementType.OP_WRITE:
				{
					if ( op1.Type != ValueType.WRITE_FILE )
					{
						throw new RuntimeException( "Operand to WRITE must be an opened write-file: '" + op1.ToString( ) + "'" );
					}

					( (StreamWriter)op1.Val ).WriteLine( op2.Val.ToString( ) );
					( (StreamWriter)op1.Val ).Flush( );
					break;
				}

				case StatementType.OP_READ:
				{
					if ( op2.Type != ValueType.READ_FILE )
					{
						throw new RuntimeException( "Operand to READ must be an opened read-file: '" + op2.ToString( ) + "'" );
					}

					string? str = ( (StreamReader)op2.Val ).ReadLine( );

					if ( str is null )
					{
						( (StreamReader)op2.Val ).Close( );
						this.SetValue( this.Current.Operand2 , Value.NULL );
					}

					Value val = str is null ? new Value( ValueType.STRING , string.Empty )
							  : long.TryParse( str , out long lval ) ? new Value( ValueType.INTEGER , lval )
							  : double.TryParse( str , out double dval ) ? new Value( ValueType.FLOAT , dval )
							  : new Value( ValueType.STRING , str );

					this.SetValue( this.Current.Operand1 , val , true );

					break;
				}

				case StatementType.OP_CLOSE:
				{
					if ( op1.Type == ValueType.READ_FILE )
					{
						( (StreamReader)op1.Val ).Close( );
					}
					else if ( op1.Type == ValueType.WRITE_FILE )
					{
						( (StreamWriter)op1.Val ).Close( );
					}
					else
					{
						throw new RuntimeException( "Can not close non-file: '" + op1.ToString( ) + "'" );
					}

					this.SetValue( this.Current.Operand1 , Value.NULL );
					break;
				}

				default:
					throw new Exception( "Program called wrong execute function." );
			}
		}

		/// <summary>
		/// Execute the current statement as a test operation statement.
		/// </summary>
		private void ExecuteTestStatement( )
		{
			Value op1 = this.GetValue( this.Current.Operand1 );
			Value op2 = this.GetValue( this.Current.Operand2 );

			if ( op1.Type == ValueType.INVALID || op1.Type == ValueType.NULL
						|| op2.Type == ValueType.INVALID || op2.Type == ValueType.NULL )
			{
				throw new RuntimeException( "Both operands must have valid values to compare" );
			}

			var result = Value.DoComparisonOperation( op1 , op2 , this.Current.Type );
			if ( result.Equals( Value.NAN ) )
			{
				throw new RuntimeException( "Could not compare operands: "
					+ "op1=" + this.Current.Operand1.ToString( ) + " op2=" + this.Current.Operand2.ToString( ) );
			}

			this.SetValue( SpecialVariables.TEST_RESULT , result );
		}

		/// <summary>
		/// Execute the current statement as a scope operation statement.
		/// </summary>
		private void ExecuteScopeStatement( )
		{
			if ( this.Current.Type == StatementType.OP_SCOPE_OPEN )
			{
				Value argument_val = this.GetValue( SpecialVariables.ARGUMENT );
				this.Variables = new Dictionary<Token , Value>( );
				this.SetValue( SpecialVariables.ARGUMENT , argument_val );
			}
			else if ( this.Current.Type == StatementType.OP_SCOPE_CLOSE )
			{
				Value return_val = this.GetValue( SpecialVariables.RETURN );
				if ( this.Scopes.Count < 2 )
				{
					throw new RuntimeException( "Cannot close final scope." );
				}
				_ = this.Scopes.Pop( );
				this.SetValue( SpecialVariables.RETURN , return_val );
			}
			else
			{
				throw new Exception( "Program called wrong execute fuction." );
			}
		}

		/// <summary>
		/// Execute the current statement as a WAIT statement.
		/// </summary>
		private void ExecuteWaitStatement( )
		{
			Value op2 = this.GetValue( this.Current.Operand2 );

			if ( op2.Type is not ValueType.INTEGER and not ValueType.FLOAT )
			{
				throw new RuntimeException( "Argument to WAIT must be numeric: " + op2.ToString( ) );
			}

			int ms = (int)( 1000 * ( op2.Type is ValueType.INTEGER ? (long)op2.Val : (double)op2.Val ) );
			try
			{
				System.Threading.Thread.Sleep( ms );
			}
			catch ( Exception e )
			{
				throw new RuntimeException( "Exception caught while waiting:\n" + e.Message );
			}
		}

		/// <summary>
		/// Execute the current statement as a GOTO statement.
		/// </summary>
		private void ExecuteGotoStatement( )
		{
			// Find line
			Value line = this.GetValue( this.Current.Operand1 );
			if ( line == Value.NULL || line.Type != ValueType.INTEGER )
			{
				throw new RuntimeException( "Found no valid line value '" + this.Current.Operand1.ToString( ) + "'" );
			}

			// Test condition
			if ( this.GetValue( this.Current.Operand2 ) != Value.FALSE )
			{
				// Go to line before label (because IP will be incremented)
				this.IP = (int)(long)line.Val - 1;
			}
		}
	}
}
