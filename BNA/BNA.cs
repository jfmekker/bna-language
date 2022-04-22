using System;
using System.Collections.Generic;
using System.IO;
using BNA.Exceptions;
using BNA.Compile;
using BNA.Resources;
using BNA.Run;

namespace BNA
{
	public enum ReturnCode
	{
		Success,
		BnaError,
		CompileError,
		RuntimeError,
		NotImplementedError,
		FileError,
		UnexpectedError
	}

	public static class BNA
	{
		/// <summary>
		/// Static random number generator.
		/// </summary>
		public static readonly Random RNG = new( DateTime.Now.Millisecond );

		/// <summary>
		/// Compile and run BNA code from the terminal or files.
		/// </summary>
		/// <param name="args">Names of BNA files to run, can be none.</param>
		public static int Main( string[]? args )
		{
			Console.WriteLine( ConsoleMessages.CLI_SEPARATOR_LINE );
			Console.WriteLine( ConsoleMessages.CLI_WELCOME_MESSAGE );
			Console.WriteLine( ConsoleMessages.CLI_SEPARATOR_LINE );

			// If no arguments, take input from command line
			if ( args is null || args.Length == 0 )
			{
				ReturnCode r = RunFromConsoleInput( );
				return (int)r;
			}
			// else, take in files
			else
			{
				ReturnCode r = RunFromFiles( args );
#if DEBUG
				// Wait to close so user can read output
				Console.WriteLine( ConsoleMessages.CLI_FINISHED_FILES );
				Console.ReadLine( );
#endif
				return (int)r;
			}
		}

		/// <summary>
		/// Run one or more BNA code files.
		/// </summary>
		/// <param name="files">Paths to .bna files.</param>
		/// <returns>The last <see cref="ReturnCode"/> from running the files.</returns>
		public static ReturnCode RunFromFiles( string[] files )
		{
			ReturnCode return_val = ReturnCode.Success;
			foreach ( string file in files ?? throw new ArgumentNullException( nameof( files ) ) )
			{
				// Output the BNA filename
				Console.WriteLine( file + ":" );

				// Check the file extension
				string ext = Path.GetExtension( file );
				if ( ext is not ".bna" )
				{
					ConsolePrintError( "Wrong extension, expected '.bna' file: " + file );
					return ReturnCode.FileError;
				}

				// Output file contents
				Console.WriteLine( ConsoleMessages.CLI_READING_FILE );
				List<string> lines = new( );
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
					ConsolePrintError( ConsoleMessages.CLI_FILE_NOT_FOUND + file );
					ConsolePrintError( e.Message );
					return ReturnCode.FileError;
				}

				// Compile to program and run
				if ( lines.Count > 0 )
				{
					return_val |= CompileAndRun( lines );
				}

				Console.WriteLine( ConsoleMessages.CLI_FINISHED_FILE );
			}
			return return_val;
		}

		private static ReturnCode RunFromConsoleInput( )
		{
			ReturnCode return_val = ReturnCode.Success;

			while ( true )
			{
				// Usage
				Console.WriteLine( ConsoleMessages.CLI_RUN_INPUT_START );

				// Read and queue lines
				List<string> lines = new( );
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
							Console.WriteLine( ConsoleMessages.CLI_INPUT_ONE_ONLY );
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

				Console.WriteLine( ConsoleMessages.CLI_RUN_INPUT_DONE );

				// Wait to continue, check for exit
				if ( Console.ReadLine( ) is "~" )
				{
					break;
				}
			}

			return return_val;
		}

		private static ReturnCode CompileAndRun( IList<string> lines )
		{
			if ( lines is null ) throw new ArgumentNullException( nameof( lines ) );

			// Compile to program and run
			try
			{
				Console.WriteLine( ConsoleMessages.CLI_COMPILING );
				Compiler comp = new( lines );
				Program prog = new( comp.Compile( ) );
				Console.WriteLine( ConsoleMessages.CLI_RUNNING );
				prog.Run( );
				Console.WriteLine( );
				return ReturnCode.Success;
			}
			catch ( CompiletimeException e )
			{
				ConsolePrintError( "Compiletime Exception caught:" );
				ConsolePrintError( e.Message );
				return ReturnCode.CompileError;
			}
			catch ( RuntimeException e )
			{
				ConsolePrintError( "Runtime Exception caught:" );
				ConsolePrintError( e.Message );
				return e.InnerException is ErrorStatementException ? ReturnCode.BnaError : ReturnCode.RuntimeError;
			}
			catch ( NotImplementedException e )
			{
				ConsolePrintError( "Not Implemented Exception caught:" );
				ConsolePrintError( e.Message );
				return ReturnCode.NotImplementedError;
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
