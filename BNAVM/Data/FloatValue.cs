using System;
using BNAB;

namespace BNAVM.Data
{
	public class FloatValue : DataValue
	{
		public double Value;

		public FloatValue( double value )
		{
			Value = value;
			Type = OperandDataType.FLOAT;
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
