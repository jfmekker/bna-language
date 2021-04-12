using BNA.Operands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BNA
{

	/// <summary>
	/// Main program of the BNA compiler. Compiles files or terminal input.
	/// </summary>
	public class Program
	{
		private Statement[] Statements;
		private Dictionary<Token , Variable> Variables;

		public Program( Statement[] statements )
		{
			this.Statements = statements;
			this.Variables = new Dictionary<Token , Variable>( );
		}

		public void Run( )
		{
			int ip = 0;
			bool running = true;
			while ( running ) {
				// Instruction pointer
				if ( ip < 0 || ip >= Statements.Length )
					throw new RuntimeException( null, "Bad instruction pointer value ( " + ip + " )" );

				// Get current statement
				Statement curr = Statements[ip];
				if ( curr == null )
					throw new RuntimeException( null , "No statement at instruction pointer ( " + ip + " )" );

				// Get operand 1
				Operand op1;
				// TODO

				// Get operand 2
				Operand op2;
				// TODO

				// Execute statement
				switch ( curr.Type ) {
					case StatementType.OP_SET:
						// TODO

						// Get value of Op2

						// Set var Op1 value in dict

						break;

					case StatementType.OP_ADD:
						// TODO

						// Get value of Op2

						// Get value of Op1

						// Add values

						// Set var Op1 value in dict

						break;

					case StatementType.OP_PRINT:
						// TODO
						break;

					case StatementType.LABEL:
						// TODO
						break;

					default:
						throw new RuntimeException( curr, "Unexpected statement type ( " + curr.Type.ToString() + " )" );
				}

				// Next statement or end
				if ( ++ip == Statements.Length )
					running = false;
			}
		}
	}
}
