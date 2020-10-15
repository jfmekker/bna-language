using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNAB
{
	/// <summary>
	/// Enum for all operation codes
	/// </summary>
	public enum OpCode : byte
	{
		SET = 0x01,
		ADD,
		PRINT,
		// TODO
	}
}
