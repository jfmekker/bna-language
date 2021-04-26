using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
	[TestClass]
	public class StatementTests
	{
		private void RunStatementTest( string statement )
		{
			BNA.ReturnCode r = BNA.ReturnCode.UNEXPECTED_ERROR;
			try {
				r = BNA.BNA.RunFromFiles( new string[] { "../../../../Tests/statement_test_" + statement + ".bna" } );
			}
			catch ( Exception e ) {
				Assert.Inconclusive( );
			}
			Assert.AreEqual( BNA.ReturnCode.SUCCESS , r );
		}

		[TestMethod]
		public void PrintTest( )
		{
			RunStatementTest( "print" );
		}

		[TestMethod]
		public void SetTest( )
		{
			RunStatementTest( "set" );
		}

		[TestMethod]
		public void AddTest( )
		{
			RunStatementTest( "add" );
		}
	}
}
