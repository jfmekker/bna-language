using System;
using System.Text;

namespace BNA.Utils
{
    /// <summary>
    /// Extension methods for <see cref="string"/> and <see cref="StringBuilder"/>.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Append a <see langword="string"/> repeated a given number of times.
        /// </summary>
        /// <param name="builder">String builder instance.</param>
        /// <param name="str">String to repeat.</param>
        /// <param name="times">Number of times to repeat.</param>
        /// <returns>The string builder.</returns>
        public static StringBuilder AppendRepeated( this StringBuilder builder, string str, int times )
        {
            ArgumentOutOfRangeException.ThrowIfLessThan( times, 0 );

            for ( int i = 0 ; i < times ; i += 1 )
            {
                _ = builder.Append( str );
            }

            return builder;
        }

        /// <summary>
        /// Append a <see langword="char"/> repeated a given number of times.
        /// </summary>
        /// <param name="builder">String builder instance.</param>
        /// <param name="c">Character to repeat.</param>
        /// <param name="times">Number of times to repeat.</param>
        /// <returns>The string builder.</returns>
        public static StringBuilder AppendRepeated( this StringBuilder builder, char c, int times )
        {
            ArgumentOutOfRangeException.ThrowIfLessThan( times, 0 );

            for ( int i = 0 ; i < times ; i += 1 )
            {
                _ = builder.Append( c );
            }

            return builder;
        }

        /// <summary>
        /// Append an object as a string with an attached prefix and suffix, but only if the object is not null.
        /// </summary>
        /// <remarks>
        /// This implementation takes reference types.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="obj"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static StringBuilder AppendNullable<T>( this StringBuilder builder, T? obj, string? prefix = null, string? suffix = null )
            where T : class
        {
            if ( obj is not null )
            {
                _ = builder.Append( prefix )
                           .Append( obj.ToString( ) )
                           .Append( suffix );
            }

            return builder;
        }

        /// <summary>
        /// Append an object as a string with an attached prefix and suffix, but only if the object is not null.
        /// </summary>
        /// <remarks>
        /// This implementation takes value types. It uses generics to avoid a boxing conversion that would come
        /// from using <see cref="object"/> as a parameter.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="obj"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static StringBuilder AppendNullable<T>( this StringBuilder builder, T? obj, string? prefix = null, string? suffix = null )
            where T : struct
        {
            if ( obj is not null )
            {
                _ = builder.Append( prefix )
                           .Append( obj.ToString( ) )
                           .Append( suffix );
            }

            return builder;
        }

        /// <summary>
        /// Repeat a <see langword="string"/> a given number of times.
        /// </summary>
        /// <param name="str">String to repeat.</param>
        /// <param name="times">Number of times to repeat.</param>
        /// <returns>The repeated string.</returns>
        public static string Repeat( this string str, int times )
        {
            ArgumentOutOfRangeException.ThrowIfLessThan( times, 0 );

            StringBuilder builder = new( times );

            for ( int i = 0 ; i < times ; i += 1 )
            {
                _ = builder.Append( str );
            }

            return builder.ToString( );
        }

        /// <summary>
        /// Return a nullable object's <see cref="object.ToString()"/> or <c>"null"</c>
        /// if the object is <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string NullableString<T>( this T? obj )
            where T : class
            => obj is null ? "null" : obj.ToString( ) ?? "null";

        /// <summary>
        /// Return a nullable object's <see cref="object.ToString()"/> or <c>"null"</c>
        /// if the object is <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string NullableString<T>( this T? obj )
            where T : struct
            => obj is null ? "null" : obj.ToString( ) ?? "null";

    }
}
