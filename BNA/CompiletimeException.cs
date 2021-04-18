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

		public readonly string BNAMessage;

		public CompiletimeException( int line_number , string line , string message )
			: base( message + "\nCompile error on line " + line_number + ":\n\t" + line )
		{
			this.Line = line;
			this.LineNumber = line_number;
			this.BNAMessage = message;
		}

		public CompiletimeException( CompiletimeException exception , int line_number , string line )
			: base( exception.BNAMessage + "\nCompile error on line " + line_number + ":\n\t" + line , exception )
		{
			this.Line = line;
			this.LineNumber = line_number;
			this.BNAMessage = exception.BNAMessage;
		}

		public CompiletimeException( string message )
			: base(message + "\nCompile error")
		{
			this.Line = null;
			this.LineNumber = -1;
			this.BNAMessage = message;
		}
	}
}
