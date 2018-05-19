using System.Collections.Generic;
using System.Threading;

namespace GzipAssessment
{
    public class BlockingQueue<T>
    {
        private readonly Queue<T> _queue = new Queue<T>();
        private readonly int _maxSize;
        bool _closing;

        public BlockingQueue(int maxSize)
        {
            _maxSize = maxSize;
        }

        public void Close()
        {
            lock (_queue)
            {
                _closing = true;
                Monitor.PulseAll(_queue);
            }
        }

        public void Enqueue(T item)
        {
            lock (_queue)
            {
                while (_queue.Count >= _maxSize)
                {
                    Monitor.Wait(_queue);
                }

                _queue.Enqueue(item);
                if (_queue.Count == 1)
                {
                    // wake up any blocked dequeue
                    Monitor.PulseAll(_queue);
                }
            }
        }

        public bool TryDequeue(out T value)
        {
            lock (_queue)
            {
                while (_queue.Count == 0)
                {
                    if (_closing)
                    {
                        value = default(T);
                        return false;
                    }

                    Monitor.Wait(_queue);
                }

                value = _queue.Dequeue();
                if (_queue.Count == _maxSize - 1)
                {
                    // wake up any blocked enqueue
                    Monitor.PulseAll(_queue);
                }
                return true;
            }
        }
    }
}
