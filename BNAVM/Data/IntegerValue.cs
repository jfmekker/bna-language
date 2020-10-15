using System;
using BNAB;

namespace BNAVM.Data
{
	public class IntegerValue : DataValue
	{
		public long Value;

		public IntegerValue( long value )
		{
			Value = value;
			Type = OperandDataType.INTEGER;
		}

		public override DataValue ADD( DataValue op2 )
		{
			if ( !( op2 is IntegerValue ) && !( op2 is FloatValue ) )
				throw new InvalidOperationException( "Cannot ADD " + op2.GetType( ).ToString( ) + " to Integer." );

			long new_val = Value + ( op2 as IntegerValue ).Value;
			return new IntegerValue( new_val );
		}

		public override void PRINT( )
		{
			Console.WriteLine( Value );
		}
	}
}
