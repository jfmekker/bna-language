using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public static string ToPython( Queue<Statement> statements )
		{
			var str = new StringBuilder( );

			// Imports
			if (statements.Any(s=>s.Type==Statement.StatementType.OP_SLEEP))
				str.AppendLine( "import time" );
			if ( statements.Any( s => s.Type == Statement.StatementType.OP_RAND ) )
				str.AppendLine( "import random" );
			str.AppendLine( );

			// Intro
			str.AppendLine( "print(\"Generated from BNA code\")"  );

			// BNA code
			while ( statements.Count > 0 ) {
				var statement = statements.Dequeue( );
				switch ( statement.Type ) {
					// Set a variable
					case Statement.StatementType.OP_SET:
						str.AppendLine( statement.Operand1.Value + " = " + statement.Operand2.Value );
						break;

					// Add to a variable
					case Statement.StatementType.OP_ADD:
						str.AppendLine( statement.Operand1.Value + " += " + statement.Operand2.Value );
						break;

					// Subtract from a variable
					case Statement.StatementType.OP_SUB:
						str.AppendLine( statement.Operand1.Value + " -= " + statement.Operand2.Value );
						break;

					// Multiply a variable
					case Statement.StatementType.OP_MUL:
						str.AppendLine( statement.Operand1.Value + " *= " + statement.Operand2.Value );
						break;

					// Divide a variable
					case Statement.StatementType.OP_DIV:
						str.AppendLine( statement.Operand1.Value + " /= " + statement.Operand2.Value );
						break;

					// Get a random number
					case Statement.StatementType.OP_RAND:
						str.AppendLine( statement.Operand1.Value + " = random.randint(0, " + statement.Operand2.Value + ")" );
						break;

					// Print a value
					case Statement.StatementType.OP_PRINT:
						str.AppendLine( "print(" + statement.Operand1.Value + ")" );
						break;

					// Sleep for a time
					case Statement.StatementType.OP_SLEEP:
						str.AppendLine( "time.sleep(" + statement.Operand1.Value + ")" );
						break;

					// Create a label
					case Statement.StatementType.LABEL:
						str.AppendLine( "# There will be a label here eventually: " + statement.Operand1.Value );
						break;

					// Shouldn't happen
					default:
						throw new Exception( "Unexpected statement type: " + statement.ToString() );
				}
			}

			return str.ToString();
		}
	}
}
