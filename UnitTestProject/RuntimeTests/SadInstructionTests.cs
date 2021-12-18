using Microsoft.VisualStudio.TestTools.UnitTesting;
using BNA.Common;
using BNA.Exceptions;
using BNA.Run;
using BNA.Values;

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
		[DataRow( Operation.RANDOM , "String" , "List" , "Mock" , DisplayName = "RANDOM" )]
		[DataRow( Operation.LIST , "Float" , "String" , "List" , "Mock" , DisplayName = "LIST" )]
		[DataRow( Operation.INPUT , "Integer" , "Float" , "List" , "Mock" , DisplayName = "INPUT" )]
		[DataRow( Operation.WAIT , "String" , "List" , "Mock" , DisplayName = "WAIT" )]
		public void Instruction_Execute_Operand2ThrowsIncorrectOperandType( Operation operation , params string[] val_types )
		{
			foreach ( string val_type in val_types )
			{
				Variable operand1 = new( new Token( "var1" , TokenType.VARIABLE ) , new MockValue( ) );
				Variable operand2 = new( new Token( "var2" , TokenType.VARIABLE ) , MockValue.GetValueOfType( val_type ) );
				this.Memory.GetValue_TokenValues = new( ) { (operand1.Token, operand1.Value) , (operand2.Token, operand2.Value) };
				Instruction inst = new( operation , operand1.Token , operand2.Token , this.Program , this.Memory );

				_ = Assert.ThrowsException<IncorrectOperandTypeException>( ( ) => inst.Execute( ) );
			}
		}

		[TestMethod]
		[DataRow( Operation.OPEN_READ , "Integer" , "Float" , "List" , "Mock" , DisplayName = "OPEN_READ" )]
		[DataRow( Operation.OPEN_WRITE , "Integer" , "Float" , "List" , "Mock" , DisplayName = "OPEN_WRITE" )]
		[DataRow( Operation.CLOSE , "Integer" , "Float" , "String" , "List" , "Mock" , DisplayName = "CLOSE" )]
		[DataRow( Operation.READ , "Integer" , "Float" , "String" , "List" , "Mock" , DisplayName = "READ" )]
		[DataRow( Operation.WRITE , "Integer" , "Float" , "String" , "List" , "Mock" , DisplayName = "WRITE" )]
		public void Instruction_Execute_Operand1ThrowsIncorrectOperandType( Operation operation , params string[] val_types )
		{
			foreach ( string val_type in val_types )
			{
				Variable operand1 = new( new Token( "var1" , TokenType.VARIABLE ) , MockValue.GetValueOfType( val_type ) );
				Variable operand2 = new( new Token( "var2" , TokenType.VARIABLE ) , new MockValue( ) );
				this.Memory.GetValue_TokenValues = new( ) { (operand1.Token, operand1.Value) , (operand2.Token, operand2.Value) };
				Instruction inst = new( operation , operand1.Token , operand2.Token , this.Program , this.Memory );

				_ = Assert.ThrowsException<IncorrectOperandTypeException>( ( ) => inst.Execute( ) );
			}
		}

		[TestMethod]
		[DataRow( int.MinValue , DisplayName = "Integer min value" )]
		[DataRow( -2 , DisplayName = "Negative 1" )]
		[DataRow( (long)int.MaxValue + 1 , DisplayName = "Integer max value" )]
		public void Instruction_Execute_GotoThrowsValueOutOfRange( long new_ip )
		{
			Variable operand1 = new( new Token( "var1" , TokenType.VARIABLE ) , new IntegerValue( new_ip ) );
			Variable operand2 = new( new Token( "var2" , TokenType.VARIABLE ) , new MockValue( ) );
			this.Memory.GetValue_TokenValues = new( ) { (operand1.Token, operand1.Value) , (operand2.Token, operand2.Value) };
			Instruction inst = new( Operation.GOTO , operand1.Token , operand2.Token , this.Program , this.Memory );

			_ = Assert.ThrowsException<ValueOutOfRangeException>( ( ) => inst.Execute( ) );
		}
	}
}
