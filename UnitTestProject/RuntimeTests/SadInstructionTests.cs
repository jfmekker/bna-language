using Microsoft.VisualStudio.TestTools.UnitTesting;
using BNA.Common;
using BNA.Exceptions;
using BNA.Run;

namespace RuntimeTests
{
	[TestClass]
	public class SadInstructionTests
	{
		public MockProgram Program;

		public MockMemory Memory;

		public SadInstructionTests( )
		{
			this.Program = new MockProgram( );
			this.Memory = new MockMemory( );
		}

		[TestMethod]
		[DataRow( "String" )]
		[DataRow( "List" )]
		[DataRow( "Mock" )]
		public void Instruction_Execute_RandomThrowsIncorrectOperandType( string val_type )
		{
			Variable operand1 = new( new Token( "var1" , TokenType.VARIABLE ) , new MockValue( ) );
			Variable operand2 = new( new Token( "var2" , TokenType.VARIABLE ) , MockValue.GetValueOfType( val_type ) );
			this.Memory.GetValue_TokenValues = new( ) { (operand1.Token, operand1.Value) , (operand2.Token, operand2.Value) };
			Instruction inst = new( Operation.RANDOM , operand1.Token , operand2.Token , this.Program , this.Memory );

			_ = Assert.ThrowsException<IncorrectOperandTypeException>( ( ) => inst.Execute( ) );
		}

		[TestMethod]
		[DataRow( "Float" )]
		[DataRow( "String" )]
		[DataRow( "List" )]
		[DataRow( "Mock" )]
		public void Instruction_Execute_ListThrowsIncorrectOperandType( string val_type )
		{
			Variable operand1 = new( new Token( "var1" , TokenType.VARIABLE ) , new MockValue( ) );
			Variable operand2 = new( new Token( "var2" , TokenType.VARIABLE ) , MockValue.GetValueOfType( val_type ) );
			this.Memory.GetValue_TokenValues = new( ) { (operand1.Token, operand1.Value) , (operand2.Token, operand2.Value) };
			Instruction inst = new( Operation.LIST , operand1.Token , operand2.Token , this.Program , this.Memory );

			_ = Assert.ThrowsException<IncorrectOperandTypeException>( ( ) => inst.Execute( ) );
		}
	}
}
