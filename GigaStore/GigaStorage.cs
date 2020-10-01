using GigaStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GigaStore
{
    public class GigaStorage
    {
        private MultiKeyDictionary<int, int, string> gigaObjects;
        private bool _virgin = true;

        public GigaStorage()
        {
            gigaObjects = new MultiKeyDictionary<int, int, string>();
        }

        public void write(int partition_id, int object_id, string value)
        {
            _virgin = false;
            Console.WriteLine("writing: " + partition_id + " " + object_id + " " + value);
            gigaObjects.Add(partition_id, object_id, value);
            Console.WriteLine("WROTE: " + gigaObjects[partition_id][object_id] );


        }

        public string read(int partition_id, int object_id)
        {
            string value;
            Console.WriteLine("Virgin: " + _virgin);
            try
            {
                Console.WriteLine("Reading: " + gigaObjects[partition_id][object_id]);
                value = gigaObjects[partition_id][object_id];
            }
            catch (KeyNotFoundException)
            {
                value = "N/A";
            }
            return value;
        }
    }

    public class MultiKeyDictionary<K1, K2, V> : Dictionary<K1, Dictionary<K2, V>>
    {

        public V this[K1 key1, K2 key2]
        {
            get
            {
                if (!ContainsKey(key1) || !this[key1].ContainsKey(key2))
                    throw new ArgumentOutOfRangeException();
                return base[key1][key2];
            }
            set
            {
                if (!ContainsKey(key1))
                    this[key1] = new Dictionary<K2, V>();
                this[key1][key2] = value;
            }
        }

        public void Add(K1 key1, K2 key2, V value)
        {
            if (!ContainsKey(key1))
                this[key1] = new Dictionary<K2, V>();
            this[key1][key2] = value;
            Console.WriteLine("ADD " + key1 + " " + key2 + " " + value);
            Console.WriteLine("ADDED " + this[key1][key2]);
        }

        public bool ContainsKey(K1 key1, K2 key2)
        {
            return base.ContainsKey(key1) && this[key1].ContainsKey(key2);
        }

        public new IEnumerable<V> Values
        {
            get
            {
                return from baseDict in base.Values
                       from baseKey in baseDict.Keys
                       select baseDict[baseKey];
            }
        }

    }
}
      
