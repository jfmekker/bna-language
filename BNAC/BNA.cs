﻿using System;
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

				// Return array of statements
				return statements.ToArray( );
			}
			catch ( Exception e ) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Error.WriteLine( "BNA caught Exception while compiling!:" );
				Console.Error.WriteLine( e.ToString( ) );
				Console.ResetColor( );
			}
			return null;
		}

		/// <summary>
		/// Compile a function to Python, or take input from the terminal
		/// </summary>
		/// <param name="args">Names of files to compile to Python, can be none.</param>
		private static void Main( string[] args )
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

					// Output 
					Compile( lines );

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
						Compile( lines );

						Console.WriteLine( "\nDone.\n" );
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
