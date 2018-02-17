using System.Threading.Tasks;
using Mystery.Register;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using Mystery.MysteryAction;
using Mystery.Users;
using Mystery.Content;

namespace Mystery.History
{
    [GlobalAvalibleObject()]
    public interface IHistoryRepository
    {
        Task AddAsync(HistoryEntry he);
        void Add(HistoryEntry he);

        IEnumerable<HistoryEntry> GetByFilter(Expression<Func<HistoryEntry, bool>> filter);

        IEnumerable<IPublishedAction> GetByTags(IEnumerable<string> tags, int max_result =0, DateTime min_date = default(DateTime));

        IEnumerable<IPublishedAction> GetByUser(User user, int max_result = 0, DateTime min_date = default(DateTime));

        IEnumerable<IPublishedAction> GetByContent(IContent content, int max_result = 0, DateTime min_date = default(DateTime));
    }
}
