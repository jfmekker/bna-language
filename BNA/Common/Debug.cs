using System;

namespace BNA.Common
{
	/// <summary>
	/// Static class to separate debug prints.
	/// </summary>
	public static class Debug
	{
		/// <summary>
		/// Color used by debug prints to visually distinguish them.
		/// </summary>
		public const ConsoleColor DEBUG_CONSOLE_COLOR = ConsoleColor.DarkGray;

		/// <summary>
		/// Add a string to the debug output.
		/// </summary>
		/// <param name="message">String to print</param>
		public static void Add( string message )
		{
#if DEBUG
			Console.ForegroundColor = DEBUG_CONSOLE_COLOR;
			Console.Write( message );
			Console.ResetColor( );
#endif
		}

		/// <summary>
		/// Add a string followed by a new line to the debug output.
		/// </summary>
		/// <param name="message">String to print</param>
		public static void AddLine( string message = "" )
		{
#if DEBUG
			Console.ForegroundColor = DEBUG_CONSOLE_COLOR;
			Console.WriteLine( message );
			Console.ResetColor( );
#endif
		}
	}
}
