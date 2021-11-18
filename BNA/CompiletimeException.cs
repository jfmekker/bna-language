using System;

namespace BNA
{
	/// <summary>
	/// A BNA exception encountered at compile time.
	/// </summary>
	public class CompiletimeException : Exception
	{
		/// <summary>
		/// The line that threw the error.
		/// </summary>
		public readonly string Line;

		/// <summary>
		/// The line number of the bad line.
		/// </summary>
		public readonly int LineNumber;

		/// <summary>
		/// Specific BNA info message.
		/// </summary>
		public readonly string BNAMessage;

		public CompiletimeException( int line_number , string line , string message )
			: base( message + "\nCompile error on line " + line_number + ":\n\t" + line )
		{
			this.Line = line;
			this.LineNumber = line_number;
			this.BNAMessage = message;
		}

		public CompiletimeException( CompiletimeException exception , int line_number , string line )
			: base( exception.BNAMessage + "\nCompile error on line " + line_number + ":\n\t" + line , exception )
		{
			this.Line = line;
			this.LineNumber = line_number;
			this.BNAMessage = exception.BNAMessage;
		}

		public CompiletimeException( string message )
			: base( message + "\nCompile error" )
		{
			this.Line = string.Empty;
			this.LineNumber = -1;
			this.BNAMessage = message;
		}
	}
}
