using System;
using BNAB;

namespace BNAVM.Data
{
	class StringValue : DataValue
	{
		public string Value;

		public StringValue( string value )
		{
			Value = value;
			Type = OperandDataType.STRING;
		}

		public override DataValue ADD( DataValue op2 )
		{
			throw new NotImplementedException( );
		}

		public override void PRINT( )
		{
			throw new NotImplementedException( );
		}
	}
}
