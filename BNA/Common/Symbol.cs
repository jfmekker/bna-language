namespace BNA.Common
{
    /// <summary>
    /// A BNA language reserved character.
    /// </summary>
    /// <remarks>
    /// The backing values (as <see langword="char"/>) are the literal symbols.
    /// In other words "#" == <see cref="Comment"/>.
    /// These values face the "user" and should be modified very carefully.
    /// </remarks>
    public enum Symbol
    {
        Null = '\0',
        Escape = '\\',
        Comment = '#',
        GreaterThan = '>',
        LessThan = '<',
        Equal = '=',
        Not = '!',
        LabelStart = '^',
        LabelEnd = ':',
        StringDelim = '"',
        Accessor = '@',
        ListStart = '(',
        ListEnd = ')',
        ListSeparator = ',',
    }
}
