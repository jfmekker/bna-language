using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BNAB;

namespace BNAC
{
	class Compiler
	{
		public static Binary CompileStatements( Queue<Statement> statement_queue )
		{
			Statement[] statements = statement_queue.ToArray( );

			var data = new DataSegment();
			var text = new TextSegment();

			var dataIDs = new Dictionary<Token , int>( );

			for ( int i = 0 ; i < statements.Length ; i += 1 ) {

				var s = statements[i];

				OpCode op_code = s.GetOpCode();

				int op1 = 0, op2 = 0;
				OperandType op2_type = OperandType.VARIABLE;

				switch ( s.Type ) {

					// no first operand
					case StatementType.OP_WAIT:
					case StatementType.OP_PRINT:
					{
						// Operand 2
						// TODO check for small literals
						Token t = s.Operand2;
						if ( dataIDs.TryGetValue( t , out int id ) ) {
							op2 = id;
						}
						else {
							id = data.GetNextId( );
							op2 = id;
							dataIDs.Add( t , id );
						}
						break;
					}

					// no second operand
					case StatementType.OP_NEG:
					case StatementType.OP_ROUND:
					case StatementType.OP_CLOSE:
					case StatementType.OP_GOTO:
					case StatementType.LABEL:
					{
						// TODO ?
						Token t = s.Operand1;
						if ( t.Type != TokenType.VARIABLE )
							throw new CompiletimeException( "First operand must be variable." );

						if ( dataIDs.TryGetValue( t , out int id ) ) {
							op1 = id;
						}
						else {
							id = data.GetNextId( );
							op1 = id;
							dataIDs.Add( t , id );
						}

						break;
					}
						

					// both operands
					case StatementType.OP_SET:
					case StatementType.OP_ADD:
					{
						// Operand 1
						Token t = s.Operand1;
						if ( t.Type != TokenType.VARIABLE )
							throw new CompiletimeException( "First operand must be variable." );

						if ( dataIDs.TryGetValue( t , out int id ) ) {
							op1 = id;
						}
						else {
							id = data.GetNextId( );
							op1 = id;
							dataIDs.Add( t , id );
						}

						// Operand 2
						// TODO check for small literals
						t = s.Operand2;
						if ( dataIDs.TryGetValue( t , out id ) ) {
							op2 = id;
						}
						else {
							id = data.GetNextId( );
							op2 = id;
							dataIDs.Add( t , id );
						}
						break;
					}
					
					default:
						throw new CompiletimeException( "Unknown statement type in compiler: " + s.Type );
				}

				// Add instruction to Text
				var inst = new Instruction( op_code , op1 , op2_type , op2 );
				text.AddInstruction( inst );

			}

			// Fill the Data segment
			foreach ( KeyValuePair<Token, int> t in dataIDs ) {
				// Initialize the value if a literal
				long val = 0;
				if ( t.Key.Type == TokenType.LITERAL ) { // TODO check other data types
					val = long.Parse( t.Key.Value );
				}

				data.SetIntData( t.Value , val );
			}

			data.WriteEntries( );
			text.WriteInstructions( );
			var binary = new Binary( data , text );
			return binary;
		}
	}
}
