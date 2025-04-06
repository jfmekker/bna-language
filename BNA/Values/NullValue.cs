using System;
using System.Diagnostics.CodeAnalysis;

namespace BNA.Values
{
    /// <summary>
    /// Null type value.
    /// </summary>
    public class NullValue : Value
    {
        [SuppressMessage( "Design", "CA1065:Do not raise exceptions in unexpected locations",
            Justification = "This is reasonable for Null" )]
        [SuppressMessage( "Usage", "CA2201:Do not raise reserved exception types",
            Justification = "TODO: Make custom runtime exception" )]
        public override object Get => throw new Exception( "Cannot get value of a NullValue." );

        public override string TypeString( ) => "NullValue";

        public override bool Equals( Value? other ) => other is NullValue;

        public override int GetHashCode( ) => 0;

        public override string ToString( ) => "null";
    }
}
