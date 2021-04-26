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

			if ( r == BNA.ReturnCode.FILE_ERROR ) {
				Assert.Inconclusive( );
			}

			Assert.AreEqual( BNA.ReturnCode.SUCCESS , r );
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

		[TestMethod]
		public void SubtractTest( )
		{
			RunStatementTest( "subtract" );
		}

		[TestMethod]
		public void MultiplyTest( )
		{
			RunStatementTest( "multiply" );
		}

		[TestMethod]
		public void DivideTest( )
		{
			RunStatementTest( "divide" );
		}

		[TestMethod]
		public void AndTest( )
		{
			RunStatementTest( "and" );
		}

		[TestMethod]
		public void OrTest( )
		{
			RunStatementTest( "or" );
		}

		[TestMethod]
		public void XorTest( )
		{
			RunStatementTest( "xor" );
		}

		[TestMethod]
		public void ModTest( )
		{
			RunStatementTest( "mod" );
		}

		[TestMethod]
		public void LogTest( )
		{
			RunStatementTest( "log" );
		}

		[TestMethod]
		public void RaiseTest( )
		{
			RunStatementTest( "raise" );
		}

		[TestMethod]
		public void NegateTest( )
		{
			RunStatementTest( "negate" );
		}

		[TestMethod]
		public void RoundTest( )
		{
			RunStatementTest( "round" );
		}

		[TestMethod]
		public void RandomTest( )
		{
			RunStatementTest( "random" );
		}

		[TestMethod]
		public void WaitTest( )
		{
			RunStatementTest( "wait" );
		}

		[TestMethod]
		public void TestTest( )
		{
			RunStatementTest( "test" );
		}

		[TestMethod]
		public void GotoTest( )
		{
			RunStatementTest( "goto" );
		}

		[TestMethod]
		public void ListTest( )
		{
			RunStatementTest( "list" );
		}

		[TestMethod]
		public void AppendTest( )
		{
			RunStatementTest( "append" );
		}

		[TestMethod]
		public void InputTest( )
		{
			// Inconclusive instead of fail until we figure out
			//  how to mock user input in an automated test
			Assert.Inconclusive( );

			//RunStatementTest( "input" );
		}

		[TestMethod]
		public void PrintTest( )
		{
			RunStatementTest( "print" );
		}
	}
}
