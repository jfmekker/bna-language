using System.Diagnostics.CodeAnalysis;
using BNA.Common;
using BNA.Values;

namespace BNA.Run
{
    [SuppressMessage( "Naming", "CA1707:Identifiers should not contain underscores", Justification = "TODO" )]
    public static class SpecialVariables
    {
        /// <summary>
        /// Variable that is set by any TEST statements.
        /// </summary>
        public static Token TEST_RESULT { get; } = new( "result", TokenType.Variable );
        public static Value TEST_RESULT_DEFAULT { get; } = Value.FALSE;

        /// <summary>
        /// Only variable that passes into a new scope.
        /// </summary>
        public static Token ARGUMENT { get; } = new( "argument", TokenType.Variable );
        public static Value ARGUMENT_DEFAULT { get; } = Value.NULL;

        /// <summary>
        /// Only variables that passes into an old scope.
        /// </summary>
        public static Token RETURN { get; } = new( "return", TokenType.Variable );
        public static Value RETURN_DEFAULT { get; } = Value.NULL;

        /// <summary>
        /// Variable to compare nulls.
        /// </summary>
        public static Token NULL { get; } = new( "null", TokenType.Variable );
        public static Value NULL_DEFAULT { get; } = Value.NULL;
    }
}
