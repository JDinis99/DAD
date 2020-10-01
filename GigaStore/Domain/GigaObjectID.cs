using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GigaStore.Domain
{
    public class GigaObjectID
    {
        private int _partition_id;
        private int _obejct_id;

        public GigaObjectID(int partition_id, int object_id)
        {
            _partition_id = partition_id;
            _obejct_id = object_id;
        }

        public int getPartition_id ()
        {
            return _partition_id;
        }

        public int getObject_id()
        {
            return _obejct_id;
        }
    }

}
