

namespace Mystery.Messaging
{
    /// <summary>
    /// Thsi interface defines a codified message. The user set the code and the language to get back a IUserMessage with the body prefilled.
    /// </summary>
    public interface ICodifiedMessage
    {
        string code { get; set; }
        string language { get; set; }
        IUserMessage message { get; }

    }
}
