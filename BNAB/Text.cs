using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNAB
{
	public class TextSegment
	{
		private ArraySegment<ulong> _raw;

		public TextSegment( ArraySegment<ulong> raw )
		{
			_raw = raw;
			// TODO more?
		}
	}

	public class Instruction
	{
	}
}
