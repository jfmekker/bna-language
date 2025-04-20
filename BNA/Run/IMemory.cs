using BNA.Compile.Tokens;
using BNA.Values;

namespace BNA.Run
{
    public interface IMemory
    {
        public void SetValue( Token token, Value newValue, bool add = false );

        public Value GetValue( Token token );

        public void OpenScope( );

        public void CloseScope( );
    }
}
