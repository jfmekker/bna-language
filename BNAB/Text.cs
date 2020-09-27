using System.Collections.Generic;
using BNAB.Util;

namespace BNAB
{
	/// <summary>
	/// Collection of instructions for a Binary file/object.
	/// </summary>
	public class TextSegment
	{
		/// <summary>
		/// Array of raw data taken from a file.
		/// </summary>
		public ulong[] Raw
		{
			get; private set;
		}

		/// <summary>
		/// List of instructions to add to when building.
		/// </summary>
		private List<Instruction> InstructionList = new List<Instruction>( );

		/// <summary>
		/// Set array of instructions.
		/// </summary>
		private Instruction[] Instructions;

		/// <summary>
		/// Construct a new <see cref="TextSegment"/> instance from raw data.
		/// </summary>
		/// <param name="raw">array of raw data words</param>
		public TextSegment( ulong[] raw )
		{
			Raw = raw;
			Instructions = new Instruction[Raw.Length];

			for ( int i = 0 ; i < Raw.Length ; i += 1 ) {
				Instructions[i] = new Instruction( Raw[i] );
			}
		}

		/// <summary>
		/// Construct a new <see cref="TextSegment"/> instance for adding instructions to.
		/// </summary>
		public TextSegment(  )
		{
		}

		/// <summary>
		/// Add an <see cref="Instruction"/> to the instruction list.
		/// </summary>
		/// <param name="inst">instruction to add</param>
		public void AddInstruction( Instruction inst )
		{
			InstructionList.Add( inst );
		}

		/// <summary>
		/// Fill the instruction array from the instuction list.
		/// </summary>
		public void FillArray( )
		{
			Instructions = InstructionList.ToArray( );
		}
	}

	/// <summary>
	/// Single instruction for a BNA program.
	/// </summary>
	public struct Instruction
	{
		/// <summary>
		/// Raw word representation of the instruction.
		/// </summary>
		private ulong _raw;

		/// <summary>
		/// Code to specify the operation to perform.
		/// </summary>
		public int OpCode
		{
			get
			{
				return (int)BitHelper.GetBits( 0 , 7 , _raw );
			}

			private set
			{
				BitHelper.SetBits( 0 , 7 , ref _raw , (ulong)value );
			}
		}

		/// <summary>
		/// Identifer for the first operand
		/// </summary>
		public int Operand1
		{
			get
			{
				return (int)BitHelper.GetBits( 8 , 23 , _raw );
			}

			private set
			{
				BitHelper.SetBits( 8 , 23 , ref _raw , (ulong)value );
			}
		}

		/// <summary>
		/// Type of the second operand.
		/// </summary>
		public int Op2Type
		{
			get
			{
				return (int)BitHelper.GetBits( 30 , 31 , _raw );
			}

			private set
			{
				BitHelper.SetBits( 30 , 31 , ref _raw , (ulong)value );
			}
		}

		/// <summary>
		/// Identifier or value for the second operand.
		/// </summary>
		public int Operand2
		{
			get
			{
				return (int)BitHelper.GetBits( 32 , 63 , _raw );
			}

			private set
			{
				BitHelper.SetBits( 32 , 63 , ref _raw , (ulong)value );
			}
		}

		/// <summary>
		/// Construct a new <see cref="Instruction"/> instance from all relevant information.
		/// </summary>
		/// <param name="opCode"><see cref="OpCode"/></param>
		/// <param name="op1"><see cref="Operand1"/></param>
		/// <param name="op2type"><see cref="Op2Type"/></param>
		/// <param name="op2"><see cref="Operand2"/></param>
		public Instruction( int opCode , int op1 , int op2type , int op2 )
		{
			_raw = 0;
			OpCode = opCode;
			Operand1 = op1;
			Op2Type = op2type;
			Operand2 = op2;
		}

		/// <summary>
		/// Construct a new <see cref="Instruction"/> instance from a raw word.
		/// </summary>
		/// <param name="raw"></param>
		public Instruction( ulong raw )
		{
			_raw = raw;
		}
	}
}
