using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BNA.Run;
using BNA.Common;
using BNA.Values;

namespace RuntimeTests
{
	[TestClass]
	public class HappyInstructionTests
	{
		public MockProgram Program;

		public MockMemory Memory;

		public HappyInstructionTests( )
		{
			this.Program = new MockProgram( );
			this.Memory = new MockMemory( );
		}

		[TestMethod]
		public void Instruction_Execute_SetOperation_SetToVariable( )
		{
			foreach ( Value value in new Value[]
				{
					new IntegerValue(0),
					new FloatValue(0),
					new StringValue(""),
					new ListValue(0),
				} )
			{
				Token operand1 = new( "var1" , TokenType.VARIABLE );
				Token operand2 = new( "var2" , TokenType.VARIABLE );
				this.Memory.GetValue_TokenValues = new( ) { (operand2, value) };
				Instruction inst = new( Operation.SET , operand1 , operand2 , this.Program , this.Memory );

				inst.Execute( );

				Assert.AreEqual( value , this.Memory.SetValue_TokenValue?.value );
			}
		}

		[TestMethod]
		public void Instruction_Execute_SetOperation_SetToLiteralListOrString( )
		{
			foreach ( (Token operand2, Value value) in new Variable[]
				{
					new( new Token( "0" , TokenType.NUMBER ) , new IntegerValue( 0 ) ),
					new( new Token( "0.1" , TokenType.NUMBER ) , new FloatValue( 0 ) ),
					new( new Token( "\"\"" , TokenType.STRING ) , new StringValue( "" ) ),
					new( new Token( "()" , TokenType.LIST ) , new ListValue( 0 ) ),
				} )
			{
				Token operand1 = new( "var1" , TokenType.VARIABLE );
				this.Memory.GetValue_TokenValues = new( ) { (operand2, value) };
				Instruction inst = new( Operation.SET , operand1 , operand2 , this.Program , this.Memory );

				inst.Execute( );

				Assert.AreEqual( value , this.Memory.SetValue_TokenValue?.value );
			}
		}
	}
}
