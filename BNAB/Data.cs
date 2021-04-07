using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNAB.Util;

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
		public ulong[] Raw
		{
			get; private set;
		}

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
		public DataSegment( ulong[] raw )
		{
			Raw = raw;
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

			// Parse word-by-word
			for ( int i = 0 ; i < Raw.Length ; ) {
				// Read id, type, and length
				int id = (int)BitHelper.GetBits(0, 15, Raw[i]);
				var type = (DataEntryType)BitHelper.GetBits(40, 47, Raw[i]);
				int length = (int)BitHelper.GetBits(48, 63, Raw[i]);
				
				// Move iterator to data and read based on type
				i += 1;
				switch ( type ) {
					case DataEntryType.LITERAL_INT:
						if ( length != 4 )
							throw new Exception( "Bad length for literal integer." );
						IntData.Add( id , (long)Raw[i] );
						i += 1;
						break;

					case DataEntryType.LITERAL_FLOAT:
						if ( length != 4 )
							throw new Exception( "Bad length for literal float." );
						FloatData.Add( id , BitHelper.WordToDouble( Raw[i] ) );
						i += 1;
						break;

					case DataEntryType.LITERAL_STRING:
						ulong[] words = new ArraySegment<ulong>( Raw , i , length ).ToArray();
						StringData.Add( id , BitHelper.WordsToString( words, length ));
						break;

					default:
						throw new Exception( "Bad data entry type: " + type );
				}
			}
		}

		/// <summary>
		/// Fill the raw data array from the data dictionaries.
		/// </summary>
		public void WriteEntries(  )
		{
			var output = new List<ulong>( );

			foreach ( KeyValuePair<int, long> kv in IntData ) {
				ulong word = 0;
				BitHelper.SetBits( 0 , 15 , ref word , (ulong)kv.Value ); // id
				BitHelper.SetBits( 40 , 47 , ref word , (ulong)DataEntryType.LITERAL_INT ); // type
				BitHelper.SetBits( 48 , 63 , ref word , 8ul ); // length
				
				output.Add( word );
				output.Add( (ulong)kv.Value );
			}

			foreach ( KeyValuePair<int , double> kv in FloatData ) {
				ulong word = 0;
				BitHelper.SetBits( 0 , 15 , ref word , (ulong)kv.Value ); // id
				BitHelper.SetBits( 40 , 47 , ref word , (ulong)DataEntryType.LITERAL_FLOAT ); // type
				BitHelper.SetBits( 48 , 63 , ref word , 8ul ); // length

				output.Add( word );
				output.Add( BitHelper.DoubleToWord( kv.Value ) );
			}

			foreach ( KeyValuePair<int , string> kv in StringData ) {
				ulong word = 0;
				word |= ( (ulong)kv.Key & 0xFFFF ); // id
				word |= ( (ulong)( DataEntryType.LITERAL_INT ) & 0xFF ) << 40; // type
				word |= ( (ulong)( 8 ) & 0xFFFF ) << 48; // length

				output.Add( word );

				ulong[] words = BitHelper.StringToWords( kv.Value );
				for ( int i = 0 ; i < words.Length ; i += 1 ) {
					output.Add( words[i] );
				}
				output.Add( word );
			}

			Raw = output.ToArray( );
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

		/// <summary>
		/// Give all the data entries in the data segment as a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString( )
		{
			var str = new StringBuilder( );
			str.AppendLine( "**** DATA ****" );

			foreach ( KeyValuePair<int , long> pair in IntData ) {
				str.Append( "Entry: type=int id=" );
				str.Append( pair.Key );
				str.Append( " value=" );
				str.Append( pair.Value );
				str.AppendLine( );
			}

			foreach ( KeyValuePair<int , double> pair in FloatData ) {
				str.Append( "Entry: type=float id=" );
				str.Append( pair.Key );
				str.Append( " value=" );
				str.Append( pair.Value );
				str.AppendLine( );
			}

			foreach ( KeyValuePair<int , string> pair in StringData ) {
				str.Append( "Entry: type=string id=" );
				str.Append( pair.Key );
				str.Append( " value=" );
				str.Append( pair.Value );
				str.AppendLine( );
			}

			return str.ToString();
		}
	}
}
