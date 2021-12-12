using System;
using System.Collections.Generic;
using BNA.Common;
using BNA.Compile; // TODO remove dependency
using BNA.Exceptions;
using BNA.Values;

namespace BNA.Run
{
	public class Memory : IMemory
	{
		private Stack<Dictionary<Token , Value>> Scopes { get; init; }

		public Dictionary<Token , Value> Variables
		{
			get => this.Scopes is not null && this.Scopes.Count > 0
				? this.Scopes.Peek( ) : throw new Exception( "No scopes found." );

			private set
			{
				this.Scopes.Push( value );
				// this.CompileLabels( ); TODO maybe just make this a list in Program?
				this.SetValue( SpecialVariables.TEST_RESULT , SpecialVariables.TEST_RESULT_DEFAULT , true );
				this.SetValue( SpecialVariables.ARGUMENT , SpecialVariables.ARGUMENT_DEFAULT , true );
				this.SetValue( SpecialVariables.RETURN , SpecialVariables.RETURN_DEFAULT , true );
				this.SetValue( SpecialVariables.NULL , SpecialVariables.NULL_DEFAULT , true );
			}
		}

		public Memory( )
		{
			this.Scopes = new( );
			this.Variables = new( );
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
				throw new CannnotCloseFinalScopeException( );
			}
			_ = this.Scopes.Pop( );
			this.SetValue( SpecialVariables.RETURN , return_val );
		}

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
					return token.Value.Length >= 2 ? new StringValue( token.Value[1..^1] ) : throw new Exception( $"String token too short to be valid: {token}" );
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
							// Should have been detected at compiletime
							throw new Exception( $"Accessor at start or end of token: {token}" );
						}

						// Get value of index part
						Token indexTok = Lexer.ReadSingleToken( token.Value[( accessor + 1 )..] );
						Value indexVal = this.GetValue( indexTok );
						if ( indexVal is not IntegerValue index )
						{
							throw new InvalidIndexValueException( indexTok , indexVal );
						}

						// Get the value
						Token accessedTok = Lexer.ReadSingleToken( token.Value.Substring( 0 , accessor ) );
						Value accessedVal = this.GetValue( accessedTok );
						return accessedVal is ListValue listVal
								? index.Get >= 0 && index.Get < listVal.Get.Count ? listVal.Get[index.Get]
								: throw new ValueOutOfRangeException( indexVal , $"List {listVal}" )
							: accessedVal is StringValue strVal
								? index.Get >= 0 && index.Get < strVal.Get.Length ? new StringValue( "" + strVal.Get[index.Get] )
								: throw new ValueOutOfRangeException( indexVal , $"String \"{strVal}\"" )
							: throw new NonIndexableValueException( accessedTok , accessedVal );
					}

					// Find variable
					if ( this.Variables.TryGetValue( token , out Value? v ) )
					{
						return v;
					}

					// No value yet
					//throw new NonExistantVariableException( token );
					return Value.NULL;
				}

				// Tokenize and evaluate the contents of a list literal
				case TokenType.LIST:
				{
					Lexer lexer = new( token.Value[1..^1] );
					List<Token> listTokens = lexer.ReadTokens( );
					List<Value> listValues = new( );

					foreach ( Token t in listTokens )
					{
						if ( t.AsSymbol( ) is not Symbol.LIST_SEPERATOR )
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

				default:
					throw new Exception( "Unexpected token type in GetValue: " + token.ToString( ) );
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
					throw new Exception( $"Accessor at start or end of token: {token}" );
				}

				// Get value of index part
				Token indexTok = Lexer.ReadSingleToken( token.Value[( accessor + 1 )..] );
				Value indexVal = this.GetValue( indexTok );
				if ( indexVal is not IntegerValue index )
				{
					throw new InvalidIndexValueException( indexTok , indexVal );
				}

				// Set the value
				Token accessedTok = Lexer.ReadSingleToken( token.Value.Substring( 0 , accessor ) );
				Value accessedVal = this.GetValue( accessedTok );
				if ( accessedVal is ListValue listVal )
				{
					if ( index.Get < 0 || index.Get >= listVal.Get.Count )
					{
						throw new ValueOutOfRangeException( indexVal , $"List {listVal}" );
					}

					listVal.Get[index.Get] = newValue;
					return;
				}
				else if ( accessedVal is StringValue )
				{
					throw new NotImplementedException( $"Setting an indexed string is not implemented ( '{accessedVal}'@{indexVal} )." );
				}
				else
				{
					throw new NonIndexableValueException( accessedTok , accessedVal );
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
				throw new NonExistantVariableException( token );
			}
		}
	}
}
