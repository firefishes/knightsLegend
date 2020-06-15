using ShipDock.Tools;
using System.Collections.Generic;

namespace KLGame
{
    public class WorldStatesMapper
    {
        private IWorldState mWorldState;
        private List<IWorldState> mWillRemoveStates;
        private KeyValueList<int, int> mUnoriented;
        private KeyValueList<int, int> mObjectiveOriented;
        private KeyValueList<int, int> mOneselfOriented;

        public WorldStatesMapper()
        {
            States = new List<IWorldState>();
            mWillRemoveStates = new List<IWorldState>();

            mUnoriented = new KeyValueList<int, int>();
            mObjectiveOriented = new KeyValueList<int, int>();
            mOneselfOriented = new KeyValueList<int, int>();
        }

        public void UpdateWorldStats()
        {
            int max = States.Count;
            for (int i = 0; i < max; i++)
            {
                mWorldState = States[i];
                if (mWorldState.WillRemoveFromWorld)
                {
                    mWillRemoveStates.Add(mWorldState);
                }
                else
                {
                    CheckWorldState();
                }
            }
            int id;
            max = mWillRemoveStates.Count;
            for (int i = 0; i < max; i++)
            {
                mWorldState = mWillRemoveStates[i];
                States.Remove(mWorldState);
                id = mWorldState.StateID;

                switch (mWorldState.OrientedType)
                {
                    case KLConsts.WORLD_STATE_ORIENTED_NONE:
                        mUnoriented.Remove(id);
                        break;
                    case KLConsts.WORLD_STATE_ORIENTED_OBJECTIVE:
                        mObjectiveOriented.Remove(id);
                        break;
                    case KLConsts.WORLD_STATE_ORIENTED_ONESELF:
                        mOneselfOriented.Remove(id);
                        break;
                }
            }
            mWillRemoveStates.Clear();
        }

        public void AddWorldStates(IWorldStateIssuer stateIssuer)
        {
            IWorldState[] worldStates = stateIssuer != default ? stateIssuer.ProvideWorldStates : default;
            if (worldStates != default)
            {
                int id, index;
                IWorldState item;
                int max = worldStates.Length;
                for (int i = 0; i < max; i++)
                {
                    index = States.Count;

                    item = worldStates[i];
                    item.Available = true;
                    States.Add(item);

                    id = item.StateID;

                    switch (item.OrientedType)
                    {
                        case KLConsts.WORLD_STATE_ORIENTED_NONE:
                            mUnoriented[id] = index;
                            break;
                        case KLConsts.WORLD_STATE_ORIENTED_OBJECTIVE:
                            mObjectiveOriented[id] = index;
                            break;
                        case KLConsts.WORLD_STATE_ORIENTED_ONESELF:
                            mOneselfOriented[id] = index;
                            break;
                    }
                }
            }
        }

        public void RemoveWorldStates(IWorldStateIssuer stateIssuer)
        {
            IWorldState[] worldStates = stateIssuer != default ? stateIssuer.ProvideWorldStates : default;
            if (worldStates != default)
            {
                IWorldState item;
                int max = worldStates.Length;
                for (int i = 0; i < max; i++)
                {
                    item = worldStates[i];
                    item.Available = false;
                    item.WillRemoveFromWorld = true;
                }
            }
        }

        private void CheckWorldState()
        {
        }

        public void RefOrientedStates(int oriented, ref List<IWorldState> result, bool isGetNew = true)
        {
            if (result == default)
            {
                result = new List<IWorldState>();
            }
            else if (isGetNew)
            {
                result.Clear();
            }
            switch (oriented)
            {
                case KLConsts.WORLD_STATE_ORIENTED_NONE:
                    GetStatesByFilters(ref mUnoriented, ref result);
                    break;
                case KLConsts.WORLD_STATE_ORIENTED_OBJECTIVE:
                    GetStatesByFilters(ref mObjectiveOriented, ref result);
                    break;
                case KLConsts.WORLD_STATE_ORIENTED_ONESELF:
                    GetStatesByFilters(ref mOneselfOriented, ref result);
                    break;
            }
        }

        private void GetStatesByFilters(ref KeyValueList<int, int> source, ref List<IWorldState> result)
        {
            int index;
            int max = source.Size;
            IWorldState item;
            List<int> values = source.Values;
            for (int i = 0; i < max; i++)
            {
                index = values[i];
                item = States[index];
                if (item.Available && !item.WillRemoveFromWorld)
                {
                    result.Add(item);
                }
            }
        }

        public List<IWorldState> States { get; private set; }
    }
}