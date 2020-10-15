using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BNAB;
using BNAVM.Data;

namespace BNAVM
{
	class OperandHandler
	{
		private DataSegment literals;
		private DataSegment variables;

		public OperandHandler( DataSegment data )
		{
			literals = data;
			variables = new DataSegment( );
		}

		private OperandDataType FindLiteralDataType( int id )
		{
			if ( literals.GetIntData( id ).HasValue )
				return OperandDataType.INTEGER;
			else if ( literals.GetFloatData( id ).HasValue )
				return OperandDataType.FLOAT;
			else if ( literals.GetStringData( id ) != null )
				return OperandDataType.STRING;

			throw new Exception( "Could not find literal with key: " + id );
		}

		private OperandDataType FindVariableDataType( int id )
		{
			if ( variables.GetIntData( id ).HasValue )
				return OperandDataType.INTEGER;
			else if ( variables.GetFloatData( id ).HasValue )
				return OperandDataType.FLOAT;
			else if ( variables.GetStringData( id ) != null )
				return OperandDataType.STRING;

			throw new Exception( "Could not find variable with key: " + id );
		}

		private long GetLiteralIntValue( int id )
		{
			long? value = literals.GetIntData( id );
			if ( !value.HasValue )
				throw new Exception( "Could not find literal int value with key: " + id );
			return (long)value;
		}

		private double GetLiteralFloatValue( int id )
		{
			double? value = literals.GetFloatData( id );
			if ( !value.HasValue )
				throw new Exception( "Could not find literal float value with key: " + id );
			return (double)value;
		}

		private string GetLiteralStringValue( int id )
		{
			string value = literals.GetStringData( id );
			if ( value == null )
				throw new Exception( "Could not find literal string value with key: " + id );
			return value;
		}

		private long GetVariableIntValue( int id )
		{
			long? value = variables.GetIntData( id );
			if ( !value.HasValue )
				throw new Exception( "Could not find int variable with key: " + id );
			return (long)value;
		}

		private double GetVariableFloatValue( int id )
		{
			double? value = variables.GetFloatData( id );
			if ( !value.HasValue )
				throw new Exception( "Could not find float variable with key: " + id );
			return (double)value;
		}

		private string GetVariableStringValue( int id )
		{
			string value = variables.GetStringData( id );
			if ( value == null )
				throw new Exception( "Could not find string variable with key: " + id );
			return value;
		}

		private void SetVariableIntValue( int id , long value )
		{
			variables.SetIntData( id , value );
		}

		private void SetVariableFloatValue( int id , double value )
		{
			variables.SetFloatData( id , value );
		}

		private void SetVariableStringValue( int id , string value )
		{
			variables.SetStringData( id , value );
		}

		public DataValue GetVariable( int id )
		{
			return null;
		}

		public DataValue GetLiteral( int id )
		{
			return null;
		}

		public void SetVariable( int id , DataValue value )
		{
		}
	}
}
