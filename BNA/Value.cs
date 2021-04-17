using System;

namespace BNA
{
	/// <summary>
	/// The data type of a value.
	/// </summary>
	public enum ValueType
	{
		INVALID = -1,
		NULL = 0,
		INTEGER = 1,
		FLOAT,
		STRING,
		LIST
	}

	/// <summary>
	/// A struct holding an abstact value of varying type.
	/// </summary>
	public struct Value
	{
		/// <summary>
		/// Perform a numeric operation on two values.
		/// </summary>
		/// <param name="op1">First operand</param>
		/// <param name="op2">Second operand</param>
		/// <param name="operation">Which operation to perform</param>
		/// <returns>Result of the operation</returns>
		public static Value DoNumericOperation( Value op1 , Value op2 , StatementType operation )
		{
			// Check types
			ValueType operationType = ValueType.INTEGER;
			if ( op1.Type == ValueType.FLOAT || op2.Type == ValueType.FLOAT ) {
				operationType = ValueType.FLOAT;
				if ( op1.Type == ValueType.INTEGER ) {
					op1 = new Value( ValueType.FLOAT , (double)(long)op1.Val );
				}
				else if ( op1.Type != ValueType.FLOAT ) {
					throw new Exception( "Value told to do numeric operation on non-numeric value: (" + op1.Type.ToString( ) + ") " + op1.ToString( ) );
				}
				if ( op2.Type == ValueType.INTEGER ) {
					op2 = new Value( ValueType.FLOAT , (double)(long)op2.Val );
				}
				else if ( op2.Type != ValueType.FLOAT ) {
					throw new Exception( "Value told to do numeric operation on non-numeric value: (" + op2.Type.ToString( ) + ") " + op2.ToString( ) );
				}
			}

			// Complete operation
			switch ( operation ) {
				case StatementType.OP_ADD:
					return operationType == ValueType.INTEGER
						? new Value( ValueType.INTEGER , (long)op1.Val + (long)op2.Val )
						: new Value( ValueType.FLOAT , (double)op1.Val + (double)op2.Val );

				case StatementType.OP_SUB:
					return operationType == ValueType.INTEGER
						? new Value( ValueType.INTEGER , (long)op1.Val - (long)op2.Val )
						: new Value( ValueType.FLOAT , (double)op1.Val - (double)op2.Val );

				case StatementType.OP_MUL:
					return operationType == ValueType.INTEGER
						? new Value( ValueType.INTEGER , (long)op1.Val * (long)op2.Val )
						: new Value( ValueType.FLOAT , (double)op1.Val * (double)op2.Val );

				case StatementType.OP_DIV:
					return operationType == ValueType.INTEGER
						? new Value( ValueType.INTEGER , (long)op1.Val / (long)op2.Val )
						: new Value( ValueType.FLOAT , (double)op1.Val / (double)op2.Val );

				case StatementType.OP_POW:
					return operationType == ValueType.INTEGER
						? new Value( ValueType.INTEGER , Math.Pow( (long)op1.Val , (long)op2.Val ) )
						: new Value( ValueType.FLOAT , Math.Pow( (double)op1.Val , (double)op2.Val ) );

				case StatementType.OP_LOG:
					return operationType == ValueType.INTEGER
						? new Value( ValueType.INTEGER , Math.Log( (long)op1.Val , (long)op2.Val ) )
						: new Value( ValueType.FLOAT , Math.Log( (double)op1.Val , (double)op2.Val ) );

				default:
					throw new Exception( "Unexpected operation type for numeric operation (" + operation.ToString() + ")." );
			}
		}

		/// <summary>
		/// Perform a bitwise operation on two values.
		/// </summary>
		/// <param name="op1">First operand</param>
		/// <param name="op2">Second operand</param>
		/// <param name="operation">Which operation to perform</param>
		/// <returns>Result of the operation</returns>
		public static Value DoBitwiseOperation( Value op1 , Value op2 , StatementType operation )
		{
			// Check type
			if ( op1.Type != ValueType.INTEGER || op2.Type != ValueType.INTEGER ) {
				throw new Exception( "Value told to do bitwise operation on non-integer type(s): "
					+ "op1:(" + op1.Type.ToString( ) + ") op2:(" + op2.Type.ToString( ) + ")" );
			}

			// Complete operation
			switch ( operation ) {
				case StatementType.OP_MOD:
					return new Value( ValueType.INTEGER , (long)op1.Val % (long)op2.Val );

				case StatementType.OP_AND:
					return new Value( ValueType.INTEGER , (long)op1.Val & (long)op2.Val );

				case StatementType.OP_OR:
					return new Value( ValueType.INTEGER , (long)op1.Val | (long)op2.Val );

				case StatementType.OP_XOR:
					return new Value( ValueType.INTEGER , (long)op1.Val ^ (long)op2.Val );

				default:
					throw new Exception( "Unexpected operation type for bitwise operation (" + operation.ToString( ) + ")." );
			}
		}

		/// <summary>
		/// Perform a comparison operation on two values.
		/// </summary>
		/// <param name="op1">First operand</param>
		/// <param name="op2">Second operand</param>
		/// <param name="operation">Which operation to perform</param>
		/// <returns>Result of the operation</returns>
		public static Value DoComparisonOperation( Value op1 , Value op2 , StatementType operation )
		{
			throw new NotImplementedException( );
		}

		/// <summary>
		/// Data type of the Value.
		/// </summary>
		public ValueType Type
		{
			get; set;
		}

		/// <summary>
		/// Actual value stored, must be casted correctly to use.
		/// </summary>
		public object Val
		{
			get; set;
		}

		/// <summary>
		/// Create a new <see cref="Value"/> instance of a given type and value.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="val"></param>
		public Value( ValueType type , object val ) : this( )
		{
			this.Type = type;
			this.Val = val;
		}

		/// <summary>
		/// Give the value as a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString( )
		{
			if ( this.Val == null ) {
				return "";
			}

			return this.Val.ToString( );
		}
	}
}
