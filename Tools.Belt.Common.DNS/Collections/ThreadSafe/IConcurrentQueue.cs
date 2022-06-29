using System.Collections.Generic;

namespace Tools.Belt.Common.Collections.ThreadSafe
{
    public interface IConcurrentQueue<T> : IConcurrentQueue
    {
        void CopyTo(T[] array, int index);
        void Enqueue(T item);
        IEnumerator<T> GetEnumerator();
        T[] ToArray();
        bool TryDequeue(out T result);
        bool TryPeek(out T result);
    }

    public interface IConcurrentQueue
    {
        int Count { get; }
        bool IsEmpty { get; }
        void Clear();
    }
}