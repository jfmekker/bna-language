using Microsoft.VisualStudio.TestTools.UnitTesting;
using BNA.Run;
using BNA.Common;
using BNA.Values;
using BNA.Exceptions;

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
		[DataRow( "Integer" , TokenType.NUMBER )]
		[DataRow( "Float" , TokenType.NUMBER )]
		[DataRow( "String" , TokenType.STRING )]
		[DataRow( "List" , TokenType.LIST )]
		[DataRow( "Integer" , TokenType.VARIABLE )]
		[DataRow( "Float" , TokenType.VARIABLE )]
		[DataRow( "String" , TokenType.VARIABLE )]
		[DataRow( "List" , TokenType.VARIABLE )]
		public void Instruction_Execute_SetOperation( string val_type , TokenType tok_type )
		{
			Token operand2_token = MockValue.GetTokenOfType( tok_type );
			Value operand2_value = MockValue.GetValueOfType( val_type );
			Token operand1_token = new( "var1" , TokenType.VARIABLE );
			this.Memory.GetValue_TokenValues = new( ) { (operand2_token, operand2_value) };
			Instruction inst = new( Operation.SET , operand1_token , operand2_token , this.Program , this.Memory );

			inst.Execute( );

			Assert.AreEqual( operand2_value , this.Memory.SetValue_TokenValue?.value );
		}

		[TestMethod]
		[DataRow( Operation.ADD , "Add" )]
		[DataRow( Operation.SUBTRACT , "Subtract" )]
		[DataRow( Operation.MULTIPLY , "Multiply" )]
		[DataRow( Operation.DIVIDE , "Divide" )]
		[DataRow( Operation.MODULUS , "Modulus" )]
		[DataRow( Operation.LOGARITHM , "Log" )]
		[DataRow( Operation.POWER , "RaiseTo" )]
		[DataRow( Operation.APPEND , "Append" )]
		[DataRow( Operation.ROUND , "Round" )]
		[DataRow( Operation.SIZE , "Size" )]
		public void Instruction_Execute_OperationCallsValueFunction( Operation operation , string function )
		{
			Variable operand1 = new( new Token( "var1" , TokenType.VARIABLE ) , new MockValue( ) );
			Variable operand2 = new( new Token( "var2" , TokenType.VARIABLE ) , new MockValue( ) );
			this.Memory.GetValue_TokenValues = new( ) { (operand1.Token, operand1.Value) , (operand2.Token, operand2.Value) };
			Instruction inst = new( operation , operand1.Token , operand2.Token , this.Program , this.Memory );

			inst.Execute( );

			Assert.AreEqual( function , ( (MockValue?)this.Memory.SetValue_TokenValue?.value )?.LastCalledFunction );
		}

		[TestMethod]
		[DataRow( Operation.TEST_LESS_THAN , "LessThan" )]
		[DataRow( Operation.TEST_GREATER_THAN , "GreaterThan" )]
		[DataRow( Operation.TEST_EQUAL , "Equals" )]
		[DataRow( Operation.TEST_NOT_EQUAL , "Equals" )]
		public void Instruction_Execute_TestOperation( Operation test_op , string test_method )
		{
			Variable operand1 = new( new Token( "var1" , TokenType.VARIABLE ) , new MockValue( ) );
			Variable operand2 = new( new Token( "var2" , TokenType.VARIABLE ) , new MockValue( ) );
			this.Memory.GetValue_TokenValues = new( ) { (operand1.Token, operand1.Value) , (operand2.Token, operand2.Value) };
			Instruction inst = new( test_op , operand1.Token , operand2.Token , this.Program , this.Memory );

			inst.Execute( );

			Assert.AreEqual( test_method , ( (MockValue)operand1.Value ).LastCalledFunction );
			Assert.AreEqual( new IntegerValue( test_op != Operation.TEST_NOT_EQUAL ? 1 : 0 ) , this.Memory.SetValue_TokenValue?.value );
		}

		[TestMethod]
		public void Instruction_Execute_ErrorOperation( )
		{
			Variable operand2 = new( new Token( "var2" , TokenType.VARIABLE ) , new MockValue( ) );
			this.Memory.GetValue_TokenValues = new( ) { (operand2.Token, operand2.Value) };
			Instruction inst = new( Operation.ERROR , null , operand2.Token , this.Program , this.Memory );

			_ = Assert.ThrowsException<ErrorStatementException>( ( ) => inst.Execute( ) );
		}
	}
}
