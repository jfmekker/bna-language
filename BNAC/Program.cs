using System;
using System.Collections.Generic;
using System.Linq;

namespace BNAC
{
	class Program
	{
		private static void TokenizeAndParse( )
		{
			Console.WriteLine( "\nWrite a line to be tokenized and parsed:" );

			// get a line
			var input = Console.ReadLine( );

			// tokenize and parse the line
			try {
				var queue = Token.TokenizeLine( input );
				Console.WriteLine( "\nTokens :" );
				foreach ( Token t in queue.ToList( ) )
					Console.WriteLine( "  " + t.ToString( ) );
				Console.WriteLine( " " + queue.Count + " total" );

				var statements = Statement.ParseStatements( queue );
				Console.WriteLine( "\nStatements :" );
				foreach ( Statement s in statements.ToList( ) )
					Console.WriteLine( s.ToString( ) );
				Console.WriteLine( " " + statements.Count + " total" );
			}
			catch ( Exception e ) {
				Console.Error.Write( "Caught exception: " );
				Console.Error.WriteLine( e.Message );
			}
		}

		static void Main( string[] args )
		{
			Console.WriteLine( "================================================================================" );
			Console.WriteLine( "Welcome to the BNA's Not Assembly Compiler!" );
			Console.WriteLine( "================================================================================" );

			Console.WriteLine( "\nInsert BNA code to translate to Python:" );

			var lines = new Queue<string>( );
			while ( true ) {
				// get a line
				var input = Console.ReadLine( );

				// end on tilda '~'
				if ( input.Equals( "~" ) )
					break;

				lines.Enqueue( input );
			}

			try {
				var tokens = Token.TokenizeProgram( lines );
				Console.WriteLine( "\nTokens :" );
				foreach ( Token t in tokens.ToList( ) )
					Console.WriteLine( "  " + t.ToString( ) );
				Console.WriteLine( " " + tokens.Count + " total" );

				var statements = Statement.ParseStatements( tokens );
				Console.WriteLine( "\nStatements :" );
				foreach ( Statement s in statements.ToList( ) )
					Console.WriteLine( "  " +  s.ToString( ) );
				Console.WriteLine( " " + statements.Count + " total" );

				Console.WriteLine( "\nCode :" );
				var output = Translator.ToPython( statements );

				Console.WriteLine( );
				Console.WriteLine( output );
			}
			catch ( Exception e ) {
				Console.Error.WriteLine( "!Caught Exception!:" );
				Console.Error.WriteLine( e.ToString( ) );
			}

			Console.ReadLine( );
		}
	}
}
