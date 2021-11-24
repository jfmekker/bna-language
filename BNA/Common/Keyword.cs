using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA.Common
{
	public enum Keyword
	{
		// Empty keyword to represent unknown
		_ = 0,

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
