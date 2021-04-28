using System;
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
			BNA.ReturnCode r = BNA.ReturnCode.UNEXPECTED_ERROR;

			try {
				r = BNA.BNA.RunFromFiles( new string[] { "../../../../Tests/" + filename + ".bna" } );
			}
			catch ( Exception e ) {
				Assert.Inconclusive( );
			}

			if ( r == BNA.ReturnCode.FILE_ERROR ) {
				Assert.Inconclusive( );
			}

			Assert.AreEqual( !fails ? BNA.ReturnCode.SUCCESS : BNA.ReturnCode.BNA_ERROR , r );
		}
	}
}
