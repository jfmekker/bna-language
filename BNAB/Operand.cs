namespace BNAB
{
	/// <summary>
	/// How an operand value is stored.
	/// </summary>
	public enum OperandType
	{
		NONE = 0,
		VARIABLE = 1,
		LITERAL,
		ELEMENT,
		LIST,
		STRING
	}

	/// <summary>
	/// What the actual data of an operand is.
	/// </summary>
	public enum OperandDataType
	{
		INTEGER = 1,
		FLOAT,
		STRING,
		// TODO add LIST and ELEMENT
	}
}
