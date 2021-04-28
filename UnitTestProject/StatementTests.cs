using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
	[TestClass]
	public class StatementTests
	{
		[TestMethod]
		public void SetTest( )
		{
			TestRunner.RunTestFile( "statement_test_set" );
		}

		[TestMethod]
		public void AddTest( )
		{
			TestRunner.RunTestFile( "statement_test_add" );
		}

		[TestMethod]
		public void SubtractTest( )
		{
			TestRunner.RunTestFile( "statement_test_subtract" );
		}

		[TestMethod]
		public void MultiplyTest( )
		{
			TestRunner.RunTestFile( "statement_test_multiply" );
		}

		[TestMethod]
		public void DivideTest( )
		{
			TestRunner.RunTestFile( "statement_test_divide" );
		}

		[TestMethod]
		public void AndTest( )
		{
			TestRunner.RunTestFile( "statement_test_and" );
		}

		[TestMethod]
		public void OrTest( )
		{
			TestRunner.RunTestFile( "statement_test_or" );
		}

		[TestMethod]
		public void XorTest( )
		{
			TestRunner.RunTestFile( "statement_test_xor" );
		}

		[TestMethod]
		public void ModTest( )
		{
			TestRunner.RunTestFile( "statement_test_mod" );
		}

		[TestMethod]
		public void LogTest( )
		{
			TestRunner.RunTestFile( "statement_test_log" );
		}

		[TestMethod]
		public void RaiseTest( )
		{
			TestRunner.RunTestFile( "statement_test_raise" );
		}

		[TestMethod]
		public void NegateTest( )
		{
			TestRunner.RunTestFile( "statement_test_negate" );
		}

		[TestMethod]
		public void RoundTest( )
		{
			TestRunner.RunTestFile( "statement_test_round" );
		}

		[TestMethod]
		public void RandomTest( )
		{
			TestRunner.RunTestFile( "statement_test_random" );
		}

		[TestMethod]
		public void WaitTest( )
		{
			DateTime start = DateTime.Now;
			TestRunner.RunTestFile( "statement_test_wait" );
			DateTime end = DateTime.Now;

			// Check that elapsed time is greater than required but not more than 0.1s off
			int expected_ms = 2320;
			TimeSpan elapsed = end.Subtract( start );
			Assert.IsTrue( elapsed.TotalMilliseconds >= expected_ms );
			Assert.IsTrue( elapsed.TotalMilliseconds < expected_ms + 100 );
		}

		[TestMethod]
		public void TestTest( )
		{
			TestRunner.RunTestFile( "statement_test_test" );
		}

		[TestMethod]
		public void GotoTest( )
		{
			TestRunner.RunTestFile( "statement_test_goto" );
		}

		[TestMethod]
		public void ListTest( )
		{
			TestRunner.RunTestFile( "statement_test_list" );
		}

		[TestMethod]
		public void AppendTest( )
		{
			TestRunner.RunTestFile( "statement_test_append" );
		}

		[TestMethod]
		public void SizeTest( )
		{
			TestRunner.RunTestFile( "statement_test_size" );
		}

		[TestMethod]
		public void InputTest( )
		{
			TestRunner.RunTestFile( "statement_test_input" );
		}

		[TestMethod]
		public void PrintTest( )
		{
			TestRunner.RunTestFile( "statement_test_print" );
		}

		[TestMethod]
		public void TypeTest( )
		{
			TestRunner.RunTestFile( "statement_test_type" );
		}

		[TestMethod]
		public void ExitTest( )
		{
			TestRunner.RunTestFile( "statement_test_exit" );
		}

		[TestMethod]
		public void ErrorTest( )
		{
			TestRunner.RunTestFile( "statement_test_error" , true );
		}

		[TestMethod]
		public void ReadTest( )
		{
			TestRunner.RunTestFile( "statement_test_read" );
		}

		[TestMethod]
		public void WriteTest( )
		{
			TestRunner.RunTestFile( "statement_test_write" );
		}

		[TestMethod]
		public void OpenReadTest( )
		{
			TestRunner.RunTestFile( "statement_test_open_read" );
		}

		[TestMethod]
		public void OpenWriteTest( )
		{
			TestRunner.RunTestFile( "statement_test_open_write" );
		}
	}
}
