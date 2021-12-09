using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BNA.Common
{
	public static class Extensions
	{
		/// <summary>
		/// Test if a nullable character is a letter.
		/// </summary>
		/// <param name="character">The <see cref="char?"/> to test.</param>
		/// <returns>True if the character is not null and is a letter.</returns>
		public static bool IsLetter( this char? character ) => character is char c && char.IsLetter( c );

		/// <summary>
		/// Test if a nullable character is a digit.
		/// </summary>
		/// <param name="character">The <see cref="char?"/> to test.</param>
		/// <returns>True if the character is not null and is a digit.</returns>
		public static bool IsDigit( this char? character ) => character is char c && char.IsDigit( c );

		/// <summary>
		/// Test if a nullable character is a letter or digit.
		/// </summary>
		/// <param name="character">The <see cref="char?"/> to test.</param>
		/// <returns>True if the character is not null and is a letter or digit.</returns>
		public static bool IsLetterOrDigit( this char? character ) => character is char c && char.IsLetterOrDigit( c );

		/// <summary>
		/// Repeat a string a given number of times.
		/// </summary>
		/// <param name="str">String to repeat.</param>
		/// <param name="times">Number of times to repeat.</param>
		/// <returns>The repeated string.</returns>
		public static string Repeat( this string str , int times )
		{
			if ( times < 0 )
			{
				throw new ArgumentOutOfRangeException( "Repeat 'times' parameter cannot be negative." );
			}

			StringBuilder builder = new( times );

			for ( int i = 0 ; i < times ; i += 1 )
			{
				_ = builder.Append( str );
			}

			return builder.ToString( );
		}

		/// <summary>
		/// Add a nullable object to a collection of the same type if the object is not null.
		/// </summary>
		/// <typeparam name="T">Type of the object and collection.</typeparam>
		/// <param name="collection">Collection to add to.</param>
		/// <param name="obj">Object to potentially add.</param>
		public static void AddIfNotNull<T>( this ICollection<T> collection , T? obj )
		{
			if ( obj is T ojb_T )
			{
				collection.Add( ojb_T );
			}
		}

		/// <summary>
		/// Construct a string from all the elements of a collection.
		/// </summary>
		/// <typeparam name="T">Generic any type with a ToString method.</typeparam>
		/// <param name="collection">Collection to iterate through.</param>
		/// <param name="start">Start of the constructed string.</param>
		/// <param name="separator">Separator string between elements.</param>
		/// <param name="end">End of the constructed string.</param>
		/// <returns></returns>
		public static string PrintElements<T>( this ICollection<T> collection , string? start = "[ " , string? separator = ", " , string? end = " ]" )
		{
			StringBuilder builder = new( start );

			foreach ( T obj in collection )
			{
				_ = builder.Append( obj );
				if ( obj is not null && !obj.Equals( collection.Last( ) ) )
				{
					_ = builder.Append( separator );
				}
			}

			return builder.Append( end ).ToString( );
		}
	}
}
