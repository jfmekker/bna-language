using System;
using System.Collections.Generic;
using BNA.Common;
using BNA.Compile;
using BNA.Exceptions;
using BNA.Values;

namespace BNA.Run
{
	/// <summary>
	/// Object class representing a BNA program.
	/// </summary>
	public class Program : IProgram
	{
		/// <summary>
		/// The statements that make up what the program does.
		/// </summary>
		private Statement[] Statements { get; set; }

		public Dictionary<Token , int> Labels { get; init; }

		/// <summary>
		/// Shortcut to the statement pointed to by <see cref="IP"/>.
		/// </summary>
		public Statement Current
		{
			get
			{
				return this.IP >= 0 && this.IP < this.Statements.Length
					? this.Statements[this.IP]
					: throw new Exception( "Bad instruction pointer value ( " + this.IP + " )" );
			}
		}

		/// <summary>
		/// Instruction pointer; what statement is the program currently on.
		/// </summary>
		public int IP { get; set; }

		/// <summary>
		/// Whether the program is currently running.
		/// </summary>
		public bool Running
		{
			get; set;
		}

		private Memory Memory { get; set; }

		/// <summary>
		/// Create a new <see cref="Program"/> instance.
		/// </summary>
		/// <param name="statements">Statements that make up the program</param>
		public Program( Statement[] statements )
		{
			this.Memory = new( );
			this.Labels = new( );
			this.Statements = statements; // TODO convert to instructions here
			this.IP = 0;
		}

		/// <summary>
		/// Run a program statement by statement.
		/// </summary>
		public void Run( )
		{
			this.CompileLabels( );

			this.Running = true;
			while ( this.Running )
			{
				try
				{
					Instruction instruction = new(
						this.Current.Type ,
						this.Current.Operand1 ,
						this.Current.Operand2 ,
						this ,
						this.Memory
					);

					instruction.Execute( );
				}
				catch ( Exception e )
				{
					if ( e is UndefinedOperationException
						   or IncorrectOperandTypeException
						   or InvalidIndexValueException
						   or NonIndexableValueException
						   or NonExistantVariableException
						   or ErrorStatementException )
					{
						throw new RuntimeException( this.IP , this.Current , e );
					}
					else
					{
						throw;
					}
				}

				if ( this.Running && ++this.IP >= this.Statements.Length )
				{
					this.Running = false;
				}
			}

			CloseAllFiles( new List<Value>( this.Memory.Variables.Values ) );
		}

		/// <summary>
		/// Close all file-type values in a list
		/// </summary>
		/// <param name="values">Values to check and close</param>
		private static void CloseAllFiles( List<Value> values )
		{
			// TODO need to go through all scopes
			foreach ( Value v in values )
			{
				if ( v is FileValue file )
				{
					file.Close( );
				}
				else if ( v is ListValue list )
				{
					CloseAllFiles( list.Get );
				}
			}
		}

		/// <summary>
		/// Read through all statements and set labels equal to their line number.
		/// </summary>
		private void CompileLabels( )
		{
			for ( int i = 0 ; i < this.Statements.Length ; i += 1 )
			{
				if ( this.Statements[i].Type == Operation.LABEL )
				{
					this.Labels.Add( this.Statements[i].Operand1 , i );
				}
			}
		}
	}
}
