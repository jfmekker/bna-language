using System;
using BNA.Exceptions;

namespace BNA.Values
{
	/// <summary>
	/// File type value.
	/// </summary>
	public abstract class FileValue : Value
	{
		/// <summary>
		/// Create a new <see cref="FileValue"/> instance.
		/// </summary>
		/// <param name="filename">Path to the file.</param>
		protected FileValue(string filename)
		{
			this.Filename = filename;
		}

		/// <summary>
		/// Throw an exception as FileValue's Get property should never be directly accessed.
		/// </summary>
		public sealed override object Get => throw new InvalidOperationException( "FileValues should not use Get property directly." );

		/// <summary>
		/// Name of the file.
		/// </summary>
		public string Filename
		{
			get; init;
		}

		/// <summary>
		/// Whether the file is opened.
		/// </summary>
		public bool Opened
		{
			get; protected set;
		}

		/// <summary>
		/// Open the file.
		/// </summary>
		/// <exception cref="RuntimeException">The file could not be opened.</exception>
		public abstract void Open( );

		/// <summary>
		/// Open the file.
		/// </summary>
		/// <exception cref="RuntimeException">The file could not be closed.</exception>
		public abstract void Close( );

		public override string TypeString( ) => "FileValue";

		public override bool Equals( Value? other )
			=> (other is FileValue fileVal && this.Filename == fileVal.Filename)
			|| (other is StringValue strVal && this.Filename == strVal.Get);
	}
}
