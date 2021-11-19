﻿using System;
using System.Collections.Generic;
using System.IO;
using BNA.Exceptions;

namespace BNA.Values
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
		LIST,
		READ_FILE,
		WRITE_FILE,
	}

	/// <summary>
	/// A struct holding an abstact value of varying type.
	/// </summary>
	public abstract class Value
	{
		public static readonly Value NULL = new NullValue( );
		public static readonly Value NAN = new NaNValue( );
		public static readonly Value TRUE = new( ValueType.INTEGER , 1L );
		public static readonly Value FALSE = new( ValueType.INTEGER , 0L );

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

		/// <summary>
		/// Give the value as a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString( )
		{
			switch ( this.Type )
			{
				case ValueType.INVALID:
					return "NaN";

				case ValueType.NULL:
					return "null";

				case ValueType.INTEGER:
				case ValueType.FLOAT:
				case ValueType.STRING:
					return this.Get.ToString( ) ?? throw new InvalidOperationException( "Value object has no value but is not NULL." );

				case ValueType.LIST:
				{
					var list = (List<Value>)this.Get;

					string str = "" + (char)Symbol.LIST_START + " ";
					int i = 0;
					while ( i < list.Count )
					{

						if ( list[i].Type == ValueType.STRING )
						{
							str += '"' + list[i].ToString( ) + '"';
						}
						else
						{
							str += list[i].ToString( );
						}

						if ( i < list.Count - 1 )
						{
							str += " " + (char)Symbol.LIST_SEPERATOR + " ";
						}
						i += 1;
					}
					str += " " + (char)Symbol.LIST_END;

					return str;
				}

				case ValueType.READ_FILE:
				{
					return "Read-file: '" + "'";
				}

				case ValueType.WRITE_FILE:
				{

					return "Write-file: '" + "'";
				}

				default:
					throw new Exception( "Unexpected value type in ToString: " + this.Type );
			}
		}

		/// <summary>
		/// Compare equality of two Values.
		/// </summary>
		/// <param name="obj">Value to compare against</param>
		/// <returns>True if the two values are equal</returns>
		public override bool Equals( object? obj )
		{
			return obj is Value value
				&& this.Type == value.Type
				&& EqualityComparer<object>.Default.Equals( this.Get , value.Get );
		}

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

		/// <summary>
		/// Do a recursive deep copy of a list type Value.
		/// </summary>
		/// <param name="listValue">Value to copy.</param>
		/// <returns>A deep copy of the list.</returns>
		public static Value DeepCopy( Value listValue )
		{
			if ( listValue.Type != ValueType.LIST )
			{
				throw new Exception( "Can not deep copy a non-list value." );
			}

			var list = (List<Value>)listValue.Get;
			var newList = new List<Value>( );

			foreach ( Value v in list )
			{
				if ( v.Type == ValueType.LIST )
				{
					newList.Add( DeepCopy( v ) );
				}
				else
				{
					newList.Add( v );
				}
			}

			return new Value( ValueType.LIST , newList );
		}

		/// <summary>
		/// Get a hash code for the Value object.
		/// </summary>
		/// <returns>Hash code.</returns>
		public override int GetHashCode( ) => HashCode.Combine( this.Get );
	}
}
