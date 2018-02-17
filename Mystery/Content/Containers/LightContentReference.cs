using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Mystery.Content
{

    [Serializable()]
    public class LightContentReferece
    {

        public LightContentReferece() { }

        public LightContentReferece(IContent c) {
            guid = c.guid;
            ReferenceText = c.ReferenceText;
            SearchText = c.SearchText;
        }

        [BsonId]
        public Guid guid { get; set; }
        public string ReferenceText { get; set; }

        public string SearchText { get; set; }

        public string url { get; set; }

        public double? TextMatchScore { get; set; }

        private class Comparer : IComparer<LightContentReferece>
        {

            public int Compare(LightContentReferece x, LightContentReferece y)
            {
                if (x == null && y == null)
                    return 0;
                if (x == null)
                    return -1;
                if (y == null)
                    return 1;
                return string.Compare(x.ReferenceText, y.ReferenceText);
            }
        }

        public static IComparer<LightContentReferece> getComparer()
        {
            return new Comparer();
        }


    }

}