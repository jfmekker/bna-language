using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA
{
	public enum VariableType
	{
		INTEGER = 1,
		FLOAT,
		STRING,
		LIST
	}

	public class Variable
	{
		public VariableType Type
		{
			get; private set;
		}

		public object Value
		{
			get; private set;
		}

		public Token Identifier
		{
			get; private set;
		}

		private Variable( )
		{
		}

		public static Variable GetVariable( Token token, Dictionary<Token, Variable> vars )
		{
			if ( token.Type != TokenType.VARIABLE )
				return null;

			// TODO check element?
			



			return null;
		}
	}
}
