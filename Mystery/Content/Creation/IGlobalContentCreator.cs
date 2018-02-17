using Mystery.Register;
using System;
namespace Mystery.Content
{

    [GlobalAvalibleObject()]
    public interface IGlobalContentCreator 
    {
        T getNewContent<T>() where T : IContent;
        T getAndAddNewContent<T>() where T : IContent;
        IContent getNewContent(Type type);
        IContent getAndAddNewContent(Type type);
    } 
}