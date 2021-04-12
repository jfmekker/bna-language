using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA
{
	public class RuntimeException : Exception
	{
		public readonly Statement statement;

		public RuntimeException( Statement badGuy, string message )
			: base( message + "\nOffending Statement:\n"
				  + ( badGuy == null ? "none" : badGuy.ToString() ) )
		{
			this.statement = badGuy;
		}
	}
}
