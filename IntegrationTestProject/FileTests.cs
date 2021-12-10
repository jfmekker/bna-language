using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BnaUnitTests
{
	[TestClass]
	public class FileTests
	{
		[TestMethod]
		public void SetTest( )
		{
			TestUtils.RunTestFile( "statement_test_set" );
		}

		[TestMethod]
		public void AddTest( )
		{
			TestUtils.RunTestFile( "statement_test_add" );
		}

		[TestMethod]
		public void SubtractTest( )
		{
			TestUtils.RunTestFile( "statement_test_subtract" );
		}

		[TestMethod]
		public void MultiplyTest( )
		{
			TestUtils.RunTestFile( "statement_test_multiply" );
		}

		[TestMethod]
		public void DivideTest( )
		{
			TestUtils.RunTestFile( "statement_test_divide" );
		}

		[TestMethod]
		public void AndTest( )
		{
			TestUtils.RunTestFile( "statement_test_and" );
		}

		[TestMethod]
		public void OrTest( )
		{
			TestUtils.RunTestFile( "statement_test_or" );
		}

		[TestMethod]
		public void XorTest( )
		{
			TestUtils.RunTestFile( "statement_test_xor" );
		}

		[TestMethod]
		public void ModTest( )
		{
			TestUtils.RunTestFile( "statement_test_mod" );
		}

		[TestMethod]
		public void LogTest( )
		{
			TestUtils.RunTestFile( "statement_test_log" );
		}

		[TestMethod]
		public void RaiseTest( )
		{
			TestUtils.RunTestFile( "statement_test_raise" );
		}

		[TestMethod]
		public void NegateTest( )
		{
			TestUtils.RunTestFile( "statement_test_negate" );
		}

		[TestMethod]
		public void RoundTest( )
		{
			TestUtils.RunTestFile( "statement_test_round" );
		}

		[TestMethod]
		public void RandomTest( )
		{
			TestUtils.RunTestFile( "statement_test_random" );
		}

		[TestMethod]
		public void WaitTest( )
		{
			DateTime start = DateTime.Now;
			TestUtils.RunTestFile( "statement_test_wait" );
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
			TestUtils.RunTestFile( "statement_test_test" );
		}

		[TestMethod]
		public void GotoTest( )
		{
			TestUtils.RunTestFile( "statement_test_goto" );
		}

		[TestMethod]
		public void ListTest( )
		{
			TestUtils.RunTestFile( "statement_test_list" );
		}

		[TestMethod]
		public void AppendTest( )
		{
			TestUtils.RunTestFile( "statement_test_append" );
		}

		[TestMethod]
		public void SizeTest( )
		{
			TestUtils.RunTestFile( "statement_test_size" );
		}

		[TestMethod]
		public void InputTest( )
		{
			TestUtils.RunTestFile( "statement_test_input" );
		}

		[TestMethod]
		public void PrintTest( )
		{
			TestUtils.RunTestFile( "statement_test_print" );
		}

		[TestMethod]
		public void TypeTest( )
		{
			TestUtils.RunTestFile( "statement_test_type" );
		}

		[TestMethod]
		public void ExitTest( )
		{
			TestUtils.RunTestFile( "statement_test_exit" );
		}

		[TestMethod]
		public void ErrorTest( )
		{
			TestUtils.RunTestFile( "statement_test_error" , true );
		}

		[TestMethod]
		public void ReadTest( )
		{
			TestUtils.RunTestFile( "statement_test_read" );
		}

		[TestMethod]
		public void WriteTest( )
		{
			TestUtils.RunTestFile( "statement_test_write" );
		}

		[TestMethod]
		public void OpenReadTest( )
		{
			TestUtils.RunTestFile( "statement_test_open_read" );
		}

		[TestMethod]
		public void OpenWriteTest( )
		{
			TestUtils.RunTestFile( "statement_test_open_write" );
		}
	}
}
