
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mystery.Content
{

    /// <summary>
    /// anything which can contains content, such as user controls, database, clipboard, memory chances etc...
    /// </summary>
    /// <remarks></remarks>
    public interface IContentContainer : ICollection<IContent>,IDisposable
    {

        /// <summary>
        /// add the contents to the container
        /// </summary>
        /// <remarks></remarks>

        void AddContents(IEnumerable<IContent> contents);
        /// <summary>
        /// remove all the given contents
        /// </summary>
        /// <param name="contents"></param>
        void RemoveContents(IEnumerable<IContent> contents);
        /// <summary>
        /// return the request content if the container have it, or nothing
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        T GetContent<T>(Guid guid) where T : IContent, new();

        /// <summary>
        /// return all the content of the required type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <remarks></remarks>
        IEnumerable<T> GetAll<T>() where T : IContent, new();

        IEnumerable<IContent> GetAllContents();
        /// <summary>
        /// return all the content of the required type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <remarks></remarks>
        IEnumerable<T> GetAllByFilter<T>(Expression<Func<T, bool>> filter) where T : IContent, new();


        /// <summary>
        /// return all the content of the required type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <remarks></remarks>
        IEnumerable<LightContentReferece> GetLightContentRereferece<T>() where T : IContent, new();


        /// <summary>
        /// return true if contains at least one object of give type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <remarks></remarks>
        bool ContainsType<T>() where T : IContent;


        /// <summary>
        /// search the container for contents
        /// </summary>
        /// <param name="search_text"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        IEnumerable<LightContentReferece> Search(string search_text,int max_results);

    }

    public static class ContentContainerExtensions {

        private interface ContentContainerHelper {
            IContent getContent(IContentContainer cd, Guid guid);
        }

        private class ContentContainerHelper<T> : ContentContainerHelper where T : IContent, new()
        {
            public IContent getContent(IContentContainer cd, Guid guid)
            {
                return cd.GetContent<T>(guid);
            }
        }

        static private IDictionary<string, ContentContainerHelper> _map = 
            new ConcurrentDictionary<string, ContentContainerHelper>();

        static public IContent GetContent(this IContentContainer cd, string content_type_name, Guid guid) {
            if(string.IsNullOrEmpty(content_type_name))
            if (_map.ContainsKey(content_type_name))
                return _map[content_type_name].getContent(cd, guid);
            var ct = ContentType.getType(content_type_name);
            if (ct == null)
                return null;
            var helper_type = typeof(ContentContainerHelper<>).MakeGenericType(ct);
            var helper = (ContentContainerHelper)Activator.CreateInstance(helper_type);
            _map[content_type_name] = helper;
            return helper.getContent(cd, guid);
        }
    }

}