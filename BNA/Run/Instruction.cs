using System;
using BNA.Common;
using BNA.Exceptions;
using BNA.Values;

namespace BNA.Run
{
	/// <summary>
	/// Executable statement object.
	/// </summary>
	public class Instruction
	{
		public Token PrimaryToken { get; init; }

		public Token SecondaryToken { get; init; }

		public Value? PrimaryValue { get; private set; }

		public Value? SecondaryValue { get; private set; }

		public Operation Type { get; init; }

		public IProgram Program { get; init; }

		public IMemory Memory { get; init; }

		public Instruction( Operation operation , Token? operand1 , Token? operand2 , IProgram program , IMemory memory )
		{
			this.Program = program;
			this.Memory = memory;
			this.PrimaryToken = operand1 ?? default;
			this.SecondaryToken = operand2 ?? default;
			this.Type = operation;
		}

		public void Execute( )
		{
			this.PrimaryValue = this.Memory.GetValue( this.PrimaryToken );
			this.SecondaryValue = this.Memory.GetValue( this.SecondaryToken );

			switch ( this.Type )
			{
				case Operation.NULL:
				case Operation.LABEL:
					break;

				case Operation.SET:
				{
					this.Memory.SetValue( this.PrimaryToken ,
						this.SecondaryValue is ListValue list ? list.DeepCopy( ) : this.SecondaryValue ,
						true );
					break;
				}

				case Operation.ADD:
				{
					this.Memory.SetValue( this.PrimaryToken , this.PrimaryValue.Add( this.SecondaryValue ) );
					break;
				}

				case Operation.SUBTRACT:
				{
					this.Memory.SetValue( this.PrimaryToken , this.PrimaryValue.Subtract( this.SecondaryValue ) );
					break;
				}

				case Operation.MULTIPLY:
				{
					this.Memory.SetValue( this.PrimaryToken , this.PrimaryValue.Multiply( this.SecondaryValue ) );
					break;
				}

				case Operation.DIVIDE:
				{
					this.Memory.SetValue( this.PrimaryToken , this.PrimaryValue.Divide( this.SecondaryValue ) );
					break;
				}

				case Operation.RANDOM:
				{
					Value randVal
						= this.SecondaryValue is IntegerValue intVal ? new IntegerValue( BNA.RNG.Next( intVal.Get ) )
						: this.SecondaryValue is FloatValue floatVal ? new FloatValue( BNA.RNG.NextDouble( ) * floatVal.Get )
						: throw new IncorrectOperandTypeException( this.Type , this.SecondaryToken , this.SecondaryValue );
					this.Memory.SetValue( this.PrimaryToken , randVal , true );
					break;
				}

				case Operation.BITWISE_OR:
				case Operation.BITWISE_AND:
				case Operation.BITWISE_XOR:
				case Operation.BITWISE_NEGATE:
					throw new NotImplementedException( $"Execution not implemented for {this.Type}." );

				case Operation.POWER:
				{
					this.Memory.SetValue( this.PrimaryToken , this.PrimaryValue.RaiseTo( this.SecondaryValue ) );
					break;
				}

				case Operation.MODULUS:
				{
					this.Memory.SetValue( this.PrimaryToken , this.PrimaryValue.Modulus( this.SecondaryValue ) );
					break;
				}

				case Operation.LOGARITHM:
				{
					this.Memory.SetValue( this.PrimaryToken , this.PrimaryValue.Log( this.SecondaryValue ) );
					break;
				}

				case Operation.ROUND:
				{
					this.Memory.SetValue( this.PrimaryToken , this.PrimaryValue.Round( ) );
					break;
				}

				case Operation.APPEND:
				{
					this.Memory.SetValue( this.PrimaryToken , this.PrimaryValue.Append( this.SecondaryValue ) );
					break;
				}

				case Operation.LIST:
				{
					int size = this.SecondaryValue is IntegerValue intVal ? intVal.Get : throw new IncorrectOperandTypeException( this.Type , this.SecondaryToken , this.SecondaryValue );
					this.Memory.SetValue( this.PrimaryToken , new ListValue( size ) , true );
					break;
				}

				case Operation.SIZE:
				{
					this.Memory.SetValue( this.PrimaryToken , this.SecondaryValue.Size( ) , true );
					break;
				}

				case Operation.OPEN_WRITE:
				case Operation.OPEN_READ:
				{
					bool read = this.Type == Operation.READ;
					string filename = this.SecondaryValue is StringValue strVal ? strVal.Get : throw new IncorrectOperandTypeException( this.Type , this.SecondaryToken , this.SecondaryValue );
					this.Memory.SetValue( this.PrimaryToken , read ? new ReadFileValue( filename ) : new WriteFileValue( filename ) , true );
					break;
				}

				case Operation.CLOSE:
				{
					if ( this.PrimaryValue is FileValue fileVal )
					{
						fileVal.Close( );
						this.Memory.SetValue( this.PrimaryToken , Value.NULL );
						break;
					}
					else
					{
						throw new IncorrectOperandTypeException( this.Type , this.PrimaryToken , this.PrimaryValue );
					}
				}

				case Operation.READ:
				{
					if ( this.PrimaryValue is ReadFileValue readFileVal )
					{
						string? str = readFileVal.ReadLine( );

						if ( str is null )
						{
							readFileVal.Close( );
							this.Memory.SetValue( this.SecondaryToken , Value.NULL );
						}

						Value val = str is null ? Value.NULL
								  : long.TryParse( str , out long lval ) ? new IntegerValue( lval )
								  : double.TryParse( str , out double dval ) ? new FloatValue( dval )
								  : new StringValue( str );

						this.Memory.SetValue( this.PrimaryToken , val , true );
						break;
					}
					else
					{
						throw new IncorrectOperandTypeException( this.Type , this.PrimaryToken , this.PrimaryValue );
					}
				}

				case Operation.WRITE:
				{
					if ( this.PrimaryValue is WriteFileValue writeFileVal )
					{
						writeFileVal.WriteLine( this.SecondaryValue.ToString( ) );
						break;
					}
					else
					{
						throw new IncorrectOperandTypeException( this.Type , this.PrimaryToken , this.PrimaryValue );
					}
				}

				case Operation.INPUT:
				{
					Console.Write(
						this.SecondaryValue is StringValue strVal ? strVal.Get
						: throw new IncorrectOperandTypeException( this.Type , this.SecondaryToken , this.SecondaryValue )
					);

					// TODO badness here, need to take in a string probably
					// Token token = Lexer.ReadSingleToken( Console.ReadLine( ) ?? string.Empty );
					// TODO actually parse it or something (just a number vs string?)
					string input = Console.ReadLine( ) ?? string.Empty;
					Token token = new( $"\"{input}\"" , TokenType.STRING );
					switch ( token.Type )
					{
						case TokenType.LITERAL:
						case TokenType.LIST:
							this.Memory.SetValue( this.PrimaryToken , this.Memory.GetValue( token ) , true );
							break;

						case TokenType.STRING:
						case TokenType.SYMBOL:
						case TokenType.VARIABLE:
							this.Memory.SetValue( this.PrimaryToken , new StringValue( token.Value ) , true );
							break;

						// case TokenType.NULL:
						// 	this.Memory.SetValue( this.PrimaryToken , Value.NULL , true );
						// 	break;

						default:
							throw new Exception( "Unexpected data from INPUT return: " + token.ToString( ) );
					}
					break;
				}

				case Operation.PRINT:
				{
					Console.WriteLine( this.PrimaryValue.ToString( ) );
					break;
				}

				case Operation.WAIT:
				{
					int ms = (int)( 1000 *
						( this.SecondaryValue is IntegerValue intVal ? intVal.Get
						: this.SecondaryValue is FloatValue floatVal ? floatVal.Get
						: throw new IncorrectOperandTypeException( this.Type , this.SecondaryToken , this.SecondaryValue ) ) );

					try
					{
						System.Threading.Thread.Sleep( ms );
					}
					catch ( Exception e )
					{
						// TODO make this a new runtime exception class?
						throw new Exception( "Exception caught while waiting:\n" + e.Message );
					}

					break;
				}

				case Operation.ERROR:
				{
					throw new ErrorStatementException( this.SecondaryValue );
				}

				case Operation.EXIT:
				{
					this.Program.Running = false;
					break;
				}

				case Operation.TEST_EQUAL:
				case Operation.TEST_NOT_EQUAL:
				case Operation.TEST_GREATER_THAN:
				case Operation.TEST_LESS_THAN:
				{
					Value result
						= this.Type == Operation.TEST_EQUAL ? this.PrimaryValue == this.SecondaryValue ? Value.TRUE : Value.FALSE
						: this.Type == Operation.TEST_NOT_EQUAL ? this.PrimaryValue != this.SecondaryValue ? Value.TRUE : Value.FALSE
						: this.Type == Operation.TEST_GREATER_THAN ? this.PrimaryValue.GreaterThan( this.SecondaryValue ) ? Value.TRUE : Value.FALSE
						: this.Type == Operation.TEST_LESS_THAN ? this.PrimaryValue.LessThan( this.SecondaryValue ) ? Value.TRUE : Value.FALSE
						: throw new Exception( $"Unexpted TEST statement type: {this.Type}" );
					this.Memory.SetValue( SpecialVariables.TEST_RESULT , result );
					break;
				}

				case Operation.TYPE:
				{
					string type = this.SecondaryValue.TypeString( )[..^"Value".Length].ToUpper( );
					this.Memory.SetValue( this.PrimaryToken , new StringValue( type ) , true );
					break;
				}

				case Operation.SCOPE_OPEN:
				{
					this.Memory.OpenScope( );
					break;
				}

				case Operation.SCOPE_CLOSE:
				{
					this.Memory.CloseScope( );
					break;
				}

				case Operation.GOTO:
				{
					// Subtract one because the IP will be incremented after this
					int newIP = this.PrimaryValue is IntegerValue intVal ? intVal.Get - 1
								: this.Program.Labels[this.PrimaryToken];

					// No negative values or overflow (after increment)
					if ( newIP is < -1 or int.MaxValue )
					{
						throw new ValueOutOfRangeException( this.PrimaryValue , "GOTO address" );
					}

					// Check condition
					if ( this.SecondaryValue != Value.FALSE )
					{
						this.Program.IP = newIP;
					}

					break;
				}

				default:
					throw new Exception( $"Unexpected statement type: {this.Type}" );
			}
		}

		public override string ToString( ) => $"{this.Type} ({this.PrimaryOperandInfo( )}) ({this.SecondaryOperandInfo( )})";

		/// <summary>
		/// Display relevant info on the primary operand.
		/// </summary>
		/// <returns>String of the token, value, and value type.</returns>
		private string PrimaryOperandInfo( ) => $"{this.PrimaryToken} {this.PrimaryValue?.TypeString( )} '{this.PrimaryValue}'";

		/// <summary>
		/// Display relevant info on the secondary operand.
		/// </summary>
		/// <returns>String of the token, value, and value type.</returns>
		private string SecondaryOperandInfo( ) => $"{this.SecondaryToken} {this.SecondaryValue?.TypeString( )} '{this.SecondaryValue}'";
	}
}
