using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNAVM.Data
{
	public abstract class DataValue
	{
		public abstract DataValue ADD( DataValue op2 );

		public abstract void PRINT( );
	}
}
