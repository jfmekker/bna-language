using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA.Values
{
	public class FileValue : Value
	{
		public override object Get => throw new NotImplementedException( );

		public override Value DoOperation( StatementType operation , Value op2 ) => throw new NotImplementedException( );
		public override bool Equals( Value? other ) => throw new NotImplementedException( );
		public override int GetHashCode( ) => throw new NotImplementedException( );
		public override string ToString( ) => throw new NotImplementedException( );
	}
}
