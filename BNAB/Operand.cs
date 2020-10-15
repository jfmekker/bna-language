using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNAB
{
	/// <summary>
	/// 
	/// </summary>
	public enum OperandType
	{
		VARIABLE = 1,
		LITERAL,
		SMALL_LITERAL
	}

	/// <summary>
	/// 
	/// </summary>
	public enum OperandDataType
	{
		INTEGER = 1,
		FLOAT,
		STRING,
		// TODO add LIST and ELEMENT
	}
}
