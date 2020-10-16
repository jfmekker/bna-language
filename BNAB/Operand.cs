namespace BNAB
{
	/// <summary>
	/// How an operand value is stored.
	/// </summary>
	public enum OperandType
	{
		VARIABLE = 1,
		LITERAL,
		SMALL_LITERAL
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
