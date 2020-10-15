using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BNAB;

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

		public OperandDataType FindLiteralDataType( int id )
		{
			if ( literals.GetIntData( id ).HasValue )
				return OperandDataType.INTEGER;
			else if ( literals.GetFloatData( id ).HasValue )
				return OperandDataType.FLOAT;
			else if ( literals.GetStringData( id ) != null )
				return OperandDataType.STRING;

			throw new Exception( "Could not find literal with key: " + id );
		}

		public OperandDataType FindVariableDataType( int id )
		{
			if ( variables.GetIntData( id ).HasValue )
				return OperandDataType.INTEGER;
			else if ( variables.GetFloatData( id ).HasValue )
				return OperandDataType.FLOAT;
			else if ( variables.GetStringData( id ) != null )
				return OperandDataType.STRING;

			throw new Exception( "Could not find variable with key: " + id );
		}

		public long GetLiteralIntValue( int id )
		{
			long? value = literals.GetIntData( id );
			if ( !value.HasValue )
				throw new Exception( "Could not find literal int value with key: " + id );
			return (long)value;
		}

		public double GetLiteralFloatValue( int id )
		{
			double? value = literals.GetFloatData( id );
			if ( !value.HasValue )
				throw new Exception( "Could not find literal float value with key: " + id );
			return (double)value;
		}

		public string GetLiteralStringValue( int id )
		{
			string value = literals.GetStringData( id );
			if ( value == null )
				throw new Exception( "Could not find literal string value with key: " + id );
			return value;
		}

		public long GetVariableIntValue( int id )
		{
			long? value = variables.GetIntData( id );
			if ( !value.HasValue )
				throw new Exception( "Could not find int variable with key: " + id );
			return (long)value;
		}

		public double GetVariableFloatValue( int id )
		{
			double? value = variables.GetFloatData( id );
			if ( !value.HasValue )
				throw new Exception( "Could not find float variable with key: " + id );
			return (double)value;
		}

		public string GetVariableStringValue( int id )
		{
			string value = variables.GetStringData( id );
			if ( value == null )
				throw new Exception( "Could not find string variable with key: " + id );
			return value;
		}

		public void SetVariableIntValue( int id , long value )
		{
			variables.SetIntData( id , value );
		}

		public void SetVariableFloatValue( int id , double value )
		{
			variables.SetFloatData( id , value );
		}

		public void SetVariableStringValue( int id , string value )
		{
			variables.SetStringData( id , value );
		}
	}
}
