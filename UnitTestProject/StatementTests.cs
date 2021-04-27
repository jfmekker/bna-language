using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
	[TestClass]
	public class StatementTests
	{
		private void RunStatementTest( string statement , bool fails = false )
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

			Assert.AreEqual( !fails ? BNA.ReturnCode.SUCCESS : BNA.ReturnCode.BNA_ERROR , r );
		}

		[TestMethod]
		public void SetTest( )
		{
			this.RunStatementTest( "set" );
		}

		[TestMethod]
		public void AddTest( )
		{
			this.RunStatementTest( "add" );
		}

		[TestMethod]
		public void SubtractTest( )
		{
			this.RunStatementTest( "subtract" );
		}

		[TestMethod]
		public void MultiplyTest( )
		{
			this.RunStatementTest( "multiply" );
		}

		[TestMethod]
		public void DivideTest( )
		{
			this.RunStatementTest( "divide" );
		}

		[TestMethod]
		public void AndTest( )
		{
			this.RunStatementTest( "and" );
		}

		[TestMethod]
		public void OrTest( )
		{
			this.RunStatementTest( "or" );
		}

		[TestMethod]
		public void XorTest( )
		{
			this.RunStatementTest( "xor" );
		}

		[TestMethod]
		public void ModTest( )
		{
			this.RunStatementTest( "mod" );
		}

		[TestMethod]
		public void LogTest( )
		{
			this.RunStatementTest( "log" );
		}

		[TestMethod]
		public void RaiseTest( )
		{
			this.RunStatementTest( "raise" );
		}

		[TestMethod]
		public void NegateTest( )
		{
			this.RunStatementTest( "negate" );
		}

		[TestMethod]
		public void RoundTest( )
		{
			this.RunStatementTest( "round" );
		}

		[TestMethod]
		public void RandomTest( )
		{
			this.RunStatementTest( "random" );
		}

		[TestMethod]
		public void WaitTest( )
		{
			var start = DateTime.Now;
			this.RunStatementTest( "wait" );
			var end = DateTime.Now;

			// Check that elapsed time is greater than required but not more than 0.1s off
			int expected_ms = 2320;
			var elapsed = end.Subtract( start );
			Assert.IsTrue( elapsed.TotalMilliseconds >= expected_ms );
			Assert.IsTrue( elapsed.TotalMilliseconds < expected_ms + 100 );
		}

		[TestMethod]
		public void TestTest( )
		{
			this.RunStatementTest( "test" );
		}

		[TestMethod]
		public void GotoTest( )
		{
			this.RunStatementTest( "goto" );
		}

		[TestMethod]
		public void ListTest( )
		{
			this.RunStatementTest( "list" );
		}

		[TestMethod]
		public void AppendTest( )
		{
			this.RunStatementTest( "append" );
		}

		[TestMethod]
		public void SizeTest( )
		{
			this.RunStatementTest( "size" );
		}

		[TestMethod]
		public void InputTest( )
		{
			this.RunStatementTest( "input" );
		}

		[TestMethod]
		public void PrintTest( )
		{
			this.RunStatementTest( "print" );
		}

		[TestMethod]
		public void TypeTest( )
		{
			this.RunStatementTest( "type" );
		}

		[TestMethod]
		public void ExitTest( )
		{
			this.RunStatementTest( "exit" );
		}

		[TestMethod]
		public void ErrorTest( )
		{
			this.RunStatementTest( "error" , true );
		}
	}
}
