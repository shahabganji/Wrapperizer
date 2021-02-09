using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Wrapperizer.Domain.Abstractions
{
    public class FirstClassCollection<T>
    {
        private readonly List<T> _list;
        public int Count => _list.Count;
        public IReadOnlyCollection<T> Items => _list.AsReadOnly();
        public FirstClassCollection()
        {
            _list = new List<T>();
        }
        public void Add(T item)
        {
            _list.Add(item);
        }
        public void AddRange(IEnumerable<T> list)
        {
            _list.AddRange(list);
        }
        public IEnumerable<T> Filter(Expression<Func<T, bool>> predicate)
        {
            return _list.AsQueryable().Where(predicate);
        }
        public T Get(int itemIndex)
        {
            return _list[itemIndex];
        }
        public IEnumerable<T> Merge(params IEnumerable<T>[] lists)
        {
            var all = lists.SelectMany(x => x).ToList();
            AddRange(all);
            return _list;
        }
        public void Remove(T item)
        {
            _list.Remove(item);
        }
        public void RemoveRange(IEnumerable<T> list)
        {
            foreach (var item in list)
                Remove(item);
        }
        public void Set(int itemIndex, T item)
        {
            _list[itemIndex] = item;
        }
    }
}
