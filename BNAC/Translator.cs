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
			if (statements.Any(s=>s.Type==Statement.StatementType.OP_SLEEP))
				str.AppendLine( "import time" );
			if ( statements.Any( s => s.Type == Statement.StatementType.OP_RAND ) )
				str.AppendLine( "import random" );
			if ( statements.Any( s => s.Type == Statement.StatementType.OP_GOTO ) )
				str.AppendLine( "from goto import with_goto" );
			str.AppendLine( );

			// Function def and intro
			if ( statements.Any( s => s.Type == Statement.StatementType.OP_GOTO ) )
				str.AppendLine( "@with_goto" );
			str.AppendLine( "def " + program_name + "():" );
			string indent = "\t";
			str.AppendLine( indent + "print(\"Generated from BNA code\")"  );

			// BNA code
			while ( statements.Count > 0 ) {
				var statement = statements.Dequeue( );
				switch ( statement.Type ) {
					// Set a variable
					case Statement.StatementType.OP_SET:
						str.AppendLine( indent + statement.Operand1.Value + " = " + statement.Operand2.Value );
						break;

					// Add to a variable
					case Statement.StatementType.OP_ADD:
						str.AppendLine( indent + statement.Operand1.Value + " += " + statement.Operand2.Value );
						break;

					// Subtract from a variable
					case Statement.StatementType.OP_SUB:
						str.AppendLine( indent + statement.Operand1.Value + " -= " + statement.Operand2.Value );
						break;

					// Multiply a variable
					case Statement.StatementType.OP_MUL:
						str.AppendLine( indent + statement.Operand1.Value + " *= " + statement.Operand2.Value );
						break;

					// Divide a variable
					case Statement.StatementType.OP_DIV:
						str.AppendLine( indent + statement.Operand1.Value + " /= " + statement.Operand2.Value );
						break;

					// Test a condition
					case Statement.StatementType.OP_TEST_GT:
						str.AppendLine( indent + "results = 1 if " + statement.Operand1.Value + " > " + statement.Operand2.Value + " else 0" );
						break;
					case Statement.StatementType.OP_TEST_LT:
						str.AppendLine( indent + "results = 1 if " + statement.Operand1.Value + " < " + statement.Operand2.Value + " else 0" );
						break;
					case Statement.StatementType.OP_TEST_EQ:
						str.AppendLine( indent + "results = 1 if " + statement.Operand1.Value + " = " + statement.Operand2.Value + " else 0" );
						break;

					// Get a random number
					case Statement.StatementType.OP_RAND:
						str.AppendLine( indent + statement.Operand1.Value + " = random.randint(0, " + statement.Operand2.Value + ")" );
						break;

					// Print a value
					case Statement.StatementType.OP_PRINT:
						str.AppendLine( indent + "print(" + statement.Operand1.Value + ")" );
						break;

					// Sleep for a time
					case Statement.StatementType.OP_SLEEP:
						str.AppendLine( indent + "time.sleep(" + statement.Operand1.Value + ")" );
						break;

					// Create a label
					case Statement.StatementType.LABEL:
						str.AppendLine( indent + "label ." + statement.Operand1.Value );
						break;

					// Goto a label on condition
					case Statement.StatementType.OP_GOTO:
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
