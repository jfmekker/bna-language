using System.Text;

namespace BNA.Utils
{
    internal static class StringBuilderExtensions
    {
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
    }
}
