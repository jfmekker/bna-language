using System;
using System.IO;
using BNA.Exceptions;

namespace BNA.Values
{
	/// <summary>
	/// File reader type value.
	/// </summary>
	public class ReadFileValue : FileValue
	{
		/// <summary>
		/// TextReader stream object that does the reading.
		/// </summary>
		private TextReader? reader;

		/// <summary>
		/// Create a new <see cref="ReadFileValue"/> instance.
		/// </summary>
		/// <param name="filename">Name of the file.</param>
		public ReadFileValue( string filename ) : base( filename ) => this.Open( );

		/// <summary>
		/// Read a character from the file.
		/// </summary>
		/// <returns>A single character, or null if the end of the file is reached.</returns>
		public char? Read( )
			=> !this.Opened ? throw new Exception( $"Cannot read from non-opened file: '{this.Filename}'" )
			 : this.reader is null ? throw new Exception( $"Tried to read from null reader ({this.Filename})" )
			 : this.reader.Read( ) is int next && next == -1 ? null : (char)next;

		/// <summary>
		/// Read a line from the file.
		/// </summary>
		/// <returns>A string of the whole next line, or null if the end of the file is reached.</returns>
		public string? ReadLine( )
			=> !this.Opened ? throw new Exception( $"Cannot read from non-opened file: '{this.Filename}'" )
			 : this.reader is null ? throw new Exception( $"Tried to read from null reader ({this.Filename})" )
			 : this.reader.ReadLine( );

		public override void Open( )
		{
			try
			{
				this.reader = new StreamReader( this.Filename , true );
				this.Opened = true;
			}
			catch ( Exception e )
			{
				// TODO make new runtime exception class
				throw new Exception( $"Exception caught while opening file: {e.Message}" );
			}
		}

		public override void Close( )
		{
			if ( !this.Opened )
				return;
			else if ( this.reader is null )
				throw new Exception( $"Tried to close null reader ({this.Filename})" );

			this.reader.Close( );
			this.reader = null;
			this.Opened = false;
		}

		public override string TypeString( ) => "ReadFileValue";

		public override string ToString( ) => $"READ FILE ({( this.Opened ? "open" : "closed" )}) '{this.Filename}'";
	}
}
