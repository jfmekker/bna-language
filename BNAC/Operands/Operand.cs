using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA.Operands
{
	public abstract class Operand
	{
		public Token Token
		{
			get; protected set;
		}
	}
}
