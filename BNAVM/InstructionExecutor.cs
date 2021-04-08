using System;
using BNAB;
using BNAVM.Data;

namespace BNAVM
{
	/// <summary>
	/// Class for handling execution of instructions.
	/// Uses the generic DataValue data type for all operations.
	/// </summary>
	class InstructionExecutor
	{
		/// <summary>
		/// Sequential array of <see cref="Instruction"/> representing the
		/// program to run.
		/// </summary>
		private Instruction[] program;

		/// <summary>
		/// The instruction pointer, an index to <see cref="program"/> array.
		/// </summary>
		private int ip = 0;

		/// <summary>
		/// Error code to return on exiting the program.
		///  0	==	Success
		/// -1	==	Failure
		/// </summary>
		private int return_code = 0;

		/// <summary>
		/// Construct a new <see cref="InstructionExecutor"/> instance from a
		/// parsed <see cref="TextSegment"/>.
		/// </summary>
		/// <param name="text">TextSegment of a program to run</param>
		public InstructionExecutor( TextSegment text )
		{
			program = text.ToArray( );
		}

		/// <summary>
		/// Execute the instructions in <see cref="program"/> starting at 0.
		/// Is *not* guaranteed to complete, based on the given program (loops
		/// are possibly infinite.
		/// </summary>
		/// <returns>The return code of the program</returns>
		public int ExecuteProgram( )
		{
			bool running = true;
			while ( running ) {

				try {
					ExecuteInstruction( );

					if ( ip == program.Length ) {
						running = false;
					}
					else {
						ip += 1;
					}
				}
				catch (Exception e) {
					Console.Error.WriteLine( e.Message );
					return_code = -1;
					running = false;
				}

			}

			return return_code;
		}

		/// <summary>
		/// Execute the <see cref="Instruction"/> in <see cref="program"/>
		/// pointed to by <see cref="ip"/>.
		/// </summary>
		private void ExecuteInstruction( )
		{
			if ( ip < 0 || ip >= program.Length )
				throw new Exception( "Instruction pointer at invalid index: " + ip );

			Instruction instr = program[ip];

			// TODO point this class and OperandHandler to a Program instance (not global variables)
			DataValue op1 = Program.OperandHandler.GetVariable( instr.Operand1 );
			DataValue op2;

			if ( (OperandType)instr.Op2Type == OperandType.LITERAL ) {
				op2 = Program.OperandHandler.GetLiteral( instr.Operand2 );
			}
			else if ( (OperandType)instr.Op2Type == OperandType.VARIABLE ) {
				op2 = Program.OperandHandler.GetVariable( instr.Operand2 );
			}
			//else if ( (OperandType)instr.Op2Type == OperandType.SMALL_LITERAL ) {
			//	op2 = new IntegerValue( instr.Operand2 );
			//}
			else {
				op2 = null;
			}


			switch ( instr.OpCode ) {
				case OpCode.SET:
					Program.OperandHandler.SetVariable( instr.Operand1 , op2 );
					break;
				case OpCode.ADD:
					Program.OperandHandler.SetVariable( instr.Operand1 , op1.ADD( op2 ) );
					break;
				case OpCode.PRINT:
					op1.PRINT( );
					break;
				
				// TODO more cases

				default:
					throw new RuntimeException("Unexpected instruction op code.", ip, instr);
			}
		}
	}
}
