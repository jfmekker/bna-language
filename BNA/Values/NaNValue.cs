using System;
using System.Diagnostics.CodeAnalysis;

namespace BNA.Values
{
    /// <summary>
    /// NaN type value.
    /// </summary>
    public class NaNValue : Value
    {
        [SuppressMessage( "Design", "CA1065:Do not raise exceptions in unexpected locations",
            Justification = "This is reasonable for NaN" )]
        [SuppressMessage( "Usage", "CA2201:Do not raise reserved exception types",
            Justification = "TODO: Make custom runtime exception" )]
        public override object Get => throw new Exception( "Cannot get value of a NaNValue." );

        public override string TypeString( ) => "NaNValue";

        public override bool Equals( Value? other ) => false;

        public override int GetHashCode( ) => 0;

        public override string ToString( ) => "NaN";
    }
}
