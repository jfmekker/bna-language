namespace BNA.Common
{
	public enum Operation
	{
		// non-operations
		NULL = 0,
		COMMENT,
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
		TEST_GTR,
		TEST_LSS,
		TEST_EQU,
		TEST_NEQ,

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
