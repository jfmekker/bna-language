using System;
using System.Collections.Generic;
using System.IO;
using BNAB.Util;

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
		public static readonly int WORD_SIZE_BYTES = sizeof(ulong);

		/// <summary>
		/// Raw words of the binary header
		/// </summary>
		public ulong[] Header
		{
			get; private set;
		} = new ulong[HEADER_SIZE_WORDS];

		/// <summary>
		/// Magic number specified in the file
		/// </summary>
		public ulong Magic
		{
			get
			{
				return Header[0];
			}

			private set
			{
				Header[0] = value;
			}
		}

		/// <summary>
		/// BNAVersion number of the binary file (to check compatability)
		/// </summary>
		public int BNAVersion
		{
			get
			{
				return (int)BitHelper.GetBits( 0 , 31 , Header[1] );
			}

			private set
			{
				BitHelper.SetBits( 0 , 31 , ref Header[1] , (ulong)value );
			}
		}

		/// <summary>
		/// Length in words of the Data segment
		/// </summary>
		public int DataLength
		{
			get
			{
				return (int)BitHelper.GetBits( 0 , 31 , Header[2] );
			}

			private set
			{
				BitHelper.SetBits( 0 , 31 , ref Header[2] , (ulong)value );
			}
		}

		/// <summary>
		/// Length in words of the Text segment
		/// </summary>
		public int TextLength
		{
			get
			{
				return (int)BitHelper.GetBits( 32 , 63 , Header[2] );
			}

			private set
			{
				BitHelper.SetBits( 32 , 63 , ref Header[2] , (ulong)value );
			}
		}

		/// <summary>
		/// File checksum listed in the binary
		/// </summary>
		public ulong Checksum
		{
			get
			{
				return Header[3];
			}

			private set
			{
				Header[3] = value;
			}
		}

		/// <summary>
		/// Instance of the <see cref="DataSegment"/> class to represent the Data segment of the binary
		/// </summary>
		public DataSegment Data
		{
			get; private set;
		}
		
		/// <summary>
		/// Instance of the <see cref="TextSegment"/> class to represent the Text segment of the binary
		/// </summary>
		public TextSegment Text
		{
			get; private set;
		}

		/// <summary>
		/// Create an empty <see cref="Binary"/> instance
		/// </summary>
		public Binary( )
		{
			Data = new DataSegment( );
			Text = new TextSegment( );
		}

		/// <summary>
		/// Construct a new <see cref="Binary"/> from a data and text segment.
		/// </summary>
		/// <param name="dataSegment"><see cref="DataSegment"/></param>
		/// <param name="textSegment"><see cref="TextSegment"/></param>
		public Binary( DataSegment dataSegment , TextSegment textSegment )
		{
			Header = new ulong[HEADER_SIZE_WORDS];
			Data = dataSegment;
			Text = textSegment;

			Magic = MAGIC_NUMBER;
			BNAVersion = 1; // TODO
			DataLength = Data.Raw.Length;
			TextLength = Text.Raw.Length;

			Checksum = 0;
			foreach ( ulong l in Data.Raw )
				Checksum += l;
			foreach ( ulong l in Text.Raw )
				Checksum += l;
		}

		/// <summary>
		/// Initialize a Binary object from a binary file
		/// </summary>
		/// <param name="reader">Binary file reader, closes the reader when done</param>
		/// <returns>True if successfully initialized, false otherwise</returns>
		public bool Init( BinaryReader reader )
		{
			// Parse Header
			var header_words = new List<ulong>( );
			try {
				// Magic number
				header_words.Add( reader.ReadUInt64( ) );
				if ( header_words[0] != MAGIC_NUMBER ) {
					throw new Exception( "Bad magic: " + header_words[0] );
				}

				// Version + spare
				header_words.Add( reader.ReadUInt64( ) );

				// Data length + Text length
				header_words.Add( reader.ReadUInt64( ) );

				// Checksum
				header_words.Add( reader.ReadUInt64( ) );
			}
			catch ( Exception e ) {
				Console.Error.Write( "Failed to read binary header." );
				Console.Error.Write( e.Message );
				reader.Close( );
				return false;
			}
			Header = header_words.ToArray( );

			// Parse Data
			var data_words = new List<ulong>( );
			try {
				for ( int i = 0 ; i < DataLength ; i += 1 ) {
					data_words.Add( reader.ReadUInt64( ) );
				}
			}
			catch ( Exception e ) {
				Console.Error.Write( "Failed to read all binary in data segment." );
				Console.Error.Write( e.Message );
				reader.Close( );
				return false;
			}
			Data = new DataSegment( data_words.ToArray( ) );

			// Parse Text
			var text_words = new List<ulong>( );
			try {
				for ( int i = 0 ; i < TextLength ; i += 1 ) {
					text_words.Add( reader.ReadUInt64( ) );
				}
			}
			catch ( Exception e ) {
				Console.Error.Write( "Failed to read all binary in data segment." );
				Console.Error.Write( e.Message );
				reader.Close( );
				return false;
			}
			Text = new TextSegment( text_words.ToArray( ) );

			// Check checksum
			if ( !ChecksumValid( ) ) {
				Console.Error.Write( "Bad checksum for file." );
				reader.Close( );
				return false;
			}

			reader.Close( );
			return true;
		}

		/// <summary>
		/// Checks if the checksum from the file matches what is calculated
		/// </summary>
		/// <returns>True if the checksum checks out, false otherwise</returns>
		public bool ChecksumValid( )
		{
			ulong sum = 0;
			foreach ( ulong l in Data.Raw )
				sum += l;
			foreach ( ulong l in Text.Raw )
				sum += l;
			return sum == Checksum;
		}

		/// <summary>
		/// Write this binary program to a file.
		/// </summary>
		/// <param name="writer">file stream to write to</param>
		public void Write( BinaryWriter writer )
		{
			try {
				for ( int i = 0 ; i < Header.Length ; i += 1 )
					writer.Write( Header[i] );
				for ( int i = 0 ; i < Data.Raw.Length ; i += 1 )
					writer.Write( Data.Raw[i] );
				for ( int i = 0 ; i < Text.Raw.Length ; i += 1 )
					writer.Write( Text.Raw[i] );
			}
			catch (Exception e) {
				Console.Error.WriteLine( "Encountered error writing binary file." );
				Console.Error.WriteLine( e.Message );
			}
			finally {
				writer.Close( );
			}
		}
	}
}
