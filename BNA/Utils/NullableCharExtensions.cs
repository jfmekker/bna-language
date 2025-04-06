namespace BNA.Utils
{
    /// <summary>
    /// Extension methods for <c>char?</c> variables.
    /// </summary>
    public static class NullableCharExtensions
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
    }
}
