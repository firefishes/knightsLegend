using ShipDock.Applications;
using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ShipDock.Server
{
    /// <summary>
    /// 
    /// 服务容器管理器
    /// 
    /// </summary>
    public class Servers : IDispose, IServersHolder
    {
        private static readonly Type resolvableAttrType = typeof(ResolvableAttribute);
        private static readonly Type serverPriorityAttrType = typeof(ServerPriority);

        private bool mCanCheckServers;
        private IServer mGlobalServer;
        private object[] mServerPriorAttrs;
        private List<IServer> mNewServers;
        private List<IServer> mServersWillCheck;
        private List<IServer> mServersWillSort;
        private ServerPriority mPriorAttributesItem;
        private KeyValueList<string, IServer> mServerMapper;
        private IntegerID<string> mAliasID;
        private IntegerID<ResolvableBinder> mBinderIDs;
        private IntegerMapper<Type> mTypeMapper;
        private Dictionary<int, ResolvableBinder> mBinderMapper;
        private Dictionary<int, IResolvable> mResolvablesMapper;
        private KeyValueList<int, IResolvableConfig> mResolvableConfigs;
        private IntegerMapper<string> mResolvableNameMapper;

        public Servers(params Action[] onInites)
        {
            mServerMapper = new KeyValueList<string, IServer>();

            mNewServers = new List<IServer>();
            mServersWillSort = new List<IServer>();
            mServersWillCheck = new List<IServer>();

            mAliasID = new IntegerID<string>();
            mTypeMapper = new IntegerMapper<Type>();
            mBinderIDs = new IntegerID<ResolvableBinder>();
            mResolvableNameMapper = new IntegerMapper<string>();

            mBinderMapper = new Dictionary<int, ResolvableBinder>();
            mResolvablesMapper = new Dictionary<int, IResolvable>();
            mResolvableConfigs = new KeyValueList<int, IResolvableConfig>();
            
            int max = onInites.Length;
            for (int i = 0; i < max; i++)
            {
                OnInit += onInites[i];
            }
        }

        public void Dispose()
        {
            IsServersReady = false;
            mCanCheckServers = false;
            IsInited = false;

            Utils.Reclaim(ref mNewServers);
            Utils.Reclaim(ref mServersWillCheck);
            Utils.Reclaim(ref mServersWillSort);
            Utils.Reclaim(ref mServerMapper, true, true);
            Utils.Reclaim(mAliasID);
            Utils.Reclaim(mTypeMapper);
            Utils.Reclaim(mBinderIDs);
            Utils.Reclaim(mResolvableNameMapper);
            Utils.Reclaim(ref mBinderMapper, true, true);
            Utils.Reclaim(ref mResolvablesMapper, true, true);
            Utils.Reclaim(ref mResolvableConfigs, true, true);

            OnFinished = default;
        }

        public void InitGlobalServer(Action onInit, Action onReady = default, string serverName = "/")
        {
            mGlobalServer = new Server(serverName)
            {
                Prioriity = -int.MaxValue
            };

            int statu = 0;
            string nameTemp = default;
            CacheServer(ref nameTemp, ref mGlobalServer, ref statu);
            onInit?.Invoke();

            mGlobalServer.ServerReady();
            onReady?.Invoke();
        }

        public void Add(IServer server)
        {
            if (mServerMapper == default)
            {
                return;
            }
            if(!IsInited)
            {
                IsInited = true;
                OnInit?.Invoke();
                OnInit = default;
            }
            string serverName = (server != default) ? server.ServerName : string.Empty;
            if (!string.IsNullOrEmpty(serverName) && !mServerMapper.IsContainsKey(serverName))
            {
                SetAndInitServer(ref server);
                SetServerPriority(server);
                mNewServers.Add(server);

                if (!mCanCheckServers)
                {
                    mCanCheckServers = true;
                    ShipDockApp.CallLater(CheckServerList);
                }
                mServerPriorAttrs = default;
            }
        }

        private void SetAndInitServer(ref IServer server)
        {
            if (server == default)
            {
                server = new Server();
            }

            server.SetServerHolder(this);
        }

        private void SetServerPriority(IServer server)
        {
            Type type = server.GetType();
            mServerPriorAttrs = type.GetCustomAttributes(serverPriorityAttrType, false);
            if (mServerPriorAttrs.Length > 0)
            {
                mPriorAttributesItem = mServerPriorAttrs[0] as ServerPriority;
                if (mPriorAttributesItem != default)
                {
                    int prioriity = mPriorAttributesItem.Priority;
                    server.Prioriity = prioriity;
                }
            }
        }

        public void Remove(IServer server)
        {
            if ((server != default) && (mServerMapper != default) && mServerMapper.IsContainsKey(server.ServerName))
            {
                mServerMapper.Remove(server.ServerName);
                Utils.Reclaim(server);
            }
        }

        /// <summary>
        /// 检测需要初始化的新容器
        /// </summary>
        private void CheckServerList(int time)
        {
            Utils.CloneFrom(ref mServersWillCheck, ref mNewServers, true);

            int max = mServersWillCheck.Count;
            if (max > 0)
            {
                IServer server = default;
                for (int i = 0; i < max; i++)
                {
                    server = mServersWillCheck[i];
                    if (server != default)
                    {
                        mServersWillSort.Add(server);
                    }
                }
            }
            mServersWillCheck.Clear();
            mCanCheckServers = false;

            ShipDockApp.CallLater(SortServersByPriority);
        }

        /// <summary>
        /// 容器初始化优先级排序
        /// </summary>
        private void SortServersByPriority(int time)
        {
            if (mNewServers.Count > 0)
            {
                return;
            }
            mServersWillSort.Sort(ComparerPriority);

            int statu;
            IServer temp = default;
            InitServers(ref temp, out statu);
            ServersReady(ref temp, ref statu);
            AfterServersInited();
        }

        private void InitServers(ref IServer target, out int statu)
        {
            statu = 0;
            string serverName = default;
            int max = mServersWillSort.Count;
            for (int i = 0; i < max; i++)
            {
                target = mServersWillSort[i];
                if (target != default)
                {
                    CacheServer(ref serverName, ref target, ref statu);
                }
                else
                {
                    statu = 2;
                }
            }
        }

        private void ServersReady(ref IServer target, ref int statu)
        {
            int max = mServersWillSort.Count;
            for (int i = 0; i < max; i++)
            {
                target = mServersWillSort[i];
                if (target != default)
                {
                    target.ServerReady();
                }
                else
                {
                    statu = 3;
                }
            }
        }

        private void CacheServer(ref string serverName, ref IServer server, ref int statu)
        {
            serverName = server.ServerName;
            if (mServerMapper.IsContainsKey(serverName))
            {
                statu = 1;
            }
            else
            {
                mServerMapper[serverName] = server;
                server.InitServer();
            }
        }

        private void AfterServersInited()
        {
            IsServersReady = true;
            if (OnFinished != default)
            {
                OnFinished.Invoke();
                OnFinished = default;
            }
        }

        public void AddOnServerFinished(Action method)
        {
            if(IsServersReady)
            {
                method?.Invoke();
            }
            else
            {
                OnFinished += method;
            }
        }

        private int ComparerPriority(IServer n, IServer m)
        {
            if (n.Prioriity > m.Prioriity)
            {
                return -1;
            }
            else if (n.Prioriity < m.Prioriity)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public int GetAliasID(ref string alias)
        {
            return mAliasID != default ? mAliasID.GetID(ref alias) : -1;
        }

        public void CheckAndCacheType(ref Type target, out int id)
        {
            if(mTypeMapper == default)
            {
                id = -1;
            }
            else
            {
                id = mTypeMapper.Add(target, out _);
            }
        }

        public Type GetCachedTypeByID(int id, out int statu)
        {
            return mTypeMapper.Get(id, out statu);
        }

        public void AddResolvableConfig(params IResolvableConfig[] args)
        {
            IResolvableConfig confItem;
            int id, statu;
            int max = args.Length;
            for (int i = 0; i < max; i++)
            {
                confItem = args[i];
                id = mResolvableNameMapper.Add(confItem.Alias, out statu);
                if(statu == 0)
                {
                    if ((confItem != default) && !mResolvableConfigs.IsContainsKey(id))
                    {
                        confItem.Create(this);
                        mResolvableConfigs[id] = confItem;
                    }
                    else
                    {
                        statu = 1;
                    }
                }
                else
                {
                    statu = 2;
                }
                if(statu > 0)
                {
                    confItem.Dispose();
                }
            }
        }

        public IResolvable[] SetResolvable<InterfaceT>(ResolveDelegate<InterfaceT> target, out int statu)
        {
            statu = 0;

            MethodInfo method = target.Method;
            object[] attributes = method.GetCustomAttributes(resolvableAttrType, false);

            IResolvable[] result = default;
            int max = attributes.Length;
            if(max == 0)
            {
                return result;
            }
            int id;
            string alias;
            IResolvableConfig item;
            ResolvableAttribute attribute;
            ResolvableBinder resolvableRef = default;
            result = new IResolvable[max];
            for (int i = 0; i < max; i++)
            {
                attribute = attributes[i] as ResolvableAttribute;
                alias = attribute.Alias;
                id = mResolvableNameMapper.ToID(ref alias);

                if (mResolvableConfigs.IsContainsKey(id))
                {
                    item = mResolvableConfigs[id];
                    if (item != default)
                    {
                        ResolvableInfo.FillResolvableInfo(id, ref item, out ResolvableInfo info);
                        CreateOrAddResolvable(ref info, ref resolvableRef, out statu);

                        if (statu == 0)
                        {
                            statu = CheckAndFillResolvable(ref resolvableRef, out IResolvable resolvable, target);
                            result[i] = resolvable;
                        }
                    }
                }
            }
            return result;
        }

        private void CreateOrAddResolvable(ref ResolvableInfo info, ref ResolvableBinder resolvableRef, out int statu)
        {
            statu = 0;
            if (mBinderMapper.ContainsKey(info.aliasID))
            {
                resolvableRef = mBinderMapper[info.aliasID];
                statu = CheckAndFillToNextRef(ref info, ref resolvableRef, out resolvableRef);
            }
            else
            {
                resolvableRef = ResolvableBinder.CreateResolvableRef(ref info, ref mTypeMapper);
                mBinderMapper[info.aliasID] = resolvableRef;
            }
        }

        private int CheckAndFillToNextRef(ref ResolvableInfo info, ref ResolvableBinder rootRef, out ResolvableBinder resultRef)
        {
            int result = (rootRef == default) ? 1 : 0;
            rootRef.RecursiveAndCheckRef(ref info, true, out ResolvableBinder newRef);
            resultRef = newRef;
            return result;
        }

        private int CheckAndFillResolvable<InterfaceT>(ref ResolvableBinder resolvableRef, out IResolvable resolvable, ResolveDelegate<InterfaceT> defaultResolver)
        {
            int statu = 0;
            int result = (mBinderIDs == default) || (mResolvablesMapper == default) ? 1 : 0;
            int binderID = mBinderIDs.GetID(ref resolvableRef);
            if (mResolvablesMapper.ContainsKey(binderID))
            {
                resolvable = mResolvablesMapper[binderID];
            }
            else
            {
                resolvableRef.SetID(binderID);

                resolvable = new Resolvable();
                resolvable.Binding(ref resolvableRef);
                resolvable.InitResolver<InterfaceT>(this, default);
                resolvable.SetResolver(Resolvable.RESOLVER_INIT, defaultResolver, out statu);

                mResolvablesMapper[binderID] = resolvable;
            }
            return result <= statu ? result : statu;
        }

        public IResolvable GetResolvable(int binderID, out int errorResult)
        {
            errorResult = 0;
            IResolvable result = default;
            if(mResolvablesMapper.ContainsKey(binderID))
            {
                result = mResolvablesMapper[binderID];
            }
            else
            {
                errorResult = 1;
            }
            return result;
        }

        public IResolvable GetResolvable(ref string alias, out int errorResult)
        {
            errorResult = 0;
            ResolvableInfo info;
            IResolvable result = default;
            int id = mResolvableNameMapper.ToID(ref alias);
            if (mResolvableConfigs.IsContainsKey(id))
            {
                IResolvableConfig item = mResolvableConfigs[id];
                if (mBinderMapper.ContainsKey(item.AliasID))
                {
                    ResolvableInfo.FillResolvableInfo(id, ref item, out info);
                    ResolvableBinder resolvableRef = mBinderMapper[info.aliasID];
                    resolvableRef.RecursiveAndCheckRef(ref info, false, out resolvableRef);
                    if (resolvableRef == default)
                    {
                        errorResult = 2;
                    }
                    else
                    {
                        int refID = mBinderIDs.GetID(ref resolvableRef);
                        if (mResolvablesMapper.ContainsKey(refID))
                        {
                            result = mResolvablesMapper[refID];
                        }
                        else
                        {
                            errorResult = 1;
                        }
                    }
                }
            }
            return result;
        }

        public T GetServer<T>(string name) where T : IServer
        {
            return (T)mServerMapper[name];
        }

        public IServer GlobalServer()
        {
            return mGlobalServer;
        }

        public bool IsServersReady { get; private set; }
        public bool IsInited { get; private set; }
        public Action OnFinished { get; private set; }
        public Action OnInit { get; set; }
    }
}

