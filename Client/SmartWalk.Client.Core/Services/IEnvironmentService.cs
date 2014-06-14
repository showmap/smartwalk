using SmartWalk.Client.Core.Model.DataContracts;

namespace SmartWalk.Client.Core.Services
{
    public interface IEnvironmentService
    {
        IReachabilityService Reachability { get; }

        void Copy(string str);
        string Paste();

        void MakePhoneCall(string name, string number);
        void ComposeEmail(string to, string cc, string subject, string body, bool isHtml);
        void OpenURL(string url);
        void ShowDirections(Address address);

        void Alert(string title, string message);
        void WriteConsoleLine(string line, params object[] arg);
    }
}