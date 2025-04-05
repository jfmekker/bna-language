using System.Collections.Generic;
using BNA.Common;
using BNA.Run;
using BNA.Values;

namespace RuntimeTests
{
	public class MockMemory : IMemory
	{
		public bool OpenScopeCalled { get; private set; }

		public void OpenScope( ) => this.OpenScopeCalled = true;

		public bool CloseScopeCalled { get; private set; }

		public void CloseScope( ) { }

		public List<(Token token, Value value)> GetValue_TokenValues { get; set; } = new( );

		public Value GetValue( Token token )
		{
			foreach ((Token gv_token, Value gv_value) in this.GetValue_TokenValues )
			{
				if (token == gv_token)
				{
					return gv_value;
				}
			}

			return Value.NULL;
		}

		public (Token token, Value value)? SetValue_TokenValue { get; private set; }

		public void SetValue( Token token , Value newValue , bool add = false )
		{
			this.SetValue_TokenValue = (token, newValue);
		}
	}
}
