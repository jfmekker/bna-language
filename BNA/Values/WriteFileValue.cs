using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA.Values
{
	public class WriteFileValue : Value
	{
		public WriteFileValue( string filename ) => throw new NotImplementedException( );

		public override object Get => throw new NotImplementedException( );

		public override bool Equals( Value? other ) => throw new NotImplementedException( );

		public void Write(string str) => throw new NotImplementedException( );

		public void WriteLine( string str ) => throw new NotImplementedException( );
	}
}
