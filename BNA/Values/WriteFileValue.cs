using System;
using System.IO;
using BNA.Exceptions;

namespace BNA.Values
{
	/// <summary>
	/// File writer type value.
	/// </summary>
	public class WriteFileValue : FileValue
	{
		/// <summary>
		/// TextWriter stream object that does the writing.
		/// </summary>
		private TextWriter? writer;

		/// <summary>
		/// Create a new <see cref="WriteFileValue"/> instance.
		/// </summary>
		/// <param name="filename">Name of the file.</param>
		public WriteFileValue( string filename ) : base( filename ) => this.Open( );

		/// <summary>
		/// Write a string to the file.
		/// </summary>
		/// <param name="str">String to write.</param>
		public void Write( string str )
		{
			if ( !this.Opened )
				throw new Exception( $"Cannot write to non-opened file: '{this.Filename}'" );
			else if ( this.writer is null )
				throw new Exception( $"Tried to write to null writer ({this.Filename})" );

			this.writer.Write( str );
		}

		/// <summary>
		/// Write a string to the file followed by a new line.
		/// </summary>
		/// <param name="str">String to write.</param>
		public void WriteLine( string str )
		{
			if ( !this.Opened )
				throw new Exception( $"Cannot write to non-opened file: '{this.Filename}'" );
			else if ( this.writer is null )
				throw new Exception( $"Tried to write to null writer ({this.Filename})" );

			this.writer.WriteLine( str );
		}

		public override void Open( )
		{
			try
			{
				this.writer = new StreamWriter( this.Filename , true );
				this.Opened = true;
			}
			catch (Exception e)
			{
				// TODO make new runtime exception class
				throw new Exception( $"Exception caught while opening file: {e.Message}" );
			}
		}

		public override void Close( )
		{
			if ( !this.Opened )
				return;
			else if ( this.writer is null )
				throw new Exception( $"Tried to close null writer ({this.Filename})" );
			
			this.writer.Close( );
			this.writer = null;
			this.Opened = false;
		}

		public override string TypeString( ) => "WriteFileValue";
		
		public override string ToString( ) => $"WRITE FILE ({(this.Opened ? "open" : "closed")}) '{this.Filename}'";
	}
}
