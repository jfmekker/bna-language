using BNA.Common;
using BNA.Exceptions;
using BNA.Run;
using BNA.Values;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject.RuntimeTests
{
    [TestClass]
    public class HappyInstructionTests
    {
        public MockProgram Program { get; }

        public MockMemory Memory { get; }

        public HappyInstructionTests( )
        {
            this.Program = new MockProgram( );
            this.Memory = new MockMemory( );
        }

        [TestMethod]
        [DataRow( "Integer", TokenType.LiteralNumber, DisplayName = "Integer literal" )]
        [DataRow( "Float", TokenType.LiteralNumber, DisplayName = "Float literal" )]
        [DataRow( "String", TokenType.LiteralString, DisplayName = "String literal" )]
        [DataRow( "List", TokenType.List, DisplayName = "List literal" )]
        [DataRow( "Integer", TokenType.Variable, DisplayName = "Integer variable" )]
        [DataRow( "Float", TokenType.Variable, DisplayName = "Float variable" )]
        [DataRow( "String", TokenType.Variable, DisplayName = "String variable" )]
        [DataRow( "List", TokenType.Variable, DisplayName = "List variable" )]
        public void Instruction_Execute_SetOperation( string val_type, TokenType tok_type )
        {
            Token operand2_token = MockValue.GetTokenOfType( tok_type );
            Value operand2_value = MockValue.GetValueOfType( val_type );
            Token operand1_token = new( "var1", TokenType.Variable );
            this.Memory.GetValue_TokenValues = [(operand2_token, operand2_value)];
            Instruction inst = new( Operation.Set, operand1_token, operand2_token, this.Program, this.Memory );

            inst.Execute( );

            Assert.AreEqual( operand2_value, this.Memory.SetValue_TokenValue?.value );
        }

        [TestMethod]
        [DataRow( Operation.Add, "Add", DisplayName = "Add" )]
        [DataRow( Operation.Subtract, "Subtract", DisplayName = "Subtract" )]
        [DataRow( Operation.Multiply, "Multiply", DisplayName = "Multiply" )]
        [DataRow( Operation.Divide, "Divide", DisplayName = "Divide" )]
        [DataRow( Operation.Modulus, "Modulus", DisplayName = "Modulus" )]
        [DataRow( Operation.Logarithm, "Log", DisplayName = "Log" )]
        [DataRow( Operation.Power, "RaiseTo", DisplayName = "RaiseTo" )]
        [DataRow( Operation.Append, "Append", DisplayName = "Append" )]
        [DataRow( Operation.Round, "Round", DisplayName = "Round" )]
        [DataRow( Operation.Size, "Size", DisplayName = "Size" )]
        public void Instruction_Execute_OperationCallsValueFunction( Operation operation, string function )
        {
            Variable operand1 = new( new Token( "var1", TokenType.Variable ), new MockValue( ) );
            Variable operand2 = new( new Token( "var2", TokenType.Variable ), new MockValue( ) );
            this.Memory.GetValue_TokenValues = [(operand1.Token, operand1.Value), (operand2.Token, operand2.Value)];
            Instruction inst = new( operation, operand1.Token, operand2.Token, this.Program, this.Memory );

            inst.Execute( );

            Assert.AreEqual( function, ((MockValue?)this.Memory.SetValue_TokenValue?.value)?.LastCalledFunction );
        }

        [TestMethod]
        [DataRow( Operation.TestLessThan, "LessThan" )]
        [DataRow( Operation.TestGreaterThan, "GreaterThan" )]
        [DataRow( Operation.TestEqualTo, "Equals" )]
        [DataRow( Operation.TestNotEqualTo, "Equals" )]
        public void Instruction_Execute_TestOperation( Operation test_op, string test_method )
        {
            Variable operand1 = new( new Token( "var1", TokenType.Variable ), new MockValue( ) );
            Variable operand2 = new( new Token( "var2", TokenType.Variable ), new MockValue( ) );
            this.Memory.GetValue_TokenValues = [(operand1.Token, operand1.Value), (operand2.Token, operand2.Value)];
            Instruction inst = new( test_op, operand1.Token, operand2.Token, this.Program, this.Memory );

            inst.Execute( );

            Assert.AreEqual( test_method, ((MockValue)operand1.Value).LastCalledFunction );
            Assert.AreEqual( new IntegerValue( test_op != Operation.TestNotEqualTo ? 1 : 0 ), this.Memory.SetValue_TokenValue?.value );
        }

        [TestMethod]
        public void Instruction_Execute_ErrorOperation( )
        {
            Variable operand2 = new( new Token( "var2", TokenType.Variable ), new MockValue( ) );
            this.Memory.GetValue_TokenValues = [(operand2.Token, operand2.Value)];
            Instruction inst = new( Operation.Error, null, operand2.Token, this.Program, this.Memory );

            _ = Assert.ThrowsException<ErrorStatementException>( ( ) => inst.Execute( ) );
        }
    }
}
