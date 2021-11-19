using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA.Values
{
	public class ReadFileValue : Value
	{
		public ReadFileValue( string filename ) => throw new NotImplementedException( );

		public override object Get => throw new NotImplementedException( );

		public override bool Equals( Value? other ) => throw new NotImplementedException( );

		public char? Read( ) => throw new NotImplementedException( );

		public string? ReadLine( ) => throw new NotImplementedException( );
	}
}
