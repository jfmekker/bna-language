namespace BNA
{
	public class SpecialVariables
	{
		public static readonly Token TEST_RESULT = new Token( "_test" , TokenType.VARIABLE );
		public static readonly Value TEST_RESULT_DEFAULT = Value.FALSE;

		public static readonly Token ARGUMENT = new Token( "argument" , TokenType.VARIABLE );
		public static readonly Value ARGUMENT_DEFAULT = Value.NULL;

		public static readonly Token RETURN = new Token( "return" , TokenType.VARIABLE );
		public static readonly Value RETURN_DEFAULT = Value.NULL;
	}
}
