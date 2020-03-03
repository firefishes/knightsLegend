using ShipDock.Interfaces;

namespace ShipDock.Tools
{
    public class IntegerMapper<MapperValueT> : IDispose
    {
        private IntegerID<MapperValueT> mIndexer;
        private KeyValueList<int, MapperValueT> mMapper;

        public IntegerMapper()
        {
            mIndexer = new IntegerID<MapperValueT>();
            mMapper = new KeyValueList<int, MapperValueT>();
        }

        public void Dispose()
        {
            mIndexer?.Dispose();
            mMapper?.Dispose();
        }

        public bool ContainsKey(ref MapperValueT key, out int id)
        {
            id = mIndexer.GetID(ref key);
            return mMapper != default ? mMapper.ContainsKey(id) : false;
        }

        public int Add(MapperValueT value, out int statu)
        {
            statu = 0;
            int id = -1;
            bool isAdded = ContainsKey(ref value, out id);
            if (isAdded)
            {
                statu = 1;
            }
            else
            {
                mMapper[id] = value;
            }
            return id;
        }

        public MapperValueT Remove(MapperValueT key, out int statu)
        {
            statu = 0;
            MapperValueT result = default;
            int id = mIndexer.GetID(ref key);
            if(mMapper.ContainsKey(id))
            {
                result = mMapper.Remove(id);
                statu = result == default ? 1 : 0;
            }
            else
            {
                statu = 2;
            }
            return result;
        }

        public MapperValueT Get(int id, out int statu)
        {
            MapperValueT result = default;
            result = mMapper.IsContainsKey(id) ? mMapper[id] : default;
            statu = result == default ? 1 : 0;
            return result;
        }

        public MapperValueT Get(int id)
        {
            MapperValueT result = mMapper.TryGet(id);
            return result;
        }

        public int ToID(ref MapperValueT value)
        {
            int id = mIndexer.GetID(ref value);
            return id;
        }

        public int GetIDByIndex(int index)
        {
            if((mMapper != default) && (mMapper.Keys.Count > index))
            {
                return mMapper.Keys[index];
            }
            return -1;
        }

        public int Size
        {
            get
            {
                return mMapper != default ? mMapper.Size : 0;
            }
        }
    }

}
