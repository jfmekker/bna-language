﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA
{
	public enum ValueType
	{
		INVALID = -1,
		NULL = 0,
		INTEGER = 1,
		FLOAT,
		STRING,
		LIST
	}

	public struct Value
	{
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
					if ( operationType == ValueType.INTEGER )
						return new Value( ValueType.INTEGER , (long)op1.Val + (long)op2.Val );
					else
						return new Value( ValueType.FLOAT , (double)op1.Val + (double)op2.Val );

				case StatementType.OP_SUB:
				case StatementType.OP_MUL:
				case StatementType.OP_DIV:
				case StatementType.OP_MOD:
				case StatementType.OP_LOG:
				case StatementType.OP_POW:
				case StatementType.OP_AND:
				case StatementType.OP_OR:
				case StatementType.OP_XOR:
				case StatementType.OP_RAND:
					throw new NotImplementedException( );

				default:
					throw new Exception( "Unexpected operation type for numeric operation." );
			}

			throw new NotImplementedException( );
		}

		public static Value DoComparisonOperation( Value op1 , Value op2 , StatementType operation )
		{
			throw new NotImplementedException( );
		}

		public ValueType Type
		{
			get; set;
		}

		public object Val
		{
			get; set;
		}

		public Value( ValueType type , object val ) : this( )
		{
			this.Type = type;
			this.Val = val;
		}

		public override string ToString( )
		{
			if ( Val == null )
				return "";
			return Val.ToString( );
		}
	}
}
