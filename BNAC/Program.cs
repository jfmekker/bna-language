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
		private int IP;
		private List<Variable> Variables;

		public Program( Statement[] statements )
		{
			this.Statements = statements;
			this.IP = 0;
			this.Variables = new List<Variable>( );
		}

		public void Run( )
		{
			bool running = true;
			while ( running ) {
				// Instruction pointer
				if ( IP < 0 || IP >= Statements.Length )
					throw new RuntimeException( null, "Bad instruction pointer value ( " + IP + " )" );

				// Get current statement
				Statement curr = Statements[IP];
				if ( curr == null )
					throw new RuntimeException( null , "No statement at instruction pointer ( " + IP + " )" );
				//Console.WriteLine( "Running statement: " + curr.ToString( ) );

				Variable op1 = GetVariable( curr.Operand1 );
				Value op2 = GetValue( curr.Operand2 );

				// Execute statement
				switch ( curr.Type ) {
					// Set operation
					case StatementType.OP_SET:
						if ( op1 == null ) {
							op1 = new Variable( curr.Operand1 );
							Variables.Add( op1 );
						}
						op1.Value = op2;
						break;

					// Arithmetic operations
					case StatementType.OP_ADD:
					case StatementType.OP_SUB:
					case StatementType.OP_MUL:
					case StatementType.OP_DIV:
						if ( op1 == null )
							throw new RuntimeException( Statements[IP] , "Cannot use variable that has not been set: " + curr.Operand1.ToString( ) );
						op1.Value = DoArithmeticOperation( op1.Value , op2 , curr.Type );
						break;

					// No side effect operations
					case StatementType.OP_PRINT:
						// TODO different print stream ?
						Console.WriteLine( "BNA: " + op2.ToString( ) );
						break;
					case StatementType.OP_WAIT:
						op2 = GetValue( curr.Operand1 );
						if ( op2.Type != ValueType.INTEGER || op2.Type != ValueType.FLOAT )
							throw new RuntimeException( Statements[IP] , "Argument to WAIT must be numeric: " + op2.ToString( ) );
						try {
							System.Threading.Thread.Sleep( (int)( 1000 * (double)op2.Val ) );
						}
						catch (Exception e) {
							throw new RuntimeException( Statements[IP] , "Exception caught while waiting:\n" + e.Message );
						}
						break;
					case StatementType.LABEL:
						// Do nothing
						break;

					default:
						throw new RuntimeException( curr, "Unexpected statement type ( " + curr.Type.ToString() + " )" );
				}

				// Next statement or end
				if ( ++IP == Statements.Length )
					running = false;
			}
		}

		private Value GetValue( Token token )
		{
			switch ( token.Type ) {
				// Parse the literal value from the token
				case TokenType.LITERAL:
					if ( long.TryParse( token.Value , out long lval ) )
						return new Value( ValueType.INTEGER , lval );
					else if ( double.TryParse( token.Value , out double dval ) )
						return new Value( ValueType.FLOAT , dval );
					else
						throw new RuntimeException( Statements[IP] , "Could not parse value from literal: " + token );

				// Capture the string contents
				case TokenType.STRING:
					if ( token.Value.Length < 2 )
						throw new RuntimeException( Statements[IP] , "String token too short to be valid: " + token );
					return new Value( ValueType.STRING , token.Value.Substring( 1 , token.Value.Length - 2) );

				// Get the Value of a variable
				case TokenType.VARIABLE:
					// TODO
					if ( token.Value.Contains( Symbol.ACCESSOR.ToString( ) ) ) return default; // TODO is element? (change token to x@# if so)

					// Find variable
					foreach ( Variable v in Variables ) {
						if ( v.Identifier.Equals( token ) ) {
							return v.Value;
						}
					}

					throw new RuntimeException( Statements[IP] , "No value yet for variable operand: " + token.ToString( ) );

				// Null operands for single operand statements
				case TokenType.NULL:
					return default;

				default:
					throw new RuntimeException( Statements[IP] , "Unexpected token type for operand: " + token.ToString( ) );
			}

			throw new RuntimeException( Statements[IP] , "Failed to get value for operand: " + token.ToString( ) );
		}

		private Variable GetVariable( Token id )
		{
			// TODO
			//if ( id.Value.Contains( Symbol.ACCESSOR.ToString( ) ) )
			//	return null; // TODO is element? (change token to x@# if so)

			// Find variable
			foreach ( Variable v in Variables ) {
				if ( v.Identifier.Equals( id ) ) {
					return v;
				}
			}

			return null;
		}

		public Value DoArithmeticOperation( Value op1 , Value op2 , StatementType operation )
		{
			// TODO
			return new Value();
		}
	}
}
