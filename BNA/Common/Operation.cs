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
		NULL = 0,
		LABEL,

		// numeric operations
		SET,
		ADD,
		SUBTRACT,
		MULTIPLY,
		DIVIDE,
		RANDOM,
		BITWISE_OR,
		BITWISE_AND,
		BITWISE_XOR,
		BITWISE_NEGATE,
		POWER,
		MODULUS,
		LOGARITHM,
		ROUND,

		// list operations
		LIST,
		APPEND,
		SIZE,

		// io operations
		OPEN_READ,
		OPEN_WRITE,
		CLOSE,
		READ,
		WRITE,
		INPUT,
		PRINT,

		// test operations
		TEST_GREATER_THAN,
		TEST_LESS_THAN,
		TEST_EQUAL,
		TEST_NOT_EQUAL,

		// scope operations
		SCOPE_OPEN,
		SCOPE_CLOSE,

		// misc operations
		WAIT,
		GOTO,
		TYPE,
		EXIT,
		ERROR,
	}
}
