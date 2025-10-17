using SMS.Core.Exceptions;
using SMS.Core.Exceptions;
using ValueOf;

namespace SMS.Core.ValueObjects
{
    public class TcpPortNumber : ValueOf<short, TcpPortNumber>
    {
        protected override void Validate()
        {
            if (Value < 1)
            {
                throw new TcpPortNumberException(Value);
            }
        }
    }
}
