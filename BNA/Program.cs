using System;
using System.Collections.Generic;
using BNA.Exceptions;
using BNA.Values;

namespace BNA
{
	/// <summary>
	/// Object class representing a BNA program.
	/// </summary>
	public partial class Program
	{
		/// <summary>
		/// The statements that make up what the program does.
		/// </summary>
		private Statement[] Statements { get; set; }

		/// <summary>
		/// Running list of variables held by the program while running.
		/// Starts with label values and special variable default values.
		/// </summary>
		private Dictionary<Token , Value> Variables
		{
			get => ( this.Scopes is null || this.Scopes.Count <= 0 ) ? throw new Exception( "No scopes found." ) : this.Scopes.Peek( );

			set
			{
				this.Scopes.Push( value );
				this.CompileLabels( );
				this.SetValue( SpecialVariables.TEST_RESULT , SpecialVariables.TEST_RESULT_DEFAULT , true );
				this.SetValue( SpecialVariables.ARGUMENT , SpecialVariables.ARGUMENT_DEFAULT , true );
				this.SetValue( SpecialVariables.RETURN , SpecialVariables.RETURN_DEFAULT , true );
				this.SetValue( SpecialVariables.NULL , SpecialVariables.NULL_DEFAULT , true );
			}
		}

		/// <summary>
		/// Stack of scopes, collections of variables.
		/// </summary>
		private Stack<Dictionary<Token , Value>> Scopes { get; set; }

		/// <summary>
		/// Shortcut to the statement pointed to by <see cref="IP"/>.
		/// </summary>
		public Statement Current
		{
			get
			{
				if ( this.IP < 0 || this.IP >= this.Statements.Length )
				{
					throw new Exception( "Bad instruction pointer value ( " + this.IP + " )" );
				}
				else if ( this.Statements[this.IP] is null )
				{
					throw new Exception( "No statement at instruction pointer ( " + this.IP + " )" );
				}
				return this.Statements[this.IP];
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

		/// <summary>
		/// Create a new <see cref="Program"/> instance.
		/// </summary>
		/// <param name="statements">Statements that make up the program</param>
		public Program( Statement[] statements )
		{
			this.Statements = statements;
			this.IP = 0;

			this.Scopes = new Stack<Dictionary<Token , Value>>( );
			this.Variables = new Dictionary<Token , Value>( );
		}

		/// <summary>
		/// Run a program statement by statement.
		/// </summary>
		public void Run( )
		{
			this.Running = true;
			while ( this.Running )
			{
				try
				{
					Instruction instruction = new( this.Current , this );

					instruction.Execute( );
					//this.ExecuteCurrentStatement( );
				}
				catch ( RuntimeException e )
				{
					throw new RuntimeException( e , this.IP , this.Current );
				}

				if ( this.Running && ++this.IP >= this.Statements.Length )
				{
					this.Running = false;
				}
			}

			CloseAllFiles( new List<Value>( this.Variables.Values ) );
		}

		/// <summary>
		/// Get the value of an operand based on a Token.
		/// </summary>
		/// <param name="token">Token to identify/convert to a value</param>
		/// <returns>Value of the operand</returns>
		public Value GetValue( Token token )
		{
			switch ( token.Type )
			{
				// Parse the literal value from the token
				case TokenType.LITERAL:
				{
					if ( long.TryParse( token.Value , out long lval ) )
					{
						return new IntegerValue( lval );
					}
					else if ( double.TryParse( token.Value , out double dval ) )
					{
						return new FloatValue( dval );
					}

					// Should have caught this in compiler?
					throw new Exception( $"Could not parse value from literal: {token}" );
					// throw new RuntimeException( this.IP , this.Statements[this.IP] , $"Could not parse value from literal: {token}" );
				}

				// Capture the string contents
				case TokenType.STRING:
				{
					return token.Value.Length >= 2 ? new StringValue( token.Value[1..^1] ) : throw new RuntimeException( this.IP , this.Statements[this.IP] , $"String token too short to be valid: {token}" );
				}

				// Get the Value of a variable
				case TokenType.VARIABLE:
				{
					// Get list element
					if ( token.Value.Contains( "" + (char)Symbol.ACCESSOR ) )
					{
						// Get last accessor first (so multi-lists are properly chained)
						int accessor = token.Value.LastIndexOf( (char)Symbol.ACCESSOR );
						if ( accessor == 0 || accessor == token.Value.Length )
						{
							throw new RuntimeException( this.IP , this.Statements[this.IP] , "Accessor at start or end of token: " + token );
						}

						// Get value of index part
						string indxPart = token.Value[( accessor + 1 )..];
						if ( this.GetValue( new Token( indxPart ) ) is not IntegerValue index )
						{
							throw new RuntimeException( this.IP , this.Statements[this.IP] , "Index is not an integer: " + token );
						}
						int i = index.Get;

						// Get the value
						string listPart = token.Value.Substring( 0 , accessor );
						Value accessedVal = this.GetValue( new Token( listPart ) );
						if ( accessedVal is ListValue listVal )
						{
							List<Value> list = listVal.Get;
							return i >= 0 && i < list.Count ? list[i]
								: throw new RuntimeException( this.IP , this.Statements[this.IP] , "Invalid index (" + i + ") for list of size " + list.Count );
						}
						else if ( accessedVal is StringValue strVal )
						{
							return new StringValue( "" + strVal.Get[i] );
						}
						else
						{
							throw new RuntimeException( this.IP , this.Statements[this.IP] , "Accessed variable not a list or string: " + token );
						}
					}

					// Find variable
					if ( this.Variables.TryGetValue( token , out Value? v ) )
					{
						return v;
					}

					// No value yet
					return Value.NULL;
				}

				// Tokenize and evaluate the contents of a list literal
				case TokenType.LIST:
				{
					List<Token> listTokens = Token.TokenizeLine( token.Value[1..^1] );
					List<Value> listValues = new( );

					foreach ( Token t in listTokens )
					{
						if ( t.Type != TokenType.SYMBOL || t.Value[0] != (char)Symbol.LIST_SEPERATOR )
						{
							listValues.Add( this.GetValue( t ) );
						}
					}

					return new ListValue( listValues );
				}

				// Null operands for single operand statements
				case TokenType.NULL:
				{
					return Value.NULL;
				}

				// Invalid token
				case TokenType.INVALID:
				case TokenType.UNKNOWN:
				{
					throw new RuntimeException( "Can not get value from invalid or unknown token: " + token.ToString( ) );
				}

				default:
					throw new RuntimeException( "Unexpected token type for operand: " + token.ToString( ) );
			}
		}

		/// <summary>
		/// Set the value of a variable based on a token identifier.
		/// </summary>
		/// <param name="token">Token to locate the value to change</param>
		/// <param name="newValue">New value to insert or change</param>
		/// <param name="add">Whether to add the variable if it does not exist</param>
		public void SetValue( Token token , Value newValue , bool add = false )
		{
			// Unexpected token type
			if ( token.Type != TokenType.VARIABLE )
			{
				throw new Exception( "Unexpected token type in SetValue: " + token );
			}

			// Handle list element
			if ( token.Value.Contains( "" + (char)Symbol.ACCESSOR ) )
			{
				// Get last accessor first (so multi-lists are properly chained)
				int accessor = token.Value.LastIndexOf( (char)Symbol.ACCESSOR );
				if ( accessor == 0 || accessor == token.Value.Length )
				{
					throw new RuntimeException( this.IP , this.Statements[this.IP] , $"Accessor at start or end of token: {token}" );
				}

				// Get value of index part
				string indxPart = token.Value[( accessor + 1 )..];
				if ( this.GetValue( new Token( indxPart ) ) is not IntegerValue index )
				{
					throw new RuntimeException( this.IP , this.Statements[this.IP] , $"Index is not an integer: {token}" );
				}

				// Set the value
				string listPart = token.Value.Substring( 0 , accessor );
				Value accessedVal = this.GetValue( new Token( listPart ) );
				if ( accessedVal is ListValue listVal )
				{
					List<Value> list = listVal.Get;

					if ( index.Get < 0 || index.Get >= list.Count )
					{
						throw new RuntimeException( this.IP , this.Statements[this.IP] , $"Invalid index for list: {index}" );
					}

					list[index.Get] = newValue;
					return;
				}
				else if ( accessedVal is StringValue )
				{
					throw new RuntimeException( this.IP , this.Statements[this.IP] , "Can not set specific index of string." );
				}
				else
				{
					throw new RuntimeException( this.IP , this.Statements[this.IP] , $"Accessed variable not a list or string: {token}" );
				}
			}

			// Set or add value
			if ( this.Variables.ContainsKey( token ) )
			{
				this.Variables[token] = newValue;
			}
			else if ( add )
			{
				this.Variables.Add( token , newValue );
			}
			else
			{
				throw new RuntimeException( this.IP , this.Statements[this.IP] , "Could not find variable to set." );
			}
		}

		public void OpenScope( )
		{
			Value argument_val = this.GetValue( SpecialVariables.ARGUMENT );
			this.Variables = new Dictionary<Token , Value>( );
			this.SetValue( SpecialVariables.ARGUMENT , argument_val );
		}

		public void CloseScope( )
		{
			Value return_val = this.GetValue( SpecialVariables.RETURN );
			if ( this.Scopes.Count < 2 )
			{
				throw new RuntimeException( "Cannot close final scope." );
			}
			_ = this.Scopes.Pop( );
			this.SetValue( SpecialVariables.RETURN , return_val );
		}

		/// <summary>
		/// Close all file-type values in a list
		/// </summary>
		/// <param name="values">Values to check and close</param>
		private static void CloseAllFiles( List<Value> values )
		{
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
				if ( this.Statements[i].Type == StatementType.LABEL )
				{
					this.SetValue( this.Statements[i].Operand1 , new IntegerValue( i ) , true );
				}
			}
		}
	}
}
