using System;
using System.Collections.Generic;
using System.IO;

namespace BNA
{
	public class BNA
	{
		public static Random RNG;

		/// <summary>
		/// Compile a function to Python, or take input from the terminal
		/// </summary>
		/// <param name="args">Names of files to compile to Python, can be none.</param>
		public static void Main( string[] args )
		{
			Console.WriteLine( "================================================================================" );
			Console.WriteLine( "Welcome to the BNA's Not Assembly Interpreter!" );
			Console.WriteLine( "================================================================================" );

			RNG = new Random( DateTime.Now.Millisecond );

			// If no arguments, take input from command line
			if ( args.Length == 0 ) {
				RunFromInput( );
			}
			// else, take in files
			else {
				RunFromFiles( args );
			}
		}

		public static void RunFromFiles( string[] files )
		{
			foreach ( string file in files ) {
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
				var lines = new List<string>( );
				try {
					using ( StreamReader sr = File.OpenText( ".\\" + file ) ) {
						string line;
						while ( ( line = sr.ReadLine( ) ) != null ) {
							Console.WriteLine( line );
							lines.Add( line );
						}
					}
				}
				catch(FileNotFoundException e) {
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine( "Failed to find file: " + file );
					Console.WriteLine( e.Message );
					Console.ResetColor( );
				}

				// Compile to program and run
				if ( lines.Count > 0 ) {
					CompileAndRun( lines );
				}

				Console.WriteLine( "Done with file.\n" );
			}

			// Wait to close so user can read output
			Console.WriteLine( "Finished, press enter to exit..." );
			Console.ReadLine( );
		}

		public static void RunFromInput( )
		{
			while ( true ) {
				// Usage
				Console.WriteLine( "\nInsert BNA code to do stuff or type '$filename.bna' to run a file (use '~' to end):" );

				// Read and queue lines
				var lines = new List<string>( );
				bool run = true;
				while ( true ) {
					// get a line
					string input = Console.ReadLine( );

					// end on tilda '~'
					if ( input.Equals( "~" ) ) {
						break;
					}

					// filename
					if ( input.Length > 0 && input[0] == '$' ) {
						if ( lines.Count > 0 ) {
							Console.WriteLine( "Input only BNA code or only a filename." );
						}
						else {
							string filename = input.Substring( 1 );
							RunFromFiles( new string[] { filename } );
						}
						run = false;
						break;
					}

					lines.Add( input );
				}

				// Check if we run the user input
				if ( run ) {
					CompileAndRun( lines );
				}
				
				Console.WriteLine( "Press enter to continue (use '~' to exit)." );

				// Wait to continue, check for exit
				if ( Console.ReadLine( ).Equals( "~" ) ) {
					break;
				}
			}
		}

		public static void CompileAndRun( List<string> lines )
		{
			// Compile to program and run
			try {
				Console.WriteLine( "Compiling Program..." );
				var comp = new Compiler( lines );
				Program prog = comp.Compile( );
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
			catch ( NotImplementedException e ) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine( "Not Implemented Exception caught:" );
				Console.WriteLine( e.Message );
				Console.ResetColor( );
			}
#if DEBUG
#else
			catch ( Exception e ) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine( "Unexpected Exception caught:" );
				Console.WriteLine( e.Message );
				Console.WriteLine( "Please report this issue on github (https://github.com/jfmekker/bna-language/issues)!" );
				Console.ResetColor( );
				Console.ReadLine( );
				return;
			}
#endif
		}
	}
}
