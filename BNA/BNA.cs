using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA
{
	public class BNA
	{
		/// <summary>
		/// Compiles a queue of BNA lines of code to statements [test].
		/// </summary>
		/// <param name="lines">The lines of BNA code to compile</param>
		/// <returns>String of the compile Python script</returns>
		public static Statement[] Compile( Queue<string> lines )
		{
			// Convert lines to token stream
			Queue<Token> tokens = Token.TokenizeProgram( lines );
			Debug.AddLine( "\nTokens :" );
			foreach ( Token t in tokens.ToList( ) ) {
				Debug.AddLine( "  " + t.ToString( ) );
			}
			Debug.AddLine( " " + tokens.Count + " total" );

			// Parse statements from token stream
			Queue<Statement> statements = Statement.ParseStatements( tokens );
			Debug.AddLine( "\nStatements :" );
			foreach ( Statement s in statements.ToList( ) ) {
				Debug.AddLine( "  " + s.ToString( ) );
			}
			Debug.AddLine( " " + statements.Count + " total" );

			// Return array of statements
			return statements.ToArray( );
		}

		/// <summary>
		/// Compile a function to Python, or take input from the terminal
		/// </summary>
		/// <param name="args">Names of files to compile to Python, can be none.</param>
		public static void Main( string[] args )
		{
			Console.WriteLine( "================================================================================" );
			Console.WriteLine( "Welcome to the BNA's Not Assembly Interpreter!" );
			Console.WriteLine( "================================================================================" );

			// If no arguments, take input from command line
			if ( args.Length == 0 ) {
				while ( true ) {
					// Usage
					Console.WriteLine( "\nInsert BNA code to do stuff (use '~' to end):" );

					// Read and queue lines
					var lines = new Queue<string>( );
					while ( true ) {
						// get a line
						string input = Console.ReadLine( );

						// end on tilda '~'
						if ( input.Equals( "~" ) ) {
							break;
						}

						lines.Enqueue( input );
					}

					// Compile to program and run
					try {
						Console.WriteLine( "Compiling Program..." );
						var prog = new Program( Compile( lines ) );
						Console.WriteLine( "\nRunning Program...\n" );
						prog.Run( );
						Console.WriteLine( );
					}
					catch (CompiletimeException e) {
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine( "Compiletime Exception caught:" );
						Console.WriteLine( e.Message );
						Console.ResetColor( );
					}
					catch (RuntimeException e) {
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine( "Runtime Exception caught:" );
						Console.WriteLine( e.Message );
						Console.ResetColor( );
					}
					catch (NotImplementedException e) {
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine( "Not Implemented Exception caught:" );
						Console.WriteLine( e.Message );
						Console.ResetColor( );
					}

					Console.WriteLine( "Press enter to continue (use '~' to exit)." );

					// Wait to continue, check for exit
					if ( Console.ReadLine( ).Equals( "~" ) ) {
						break;
					}
				}
			}
			// else, take in files
			else {
				foreach ( string file in args ) {
					// Output the BNA filename
					Console.WriteLine( file + ":" );

					// Check the file extension
					string[] split_filename = file.Split( new char[] { '.' } );
					string filename = split_filename[0];
					string extension = split_filename[1];
					if ( !extension.Equals( "bna" ) ) {
						throw new Exception( "Wrong file type: " + file );
					}

					// Output file contents
					Console.WriteLine( "Reading file..." );
					var lines = new Queue<string>( );
					using ( StreamReader sr = File.OpenText( ".\\" + file ) ) {
						string line;
						while ( ( line = sr.ReadLine( ) ) != null ) {
							Console.WriteLine( line );
							lines.Enqueue( line );
						}
					}

					// Compile to program and run
					try {
						Console.WriteLine( "Compiling Program..." );
						var prog = new Program( Compile( lines ) );
						Console.WriteLine( "\nRunning Program...\n" );
						prog.Run( );
						Console.WriteLine( );
					}
					catch ( CompiletimeException e ) {
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine( "Compiletime Exception caught:" );
						Console.WriteLine( e.Message );
						Console.ResetColor( );
					}
					catch ( RuntimeException e ) {
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine( "Runtime Exception caught:" );
						Console.WriteLine( e.Message );
						Console.ResetColor( );
					}

					Console.WriteLine( "\nDone with file.\n" );
				}

				// Wait to close so user can read output
				Console.WriteLine( "\nFinished, press enter to exit..." );
				Console.ReadLine( );
			}
		}
	}
}
