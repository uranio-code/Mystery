using System;
namespace Mystery.Content
{


    public interface IContent
    {
        Guid guid { get; set; }

        /// <summary>
        /// make the content ready to be sent into a storage, network etc
        /// </summary>
        /// <remarks>called by the database before the saving, last possibility to actually do something</remarks>

        void seal();

        string ReferenceText { get; }
        string SearchText { get; }

        event ContentEventHandler contentSealed;
    }

    /// <summary>
    /// a content with a user friendly uid
    /// </summary>
    /// <remarks></remarks>
    public interface IContentWithUid : IContent
    {
        string  uid { get; set; }
    }


    public delegate void ContentEventHandler(IContent content); 
}