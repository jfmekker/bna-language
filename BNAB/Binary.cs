using System;
using System.Collections.Generic;
using System.IO;

namespace BNAB
{
	/// <summary>
	/// Class to represent a BNA compiled binary
	/// </summary>
	public class Binary
	{
		/// <summary>
		/// Magic number identifier to mark a .bb file
		/// </summary>
		public static readonly ulong MAGIC_NUMBER = 0xF09F8D8C_F09F8D8C;

		/// <summary>
		/// Size of the binary header in words
		/// </summary>
		public static readonly int HEADER_SIZE_WORDS = 4;

		/// <summary>
		/// Number of bytes in each word
		/// </summary>
		public static readonly int WORD_SIZE_BYTES = 8;

		/// <summary>
		/// Raw words of the binary
		/// </summary>
		private ulong[] raw;

		/// <summary>
		/// Version number of the binary file (to check compatability)
		/// </summary>
		public int Version
		{
			get
			{
				return (int)( ( this.raw[1] >> 32 ) & 0xFFFF_FFFF );
			}
		}

		/// <summary>
		/// Length in words of the Text segment
		/// </summary>
		public uint TextLength
		{
			get
			{
				return (uint)( this.raw[2] & 0xFFFF_FFFF );
			}
		}

		/// <summary>
		/// Length in words of the Data segment
		/// </summary>
		public uint DataLength
		{
			get
			{
				return (uint)( ( this.raw[2] >> 32 ) & 0xFFFF_FFFF );
			}
		}

		/// <summary>
		/// File checksum listed in the binary
		/// </summary>
		public ulong Checksum
		{
			get
			{
				return raw[3];
			}
		}

		/// <summary>
		/// Backing variable for the Data field
		/// </summary>
		private DataSegment _data;

		/// <summary>
		/// Instance of the <see cref="DataSegment"/> class to represent the Data segment of the binary
		/// </summary>
		public DataSegment Data
		{
			get
			{
				if ( this._data == null ) {
					this._data = new DataSegment( new ArraySegment<ulong>( this.raw , HEADER_SIZE_WORDS , (int)this.DataLength ) );
				}
				return this._data;
			}
		}

		/// <summary>
		/// Backing variable for the Text field
		/// </summary>
		private TextSegment _text;

		/// <summary>
		/// Instance of the <see cref="TextSegment"/> class to represent the Text segment of the binary
		/// </summary>
		public TextSegment Text
		{
			get
			{
				if ( this._text == null ) {
					this._text = new TextSegment( new ArraySegment<ulong>( this.raw , (int)( HEADER_SIZE_WORDS + this.DataLength ) , (int)this.TextLength ) );
				}
				return this._text;
			}
		}

		/// <summary>
		/// Create an empty instance of the <see cref="Binary"/>
		/// </summary>
		public Binary( BinaryReader file )
		{
		}

		/// <summary>
		/// Initialize a Binary object from a binary file
		/// </summary>
		/// <param name="file">Binary file reader, closes the reader when done</param>
		/// <returns>True if successfully initialized, false otherwise</returns>
		public bool Init( BinaryReader file )
		{
			var words = new List<ulong>( );

			// Parse Header
			try {
				// Magic number
				words.Add( file.ReadUInt64( ) );
				if ( words[0] != MAGIC_NUMBER ) {
					throw new Exception( "Bad magic: " + words[0] );
				}

				// Version + spare
				words.Add( file.ReadUInt64( ) );

				// Data length + Text length
				words.Add( file.ReadUInt64( ) );

				// Checksum
				words.Add( file.ReadUInt64( ) );
			}
			catch ( Exception e ) {
				Console.Error.Write( "Failed to read binary header." );
				Console.Error.Write( e.Message );
				file.Close( );
				return false;
			}

			// Set current 'words' as 'raw', but we're not done with the list yet
			this.raw = words.ToArray( );

			// Parse Data + Text
			try {
				for ( int i = 0 ; i < this.DataLength + this.TextLength ; i += 1 ) {
					words.Add( file.ReadUInt64( ) );
				}
			}
			catch ( Exception e ) {
				Console.Error.Write( "Failed to read all binary data." );
				Console.Error.Write( e.Message );
				file.Close( );
				return false;
			}
			this.raw = words.ToArray( );

			// Check checksum
			if ( !this.ChecksumValid( ) ) {
				Console.Error.Write( "Bad checksum for file." );
				file.Close( );
				return false;
			}

			file.Close( );
			return true;
		}

		/// <summary>
		/// Checks if the checksum from the file matches what is calculated
		/// </summary>
		/// <returns>True if the checksum checks out, false otherwise</returns>
		public bool ChecksumValid( )
		{
			ulong sum = 0;
			for ( int i = HEADER_SIZE_WORDS ; i < HEADER_SIZE_WORDS + this.DataLength + this.TextLength ; i += 1 ) {
				sum += this.raw[i];
			}
			return sum == this.Checksum;
		}

		/// <summary>
		/// Executes the binary program
		/// </summary>
		/// <returns>Exit code from program</returns>
		public int Execute( )
		{
			// TODO
			return -1;
		}
	}
}
