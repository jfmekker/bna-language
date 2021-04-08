using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BNAB;

namespace BNAVM
{
	public class RuntimeException : Exception
	{
		public int InstructionNumber
		{
			get; private set;
		}

		public Instruction BadInstruction
		{
			get; private set;
		}

		public RuntimeException( string message, int instructionNumber , Instruction badInstruction )
			: base ( message )
		{
			this.InstructionNumber = instructionNumber;
			this.BadInstruction = badInstruction;
		}

		public override string ToString( )
		{
			return "RuntimeException at instruction " + this.InstructionNumber + ": " + this.BadInstruction.ToString( ) + "\n" + base.ToString( );
		}
	}
}
