using System;
using BNA.Common;
using BNA.Exceptions;
using BNA.Run;
using BNA.Values;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject.RuntimeTests
{
    [TestClass]
    public class SadInstructionTests
    {
        public MockProgram Program { get; }

        public MockMemory Memory { get; }

        public SadInstructionTests( )
        {
            this.Program = new MockProgram( );
            this.Memory = new MockMemory( );
        }

        [TestMethod]
        [DataRow( Operation.Random, "String", "List", "Mock", DisplayName = "RANDOM" )]
        [DataRow( Operation.List, "Float", "String", "List", "Mock", DisplayName = "LIST" )]
        [DataRow( Operation.Input, "Integer", "Float", "List", "Mock", DisplayName = "INPUT" )]
        [DataRow( Operation.Wait, "String", "List", "Mock", DisplayName = "WAIT" )]
        public void Instruction_Execute_Operand2ThrowsIncorrectOperandType( Operation operation, params string[] val_types )
        {
            ArgumentNullException.ThrowIfNull( val_types );
            foreach ( string val_type in val_types )
            {
                Variable operand1 = new( new Token( "var1", TokenType.Variable ), new MockValue( ) );
                Variable operand2 = new( new Token( "var2", TokenType.Variable ), MockValue.GetValueOfType( val_type ) );
                this.Memory.GetValue_TokenValues = [(operand1.Token, operand1.Value), (operand2.Token, operand2.Value)];
                Instruction inst = new( operation, operand1.Token, operand2.Token, this.Program, this.Memory );

                _ = Assert.ThrowsException<IncorrectOperandTypeException>( ( ) => inst.Execute( ) );
            }
        }

        [TestMethod]
        [DataRow( Operation.OpenRead, "Integer", "Float", "List", "Mock", DisplayName = "OPEN_READ" )]
        [DataRow( Operation.OpenWrite, "Integer", "Float", "List", "Mock", DisplayName = "OPEN_WRITE" )]
        [DataRow( Operation.Close, "Integer", "Float", "String", "List", "Mock", DisplayName = "CLOSE" )]
        [DataRow( Operation.Read, "Integer", "Float", "String", "List", "Mock", DisplayName = "READ" )]
        [DataRow( Operation.Write, "Integer", "Float", "String", "List", "Mock", DisplayName = "WRITE" )]
        public void Instruction_Execute_Operand1ThrowsIncorrectOperandType( Operation operation, params string[] val_types )
        {
            ArgumentNullException.ThrowIfNull( val_types );
            foreach ( string val_type in val_types )
            {
                Variable operand1 = new( new Token( "var1", TokenType.Variable ), MockValue.GetValueOfType( val_type ) );
                Variable operand2 = new( new Token( "var2", TokenType.Variable ), new MockValue( ) );
                this.Memory.GetValue_TokenValues = [(operand1.Token, operand1.Value), (operand2.Token, operand2.Value)];
                Instruction inst = new( operation, operand1.Token, operand2.Token, this.Program, this.Memory );

                _ = Assert.ThrowsException<IncorrectOperandTypeException>( ( ) => inst.Execute( ) );
            }
        }

        [TestMethod]
        [DataRow( int.MinValue, DisplayName = "Integer min value" )]
        [DataRow( -2, DisplayName = "Negative 1" )]
        [DataRow( (long)int.MaxValue + 1, DisplayName = "Integer max value" )]
        public void Instruction_Execute_GotoThrowsValueOutOfRange( long new_ip )
        {
            Variable operand1 = new( new Token( "var1", TokenType.Variable ), new IntegerValue( new_ip ) );
            Variable operand2 = new( new Token( "var2", TokenType.Variable ), new MockValue( ) );
            this.Memory.GetValue_TokenValues = [(operand1.Token, operand1.Value), (operand2.Token, operand2.Value)];
            Instruction inst = new( Operation.Goto, operand1.Token, operand2.Token, this.Program, this.Memory );

            _ = Assert.ThrowsException<ValueOutOfRangeException>( ( ) => inst.Execute( ) );
        }
    }
}
