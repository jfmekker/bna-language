using System;
using BNAB;
using BNAVM.Data;

namespace BNAVM
{
	/// <summary>
	/// Class for handling storage and editing of operands and data.
	/// </summary>
	class OperandHandler
	{
		/// <summary>
		/// Values read from a parsed <see cref="DataSegment"/> of a program.
		/// </summary>
		private DataSegment literals;

		/// <summary>
		/// Collection of all variables and their values in a program.
		/// </summary>
		private DataSegment variables;

		/// <summary>
		/// Construct a new <see cref="OperandHandler"/> instance.
		/// </summary>
		/// <param name="data">Data segment of literals from a program</param>
		public OperandHandler( DataSegment data )
		{
			literals = data;
			variables = new DataSegment( );
		}

		/// <summary>
		/// Find the data type of a literal by searching all the dictionaries.
		/// </summary>
		/// <param name="id">The dictionary key to search for</param>
		/// <returns>The first data type that has the given key</returns>
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

		/// <summary>
		/// Find the data type of a variable by searching all the dictionaries.
		/// </summary>
		/// <param name="id">The dictionary key to search for</param>
		/// <returns>The first data type that has the given key</returns>
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

		/// <summary>
		/// Get the value of a variable as a <see cref="DataValue"/> instance.
		/// </summary>
		/// <param name="id">Identifier key of the variable</param>
		/// <returns>Value of the variable</returns>
		public DataValue GetVariable( int id )
		{
			var type = FindVariableDataType( id );
			switch ( type ) {
				case OperandDataType.INTEGER:
					return new IntegerValue( GetVariableIntValue( id ) );
				case OperandDataType.FLOAT:
					return new FloatValue( GetVariableFloatValue( id ) );
				case OperandDataType.STRING:
					return new StringValue( GetVariableStringValue( id ) );
				default:
					throw new Exception( "Unexpected operand type: " + type );
			}
		}

		/// <summary>
		/// Get the value of a literal as a <see cref="DataValue"/> instance.
		/// </summary>
		/// <param name="id">Identifier key of the literal</param>
		/// <returns>Value of the literal</returns>
		public DataValue GetLiteral( int id )
		{
			var type = FindLiteralDataType( id );
			switch ( type ) {
				case OperandDataType.INTEGER:
					return new IntegerValue( GetLiteralIntValue( id ) );
				case OperandDataType.FLOAT:
					return new FloatValue( GetLiteralFloatValue( id ) );
				case OperandDataType.STRING:
					return new StringValue( GetLiteralStringValue( id ) );
				default:
					throw new Exception( "Unexpected operand type: " + type );
			}
		}

		/// <summary>
		/// Set a variable from a <see cref="DataValue"/> instance.
		/// </summary>
		/// <param name="id">Identifier key of the variable</param>
		/// <returns>Value of the variable</returns>
		public void SetVariable( int id , DataValue value )
		{
			var type = value.Type;
			switch ( type ) {
				case OperandDataType.INTEGER:
					SetVariableIntValue( id , ( value as IntegerValue ).Value );
					break;
				case OperandDataType.FLOAT:
					SetVariableFloatValue( id , ( value as FloatValue ).Value );
					break;
				case OperandDataType.STRING:
					SetVariableStringValue( id , ( value as StringValue ).Value );
					break;
				default:
					throw new Exception( "Unexpected operand type: " + type );
			}
		}
	}
}
