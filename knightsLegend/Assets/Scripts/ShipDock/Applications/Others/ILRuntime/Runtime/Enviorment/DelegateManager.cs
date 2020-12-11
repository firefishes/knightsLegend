﻿using ILRuntime.CLR.Method;
using ILRuntime.Other;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections.Generic;
using System.Text;

namespace ILRuntime.Runtime.Enviorment
{
    public class DelegateManager
    {
        List<DelegateMapNode> methods = new List<DelegateMapNode>();
        List<DelegateMapNode> functions = new List<DelegateMapNode>();
        IDelegateAdapter zeroParamMethodAdapter = new MethodDelegateAdapter();
        IDelegateAdapter dummyAdapter = new DummyDelegateAdapter();
        Dictionary<Type, Func<Delegate, Delegate>> clrDelegates = new Dictionary<Type, Func<Delegate, Delegate>>(new ByReferenceKeyComparer<Type>());
        Func<Delegate, Delegate> defaultConverter;
        AppDomain appdomain;

        public DelegateManager(AppDomain appdomain)
        {
            this.appdomain = appdomain;
            defaultConverter = DefaultConverterStub;
        }

        static Delegate DefaultConverterStub(Delegate dele)
        {
            return dele;
        }

        public void RegisterDelegateConvertor<T>(Func<Delegate, Delegate> action)
        {
            var type = typeof(T);
            if (type.IsSubclassOf(typeof(Delegate)))
            {
                clrDelegates[type] = action;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private Dictionary<string, string> mGernericsMapper = new Dictionary<string, string>();

        private void CheckGeneraicMapper(DelegateMapNode node)
        {
            var list = node.ParameterTypes;
            int max = list.Length;
            Type type;
            for (int i = 0; i < max; i++)
            {
                type = list[i];
                if (type.Name.Contains("`"))
                {
                    mClsNameTemp = type.Name;
                    mClsNameTemp = mClsNameTemp + "&";
                    //UnityEngine.Debug.Log("asdfas  " + type.Name);
                    //UnityEngine.Debug.Log("put " + type.Name);
                    mGernericsMapper[type.Name] = mClsNameTemp;
                }
            }
        }

        public void RegisterMethodDelegate<T1>()
        {
            DelegateMapNode node = new DelegateMapNode
            {
                Adapter = new MethodDelegateAdapter<T1>(),
                ParameterTypes = new Type[] { typeof(T1) }
            };
            methods.Add(node);
            CheckGeneraicMapper(node);
            RegisterDelegateConvertor<Action<T1>>(defaultConverter);
        }

        public void RegisterMethodDelegate<T1, T2>()
        {
            DelegateMapNode node = new DelegateMapNode
            {
                Adapter = new MethodDelegateAdapter<T1, T2>(),
                ParameterTypes = new Type[] { typeof(T1), typeof(T2) }
            };
            methods.Add(node);
            CheckGeneraicMapper(node);
            RegisterDelegateConvertor<Action<T1, T2>>(defaultConverter);
        }

        public void RegisterMethodDelegate<T1, T2, T3>()
        {
            DelegateMapNode node = new DelegateMapNode
            {
                Adapter = new MethodDelegateAdapter<T1, T2, T3>(),
                ParameterTypes = new Type[] { typeof(T1), typeof(T2), typeof(T3) }
            };
            methods.Add(node);
            CheckGeneraicMapper(node);
            RegisterDelegateConvertor<Action<T1, T2, T3>>(defaultConverter);
        }

        public void RegisterMethodDelegate<T1, T2, T3, T4>()
        {
            DelegateMapNode node = new DelegateMapNode
            {
                Adapter = new MethodDelegateAdapter<T1, T2, T3, T4>(),
                ParameterTypes = new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }
            };
            methods.Add(node);
            CheckGeneraicMapper(node);
            RegisterDelegateConvertor<Action<T1, T2, T3, T4>>(defaultConverter);
        }

        public void RegisterFunctionDelegate<TResult>()
        {
            DelegateMapNode node = new DelegateMapNode
            {
                Adapter = new FunctionDelegateAdapter<TResult>(),
                ParameterTypes = new Type[] { typeof(TResult) }
            };
            functions.Add(node);
            RegisterDelegateConvertor<Func<TResult>>(defaultConverter);
        }

        public void RegisterFunctionDelegate<T1, TResult>()
        {
            DelegateMapNode node = new DelegateMapNode
            {
                Adapter = new FunctionDelegateAdapter<T1, TResult>(),
                ParameterTypes = new Type[] { typeof(T1), typeof(TResult) }
            };
            functions.Add(node);
            RegisterDelegateConvertor<Func<T1, TResult>>(defaultConverter);
        }

        public void RegisterFunctionDelegate<T1, T2, TResult>()
        {
            DelegateMapNode node = new DelegateMapNode();
            node.Adapter = new FunctionDelegateAdapter<T1, T2, TResult>();
            node.ParameterTypes = new Type[] { typeof(T1), typeof(T2), typeof(TResult) };
            functions.Add(node);
            RegisterDelegateConvertor<Func<T1, T2, TResult>>(defaultConverter);
        }

        public void RegisterFunctionDelegate<T1, T2, T3, TResult>()
        {
            DelegateMapNode node = new DelegateMapNode
            {
                Adapter = new FunctionDelegateAdapter<T1, T2, T3, TResult>(),
                ParameterTypes = new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(TResult) }
            };
            functions.Add(node);
            RegisterDelegateConvertor<Func<T1, T2, T3, TResult>>(defaultConverter);
        }

        public void RegisterFunctionDelegate<T1, T2, T3, T4, TResult>()
        {
            DelegateMapNode node = new DelegateMapNode
            {
                Adapter = new FunctionDelegateAdapter<T1, T2, T3, T4, TResult>(),
                ParameterTypes = new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(TResult) }
            };
            functions.Add(node);
            RegisterDelegateConvertor<Func<T1, T2, T3, T4, TResult>>(defaultConverter);
        }

        internal Delegate ConvertToDelegate(Type clrDelegateType, IDelegateAdapter adapter)
        {
            Func<Delegate, Delegate> func;
            if (adapter is DummyDelegateAdapter)
            {
                DelegateAdapter.ThrowAdapterNotFound(adapter.Method);
                return null;
            }
            if (clrDelegates.TryGetValue(clrDelegateType, out func))
            {
                return func(adapter.Delegate);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                string clsName, rName;
                bool isByRef;
                clrDelegateType.GetClassName(out clsName, out rName, out isByRef);
                sb.AppendLine("Cannot find convertor for " + rName);
                sb.AppendLine("Please add following code:");
                sb.Append("appdomain.DelegateManager.RegisterDelegateConvertor<");
                sb.Append(rName);
                sb.AppendLine(">((act) =>");
                sb.AppendLine("{");
                sb.Append("    return new ");
                sb.Append(rName);
                sb.Append("((");
                var mi = clrDelegateType.GetMethod("Invoke");
                bool first = true;
                foreach (var i in mi.GetParameters())
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(", ");
                    }

                    sb.Append(i.Name);
                }
                sb.AppendLine(") =>");
                sb.AppendLine("    {");
                if (mi.ReturnType != appdomain.VoidType.TypeForCLR)
                {
                    sb.Append("        return ((Func<");
                    first = true;
                    foreach (var i in mi.GetParameters())
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            sb.Append(", ");
                        }

                        i.ParameterType.GetClassName(out clsName, out rName, out isByRef);
                        sb.Append(rName);
                    }
                    if (!first)
                    {
                        sb.Append(", ");
                    }

                    mi.ReturnType.GetClassName(out clsName, out rName, out isByRef);
                    sb.Append(rName);
                }
                else
                {
                    sb.Append("        ((Action<");
                    first = true;
                    foreach (var i in mi.GetParameters())
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            sb.Append(", ");
                        }

                        i.ParameterType.GetClassName(out clsName, out rName, out isByRef);
                        sb.Append(rName);
                    }
                }
                sb.Append(">)act)(");
                first = true;
                foreach (var i in mi.GetParameters())
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(", ");
                    }

                    sb.Append(i.Name);
                }
                sb.AppendLine(");");
                sb.AppendLine("    });");
                sb.AppendLine("});");
                throw new KeyNotFoundException(sb.ToString());
            }
        }

        private string mClsNameTemp;

        internal IDelegateAdapter FindDelegateAdapter(ILTypeInstance instance, ILMethod method)
        {
            IDelegateAdapter res;
            if (method.ReturnType == appdomain.VoidType)
            {
                if (method.ParameterCount == 0)
                {
                    res = zeroParamMethodAdapter.Instantiate(appdomain, instance, method);
                    if (instance != null)
                    {
                        instance.SetDelegateAdapter(method, res);
                    }

                    return res;
                }
                foreach (var i in methods)
                {
                    if (i.ParameterTypes.Length == method.ParameterCount)
                    {
                        bool match = true;
                        for (int j = 0; j < method.ParameterCount; j++)
                        {
                            if (i.ParameterTypes[j] != method.Parameters[j].TypeForCLR)
                            {
                                //UnityEngine.Debug.Log(i.ParameterTypes[j].Name + "=====" + method.Parameters[j].TypeForCLR.Name);
                                if (mGernericsMapper.ContainsKey(i.ParameterTypes[j].Name))
                                {
                                    mClsNameTemp = mGernericsMapper[i.ParameterTypes[j].Name];
                                    if (mClsNameTemp == method.Parameters[j].TypeForCLR.Name)
                                    {
                                        UnityEngine.Debug.Log("Match by another way!!!");
                                        match = true;
                                        break;
                                    }
                                    else
                                    {
                                        UnityEngine.Debug.Log(mClsNameTemp + " == " + method.Parameters[j].TypeForCLR.Name);
                                    }
                                    //UnityEngine.Debug.Log(mClsNameTemp + "=====" + method.Parameters[j].TypeForCLR.Name);
                                }
                                match = false;
                                break;
                            }
                        }
                        if (match)
                        {
                            res = i.Adapter.Instantiate(appdomain, instance, method);
                            if (instance != null)
                            {
                                instance.SetDelegateAdapter(method, res);
                            }

                            return res;
                        }
                    }
                }
            }
            else
            {
                foreach (var i in functions)
                {
                    if (i.ParameterTypes.Length == method.ParameterCount + 1)
                    {
                        bool match = true;
                        for (int j = 0; j < method.ParameterCount; j++)
                        {
                            if (i.ParameterTypes[j] != method.Parameters[j].TypeForCLR)
                            {
                                match = false;
                                break;
                            }
                        }
                        if (match)
                        {
                            if (method.ReturnType.TypeForCLR == i.ParameterTypes[method.ParameterCount])
                            {
                                res = i.Adapter.Instantiate(appdomain, instance, method);
                                if (instance != null)
                                {
                                    instance.SetDelegateAdapter(method, res);
                                }

                                return res;
                            }
                        }
                    }
                }
            }

            res = dummyAdapter.Instantiate(appdomain, instance, method);
            if (instance != null)
            {
                instance.SetDelegateAdapter(method, res);
            }

            return res;
        }

        class DelegateMapNode
        {
            public IDelegateAdapter Adapter { get; set; }
            public Type[] ParameterTypes { get; set; }
        }
    }
}
