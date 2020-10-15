using BNAB.Util;

namespace BNAB
{
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
		public OpCode OpCode
		{
			get
			{
				return (OpCode)BitHelper.GetBits( 0 , 7 , _raw );
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
		public Instruction( OpCode opCode , int op1 , int op2type , int op2 )
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
