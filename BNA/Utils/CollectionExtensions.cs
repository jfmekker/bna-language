using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BNA.Utils
{
    /// <summary>
    /// Extension methods for <see cref="ICollection{T}"/> variables.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Add a nullable <see cref="T"/> to an <see cref="ICollection{T}"/> of the same type if a condition is
        /// met and the object is not null.
        /// </summary>
        /// <typeparam name="T">Type of the object and collection.</typeparam>
        /// <param name="collection">Collection to add to.</param>
        /// <param name="condition">Condition to add based on.</param>
        /// <param name="obj">Object to potentially add.</param>
        public static void AddIf<T>( this ICollection<T> collection, bool condition, T? obj )
        {
            ArgumentNullException.ThrowIfNull( collection );

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
        public static string PrintElements<T>( this ICollection<T> collection, string? start = "[ ", string? separator = ", ", string? end = " ]" )
        {
            ArgumentNullException.ThrowIfNull( collection );
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
