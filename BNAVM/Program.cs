using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNAVM
{
	class Program
	{
		public static OperandHandler OperandHandler
		{
			get; private set;
		}

		public static InstructionExecutor InstructionExecutor
		{
			get; private set;
		}

		static void Main( string[] args )
		{
			Console.WriteLine( "================================================================================" );
			Console.WriteLine( "Welcome to the BNA's Not Assembly Virtual Machine!" );
			Console.WriteLine( "================================================================================" );


		}
	}
}
