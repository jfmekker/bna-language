using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BNAC
{
	/// <summary>
	/// Coverts BNA statements to code
	/// </summary>
	internal class Translator
	{
		/// <summary>
		/// Get an operand of a statement translated to Python.
		/// </summary>
		/// <param name="statement">Statement to take operand from</param>
		/// <param name="operand">which operand to translate (defaults to 1)</param>
		/// <returns>Python translation of a statement's operand</returns>
		private static string PythonOperand( Statement statement , int operand = 1 )
		{
			string variable = operand == 1 ? statement.Operand1.Value : statement.Operand2.Value;
			if ( operand == 1 ) {
				variable = statement.Operand1.Value;
			}
			else if ( operand == 2 ) {
				variable = statement.Operand2.Value;
			}

			if ( variable.Contains( '@' ) ) {
				var regex = new System.Text.RegularExpressions.Regex( "^([A-Za-z_])+@([A-Za-z_]|[0-9])+$" );

				if ( regex.IsMatch( variable ) ) {
					int index = variable.IndexOf( '@' );
					string list_part = variable.Substring( 0 , index );
					string access_part = variable.Substring( index + 1 , variable.Length - ( index + 1 ) );
					return list_part + "[" + access_part + "]";
				}
				else {
					throw new Exception( "Found accessor in variable, but operand doesn't match list access." );
				}
			}
			return variable;
		}

		/// <summary>
		/// Convert queue of Statements to Python code
		/// </summary>
		/// <param name="statements">Queue of BNA statements</param>
		/// <returns>Python script in a string</returns>
		public static string ToPython( Queue<Statement> statements , string program_name = "program" )
		{
			var str = new StringBuilder( );

			// Imports
			//	Will need to run "pip install goto-statement" before using goto statements
			if ( statements.Any( s => s.Type == StatementType.OP_WAIT ) ) {
				str.AppendLine( "import time" );
			}

			if ( statements.Any( s => s.Type == StatementType.OP_RAND ) ) {
				str.AppendLine( "import random" );
			}

			if ( statements.Any( s => s.Type == StatementType.OP_LOG ) ) {
				str.AppendLine( "import math" );
			}

			if ( statements.Any( s => s.Type == StatementType.OP_GOTO ) ) {
				str.AppendLine( "from goto import with_goto" );
			}

			str.AppendLine( );

			// Function def and intro
			if ( statements.Any( s => s.Type == StatementType.OP_GOTO ) ) {
				str.AppendLine( "@with_goto" );
			}

			str.AppendLine( "def " + program_name + "():" );
			string indent = "\t";
			str.AppendLine( indent + "print(\"Generated from BNA code\")" );

			// BNA code
			while ( statements.Count > 0 ) {
				Statement statement = statements.Dequeue( );
				switch ( statement.Type ) {
					// Math operations
					case StatementType.OP_SET:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + " = " + PythonOperand( statement , 2 ) );
						break;
					case StatementType.OP_ADD:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + " += " + PythonOperand( statement , 2 ) );
						break;
					case StatementType.OP_SUB:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + " -= " + PythonOperand( statement , 2 ) );
						break;
					case StatementType.OP_MUL:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + " *= " + PythonOperand( statement , 2 ) );
						break;
					case StatementType.OP_DIV:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + " /= " + PythonOperand( statement , 2 ) );
						break;
					case StatementType.OP_OR:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + " |= " + PythonOperand( statement , 2 ) );
						break;
					case StatementType.OP_AND:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + " &= " + PythonOperand( statement , 2 ) );
						break;
					case StatementType.OP_XOR:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + " ^= " + PythonOperand( statement , 2 ) );
						break;
					case StatementType.OP_NEG:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + " = ~" + PythonOperand( statement , 1 ) );
						break;
					case StatementType.OP_POW:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + " **= " + PythonOperand( statement , 2 ) );
						break;
					case StatementType.OP_MOD:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + " %= " + PythonOperand( statement , 2 ) );
						break;
					case StatementType.OP_LOG:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + " = math.log(" + PythonOperand( statement , 1 ) + ", " + PythonOperand( statement , 2 ) + ")" );
						break;
					case StatementType.OP_ROUND:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + " = round(" + PythonOperand( statement , 1 ) + ")" );
						break;
					case StatementType.OP_LIST:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + " = [0] * " + PythonOperand( statement , 2 ) );
						break;
					case StatementType.OP_APPEND:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + ".append(" + PythonOperand( statement , 2 ) + ")" );
						break;

					// Test operations
					case StatementType.OP_TEST_GT:
						str.AppendLine( indent + "success = 1 if " + PythonOperand( statement , 1 ) + " > " + PythonOperand( statement , 2 ) + " else 0" );
						break;
					case StatementType.OP_TEST_LT:
						str.AppendLine( indent + "success = 1 if " + PythonOperand( statement , 1 ) + " < " + PythonOperand( statement , 2 ) + " else 0" );
						break;
					case StatementType.OP_TEST_EQ:
						str.AppendLine( indent + "success = 1 if " + PythonOperand( statement , 1 ) + " = " + PythonOperand( statement , 2 ) + " else 0" );
						break;

					// Misc operations
					case StatementType.OP_RAND:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + " = random.randint(0, " + PythonOperand( statement , 2 ) + ")" );
						break;
					case StatementType.OP_PRINT:
						str.AppendLine( indent + "print(" + PythonOperand( statement , 1 ) + ")" );
						break;
					case StatementType.OP_WAIT:
						str.AppendLine( indent + "time.sleep(" + PythonOperand( statement , 1 ) + ")" );
						break;

					// Control flow
					case StatementType.LABEL:
						str.AppendLine( indent + "label ." + PythonOperand( statement , 1 ) );
						break;
					case StatementType.OP_GOTO:
						str.AppendLine( indent + "if " + PythonOperand( statement , 2 ) + " != 0 :" );
						str.AppendLine( indent + "\tgoto ." + PythonOperand( statement , 1 ) );
						break;

					// IO
					case StatementType.OP_OPEN:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + " = open(" + PythonOperand( statement , 2 ) + ")" );
						break;
					case StatementType.OP_CLOSE:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + ".close()" );
						break;
					case StatementType.OP_WRITE:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + ".write(" + PythonOperand( statement , 2 ) + ")" );
						break;
					case StatementType.OP_READ:
						str.AppendLine( indent + PythonOperand( statement , 1 ) + " = " + PythonOperand( statement , 2 ) + ".readline()" );
						break;

					// Shouldn't happen
					default:
						throw new Exception( "Unexpected statement type: " + statement.ToString( ) );
				}
			}

			// Run the program
			str.AppendLine( );
			str.AppendLine( program_name + "()" );

			return str.ToString( );
		}
	}
}
