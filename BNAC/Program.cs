using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNAC
{
	class Program
	{
		static void Main( string[] args )
		{
			Console.WriteLine( "================================================================================" );
			Console.WriteLine( "Welcome to the BNA's Not Assembly Compiler!" );
			Console.WriteLine( "================================================================================" );

			while ( true ) {
				Console.WriteLine( "\nWrite a line to be tokenized and parsed:" );

				// get a line
				var input = Console.ReadLine( );

				// end on tilda '~'
				if ( input.Equals( "~" ) )
					break;

				// tokenize and parse the line
				try {
					var queue = Token.TokenizeLine( input );
					Console.WriteLine( "\nTokens :" );
					foreach ( Token t in queue.ToList() )
						Console.WriteLine( "  " + t.ToString( ) );
					Console.WriteLine( " " + queue.Count + " total" );

					var statements = Statement.ParseStatements( queue );
					Console.WriteLine( "\nStatements :" );
					foreach ( Statement s in statements.ToList() )
						Console.WriteLine( s.ToString( ) );
					Console.WriteLine( " " + statements.Count + " total" );
				}
				catch (Exception e) {
					Console.Error.Write( "Caught exception: " );
					Console.Error.WriteLine( e.Message );
				}
			}

		}
	}
}
