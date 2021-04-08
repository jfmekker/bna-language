using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNAC
{
	public class CompiletimeException : Exception
	{
		public CompiletimeException( string message ) : base( message )
		{
		}
	}
}
