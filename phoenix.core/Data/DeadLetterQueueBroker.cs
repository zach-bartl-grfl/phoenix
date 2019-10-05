using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace phoenix.core.Data
{
  /// <summary>
  /// Defines a message that could not be delivered for one reason or another.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class DeadLetter<T>
  {
    public string Exception { get; set; }
    public T Data { get; set; }
  }
  
  /// <summary>
  /// Provides an interface for interacting with the Dead Letters for Phoenix
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IDeadLetterQueueBroker<T>
  {
    void Publish(DeadLetter<T> item);
    DeadLetter<T> Retrieve();
    bool Any();
  }
  
  /// <inheritdoc />
  /// <summary>
  /// A quick in memory implementation of a dead letter queue for Phoenix
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class DeadLetterQueueBroker<T> : IDeadLetterQueueBroker<T>
  {
    private static string CacheKey => $"{nameof(T)}-dead-letters";
    private readonly Queue<DeadLetter<T>> _queue;

    public DeadLetterQueueBroker(IMemoryCache cache)
    {
      if (cache.TryGetValue(CacheKey, out _queue)) return;
      
      _queue = new Queue<DeadLetter<T>>(); 
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromMinutes(30)); // TODO: config
      cache.Set(CacheKey, _queue, cacheEntryOptions);
    }

    public void Publish(DeadLetter<T> item)
    {
      _queue.Append(item);
    }

    public DeadLetter<T> Retrieve()
    {
      return _queue.Dequeue();
    }

    public bool Any()
    {
      return _queue.Any();
    }
  }
}
