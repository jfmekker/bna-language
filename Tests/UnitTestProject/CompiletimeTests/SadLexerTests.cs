using Microsoft.VisualStudio.TestTools.UnitTesting;
using BNA.Compile;
using BNA.Exceptions;

namespace CompiletimeTests
{
	[TestClass]
	public class SadLexerTests
	{
		[TestMethod]
		[DataRow( "0 0" , DisplayName = "Multiple tokens" )]
		[DataRow( "( 0 0 )" , DisplayName = "List with no separator" )]
		[DataRow( "( > )" , DisplayName = "Symbol in list" )]
		public void Lexer_ReadSingleToken_ThrowsIllegalTokenException( string str )
		{
			_ = Assert.ThrowsException<IllegalTokenException>(
				( ) => Lexer.ReadSingleToken( str ) );
		}

		[TestMethod]
		[DataRow( "(" , DisplayName = "Empty list" )]
		[DataRow( "(0," , DisplayName = "One element list" )]
		[DataRow( "\"" , DisplayName = "Empty string" )]
		[DataRow( "\"string" , DisplayName = "String with word" )]
		[DataRow( "\"string\\\"" , DisplayName = "String with escaped quote" )]
		public void Lexer_ReadSingleToken_ThrowsMissingTerminatorException( string str )
		{
			_ = Assert.ThrowsException<MissingTerminatorException>(
				( ) => Lexer.ReadSingleToken( str ) );
		}

		[TestMethod]
		[DataRow( "*" )]
		[DataRow( "/" )]
		[DataRow( "\\" )]
		[DataRow( "@" )]
		[DataRow( "$" )]
		[DataRow( "%" )]
		[DataRow( "&" )]
		public void Lexer_ReadSingleToken_ThrowsUnexpectedSymbolException( string str )
		{
			_ = Assert.ThrowsException<UnexpectedSymbolException>(
				( ) => Lexer.ReadSingleToken( str ) );
		}

		[TestMethod]
		[DataRow( "x@" , DisplayName = "Accessor at end" )]
		[DataRow( "x@@1" , DisplayName = "Double accessors" )]
		[DataRow( "0.." , DisplayName = "Double decimal points" )]
		[DataRow( "0-" , DisplayName = "Number then negative sign" )]
		[DataRow( "0+" , DisplayName = "Number then positive sign" )]
		public void Lexer_ReadSingleToken_ThrowsInvalidTokenException( string str )
		{
			_ = Assert.ThrowsException<InvalidTokenException>(
				( ) => Lexer.ReadSingleToken( str ) );
		}
	}
}
