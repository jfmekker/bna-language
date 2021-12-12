using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BNA.Run;
using BNA.Compile;
using BNA.Common;

namespace UnitTestProject.RuntimeTests
{
	[TestClass]
	public class HappyInstructionTests
	{
		public MockProgram Program;

		public HappyInstructionTests( )
		{
			this.Program = new MockProgram( );
		}

		// public void Instruction_Execute_SetOperation( )
		// {
		// 	Token operand1 = new Token( "var1" , TokenType.VARIABLE );
		// 	Statement statement = new( string.Empty, Operation.SET, operand1 , new Token( "var2" , TokenType.VARIABLE ) );
		// 	Instruction inst = new( statement , this.Program );
		// 
		// 	inst.Execute( );
		// 
		// 	// TODO assert something
		// }
	}
}
