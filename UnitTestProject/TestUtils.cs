using System;
using System.Threading.Tasks;
using BNA;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BnaUnitTests
{
	public static class TestUtils
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
			catch ( Exception e )
			{
				Assert.Inconclusive( $"Exception caught while running test: {e.Message}" );
			}

			if ( r == ReturnCode.FILE_ERROR )
			{
				Assert.Inconclusive( "File error: could not run test .bna file." );
			}

			ReturnCode expected = !fails ? ReturnCode.SUCCESS : ReturnCode.BNA_ERROR;
			Assert.AreEqual(  expected, r , $"Return is {r} when {expected} was expected.");
		}
	}
}
