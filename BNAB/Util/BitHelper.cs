using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNAB.Util
{
	/// <summary>
	/// Helper functions for bit operations. Assumes a word length of 64 bits.
	/// </summary>
	class BitHelper
	{
		/// <summary>
		/// Construct a 64-bit bit mask, enabling all bits between two indices.
		/// </summary>
		/// <param name="start">starting bit index</param>
		/// <param name="end">ending bit index</param>
		/// <returns>64-bit bit mask</returns>
		public static ulong BitMask( int start , int end )
		{
			// Check arguments
			if ( start < 0 || end < 0 || start >= 64 || end >= 64 ) {
				throw new ArgumentException( "Invalid bit indices: start=" + start + " end=" + end );
			}

			ulong mask = 0;
			for ( int i = start ; i <= end ; i += 1 ) {
				mask |= (1ul << i);
			}

			return mask;
		}

		/// <summary>
		/// Set the specified bits in a word to a value.
		/// </summary>
		/// <param name="start">bit index to start from, inclusive</param>
		/// <param name="end">bit index to end with, inclusive</param>
		/// <param name="word">word variable to change bits within</param>
		/// <param name="value">value to put in the word</param>
		public static void SetBits( int start , int end , ref ulong word , ulong value )
		{
			int size = ( end - start ) + 1;
			
			// Check arguments
			if ( start < 0 || end < 0 || size <= 0 || size > 64 ) {
				throw new ArgumentException( "Invalid bit indices: start=" + start + " end=" + end );
			}

			// Check if value fits in size
			if ( value >> size != 0 ) {
				Console.Error.WriteLine( "SetBits: value will be truncated to size" );
			}

			// Move and isolate value
			value = ( value << start ) & BitMask( start , end );

			// Set bits in word
			word &= ~BitMask( start , end );
			word |= value;
		}

		/// <summary>
		/// Return the value in a word between the specified bits.
		/// </summary>
		/// <param name="start">bit index to start from, inclusive</param>
		/// <param name="end">bit index to end with, inclusive</param>
		/// <param name="word">word to get value from</param>
		/// <returns>the integer value starting from <paramref name="start"/> ending with <paramref name="end"/></returns>
		public static ulong GetBits( int start , int end , ulong word )
		{
			return ( word & BitMask( start , end ) ) >> start;
		}

		/// <summary>
		/// Convert the bit representation of a <see cref="double"/> to a <see cref="ulong"/>.
		/// </summary>
		/// <param name="value">the floating point value to convert</param>
		/// <returns>bit representation of the given value</returns>
		public static ulong DoubleToWord( double value )
		{
			ulong val = 0;

			byte[] bytes = BitConverter.GetBytes( value );

			for ( int i = 0 ; i < 8 ; i += 1 ) {
				SetBits( i * 8 , ( i * 8 ) + 7 , ref val , bytes[i] );
			}

			return val;
		}

		/// <summary>
		/// Get a <see cref="double"/> value from a representation stored in a <see cref="ulong"/>
		/// </summary>
		/// <param name="value">integer value storing a floating point representation</param>
		/// <returns>value of the stored floating point</returns>
		public static double WordToDouble( ulong value )
		{
			return BitConverter.ToDouble( BitConverter.GetBytes( value ) , 0 );
		}


		/// <summary>
		/// Store a string in an array of words
		/// </summary>
		/// <param name="str">string to store</param>
		/// <returns>an array of words with the string stored</returns>
		public static ulong[] StringToWords( string str )
		{
			// Get char list with length as a multiple of 8
			var chars = str.ToList();
			for ( int i = chars.Count % 8 ; i > 0 ; i -= 1 ) {
				chars.Add( '\0' );
			}

			// Convert to byte array
			var bytes = new byte[chars.Count];
			for ( int i = 0 ; i < chars.Count ; i += 1 ) bytes[i] = (byte)chars[i]; 

			// Use bytes to fill word array
			var words = new ulong[bytes.Length / 8];
			for ( int i = 0 ; i < words.Length ; i += 1 ) {
				words[i] = BitConverter.ToUInt64( bytes , i * 8 );
			}

			return words;
		}

		/// <summary>
		/// Get a string of certain length from an array of words
		/// </summary>
		/// <param name="words">array of words with stored string</param>
		/// <param name="length">length of the string in bytes</param>
		/// <returns>the stored string</returns>
		public static string WordsToString( ulong[] words, int length )
		{
			if ( length > words.Length ) {
				throw new ArgumentException( "Length longer than array length." );
			}

			var str = new StringBuilder();
			for ( int i = 0 ; i < length ; i += 1 ) {
				str.Append( (char)GetBits( i % 8 , ( i % 8 ) + 7 , words[i / 8] ) );
			}

			return str.ToString();
		}
	}
}
