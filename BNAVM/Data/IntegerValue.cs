using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNAVM.Data
{
	public class IntegerValue : DataValue
	{
		public long Value;

		public IntegerValue( long value )
		{
			Value = value;
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
