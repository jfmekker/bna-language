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
				Console.WriteLine( "\nWrite a line to be tokenized:" );

				// get a line
				var input = Console.ReadLine( );

				// end for empty lines (for now)
				if ( input.Equals( "" ) )
					break;

				try {
					var queue = Token.TokenizeLine( input );
					foreach ( Token t in queue )
						Console.WriteLine( t.ToString( ) );
				}
				catch (Exception e) {
					Console.Error.WriteLine( e.Message );
				}
			}

		}
	}
}
