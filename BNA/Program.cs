using System;
using System.Collections.Generic;
using System.IO;

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
		private Statement[] Statements;

		/// <summary>
		/// Instruction pointer; what statement is the program currently on.
		/// </summary>
		private int IP;

		/// <summary>
		/// Running list of variables held by the program while running.
		/// Starts with label values and special variable default values.
		/// </summary>
		private Dictionary<Token , Value> Variables
		{
			get
			{
				if ( this.Scopes == null || this.Scopes.Count <= 0 ) {
					throw new Exception( "No scopes found." );
				}
				return this.Scopes.Peek( );
			}

			set
			{
				this.Scopes.Push( value );
				this.CompileLabels( );
				this.SetValue( SpecialVariables.TEST_RESULT , SpecialVariables.TEST_RESULT_DEFAULT , true );
				this.SetValue( SpecialVariables.ARGUMENT , SpecialVariables.ARGUMENT_DEFAULT , true );
				this.SetValue( SpecialVariables.RETURN , SpecialVariables.RETURN_DEFAULT , true );
			}
		}

		/// <summary>
		/// Stack of scopes, collections of variables.
		/// </summary>
		private Stack<Dictionary<Token , Value>> Scopes;

		/// <summary>
		/// Shortcut to the statement pointed to by <see cref="IP"/>.
		/// </summary>
		private Statement Current
		{
			get
			{
				if ( this.IP < 0 || this.IP >= this.Statements.Length ) {
					throw new Exception( "Bad instruction pointer value ( " + this.IP + " )" );
				}
				if ( this.Statements[this.IP] == null ) {
					throw new Exception( "No statement at instruction pointer ( " + this.IP + " )" );
				}
				return this.Statements[this.IP];
			}
		}

		/// <summary>
		/// Whether the program is currently running.
		/// </summary>
		public bool Running
		{
			get; private set;
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
			if ( this.Statements == null ) {
				throw new Exception( "Program told to run but Statements is null" );
			}

			this.Running = true;
			while ( this.Running ) {
				try {
					this.ExecuteCurrentStatement( );
				}
				catch ( RuntimeException e ) {
					throw new RuntimeException( e , this.IP , this.Current );
				}

				if ( this.Running && ++this.IP == this.Statements.Length ) {
					this.Running = false;
				}
			}

			var values = new List<Value>( this.Variables.Values );
			this.CloseAllFiles( values );
		}

		/// <summary>
		/// Get the value of an operand based on a Token.
		/// </summary>
		/// <param name="token">Token to identify/convert to a value</param>
		/// <returns>Value of the operand</returns>
		private Value GetValue( Token token )
		{
			switch ( token.Type ) {
				// Parse the literal value from the token
				case TokenType.LITERAL: {
					if ( long.TryParse( token.Value , out long lval ) ) {
						return new Value( ValueType.INTEGER , lval );
					}
					else if ( double.TryParse( token.Value , out double dval ) ) {
						return new Value( ValueType.FLOAT , dval );
					}
					else {
						throw new RuntimeException( this.IP , this.Statements[this.IP] , "Could not parse value from literal: " + token );
					}
				}

				// Capture the string contents
				case TokenType.STRING: {
					if ( token.Value.Length < 2 ) {
						throw new RuntimeException( this.IP , this.Statements[this.IP] , "String token too short to be valid: " + token );
					}

					return new Value( ValueType.STRING , token.Value.Substring( 1 , token.Value.Length - 2 ) );
				}

				// Get the Value of a variable
				case TokenType.VARIABLE: {
					// Get list element
					if ( token.Value.Contains( "" + (char)Symbol.ACCESSOR ) ) {
						// Get last accessor first (so multi-lists are properly chained)
						int accessor = token.Value.LastIndexOf( (char)Symbol.ACCESSOR );
						if ( accessor == 0 || accessor == token.Value.Length ) {
							throw new RuntimeException( this.IP , this.Statements[this.IP] , "Accessor at start or end of token: " + token );
						}

						// Get value of list part
						string listPart = token.Value.Substring( 0 , accessor );
						Value list = this.GetValue( new Token( listPart ) );
						if ( list.Type != ValueType.LIST && list.Type != ValueType.STRING ) {
							throw new RuntimeException( this.IP , this.Statements[this.IP] , "Accessed variable not a list or string: " + token );
						}

						// Get value of index part
						string indxPart = token.Value.Substring( accessor + 1 );
						Value index = this.GetValue( new Token( indxPart ) );
						if ( index.Type != ValueType.INTEGER ) {
							throw new RuntimeException( this.IP , this.Statements[this.IP] , "Index is not an integer: " + token );
						}
						int i = (int)(long)index.Val;

						// Get the value
						if ( list.Type == ValueType.LIST ) {
							var l = (List<Value>)list.Val;

							if ( i < 0 || i >= l.Count ) {
								throw new RuntimeException( this.IP , this.Statements[this.IP] , "Invalid index (" + i + ") for list of size " + l.Count );
							}

							return l[i];
						}
						else {
							string s = (string)list.Val;
							return new Value( ValueType.STRING , "" + s[i] );
						}
					}

					// Find variable
					if ( this.Variables.TryGetValue( token , out Value v ) ) {
						return v;
					}

					// No value yet
					return Value.NULL;
				}

				// Tokenize and evaluate the contents of a list literal
				case TokenType.LIST: {
					List<Token> listTokens = Token.TokenizeLine( token.Value.Substring( 1 , token.Value.Length - 2 ) );
					var listValues = new List<Value>( );

					foreach ( Token t in listTokens ) {
						if ( t.Type != TokenType.SYMBOL || t.Value[0] != (char)Symbol.LIST_SEPERATOR ) {
							listValues.Add( this.GetValue( t ) );
						}
					}

					return new Value( ValueType.LIST , listValues );
				}

				// Null operands for single operand statements
				case TokenType.NULL: {
					return Value.NULL;
				}

				// Invalid token
				case TokenType.INVALID:
				case TokenType.UNKNOWN: {
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
		private void SetValue( Token token , Value newValue , bool add = false )
		{
			// Unexpected token type
			if ( token.Type != TokenType.VARIABLE ) {
				throw new Exception( "Unexpected token type in SetValue: " + token );
			}

			// Handle list element
			if ( token.Value.Contains( "" + (char)Symbol.ACCESSOR ) ) {
				// Get last accessor first (so multi-lists are properly chained)
				int accessor = token.Value.LastIndexOf( (char)Symbol.ACCESSOR );
				if ( accessor == 0 || accessor == token.Value.Length ) {
					throw new RuntimeException( this.IP , this.Statements[this.IP] , "Accessor at start or end of token: " + token );
				}

				// Get value of list part
				string listPart = token.Value.Substring( 0 , accessor );
				Value list = this.GetValue( new Token( listPart ) );
				if ( list.Type != ValueType.LIST && list.Type != ValueType.STRING ) {
					throw new RuntimeException( this.IP , this.Statements[this.IP] , "Accessed variable not a list or string: " + token );
				}

				// Get value of index part
				string indxPart = token.Value.Substring( accessor + 1 );
				Value index = this.GetValue( new Token( indxPart ) );
				if ( index.Type != ValueType.INTEGER ) {
					throw new RuntimeException( this.IP , this.Statements[this.IP] , "Index is not an integer: " + token );
				}
				int i = (int)(long)index.Val;

				// Set the value
				if ( list.Type == ValueType.LIST ) {
					var l = (List<Value>)list.Val;

					if ( i < 0 || i >= l.Count ) {
						throw new RuntimeException( this.IP , this.Statements[this.IP] , "Invalid index for list: " + i );
					}

					l[i] = newValue;
					return;
				}
				else {
					throw new RuntimeException( this.IP , this.Statements[this.IP] , "Can not set specific index of string." );
				}
			}

			// Set or add value
			if ( this.Variables.ContainsKey( token ) ) {
				this.Variables[token] = newValue;
			}
			else if ( add ) {
				this.Variables.Add( token , newValue );
			}
			else {
				throw new RuntimeException( this.IP , this.Statements[this.IP] , "Could not find variable to set." );
			}
		}

		/// <summary>
		/// Close all file-type values in a list
		/// </summary>
		/// <param name="values">Values to check and close</param>
		private void CloseAllFiles( List<Value> values )
		{
			foreach ( Value v in values ) {
				if ( v.Type == ValueType.READ_FILE ) {
					( (StreamReader)v.Val ).Close( );
				}
				else if ( v.Type == ValueType.WRITE_FILE ) {
					( (StreamWriter)v.Val ).Close( );
				}
				else if ( v.Type == ValueType.LIST ) {
					this.CloseAllFiles( (List<Value>)v.Val );
				}
			}
		}

		/// <summary>
		/// Read through all statements and set labels equal to their line number.
		/// </summary>
		private void CompileLabels( )
		{
			for ( int i = 0 ; i < this.Statements.Length ; i += 1 ) {
				if ( this.Statements[i].Type == StatementType.LABEL ) {
					this.SetValue( this.Statements[i].Operand1 , new Value( ValueType.INTEGER , (long)i ) , true );
				}
			}
		}
	}
}
