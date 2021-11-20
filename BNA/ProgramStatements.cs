using System;
using System.Collections.Generic;
using System.IO;
using BNA.Exceptions;
using BNA.Values;

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
					this.SetValue( this.Current.Operand1 , new StringValue( this.GetValue( this.Current.Operand2 ).TypeString( ) ) , true );
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
					throw new Exception( "Unexpected statement type ( " + this.Current.Type.ToString( ) + " )" );
			}
		}

		/// <summary>
		/// Execute the current statement as a SET statement.
		/// </summary>
		private void ExecuteSetStatement( )
		{
			Value op2 = this.GetValue( this.Current.Operand2 );

			if ( op2 is ListValue listVal )
			{
				ListValue l = listVal.DeepCopy( );
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
				throw new RuntimeException( $"Cannot use variable that has not been set: {this.Current.Operand1}" );
			}

			if ( op1 is not IntegerValue and not FloatValue )
			{
				throw new RuntimeException( $"Operand 1 of incorrect type ({op1.TypeString( )}) for numeric operation" );
			}

			if ( op2 is not IntegerValue and not FloatValue )
			{
				throw new RuntimeException( $"Operand 2 of incorrect type ({op2.TypeString( )}) for numeric operation" );
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

			if ( op1 is not IntegerValue )
			{
				throw new RuntimeException( $"Operand 1 of incorrect type ({op1.TypeString( )}) for bitwise operation" );
			}

			if ( op2 is not IntegerValue )
			{
				throw new RuntimeException( $"Operand 2 of incorrect type ({ op2.TypeString( ) }) for bitwise operation" );
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
				if ( op2 is IntegerValue intVal )
				{
					this.SetValue( this.Current.Operand1 , new IntegerValue( BNA.RNG.Next( intVal.Get ) ) , true );
				}
				else if ( op2 is FloatValue floatVal )
				{
					this.SetValue( this.Current.Operand1 , new FloatValue( BNA.RNG.NextDouble( ) * floatVal.Get ) , true );
				}
				else
				{
					throw new RuntimeException( $"Operand 2 of incorrect type ({op2.TypeString( )}) for numeric operation" );
				}
			}
			// NEGATE
			else if ( this.Current.Type == StatementType.OP_NEG )
			{
				if ( op1 is IntegerValue intVal )
				{
					this.SetValue( this.Current.Operand1 , new IntegerValue( (long)( (ulong)intVal.Get ^ ulong.MaxValue ) ) );
				}
				else
				{
					throw new RuntimeException( $"Operand 2 of incorrect type ({op2.TypeString( )}) for numeric operation" );
				}
			}
			// ROUND
			else if ( this.Current.Type == StatementType.OP_ROUND )
			{
				if ( op1 is FloatValue floatVal )
				{
					this.SetValue( this.Current.Operand1 , new IntegerValue( (long)Math.Round( floatVal.Get ) ) );
				}
				else if ( op1 is not IntegerValue )
				{
					throw new RuntimeException( $"Operand 2 of incorrect type ({op2.TypeString( )}) for numeric operation" );
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
					if ( op2 is not IntegerValue intVal || intVal.Get < 0 )
						throw new RuntimeException( "List size must be non-negative integer" );

					this.SetValue( this.Current.Operand1 , new ListValue( intVal.Get ) , true );
					break;
				}

				case StatementType.OP_APPEND:
				{
					if ( op1 is ListValue listVal1 )
					{
						if ( op2 is ListValue listVal2 )
						{
							listVal1.Get.Add( listVal2.DeepCopy( ) );
						}
						else
						{
							listVal1.Get.Add( op2 );
						}
					}
					else if ( op1 is StringValue strVal )
					{
						this.SetValue( this.Current.Operand1 , new StringValue( strVal + op2.ToString( ) ) );
					}
					else
					{
						throw new RuntimeException( $"Can not append to non-list-like type: {op1.TypeString( )}" );
					}

					break;
				}

				case StatementType.OP_SIZE:
				{
					long size = op2 is ListValue listVal ? listVal.Get.Count
							  : op2 is StringValue strVal ? strVal.Get.Length
							  : throw new RuntimeException( $"Can not size a non-list-like value: ({op2.TypeString( )}) '{op2}'" );
					this.SetValue( this.Current.Operand1 , new IntegerValue( size ) , true );
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
					if ( op2 is not StringValue strVal )
					{
						throw new RuntimeException( $"Filename must be string: {op2.TypeString( )} '{op2}'" );
					}

					this.SetValue( this.Current.Operand1 , new ReadFileValue( strVal.Get ) , true );
					break;
				}

				case StatementType.OP_OPEN_W:
				{
					if ( op2 is not StringValue strVal )
					{
						throw new RuntimeException( $"Filename must be string: {op2.TypeString( )} '{op2}'" );
					}

					this.SetValue( this.Current.Operand1 , new WriteFileValue( strVal.Get ) , true );
					break;
				}

				case StatementType.OP_WRITE:
				{
					if ( op1 is not WriteFileValue writeFileVal )
					{
						throw new RuntimeException( $"Operand to WRITE must be a write-file: {op1.TypeString( )} '{op1}'" );
					}

					writeFileVal.WriteLine( op2.ToString( ) );
					break;
				}

				case StatementType.OP_READ:
				{
					if ( op2 is not ReadFileValue readFileVal )
					{
						throw new RuntimeException( $"Operand to READ must be a read-file: {op2.TypeString( )} '{op2}'" );
					}

					string? str = readFileVal.ReadLine( );

					if ( str is null )
					{
						readFileVal.Close( );
						this.SetValue( this.Current.Operand2 , Value.NULL );
					}

					Value val = str is null ? new StringValue( )
							  : long.TryParse( str , out long lval ) ? new IntegerValue( lval )
							  : double.TryParse( str , out double dval ) ? new FloatValue( dval )
							  : new StringValue( str );

					this.SetValue( this.Current.Operand1 , val , true );

					break;
				}

				case StatementType.OP_CLOSE:
				{
					if ( op1 is FileValue fileVal )
					{
						fileVal.Close( );
					}
					else
					{
						throw new RuntimeException( $"Can not close non-file: {op1.TypeString( )} '{op1}'" );
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

			if ( op2 is not IntegerValue and not FloatValue )
			{
				throw new RuntimeException( "Argument to WAIT must be numeric: " + op2.ToString( ) );
			}

			int ms = (int)( 1000 *
				( op2 is IntegerValue intVal ? intVal.Get
				: op2 is FloatValue floatVal ? floatVal.Get
				: throw new RuntimeException( $"Argument to WAIT must be numeric: {op2.TypeString( )} '{op2}'" ) ) );

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
			Value op1 = this.GetValue( this.Current.Operand1 );
			if ( op1 == Value.NULL || op1 is not IntegerValue line ) // TODO more checks
			{
				throw new RuntimeException( $"Found no valid line value {op1.TypeString( )} '{this.Current.Operand1}={op1}'" );
			}

			// Test condition
			if ( this.GetValue( this.Current.Operand2 ) != Value.FALSE )
			{
				// Go to line before label (because IP will be incremented)
				this.IP = line.Get - 1;
			}
		}
	}
}
