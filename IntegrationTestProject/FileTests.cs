using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
	[TestClass]
	public class FileTests
	{
		[TestMethod]
		public void SetTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_set" );
		}

		[TestMethod]
		public void AddTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_add" );
		}

		[TestMethod]
		public void SubtractTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_subtract" );
		}

		[TestMethod]
		public void MultiplyTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_multiply" );
		}

		[TestMethod]
		public void DivideTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_divide" );
		}

		[TestMethod]
		public void AndTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_and" );
		}

		[TestMethod]
		public void OrTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_or" );
		}

		[TestMethod]
		public void XorTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_xor" );
		}

		[TestMethod]
		public void ModTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_mod" );
		}

		[TestMethod]
		public void LogTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_log" );
		}

		[TestMethod]
		public void RaiseTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_raise" );
		}

		[TestMethod]
		public void NegateTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_negate" );
		}

		[TestMethod]
		public void RoundTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_round" );
		}

		[TestMethod]
		public void RandomTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_random" );
		}

		[TestMethod]
		public void WaitTest( )
		{
			DateTime start = DateTime.Now;
			TestFileRunner.RunTestFile( "statement_test_wait" );
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
			TestFileRunner.RunTestFile( "statement_test_test" );
		}

		[TestMethod]
		public void GotoTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_goto" );
		}

		[TestMethod]
		public void ListTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_list" );
		}

		[TestMethod]
		public void AppendTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_append" );
		}

		[TestMethod]
		public void SizeTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_size" );
		}

		[TestMethod]
		public void InputTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_input" );
		}

		[TestMethod]
		public void PrintTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_print" );
		}

		[TestMethod]
		public void TypeTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_type" );
		}

		[TestMethod]
		public void ExitTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_exit" );
		}

		[TestMethod]
		public void ErrorTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_error" , true );
		}

		[TestMethod]
		public void ReadTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_read" );
		}

		[TestMethod]
		public void WriteTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_write" );
		}

		[TestMethod]
		public void OpenReadTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_open_read" );
		}

		[TestMethod]
		public void OpenWriteTest( )
		{
			TestFileRunner.RunTestFile( "statement_test_open_write" );
		}
	}
}
