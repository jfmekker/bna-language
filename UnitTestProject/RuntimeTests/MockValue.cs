using System;
using BNA.Common;
using BNA.Values;

namespace RuntimeTests
{
	public class MockValue : Value
	{
		public string? LastCalledFunction { get; private set; }

		public override object Get => new( );

		public override string ToString( ) => base.ToString( );
		public override string TypeString( ) => "MockValue";
		public override int GetHashCode( ) => base.GetHashCode( );

		public override bool GreaterThan( Value value ) { this.LastCalledFunction = "GreaterThan"; return true; }
		public override bool LessThan( Value value ) { this.LastCalledFunction = "LessThan"; return true; }
		public override bool Equals( Value? other ) { this.LastCalledFunction = "Equals"; return true; }
		public override Value Add( Value value ) { this.LastCalledFunction = "Add"; return this; }
		public override Value Subtract( Value value ) { this.LastCalledFunction = "Subtract"; return this; }
		public override Value Multiply( Value value ) { this.LastCalledFunction = "Multiply"; return this; }
		public override Value Divide( Value value ) { this.LastCalledFunction = "Divide"; return this; }
		public override Value Modulus( Value value ) { this.LastCalledFunction = "Modulus"; return this; }
		public override Value Log( Value value ) { this.LastCalledFunction = "Log"; return this; }
		public override Value Exponentiate( Value value ) { this.LastCalledFunction = "RaiseTo"; return this; }
		public override Value Append( Value value ) { this.LastCalledFunction = "Append"; return this; }
		public override Value Round( ) { this.LastCalledFunction = "Round"; return this; }
		public override Value Size( ) { this.LastCalledFunction = "Size"; return this; }

		public static Value GetValueOfType( string val_type ) => val_type switch
		{
			"Integer" => new IntegerValue( 0 ),
			"Float" => new FloatValue( 0 ),
			"String" => new StringValue( "" ),
			"List" => new ListValue( 0 ),
			"Mock" => new MockValue( ),
			_ => throw new Exception( "Unknow value type." ),
		};

		public static Token GetTokenOfType( TokenType type ) => type switch
		{
			TokenType.NUMBER => new Token( "0" , type ),
			TokenType.STRING => new Token( "\"\"" , type ),
			TokenType.LIST => new Token( "()" , type ),
			TokenType.VARIABLE => new Token( "var" , type ),
			_ => throw new Exception( "Unknow token type." ),
		};
	}
}
