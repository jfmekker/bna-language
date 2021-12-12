using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BNA.Common;
using BNA.Run;
using BNA.Values;

namespace UnitTestProject.RuntimeTests
{
	public class MockProgram : IProgram
	{
		public int IP { get => throw new NotImplementedException( ); set => throw new NotImplementedException( ); }

		public bool Running { get => throw new NotImplementedException( ); set => throw new NotImplementedException( ); }

		public void CloseScope( ) => throw new NotImplementedException( );

		public Value GetValue( Token token ) => throw new NotImplementedException( );

		public void OpenScope( ) => throw new NotImplementedException( );

		public void SetValue( Token token , Value newValue , bool add = false ) => throw new NotImplementedException( );
	}
}
