
namespace QPP.Diagnostic
{
    public interface ITrace
    {
        void Write(object message);
        void WriteLine(object message);
    }
}
