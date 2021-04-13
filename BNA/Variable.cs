using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA
{
	public class Variable
	{
		public Token Identifier
		{
			get; private set;
		}

		public Value Value
		{
			get; set;
		}

		public Variable( Token token, Value value = new Value() )
		{
			this.Identifier = token;
			this.Value = value;
		}
		
	}
}
