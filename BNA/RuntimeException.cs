﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA
{
	public class RuntimeException : Exception
	{
		public readonly Statement Line;

		public readonly int LineNumber;

		public RuntimeException( int line_number, Statement badGuy, string message )
			: base( message + "\nRuntime error line " + line_number + ":\n\t" + ( badGuy == null ? "none" : badGuy.RawString( ) ) )
		{
			this.Line = badGuy;
			this.LineNumber = line_number;
		}

		public RuntimeException( string message )
			: base( message + "\nRuntime error" )
		{
			this.Line = null;
			this.LineNumber = -1;
		}
	}
}