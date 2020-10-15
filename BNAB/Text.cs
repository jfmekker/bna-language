using System.Collections.Generic;

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
		/// Construct a new <see cref="TextSegment"/> instance from raw data.
		/// </summary>
		/// <param name="raw">array of raw data words</param>
		public TextSegment( ulong[] raw )
		{
			Raw = raw;
			Instruction[] Instructions = new Instruction[Raw.Length];

			for ( int i = 0 ; i < Raw.Length ; i += 1 ) {
				Instructions[i] = new Instruction( Raw[i] );
			}

			InstructionList = new List<Instruction>( Instructions );
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
		public Instruction[] ToArray( )
		{
			return InstructionList.ToArray( );
		}
	}
}
