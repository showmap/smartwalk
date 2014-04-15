namespace SmartWalk.Client.Core.Services
{
    public interface IClipboard
    {
        void Copy(string str);

        string Paste();
    }
}