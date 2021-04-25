using System;
using System.Collections.Generic;

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
		public static readonly Value NULL = new Value( ValueType.NULL , 0 );
		public static readonly Value NAN = new Value( ValueType.INVALID , 0 );

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
					throw new Exception( "Unexpected operation type for numeric operation (" + operation.ToString( ) + ")." );
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
		/// <returns>Result of the operation, or a null value if the types are incompatiible</returns>
		public static Value DoComparisonOperation( Value op1 , Value op2 , StatementType operation )
		{
			var true_value = new Value( ValueType.INTEGER , 1L );
			var false_value = new Value( ValueType.INTEGER , 0L );

			switch ( op1.Type ) {

				case ValueType.INTEGER:
				case ValueType.FLOAT: {
					if ( op2.Type != ValueType.INTEGER && op2.Type != ValueType.FLOAT ) {
						return NAN;
					}

					double v1 = ( op1.Type == ValueType.INTEGER ) ? (long)op1.Val : (double)op1.Val;
					double v2 = ( op2.Type == ValueType.INTEGER ) ? (long)op2.Val : (double)op2.Val;

					switch ( operation ) {
						case StatementType.OP_TEST_EQ:
							return ( v1 == v2 ) ? true_value : false_value;
						case StatementType.OP_TEST_GT:
							return ( v1 > v2 ) ? true_value : false_value;
						case StatementType.OP_TEST_LT:
							return ( v1 < v2 ) ? true_value : false_value;
						default:
							throw new Exception( "Unexpected operation type for comparison operation (" + operation.ToString( ) + ")." );
					}
				}

				case ValueType.STRING: {
					if ( op2.Type == ValueType.STRING ) {
						return ( (string)op1.Val ).Equals( (string)op2.Val ) ? true_value : false_value;
					}
					else if ( op2.Type == ValueType.INTEGER ) {
						// TODO do we want this to be a valid comparison?
						string s = (string)op1.Val;
						long i = (long)op2.Val;
						switch ( operation ) {
							case StatementType.OP_TEST_EQ:
								return ( s.Length == i ) ? true_value : false_value;
							case StatementType.OP_TEST_GT:
								return ( s.Length > i ) ? true_value : false_value;
							case StatementType.OP_TEST_LT:
								return ( s.Length < i ) ? true_value : false_value;
							default:
								throw new Exception( "Unexpected operation type for comparison operation (" + operation.ToString( ) + ")." );
						}
					}
					else {
						return NAN;
					}
				}

				case ValueType.LIST: {
					throw new NotImplementedException( );
				}


				default:
					throw new Exception( "Invalid token type for comparison operation: "
						+ "op1:(" + op1.Type.ToString( ) + ") op2:(" + op2.Type.ToString( ) + ")" );
			}
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
			switch ( this.Type ) {
				case ValueType.INVALID:
					return "NaN";

				case ValueType.NULL:
					return "null";

				case ValueType.INTEGER:
				case ValueType.FLOAT:
				case ValueType.STRING:
					return this.Val.ToString( );

				case ValueType.LIST:
					var list = (List<Value>)this.Val;

					string str = "{ ";
					int i = 0;
					while ( i < list.Count ) {
						str += list[i].ToString( );
						if ( i < list.Count - 1 ) {
							str += " , ";
						}
						i += 1;
					}
					str += " }";

					return str;

				default:
					throw new Exception( "Unexpected value type in ToString: " + this.Type );
			}
		}

		/// <summary>
		/// Compare equality of two Values.
		/// </summary>
		/// <param name="obj">Value to compare against</param>
		/// <returns>True if the two values are equal</returns>
		public override bool Equals( object obj )
		{
			if ( !( obj is Value ) ) {
				return false;
			}

			var value = (Value)obj;
			return this.Type == value.Type &&
					EqualityComparer<object>.Default.Equals( this.Val , value.Val );
		}

		/// <summary>
		/// Defined '==' operator for Value.
		/// </summary>
		/// <param name="first">First value to compare</param>
		/// <param name="second">Second value to compare</param>
		/// <returns>True if the two values are equal</returns>
		public static bool operator ==( Value first, Value second )
		{
			return first.Equals( second );
		}

		/// <summary>
		/// Defined '!=' operator for Value.
		/// </summary>
		/// <param name="first">First value to compare</param>
		/// <param name="second">Second value to compare</param>
		/// <returns>True if the two values are not equal</returns>
		public static bool operator !=( Value first , Value second )
		{
			return !first.Equals( second );
		}

		public static Value DeepCopy( Value listValue )
		{
			if ( listValue.Type != ValueType.LIST ) {
				throw new Exception( "Can not deep copy a non-list value." );
			}

			var list = (List<Value>)listValue.Val;
			var newList = new List<Value>( );

			foreach ( Value v in list ) {
				if ( v.Type == ValueType.LIST ) {
					newList.Add( DeepCopy( v ) );
				}
				else {
					newList.Add( v );
				}
			}

			return new Value( ValueType.LIST , newList );
		}

		public override int GetHashCode( )
		{
			var hashCode = 1893053585;
			hashCode = hashCode * -1521134295 + this.Type.GetHashCode( );
			hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode( this.Val );
			return hashCode;
		}
	}
}
