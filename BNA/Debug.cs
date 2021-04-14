using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA
{
	public static class Debug
	{
		public static ConsoleColor DEBUG_CONSOLE_COLOR = ConsoleColor.DarkGray;

		public static void Add( string message )
		{
#if DEBUG
			Console.ForegroundColor = DEBUG_CONSOLE_COLOR;
			Console.Write( message );
			Console.ResetColor( );
#endif
		}

		public static void AddLine( string message )
		{
#if DEBUG
			Console.ForegroundColor = DEBUG_CONSOLE_COLOR;
			Console.WriteLine( message );
			Console.ResetColor( );
#endif
		}
	}
}
