using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA
{
	public class CompiletimeException : Exception
	{
		public readonly string Line;

		public readonly int LineNumber;

		public CompiletimeException( int line_number , string line , string message )
			: base( message + "\nCompile error on line " + line_number + ":\n\t" + line )
		{
			this.Line = line;
			this.LineNumber = line_number;
		}

		public CompiletimeException( string message )
			: base(message + "\nCompile error")
		{
			this.Line = null;
			this.LineNumber = -1;
		}
	}
}
