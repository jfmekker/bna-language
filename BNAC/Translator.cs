using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BNAC
{
	/// <summary>
	/// Coverts BNA statements to code
	/// </summary>
	class Translator
	{
		/// <summary>
		/// Convert queue of Statements to Python code
		/// </summary>
		/// <param name="statements">Queue of BNA statements</param>
		/// <returns>Python script in a string</returns>
		public static string ToPython( Queue<Statement> statements, string program_name = "program" )
		{
			var str = new StringBuilder( );

			// Imports
			//	Will need to run "pip install goto-statement" before using goto statements
			if (statements.Any(s=>s.Type==StatementType.OP_WAIT))
				str.AppendLine( "import time" );
			if ( statements.Any( s => s.Type == StatementType.OP_RAND ) )
				str.AppendLine( "import random" );
			if ( statements.Any( s => s.Type == StatementType.OP_LOG ) )
				str.AppendLine( "import math" );
			if ( statements.Any( s => s.Type == StatementType.OP_GOTO ) )
				str.AppendLine( "from goto import with_goto" );
			str.AppendLine( );

			// Function def and intro
			if ( statements.Any( s => s.Type == StatementType.OP_GOTO ) )
				str.AppendLine( "@with_goto" );
			str.AppendLine( "def " + program_name + "():" );
			string indent = "\t";
			str.AppendLine( indent + "print(\"Generated from BNA code\")"  );

			// BNA code
			while ( statements.Count > 0 ) {
				var statement = statements.Dequeue( );
				switch ( statement.Type ) {
					// Math operations
					case StatementType.OP_SET:
						str.AppendLine( indent + statement.Operand1.Value + " = " + statement.Operand2.Value );
						break;
					case StatementType.OP_ADD:
						str.AppendLine( indent + statement.Operand1.Value + " += " + statement.Operand2.Value );
						break;
					case StatementType.OP_SUB:
						str.AppendLine( indent + statement.Operand1.Value + " -= " + statement.Operand2.Value );
						break;
					case StatementType.OP_MUL:
						str.AppendLine( indent + statement.Operand1.Value + " *= " + statement.Operand2.Value );
						break;
					case StatementType.OP_DIV:
						str.AppendLine( indent + statement.Operand1.Value + " /= " + statement.Operand2.Value );
						break;
					case StatementType.OP_OR:
						str.AppendLine( indent + statement.Operand1.Value + " |= " + statement.Operand2.Value );
						break;
					case StatementType.OP_AND:
						str.AppendLine( indent + statement.Operand1.Value + " &= " + statement.Operand2.Value );
						break;
					case StatementType.OP_XOR:
						str.AppendLine( indent + statement.Operand1.Value + " ^= " + statement.Operand2.Value );
						break;
					case StatementType.OP_NEG:
						str.AppendLine( indent + statement.Operand1.Value + " = ~" + statement.Operand1.Value );
						break;
					case StatementType.OP_POW:
						str.AppendLine( indent + statement.Operand1.Value + " **= " + statement.Operand2.Value );
						break;
					case StatementType.OP_MOD:
						str.AppendLine( indent + statement.Operand1.Value + " %= " + statement.Operand2.Value );
						break;
					case StatementType.OP_LOG:
						str.AppendLine( indent + statement.Operand1.Value + " = math.log(" + statement.Operand1.Value + ", " + statement.Operand2.Value + ")" );
						break;
					case StatementType.OP_ROUND:
						str.AppendLine( indent + statement.Operand1.Value + " = round(" + statement.Operand1.Value + ")" );
						break;
					case StatementType.OP_LIST:
						str.AppendLine( indent + statement.Operand1.Value + " = [0] * " + statement.Operand2.Value );
						break;
					
					// Test operations
					case StatementType.OP_TEST_GT:
						string op1 = statement.Operand1.Value;
						string op2 = statement.Operand2.Value;
						if ( statement.Operand1.Type == TokenType.STRING || statement.Operand2.Type == TokenType.STRING ) {
							op1 = "str(" + statement.Operand1.Value + ")";
							op2 = "str(" + statement.Operand2.Value + ")";
						}
						str.AppendLine( indent + "SUCCESS = 1 if " + statement.Operand1.Value + " > " + statement.Operand2.Value + " else 0" );
						break;
					case StatementType.OP_TEST_LT:
						str.AppendLine( indent + "SUCCESS = 1 if " + statement.Operand1.Value + " < " + statement.Operand2.Value + " else 0" );
						break;
					case StatementType.OP_TEST_EQ:
						str.AppendLine( indent + "SUCCESS = 1 if " + statement.Operand1.Value + " = " + statement.Operand2.Value + " else 0" );
						break;

					// Misc operations
					case StatementType.OP_RAND:
						str.AppendLine( indent + statement.Operand1.Value + " = random.randint(0, " + statement.Operand2.Value + ")" );
						break;
					case StatementType.OP_PRINT:
						str.AppendLine( indent + "print(" + statement.Operand1.Value + ")" );
						break;
					case StatementType.OP_WAIT:
						str.AppendLine( indent + "time.sleep(" + statement.Operand1.Value + ")" );
						break;

					// Control flow
					case StatementType.LABEL:
						str.AppendLine( indent + "label ." + statement.Operand1.Value );
						break;
					case StatementType.OP_GOTO:
						str.AppendLine( indent + "if " + statement.Operand2.Value + " != 0 :" );
						str.AppendLine( indent + "\tgoto ." + statement.Operand1.Value );
						break;

					// Shouldn't happen
					default:
						throw new Exception( "Unexpected statement type: " + statement.ToString() );
				}
			}

			// Run the program
			str.AppendLine( );
			str.AppendLine( program_name + "()" );

			return str.ToString();
		}
	}
}
