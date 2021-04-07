using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BNAB;

namespace BNAC
{

	/// <summary>
	/// Main program of the BNA compiler. Compiles files or terminal input.
	/// </summary>
	internal class Program
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

				// Translate to python
				Console.WriteLine( "\nCode :" );
				string output = Translator.ToPython( statements );
				Console.WriteLine( output );

				return output;
			}
			catch ( Exception e ) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Error.WriteLine( "BNAC caught Exception while compiling!:" );
				Console.Error.WriteLine( e.ToString( ) );
				Console.ResetColor( );
			}
			return "";
		}


		private static void CompileToBinary( Queue<string> lines , FileStream file )
		{
			var writer = new BinaryWriter( file );

			try {
				// Convert lines to token stream
				Queue<Token> tokens = Token.TokenizeProgram( lines );
				Console.WriteLine( "\nTokens :" );
				foreach ( Token t in tokens ) {
					Console.WriteLine( "  " + t.ToString( ) );
				}

				Console.WriteLine( " " + tokens.Count + " total" );

				// Parse statements from token stream
				Queue<Statement> statements = Statement.ParseStatements( tokens );
				Console.WriteLine( "\nStatements :" );
				foreach ( Statement s in statements ) {
					Console.WriteLine( "  " + s.ToString( ) );
				}

				Console.WriteLine( " " + statements.Count + " total" );

				// Compile to binary
				Binary bin = Compiler.CompileStatements( statements );
				bin.Write(writer);
				int num_words = Binary.HEADER_SIZE_WORDS + bin.TextLength + bin.DataLength;

				Console.WriteLine( "Size of binary: " + num_words + " words ( " + ( num_words * Binary.WORD_SIZE_BYTES ) + " bytes )" );
			}
			catch ( Exception e ) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Error.WriteLine( "BNAC caught Exception while compiling!:" );
				Console.Error.WriteLine( e.ToString( ) );
				Console.ResetColor( );
			}
			finally {
				writer.Close( );
				file.Close( );
			}
		}

		/// <summary>
		/// Compile a function to Python, or take input from the terminal
		/// </summary>
		/// <param name="args">Names of files to compile to Python, can be none.</param>
		private static void Main( string[] args )
		{
			Console.WriteLine( "================================================================================" );
			Console.WriteLine( "Welcome to the BNA's Not Assembly Compiler!" );
			Console.WriteLine( "================================================================================" );

			// If no arguments, take input from command line
			if ( args.Length == 0 ) {
				Console.Write( "Output file name (.bb will be appended): " );
				string filename = Console.ReadLine( );

				while ( true ) {
					// Usage
					Console.WriteLine( "\nInsert BNA code to compile (use '~' to end):" );

					// Read and queue lines
					var lines = new Queue<string>( );
					while ( true ) {
						// get a line
						string input = Console.ReadLine( );

						// end on tilda '~'
						if ( input.Equals( "~" ) ) {
							if ( lines.Count == 0 )
								return;
							break;
						}

						lines.Enqueue( input );
					}

					// Open output file and output
					try {
						var file = File.Open( filename + ".bb" , FileMode.CreateNew , FileAccess.Write );
						CompileToBinary( lines , file );
					}
					catch (Exception e) {
						Console.ForegroundColor = ConsoleColor.Red;
						Console.Error.WriteLine( "BNAC caught Exception while compiling!:" );
						Console.Error.WriteLine( e.ToString( ) );
						Console.ResetColor( );
					}
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

						// Compile it
						Console.WriteLine( "Compiling file..." );
						string compiled_code = CompileToPython( lines );

						// Propmt to delete existing file
						if ( File.Exists( ".\\" + filename + ".py" ) ) {
							Console.Write( filename + ".py already exists, delete it? (y/n): " );
							if ( !Console.ReadLine( ).Equals( "y" ) ) {
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
					catch ( Exception e ) {
						Console.Error.WriteLine( "!!! Caught Exception compiling file !!!:" );
						Console.Error.WriteLine( e.Message );
					}
				}

				// Wait to close so user can read output
				Console.WriteLine( "\nPress enter to exit..." );
				Console.ReadLine( );
			}
		}
	}
}
