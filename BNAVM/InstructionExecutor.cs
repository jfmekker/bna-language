using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BNAB;

namespace BNAVM
{
	class InstructionExecutor
	{
		private Instruction[] program;

		private int ip = 0;

		private int return_code;

		public InstructionExecutor( TextSegment text )
		{
			program = text.ToArray( );
		}

		public int ExecuteProgram( )
		{
			bool running = true;
			while ( running ) {

				ExecuteInstruction( );

				if ( ip == program.Length ) {
					running = false;
				}
				else {
					ip += 1;
				}
			}

			return return_code;
		}

		private void ExecuteInstruction( )
		{
			Instruction instr = program[ip];

			switch ( instr.OpCode ) {
				case OpCode.SET:
				case OpCode.ADD:
				case OpCode.PRINT:
				default:
					throw new Exception("Unexpected instruction op code.");
			}
		}
	}
}
