using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNAB
{
	public enum DataEntryType : byte
	{
		NONE, LITERAL_INT, LITERAL_FLOAT, LITERAL_STRING
	}

	public class DataSegment
	{
		private ArraySegment<ulong> _raw;

		private Dictionary<int,   long> IntData;
		private Dictionary<int, double> FloatData;
		private Dictionary<int, string> StringData;

		public DataSegment( ArraySegment<ulong> raw )
		{
			_raw = raw;
			ReadEntries( );
		}

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

		public long? GetIntData( int id )
		{
			return IntData.TryGetValue( id , out long val ) ? val : (long?)null;
		}

		public double? GetFloatData( int id )
		{
			return FloatData.TryGetValue( id , out double val ) ? val : (double?)null;
		}

		public string GetStringData( int id )
		{
			return StringData.TryGetValue( id , out string val ) ? val : null;
		}

		public void SetIntData( int id , long value )
		{
			IntData.Add( id , value );
		}

		public void SetFloatData( int id , double value )
		{
			FloatData.Add( id , value );
		}

		public void SetStringData( int id , string value )
		{
			StringData.Add( id , value );
		}
	}

}
