using System.Diagnostics.CodeAnalysis;

namespace BNA.Common
{
    /// <summary>
    /// A BNA language reserved keyword.
    /// </summary>
    /// <remarks>
    /// The backing values (as <see langword="char"/>) are the literal symbols.
    /// In other words "#" == <see cref="COMMENT"/>.
    /// These values face the "user" and should be modified very carefully.
    /// </remarks>
    [SuppressMessage( "Naming", "CA1707:Identifiers should not contain underscores", Justification = "TODO" )]
    public enum Symbol
    {
        NULL = '\0',
        ESCAPE = '\\',
        COMMENT = '#',
        GREATER_THAN = '>',
        LESS_THAN = '<',
        EQUAL = '=',
        NOT = '!',
        LABEL_START = '^',
        LABEL_END = ':',
        STRING_MARKER = '"',
        ACCESSOR = '@',
        LIST_START = '(',
        LIST_END = ')',
        LIST_SEPARATOR = ',',
    }
}
