namespace BNA
{
	public class SpecialVariables
	{
		/// <summary>
		/// Variable that is set by any TEST statements.
		/// </summary>
		public static readonly Token TEST_RESULT = new( "result" , TokenType.VARIABLE );
		public static readonly Value TEST_RESULT_DEFAULT = Value.FALSE;

		/// <summary>
		/// Only variable that passes into a new scope.
		/// </summary>
		public static readonly Token ARGUMENT = new( "argument" , TokenType.VARIABLE );
		public static readonly Value ARGUMENT_DEFAULT = Value.NULL;

		/// <summary>
		/// Only variables that passes into an old scope.
		/// </summary>
		public static readonly Token RETURN = new( "return" , TokenType.VARIABLE );
		public static readonly Value RETURN_DEFAULT = Value.NULL;
	}
}
