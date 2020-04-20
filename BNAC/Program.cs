using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BNAC
{

	/// <summary>
	/// Main program of the BNA compiler. Compiles files or terminal input.
	/// </summary>
	class Program
	{
		/// <summary>
		/// Compiles a queue of BNA lines of code to Python.
		/// </summary>
		/// <param name="lines">The lines of BNA code to compile</param>
		/// <returns>String of the compile Python script</returns>
		private static string CompileToPython( Queue<string> lines )
		{
			try {
				// Convert lines to token stream
				var tokens = Token.TokenizeProgram( lines );
				Console.WriteLine( "\nTokens :" );
				foreach ( Token t in tokens.ToList( ) )
					Console.WriteLine( "  " + t.ToString( ) );
				Console.WriteLine( " " + tokens.Count + " total" );

				// Parse statements from token stream
				var statements = Statement.ParseStatements( tokens );
				Console.WriteLine( "\nStatements :" );
				foreach ( Statement s in statements.ToList( ) )
					Console.WriteLine( "  " + s.ToString( ) );
				Console.WriteLine( " " + statements.Count + " total" );

				// Translate to python
				Console.WriteLine( "\nCode :" );
				var output = Translator.ToPython( statements );
				Console.WriteLine( output );

				return output;
			}
			catch ( Exception e ) {
				Console.Error.WriteLine( "!!! Caught Exception while compiling !!!:" );
				Console.Error.WriteLine( e.ToString( ) );
			}
			return "";
		}

		/// <summary>
		/// Compile a function to Python, or take input from the terminal
		/// </summary>
		/// <param name="args">Names of files to compile to Python, can be none.</param>
		static void Main( string[] args )
		{
			Console.WriteLine( "================================================================================" );
			Console.WriteLine( "Welcome to the BNA's Not Assembly Compiler!" );
			Console.WriteLine( "================================================================================" );

			// If no arguments, take input from command line
			if ( args.Length == 0 ) {
				while ( true ) {
					// Usage
					Console.WriteLine( "\nInsert BNA code to translate to Python (use '~' to exit):" );

					// Read and queue lines
					var lines = new Queue<string>( );
					while ( true ) {
						// get a line
						var input = Console.ReadLine( );

						// end on tilda '~'
						if ( input.Equals( "~" ) )
							break;

						lines.Enqueue( input );
					}

					// Output 
					CompileToPython( lines );
					Console.WriteLine( );

					// Wait to continue, check for exit
					if ( Console.ReadLine( ).Equals( "~" ) )
						break;
				}
			}
			// else, take in files
			else {
				foreach ( string file in args ) {
					try {
						// Output the BNA filename
						Console.WriteLine( file + ":" );

						// Check the file extension
						string[] split_filename = file.Split( new char[] { '.' } );
						string filename = split_filename[0];
						string extension = split_filename[1];
						if ( !extension.Equals( "bna" ) )
							throw new Exception( "Wrong file type: " + file );

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

						// Compile it
						Console.WriteLine( "Compiling file..." );
						string compiled_code = CompileToPython( lines );

						// Propmt to delete existing file
						if ( File.Exists( ".\\" + filename + ".py" ) ) {
							Console.Write( filename + ".py already exists, delete it? (y/n): " );
							if ( !Console.ReadLine( ).Equals("y") ) {
								Console.WriteLine( "Skipping file." );
								continue;
							}
						}

						// Write to .py file
						Console.WriteLine( "Writing file..." );
						using ( StreamWriter sw = File.CreateText( ".\\" + filename + ".py" ) ) {
							sw.Write( compiled_code );
						}

						Console.WriteLine( "Done.\n" );
					}
					catch (Exception e) {
						Console.Error.WriteLine( "!!! Caught Exception compiling file !!!:" );
						Console.Error.WriteLine( e.Message );
					}
				}

				// Wait to close so user can read output
				Console.ReadLine( );
			}
		}
	}
}
