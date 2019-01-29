using System.Collections.Generic;

namespace Twidlle.Infrastructure
{
    public interface ICollectionStorage<T>
    {
        IEnumerable<T> Load();

        void Save(IReadOnlyCollection<T> data);
    }
}
