using System.Collections.Generic;
using BNA.Common;

namespace BNA.Run
{
	public interface IProgram
	{
		public int IP { get; set; }

		public bool Running { get; set; }

		public Dictionary<Token , int> Labels { get; }
	}
}
