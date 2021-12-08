using BNA.Compile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA.Common
{
	public static class Extensions
	{
		public static bool IsLetter( this char? character ) => character is char c && char.IsLetter( c );

		public static bool IsDigit( this char? character ) => character is char c && char.IsDigit( c );

		public static bool IsLetterOrDigit( this char? character ) => character is char c && char.IsLetterOrDigit( c );

		public static string Repeat( this string str , int times )
		{
			StringBuilder builder = new( times );

			for ( int i = 0 ; i < times ; i += 1 )
			{
				_ = builder.Append( str );
			}

			return builder.ToString( );
		}

		public static void AddIfNotNull<T>( this ICollection<T> collection , T? obj )
		{
			if ( obj is T ojb_T )
			{
				collection.Add( ojb_T );
			}
		}

		public static string PrintElements<T>( this ICollection<T> collection )
		{
			StringBuilder builder = new( "[ " );

			foreach (T obj in collection)
			{
				_ = builder.Append( obj ).Append( ' ' );
			}

			return builder.Append( ']' ).ToString( );
		}
	}
}
