using System.Collections.Generic;
using System.Threading.Tasks;
using Mystery.Register;
using Mystery.Json;
using System;
using System.Linq;
using System.Linq.Expressions;
using Mystery.MysteryAction;
using Mystery.Users;
using Mystery.Content;

namespace Mystery.History
{

    [GlobalAvalibleObjectImplementation(implementation_of =typeof(IHistoryRepository),overrides_exsisting =false,singleton =true)]
    public class InMemoryHistoryRepository : IHistoryRepository
    {
        private ICollection<HistoryEntry> _history { get; set; } = new LinkedList<HistoryEntry>();

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task AddAsync(HistoryEntry he)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            this.Add(he);
        }
        public void Add(HistoryEntry he)
        {
            IMysteryJsonConverter js = this.getGlobalObject<IMysteryJsonConverter>();
            string he_json = js.getJson(he);
            _history.Add(he);
        }

        public IEnumerable<HistoryEntry> GetByFilter(Expression<Func<HistoryEntry, bool>> filter)
        {
            var compiled = filter.Compile();
            return (from HistoryEntry x in _history where compiled(x) select x);
        }
        public IEnumerable<IPublishedAction> GetByTags(
            IEnumerable<string> tags, int max_result = 0, DateTime min_date = default(DateTime)) {
            if (tags == null || tags.Count() == 0)
                return new List<IPublishedAction>();
            ICollection<IPublishedAction> result = new LinkedList<IPublishedAction>();
            foreach (var entry in _history.OrderByDescending(x => x.date)) {
                foreach (var log in entry.logs) {
                    if (max_result > 0 && result.Count >= max_result)
                        return result;
                    if (entry.date < min_date)
                        return result;
                    if (log.history_tags.Intersect(tags).FirstOrDefault() == null)
                        continue;
                    result.Add(log);
                }
            }
            return result;
        }
        public IEnumerable<IPublishedAction> GetByUser(
            User user, int max_result = 0, DateTime min_date = default(DateTime))
        {
            if (user == null)
                return new LinkedList<IPublishedAction>();
            ICollection<IPublishedAction> result = new LinkedList<IPublishedAction>();
            foreach (var entry in _history.Where(x=>x.autheticated_user?.guid == user.guid).OrderByDescending(x => x.date))
            {
                foreach (var log in entry.logs)
                {
                    if (max_result > 0 && result.Count >= max_result)
                        return result;
                    if (entry.date < min_date)
                        return result;
                    result.Add(log);
                }
            }
            return result;
        }

        public IEnumerable<IPublishedAction> GetByContent(IContent content, int max_result = 0, DateTime min_date = default(DateTime))
        {
            if (content == null)
                return new LinkedList<IPublishedAction>();
            ICollection<IPublishedAction> result = new LinkedList<IPublishedAction>();
            foreach (var entry in _history.Where(x => x.guids.Contains(content.guid)).OrderByDescending(x => x.date))
            {
                foreach (var log in entry.logs)
                {
                    if (max_result > 0 && result.Count >= max_result)
                        return result;
                    if (entry.date < min_date)
                        return result;
                    result.Add(log);
                }
            }
            return result;
        }
    }
}
