using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BNA;

namespace IntegrationTests
{
	public static class TestFileRunner
	{
		/// <summary>
		/// Run a .bna test file in the Tests folder.
		/// </summary>
		/// <param name="filename">Name of the file to run (minus the extension)</param>
		/// <param name="fails">True if a BNA_ERROR should be expected</param>
		public static void RunTestFile( string filename , bool fails = false )
		{
			ReturnCode r = BNA.BNA.RunFromFiles( new string[] { "../../../../Tests/" + filename + ".bna" } );

			if ( r == ReturnCode.FileError )
			{
				Assert.Inconclusive( "File error: could not run test .bna file." );
			}

			ReturnCode expected = !fails ? ReturnCode.Success : ReturnCode.BnaError;
			Assert.AreEqual(  expected, r , $"Return is {r} when {expected} was expected.");
		}
	}
}
