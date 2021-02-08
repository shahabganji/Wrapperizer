using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Wrapperizer.Domain.Abstractions
{
    public class FirstClassCollection<T>
    {
        private readonly IList<T> _collection;

        public FirstClassCollection()
        {
            _collection = new List<T>();
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
                Add(item);
        }

        public void Add(T item)
        {
            _collection.Add(item);
        }

        public void RemoveRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
                Remove(item);
        }

        public void Remove(T item)
        {
            _collection.Remove(item);
        }

        public IReadOnlyCollection<T> GetAll()
        {
            return new ReadOnlyCollection<T>(_collection);
        }
    }
}
