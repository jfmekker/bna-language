namespace BNA.Common
{
	/// <summary>
	/// A BNA language reserved keyword.
	/// </summary>
	/// <remarks>
	/// The names of the enum values are the literal keywords that they match to.
	/// In other words "ADD" == <see cref="ADD"/> (case insensitive).
	/// These values face the "user" and should be modified very carefully.
	/// </remarks>
	public enum Keyword
	{
		// Operation start keywords
		SET,
		ADD,
		SUBTRACT,
		MULTIPLY,
		DIVIDE,
		WAIT,
		RANDOM,
		TEST,
		GOTO,
		OR,
		AND,
		XOR,
		NEGATE,
		RAISE,
		MOD,
		LOG,
		ROUND,
		LIST,
		APPEND,
		SIZE,
		OPEN,
		CLOSE,
		READ,
		WRITE,
		INPUT,
		PRINT,
		TYPE,
		EXIT,
		ERROR,
		SCOPE,

		// Operation mid keywords
		TO,
		BY,
		FROM,
		MAX,
		IF,
		WITH,
		OF,
		AS,
	}
}
