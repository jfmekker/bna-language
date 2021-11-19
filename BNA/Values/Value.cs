using System;
using System.Collections.Generic;
using System.IO;
using BNA.Exceptions;

namespace BNA.Values
{
	/// <summary>
	/// A struct holding an abstact value of varying type.
	/// </summary>
	public abstract class Value : IEquatable<Value>
	{
		public static readonly Value NULL = new NullValue( );
		public static readonly Value NAN = new NaNValue( );
		public static readonly Value TRUE = new IntegerValue( 1 );
		public static readonly Value FALSE = new IntegerValue( 0 );

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
			if ( op1.Type == ValueType.FLOAT || op2.Type == ValueType.FLOAT )
			{
				operationType = ValueType.FLOAT;
				if ( op1.Type == ValueType.INTEGER )
				{
					op1 = new Value( ValueType.FLOAT , (double)(long)op1.Get );
				}
				else if ( op1.Type != ValueType.FLOAT )
				{
					throw new Exception( "Value told to do numeric operation on non-numeric value: (" + op1.Type.ToString( ) + ") " + op1.ToString( ) );
				}
				if ( op2.Type == ValueType.INTEGER )
				{
					op2 = new Value( ValueType.FLOAT , (double)(long)op2.Get );
				}
				else if ( op2.Type != ValueType.FLOAT )
				{
					throw new Exception( "Value told to do numeric operation on non-numeric value: (" + op2.Type.ToString( ) + ") " + op2.ToString( ) );
				}
			}

			// Complete operation
			switch ( operation )
			{
				case StatementType.OP_ADD:
					return operationType == ValueType.INTEGER
						? new Value( ValueType.INTEGER , (long)op1.Get + (long)op2.Get )
						: new Value( ValueType.FLOAT , (double)op1.Get + (double)op2.Get );

				case StatementType.OP_SUB:
					return operationType == ValueType.INTEGER
						? new Value( ValueType.INTEGER , (long)op1.Get - (long)op2.Get )
						: new Value( ValueType.FLOAT , (double)op1.Get - (double)op2.Get );

				case StatementType.OP_MUL:
					return operationType == ValueType.INTEGER
						? new Value( ValueType.INTEGER , (long)op1.Get * (long)op2.Get )
						: new Value( ValueType.FLOAT , (double)op1.Get * (double)op2.Get );

				case StatementType.OP_DIV:
					return operationType == ValueType.INTEGER
						? new Value( ValueType.INTEGER , (long)op1.Get / (long)op2.Get )
						: new Value( ValueType.FLOAT , (double)op1.Get / (double)op2.Get );

				case StatementType.OP_POW:
					return operationType == ValueType.INTEGER
						? new Value( ValueType.INTEGER , (long)Math.Pow( (long)op1.Get , (long)op2.Get ) )
						: new Value( ValueType.FLOAT , Math.Pow( (double)op1.Get , (double)op2.Get ) );

				case StatementType.OP_LOG:
					// Only do logs with floats
					if ( op1.Type == ValueType.INTEGER )
					{
						op1 = new Value( ValueType.FLOAT , (double)(long)op1.Get );
					}
					if ( op2.Type == ValueType.INTEGER )
					{
						op2 = new Value( ValueType.FLOAT , (double)(long)op2.Get );
					}
					return new Value( ValueType.FLOAT , Math.Log( (double)op1.Get , (double)op2.Get ) );

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
			if ( op1.Type != ValueType.INTEGER || op2.Type != ValueType.INTEGER )
			{
				throw new Exception( "Value told to do bitwise operation on non-integer type(s): "
					+ "op1:(" + op1.Type.ToString( ) + ") op2:(" + op2.Type.ToString( ) + ")" );
			}

			// Complete operation
			switch ( operation )
			{
				case StatementType.OP_MOD:
					return new Value( ValueType.INTEGER , (long)op1.Get % (long)op2.Get );

				case StatementType.OP_AND:
					return new Value( ValueType.INTEGER , (long)op1.Get & (long)op2.Get );

				case StatementType.OP_OR:
					return new Value( ValueType.INTEGER , (long)op1.Get | (long)op2.Get );

				case StatementType.OP_XOR:
					return new Value( ValueType.INTEGER , (long)op1.Get ^ (long)op2.Get );

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
			switch ( op1.Type )
			{
				case ValueType.INTEGER:
				case ValueType.FLOAT:
				{
					if ( op2.Type is not ValueType.INTEGER and not ValueType.FLOAT )
					{
						return NAN;
					}

					double v1 = ( op1.Type == ValueType.INTEGER ) ? (long)op1.Get : (double)op1.Get;
					double v2 = ( op2.Type == ValueType.INTEGER ) ? (long)op2.Get : (double)op2.Get;

					switch ( operation )
					{
						case StatementType.OP_TEST_EQ:
							return ( v1 == v2 ) ? TRUE : FALSE;
						case StatementType.OP_TEST_NE:
							return ( v1 != v2 ) ? TRUE : FALSE;
						case StatementType.OP_TEST_GT:
							return ( v1 > v2 ) ? TRUE : FALSE;
						case StatementType.OP_TEST_LT:
							return ( v1 < v2 ) ? TRUE : FALSE;
						default:
							throw new Exception( "Unexpected operation type for comparison operation (" + operation.ToString( ) + ")." );
					}
				}

				case ValueType.STRING:
				{
					if ( op2.Type == ValueType.STRING )
					{
						bool equal = ( (string)op1.Get ).Equals( (string)op2.Get );
						return operation == StatementType.OP_TEST_EQ ? ( equal ? TRUE : FALSE )
							 : operation == StatementType.OP_TEST_NE ? ( !equal ? TRUE : FALSE )
							 : throw new RuntimeException( "Can only test string equality or inequality" );
					}
					else if ( op2.Type == ValueType.INTEGER )
					{
						string s = (string)op1.Get;
						long i = (long)op2.Get;
						switch ( operation )
						{
							case StatementType.OP_TEST_EQ:
								return ( s.Length == i ) ? TRUE : FALSE;
							case StatementType.OP_TEST_NE:
								return ( s.Length != i ) ? TRUE : FALSE;
							case StatementType.OP_TEST_GT:
								return ( s.Length > i ) ? TRUE : FALSE;
							case StatementType.OP_TEST_LT:
								return ( s.Length < i ) ? TRUE : FALSE;
							default:
								throw new Exception( "Unexpected operation type for comparison operation (" + operation.ToString( ) + ")." );
						}
					}
					else
					{
						return NAN;
					}
				}

				case ValueType.LIST:
				{
					if ( op2.Type == ValueType.LIST )
					{
						bool test = true;
						var list1 = (List<Value>)op1.Get;
						var list2 = (List<Value>)op2.Get;

						if ( operation != StatementType.OP_TEST_EQ )
						{
							return NAN;
						}

						for ( int i = 0 ; i < list1.Count ; i += 1 )
						{
							if ( i >= list2.Count )
							{
								test = false;
								break;
							}

							Value elementTest = DoComparisonOperation( list1[i] , list2[i] , operation );
							if ( elementTest == FALSE )
							{
								test = false;
								break;
							}
						}

						return test ? TRUE : FALSE;
					}
					else
					{
						return NAN;
					}
				}

				default:
					throw new Exception( "Invalid token type for comparison operation: "
						+ "op1:(" + op1.Type.ToString( ) + ") op2:(" + op2.Type.ToString( ) + ")" );
			}
		}

		/// <summary>
		/// Actual value stored, must be casted correctly to use.
		/// </summary>
		public abstract object Get { get; }

		public abstract Value DoOperation( StatementType operation , Value op2 );

		public override sealed bool Equals( object? obj ) => this.Equals( obj as Value );

		public override int GetHashCode( ) => this.Get.GetHashCode( );

		public abstract bool Equals( Value? other );

		/// <summary>
		/// Defined '==' operator for Value.
		/// </summary>
		/// <param name="first">First value to compare</param>
		/// <param name="second">Second value to compare</param>
		/// <returns>True if the two values are equal</returns>
		public static bool operator ==( Value first , Value second ) => first.Equals( second );

		/// <summary>
		/// Defined '!=' operator for Value.
		/// </summary>
		/// <param name="first">First value to compare</param>
		/// <param name="second">Second value to compare</param>
		/// <returns>True if the two values are not equal</returns>
		public static bool operator !=( Value first , Value second ) => !first.Equals( second );
	}
}
