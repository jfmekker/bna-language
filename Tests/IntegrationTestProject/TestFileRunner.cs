using BNA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace IntegrationTestProject
{
    public static class TestFileRunner
    {
        /// <summary>
        /// Run a .bna test file in the Tests folder.
        /// </summary>
        /// <param name="filename">Name of the file to run (minus the extension)</param>
        /// <param name="fails">True if a BNA_ERROR should be expected</param>
        public static void RunTestFile( string filename, bool fails = false )
        {
            ReturnCode r;

            try
            {
                // TODO check file exists
                r = BNA.BNA.RunFromFiles( ["TestFiles/" + filename + ".bna"] );
            }
            catch ( Exception e )
            {
                Assert.Inconclusive( $"Exception caught while running test: {e}" );
                throw;
            }

            if ( r == ReturnCode.FILE_ERROR )
            {
                Assert.Inconclusive( "File error: could not run test .bna file." );
            }

            ReturnCode expected = !fails ? ReturnCode.SUCCESS : ReturnCode.BNA_ERROR;
            Assert.AreEqual( expected, r, $"Return is {r} when {expected} was expected." );
        }
    }
}
