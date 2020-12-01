using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GigaClient
{
    /**
     * Least Recently Used (LRU) Cache implementation.
     * Based on https://gist.github.com/lxalln/71480a1d610d51506206
     */
    public class LRUCache<K, V>
    {
        private readonly int _capacity;
        private readonly Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>> _cacheMap = new Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>>();
        private readonly LinkedList<LRUCacheItem<K, V>> _lruList = new LinkedList<LRUCacheItem<K, V>>();

        public LRUCache(int capacity) {
            _capacity = capacity;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public V Get(K key) {
            LinkedListNode<LRUCacheItem<K, V>> node;
            if (_cacheMap.TryGetValue(key, out node)) {
                var value = node.Value.Value;
                _lruList.Remove(node);
                _lruList.AddLast(node);
                return value;
            }
            return default(V);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Add(K key, V val) {
            var cacheItem = new LRUCacheItem<K, V>(key, val);
            var node = new LinkedListNode<LRUCacheItem<K, V>>(cacheItem);

            // Key already exists -> Update value
            if (_cacheMap.TryGetValue(key, out var oldNode)) {
                _lruList.Remove(oldNode);
                _lruList.AddLast(node);
                _cacheMap[key] = node;
                return;
            }

            if (_cacheMap.Count >= _capacity) {
                RemoveFirst();
            }

            _lruList.AddLast(node);
            _cacheMap.Add(key, node);
        }

        private void RemoveFirst() {
            // Remove from LRUPriority
            var node = _lruList.First;
            _lruList.RemoveFirst();

            // Remove from cache
            _cacheMap.Remove(node.Value.Key);
        }
    }

    class LRUCacheItem<K, V>
    {
        public LRUCacheItem(K k, V v) {
            Key = k;
            Value = v;
        }

        public readonly K Key;
        public readonly V Value;
    }

}
