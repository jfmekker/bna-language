using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BNA.Common
{
	/// <summary>
	/// Collection of various extension methods.
	/// </summary>
	/// <seealso href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods"/>
	public static class Extensions
	{
		/// <summary>
		/// Test if a nullable <see langword="char"/> is a letter.
		/// </summary>
		/// <param name="character">The charcter to test.</param>
		/// <returns>True if the character is not <see langword="null"/> and is a letter.</returns>
		public static bool IsLetter( this char? character ) => character is char c && char.IsLetter( c );

		/// <summary>
		/// Test if a nullable <see langword="char"/> is a digit.
		/// </summary>
		/// <param name="character">The charcter to test.</param>
		/// <returns>True if the character is not <see langword="null"/> and is a digit.</returns>
		public static bool IsDigit( this char? character ) => character is char c && char.IsDigit( c );

		/// <summary>
		/// Test if a nullable <see langword="char"/> is a letter or digit.
		/// </summary>
		/// <param name="character">The character to test.</param>
		/// <returns>True if the character is not <see langword="null"/> and is a letter or digit.</returns>
		public static bool IsLetterOrDigit( this char? character ) => character is char c && char.IsLetterOrDigit( c );

		/// <summary>
		/// Repeat a <see langword="string"/> a given number of times.
		/// </summary>
		/// <param name="str">String to repeat.</param>
		/// <param name="times">Number of times to repeat.</param>
		/// <returns>The repeated string.</returns>
		public static string Repeat( this string str , int times )
		{
			if ( times < 0 )
			{
				throw new ArgumentOutOfRangeException( times.ToString( ) , "Repeat 'times' parameter cannot be negative." );
			}

			StringBuilder builder = new( times );

			for ( int i = 0 ; i < times ; i += 1 )
			{
				_ = builder.Append( str );
			}

			return builder.ToString( );
		}

		/// <summary>
		/// Add a nullable <see cref="T"/> to an <see cref="ICollection{T}"/> of the same type if a condition is
		/// met and the object is not null.
		/// </summary>
		/// <typeparam name="T">Type of the object and collection.</typeparam>
		/// <param name="collection">Collection to add to.</param>
		/// <param name="condition">Condition to add based on.</param>
		/// <param name="obj">Object to potentially add.</param>
		public static void AddIf<T>( this ICollection<T> collection , bool condition , T? obj )
		{
			if ( condition && obj is not null )
			{
				collection.Add( obj );
			}
		}

		/// <summary>
		/// Construct a <see langword="string"/> from all the elements of an <see cref="ICollection{T}"/>.
		/// </summary>
		/// <typeparam name="T">Generic any type with a ToString method.</typeparam>
		/// <param name="collection">Collection to iterate through.</param>
		/// <param name="start">Start of the constructed string.</param>
		/// <param name="separator">Separator string between elements.</param>
		/// <param name="end">End of the constructed string.</param>
		/// <returns>Constructed string.</returns>
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
