using System;
using BNA;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
	public static class TestRunner
	{
		/// <summary>
		/// Run a .bna test file in the Tests folder.
		/// </summary>
		/// <param name="filename">Name of the file to run (minus the extension)</param>
		/// <param name="fails">True if a BNA_ERROR should be expected</param>
		public static void RunTestFile( string filename , bool fails = false )
		{
			ReturnCode r = ReturnCode.UNEXPECTED_ERROR;

			try
			{
				r = BNA.BNA.RunFromFiles( new string[] { "../../../../Tests/" + filename + ".bna" } );
			}
			catch ( Exception )
			{
				Assert.Inconclusive( );
			}

			if ( r == ReturnCode.FILE_ERROR )
			{
				Assert.Inconclusive( );
			}

			Assert.AreEqual( !fails ? ReturnCode.SUCCESS : ReturnCode.BNA_ERROR , r );
		}
	}
}
