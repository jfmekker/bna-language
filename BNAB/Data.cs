using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNAB
{
	/// <summary>
	/// Data type of a DataEntry.
	/// </summary>
	public enum DataEntryType : byte
	{
		NONE, LITERAL_INT, LITERAL_FLOAT, LITERAL_STRING
	}

	/// <summary>
	/// Collection of data entries for a Binary file/object.
	/// </summary>
	public class DataSegment
	{
		/// <summary>
		/// Array of raw data taken from a file
		/// </summary>
		private ArraySegment<ulong> _raw;

		/// <summary>
		/// Dictionaries of Integer/Float/String data values
		/// </summary>
		private Dictionary<int,   long> IntData    = new Dictionary<int,   long>();
		private Dictionary<int, double> FloatData  = new Dictionary<int, double>();
		private Dictionary<int, string> StringData = new Dictionary<int, string>();

		/// <summary>
		/// Construct a new <see cref="DataSegment"/> instance from raw data.
		/// </summary>
		/// <param name="raw">Raw data taken from a file.</param>
		public DataSegment( ArraySegment<ulong> raw )
		{
			_raw = raw;
			ReadEntries( );
		}

		/// <summary>
		/// Construct an empty <see cref="DataSegment"/> instance to add data to.
		/// </summary>
		public DataSegment( )
		{
		}

		/// <summary>
		/// Fill the data dictionaries from the raw data.
		/// </summary>
		private void ReadEntries( )
		{
			if ( IntData != null && FloatData != null && StringData != null )
				return;

			ulong[] raw_data = _raw.ToArray( );

			// Parse word-by-word
			for ( int i = 0 ; i < _raw.Count ; ) {
				// Read id, type, and length
				int id = (int)(raw_data[i] & 0xFFFF); // bits 0 - 15
				var type = (DataEntryType)(( raw_data[i] >> 40 ) & 0xFF); // bits 40 - 47
				int length = (int)( ( raw_data[i] >> 48 ) & 0xFFFF ); // bits 48 - 63
				
				// Move iterator to data and read based on type
				i += 1;
				switch ( type ) {
					case DataEntryType.LITERAL_INT:
						if ( length != 4 )
							throw new Exception( "Bad length for literal integer." );
						IntData.Add( id , (long)raw_data[i] );
						i += 1;
						break;

					case DataEntryType.LITERAL_FLOAT:
						if ( length != 4 )
							throw new Exception( "Bad length for literal float." );
						FloatData.Add( id , BitConverter.ToDouble(BitConverter.GetBytes( raw_data[i] ), 0) );
						i += 1;
						break;

					case DataEntryType.LITERAL_STRING:
						var builder = new StringBuilder( length );
						// Parse byte-by-byte (8 bytes per word)
						int j = i;
						for ( ; j < i + ( length / 8 ) + ( length % 8 > 0 ? 1 : 0 ) ; j += 1 ) {
							for ( int k = 0 ; k < length % 8 ; k += 1 ) {
								builder.Append( (char)( ( raw_data[j] << (k * 8) ) & 0xFF ) );
							}
						}
						i = j + 1;
						StringData.Add( id , builder.ToString( ) );
						break;

					default:
						throw new Exception( "Bad data entry type: " + type );
				}
			}
		}

		/// <summary>
		/// Append data entries from data dictionaries.
		/// </summary>
		/// <param name="output">List to append data to.</param>
		public void WriteEntries( List<ulong> output )
		{
			foreach ( KeyValuePair<int, long> kv in IntData ) {
				ulong word = 0;
				word |= ((ulong)kv.Key & 0xFFFF); // id
				word |= ((ulong)( DataEntryType.LITERAL_INT ) & 0xFF) << 40; // type
				word |= ( (ulong)( 8 ) & 0xFFFF ) << 48; // length

				output.Add( word );
				output.Add( (ulong)kv.Value );
			}

			foreach ( KeyValuePair<int , double> kv in FloatData ) {
				ulong word = 0;
				word |= ( (ulong)kv.Key & 0xFFFF ); // id
				word |= ( (ulong)( DataEntryType.LITERAL_INT ) & 0xFF ) << 40; // type
				word |= ( (ulong)( 8 ) & 0xFFFF ) << 48; // length

				output.Add( word );

				word = 0;
				byte[] bytes = BitConverter.GetBytes( kv.Value );
				for ( int i = 0 ; i < bytes.Length ; i += 1 ) {
					word |= ( (ulong)bytes[i] << (i * 8) );
				}
				output.Add( word );
			}

			foreach ( KeyValuePair<int , string> kv in StringData ) {
				ulong word = 0;
				word |= ( (ulong)kv.Key & 0xFFFF ); // id
				word |= ( (ulong)( DataEntryType.LITERAL_INT ) & 0xFF ) << 40; // type
				word |= ( (ulong)( 8 ) & 0xFFFF ) << 48; // length

				output.Add( word );

				word = 0;
				char[] bytes = kv.Value.ToCharArray( );
				for ( int i = 0 ; i < bytes.Length ; i += 1 ) {
					word |= ( (ulong)bytes[i] << ( i * 8 ) );
				}
				output.Add( word );
			}
		}

		/// <summary>
		/// Get an Integer based on an identifier.
		/// </summary>
		/// <param name="id">Data identifier.</param>
		/// <returns>Nullable integer data.</returns>
		public long? GetIntData( int id )
		{
			return IntData.TryGetValue( id , out long val ) ? val : (long?)null;
		}

		/// <summary>
		/// Get a Float based on an identifier.
		/// </summary>
		/// <param name="id">Data identifier.</param>
		/// <returns>Nullable float data.</returns>
		public double? GetFloatData( int id )
		{
			return FloatData.TryGetValue( id , out double val ) ? val : (double?)null;
		}

		/// <summary>
		/// Get a String based on an identifier.
		/// </summary>
		/// <param name="id">Data identifier.</param>
		/// <returns>Nullable string data.</returns>
		public string GetStringData( int id )
		{
			return StringData.TryGetValue( id , out string val ) ? val : null;
		}

		/// <summary>
		/// Add or set an Integer data value.
		/// </summary>
		/// <param name="id">Identifier to set or add to.</param>
		/// <param name="value">Value to set identifier to.</param>
		public void SetIntData( int id , long value )
		{
			IntData.Add( id , value );
		}

		/// <summary>
		/// Add or set a Float data value.
		/// </summary>
		/// <param name="id">Identifier to set or add to.</param>
		/// <param name="value">Value to set identifier to.</param>
		public void SetFloatData( int id , double value )
		{
			FloatData.Add( id , value );
		}

		/// <summary>
		/// Add or set a String data value.
		/// </summary>
		/// <param name="id">Identifier to set or add to.</param>
		/// <param name="value">Value to set identifier to.</param>
		public void SetStringData( int id , string value )
		{
			StringData.Add( id , value );
		}

		/// <summary>
		/// The next unique data identifier for building a data segment.
		/// </summary>
		private int _next_id = 1;

		/// <summary>
		/// Get the next unique id.
		/// </summary>
		/// <returns>A unique data identifier.</returns>
		public int GetNextId( )
		{
			return _next_id++;
		}
	}

}
