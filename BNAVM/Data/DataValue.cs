using BNAB;

namespace BNAVM.Data
{
	public abstract class DataValue
	{
		public OperandDataType Type
		{
			get; protected set;
		}

		public abstract DataValue ADD( DataValue op2 );

		public abstract void PRINT( );
	}
}
