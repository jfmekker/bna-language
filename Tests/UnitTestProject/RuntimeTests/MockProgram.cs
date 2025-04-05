using BNA.Common;
using BNA.Run;
using System.Collections.Generic;

namespace RuntimeTests
{
    public class MockProgram : IProgram
    {
        public int IP { get; set; }

        public bool Running { get; set; }

        public Dictionary<Token, int> Labels { get; init; }

        public MockProgram( )
        {
            this.Labels = new( );
        }
    }
}
