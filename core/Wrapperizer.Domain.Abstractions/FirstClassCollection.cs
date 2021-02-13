using System;
using System.Collections.Generic;
using System.Linq;

namespace Wrapperizer.Domain.Abstractions
{
    public class FirstClassCollection<T>
    {
        private readonly List<T> _list;
        public int Count => _list.Count;
        public IReadOnlyCollection<T> Items => _list.AsReadOnly();

        public T this[int index]
        {
            get { return _list[index]; }
        }

        public FirstClassCollection()
        {
            _list = new List<T>();
        }
        public FirstClassCollection(IEnumerable<T> list) : this()
        {
            _list.AddRange(list);
        }

        public void Add(T item)
        {
            _list.Add(item);
        }
        public void AddRange(IEnumerable<T> list)
        {
            _list.AddRange(list);
        }
        public FirstClassCollection<T> Where(Func<T, bool> predicate)
        {
            return new FirstClassCollection<T>(_list.Where(predicate));
        }

        public FirstClassCollection<T> Merge(params IEnumerable<T>[] lists)
        {
            var all = lists.SelectMany(x => x).ToList();
            var merged = _list.Concat(all);
            return new FirstClassCollection<T>(merged);
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
    }
}
