using System;

namespace BNA
{
	/// <summary>
	/// A BNA exception encountered at runtime.
	/// </summary>
	public class RuntimeException : Exception
	{
		/// <summary>
		/// The statement that threw the error.
		/// </summary>
		public readonly Statement Line;

		/// <summary>
		/// The line number of the statement.
		/// </summary>
		public readonly int LineNumber;

		/// <summary>
		/// Specific BNA info message.
		/// </summary>
		public readonly string BNAMessage;

		/// <summary>
		/// True if this was thrown by the ERROR statement.
		/// </summary>
		public readonly bool BNAError;

		public RuntimeException( int line_number , Statement badGuy , string message )
			: base( message + "\nRuntime error statement " + line_number + ":\n\t" + ( badGuy == null ? "none" : badGuy.RawString( ) ) )
		{
			this.Line = badGuy;
			this.LineNumber = line_number;
			this.BNAMessage = message;
		}

		public RuntimeException( RuntimeException exception, int line_number , Statement badGuy )
			: base( exception.BNAMessage + "\nRuntime error statement " + line_number + ":\n\t" + ( badGuy == null ? "none" : badGuy.RawString( ) ) )
		{
			this.Line = badGuy;
			this.LineNumber = line_number;
			this.BNAMessage = exception.BNAMessage;
			this.BNAError = exception.BNAError;
		}

		public RuntimeException( string message , bool bnaError = false )
			: base( message + "\nRuntime error" )
		{
			this.Line = null;
			this.LineNumber = -1;
			this.BNAMessage = message;
			this.BNAError = bnaError;
		}
	}
}
