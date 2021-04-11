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
		/// <summary>
		/// Compiles a queue of BNA lines of code to statements [test].
		/// </summary>
		/// <param name="lines">The lines of BNA code to compile</param>
		/// <returns>String of the compile Python script</returns>
		public static void Compile( Queue<string> lines )
		{
			try {
				// Convert lines to token stream
				Queue<Token> tokens = Token.TokenizeProgram( lines );
				Console.WriteLine( "\nTokens :" );
				foreach ( Token t in tokens.ToList( ) ) {
					Console.WriteLine( "  " + t.ToString( ) );
				}
				Console.WriteLine( " " + tokens.Count + " total" );

				// Parse statements from token stream
				Queue<Statement> statements = Statement.ParseStatements( tokens );
				Console.WriteLine( "\nStatements :" );
				foreach ( Statement s in statements.ToList( ) ) {
					Console.WriteLine( "  " + s.ToString( ) );
				}
				Console.WriteLine( " " + statements.Count + " total" );
			}
			catch ( Exception e ) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Error.WriteLine( "BNA caught Exception while compiling!:" );
				Console.Error.WriteLine( e.ToString( ) );
				Console.ResetColor( );
			}
		}

		
	}
}
