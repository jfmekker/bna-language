namespace BNA.Common
{
    /// <summary>
    /// The type of an operation for <see cref="Compile.Statement"/> or <see cref="Run.Instruction"/>
    /// </summary>
    /// <remarks>
    /// This is not "user" facing, so values can be renamed without too much issue.
    /// </remarks>
    public enum Operation
    {
        // non-operations
        None = 0,
        Label,

        // numeric operations
        Set,
        Add,
        Subtract,
        Multiply,
        Divide,
        Random,
        BitwiseOR,
        BitwiseAND,
        BitwiseXOR,
        BitwiseNOT,
        Power,
        Modulus,
        Logarithm,
        Round,

        // list operations
        List,
        Append,
        Size,

        // io operations
        OpenRead,
        OpenWrite,
        Close,
        Read,
        Write,
        Input,
        Print,

        // test operations
        TestGreaterThan,
        TestLessThan,
        TestEqualTo,
        TestNotEqualTo,

        // scope operations
        ScopeOpen,
        ScopeClose,

        // misc operations
        Wait,
        Goto,
        Type,
        Exit,
        Error,
    }
}
