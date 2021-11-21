using BNA.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace BNA
{
	public enum ReturnCode : byte
	{
		SUCCESS = 0,
		BNA_ERROR = 1,
		COMPILE_ERROR = 1 << 1,
		RUNTIME_ERROR = 1 << 2,
		NOT_IMPLEMENTED_ERROR = 1 << 3,
		// UNUSED = 1 << 4,
		// UNUSED = 1 << 5,
		FILE_ERROR = 1 << 6,
		UNEXPECTED_ERROR = 1 << 7
	}

	public class BNA
	{
		/// <summary>
		/// Static random number generator.
		/// </summary>
		public static readonly Random RNG = new( DateTime.Now.Millisecond );

		/// <summary>
		/// Compile a function to Python, or take input from the terminal
		/// </summary>
		/// <param name="args">Names of files to compile to Python, can be none.</param>
		public static int Main( string[] args )
		{
			Console.WriteLine( "================================================================================" );
			Console.WriteLine( "Welcome to the BNA's Not Assembly Interpreter!" );
			Console.WriteLine( "================================================================================" );

			// If no arguments, take input from command line
			if ( args.Length == 0 )
			{
				ReturnCode r = RunFromInput( );
				return (int)r;
			}
			// else, take in files
			else
			{
				ReturnCode r = RunFromFiles( args );
#if DEBUG
				// Wait to close so user can read output
				Console.WriteLine( "Finished, press enter to exit..." );
				Console.ReadLine( );
#endif
				return (int)r;
			}
		}

		public static ReturnCode RunFromFiles( string[] files )
		{
			ReturnCode return_val = 0;
			foreach ( string file in files )
			{
				// Output the BNA filename
				Console.WriteLine( file + ":" );

				// Check the file extension
				int lastDirIndex = file.LastIndexOfAny( new char[] { '/' , '\\' } );
				string[] split_filename = file[( lastDirIndex + 1 )..].Split( new char[] { '.' } );
				string filename = split_filename[0];
				string extension = split_filename[1];
				if ( !extension.Equals( "bna" ) )
				{
					ConsolePrintError( "Wrong extension, expected '.bna' file: " + file );
					return ReturnCode.FILE_ERROR;
				}

				// Output file contents
				Console.WriteLine( "Reading file..." );
				var lines = new List<string>( );
				try
				{
					using StreamReader sr = File.OpenText( ".\\" + file );
					while ( sr.ReadLine( ) is string line )
					{
						Console.WriteLine( line );
						lines.Add( line );
					}
				}
				catch ( FileNotFoundException e )
				{
					ConsolePrintError( "Failed to find file: " + file );
					ConsolePrintError( e.Message );
					return ReturnCode.FILE_ERROR;
				}

				// Compile to program and run
				if ( lines.Count > 0 )
				{
					return_val |= CompileAndRun( lines );
				}

				Console.WriteLine( "Done with file.\n" );
			}
			return return_val;
		}

		public static ReturnCode RunFromInput( )
		{
			ReturnCode return_val = ReturnCode.SUCCESS;

			while ( true )
			{
				// Usage
				Console.WriteLine( "\nInsert BNA code to do stuff or type '$filename.bna' to run a file (use '~' to end):" );

				// Read and queue lines
				var lines = new List<string>( );
				bool run = true;
				while ( true )
				{
					// get a line
					string input = Console.ReadLine( ) ?? string.Empty;

					// end on tilda '~'
					if ( input is "~" )
					{
						break;
					}

					// filename
					if ( input.Length > 0 && input[0] == '$' )
					{
						if ( lines.Count > 0 )
						{
							Console.WriteLine( "Input only BNA code or only a filename." );
						}
						else
						{
							string filename = input[1..];
							_ = RunFromFiles( new string[] { filename } );
						}
						run = false;
						break;
					}

					lines.Add( input );
				}

				// Check if we run the user input
				if ( run )
				{
					return_val = CompileAndRun( lines );
				}

				Console.WriteLine( "Press enter to continue (use '~' to exit)." );

				// Wait to continue, check for exit
				if ( Console.ReadLine( ) is "~" )
				{
					break;
				}
			}

			return return_val;
		}

		public static ReturnCode CompileAndRun( List<string> lines )
		{
			// Compile to program and run
			try
			{
				Console.WriteLine( "Compiling Program..." );
				var comp = new Compiler( lines );
				Program prog = comp.Compile( );
				Console.WriteLine( "\nRunning Program...\n" );
				prog.Run( );
				Console.WriteLine( );
				return ReturnCode.SUCCESS;
			}
			catch ( CompiletimeException e )
			{
				ConsolePrintError( "Compiletime Exception caught:" );
				ConsolePrintError( e.Message );
				return ReturnCode.COMPILE_ERROR;
			}
			catch ( RuntimeException e )
			{
				ConsolePrintError( "Runtime Exception caught:" );
				ConsolePrintError( e.Message );
				return e.BNAError ? ReturnCode.BNA_ERROR : ReturnCode.RUNTIME_ERROR;
			}
			catch ( NotImplementedException e )
			{
				ConsolePrintError( "Not Implemented Exception caught:" );
				ConsolePrintError( e.Message );
				return ReturnCode.NOT_IMPLEMENTED_ERROR;
			}
#if DEBUG
#else
			catch ( Exception e ) {
				ConsolePrintError( "Unexpected Exception caught:" );
				ConsolePrintError( e.Message );
				ConsolePrintError( "Please report this issue on github (https://github.com/jfmekker/bna-language/issues)!" );
				return ReturnCode.UNEXPECTED_ERROR;
			}
#endif
		}

		private static void ConsolePrintError( string message )
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine( message );
			Console.ResetColor( );
		}
	}
}
