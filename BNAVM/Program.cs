using System;
using System.IO;
using BNAB;

namespace BNAVM
{
	class Program
	{
		public static OperandHandler OperandHandler
		{
			get; private set;
		}

		public static InstructionExecutor InstructionExecutor
		{
			get; private set;
		}

		static void Main( string[] args )
		{
			// Intro banner
			Console.WriteLine( "================================================================================" );
			Console.WriteLine( "Welcome to the BNA's Not Assembly Virtual Machine!" );
			Console.WriteLine( "================================================================================" );

			// Get the file name
			string filename;
			if ( args.Length == 0 ) {
				Console.Write( "filename: " );
				filename = Console.ReadLine( );
			}
			else {
				filename = args[0];
			}

			// Read file
			var file = File.Open( filename , FileMode.Open , FileAccess.Read );
			var reader = new BinaryReader( file );

			// Create Text and Data segments in Binary object
			Console.WriteLine( "Parsing binary file..." );
			var binary = new Binary( );
			bool parsed = binary.Init( reader );
			file.Close( );
			if ( !parsed ) // TODO replace with try catch?
				return;

			int size = Binary.HEADER_SIZE_WORDS + binary.DataLength + binary.TextLength;
			Console.WriteLine( "File parsed (" + (size * Binary.WORD_SIZE_BYTES) + " bytes):\n" );
			Console.WriteLine( binary.ToString( ) );

			// Create Handler and Executor
			//OperandHandler = new OperandHandler( binary.Data );
			//InstructionExecutor = new InstructionExecutor( binary.Text );

			// Execute
			//Console.WriteLine( "Running program..." );
			//int return_code = InstructionExecutor.ExecuteProgram( );
			//Console.WriteLine( "Program completed with return=" + return_code );

			Console.Write( "hit enter to exit..." );
			Console.ReadLine( );
		}
	}
}
