using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNA.Common
{
	public enum Symbol
	{
		// Default to 'null' value
		NULL = '\0',
		COMMENT = '#',
		GREATER_THAN = '>',
		LESS_THAN = '<',
		EQUAL = '=',
		NOT = '!',
		LABEL_START = '^',
		LABEL_END = ':',
		LINE_END = '\n',
		STRING_MARKER = '"',
		ACCESSOR = '@',
		LIST_START = '(',
		LIST_END = ')',
		LIST_SEPERATOR = ',',
	}
}
