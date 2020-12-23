using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Datas
{
    /// <summary>
    /// 
    /// 本地化多语言管理器
    /// 
    /// </summary>
    public class Locals
    {
        public AssetBundle languageAssetBundle;

        private int mPairKeyIndex;
        private int mPairValueIndex;
        private char mPairSpliter = '=';
        private string mKey;
        private string mValue;
        private string mTail;
        private string[] mKeyPair;
        private Dictionary<string, string> mLanguage;

        public Locals()
        {
            mPairKeyIndex = 0;
            mPairValueIndex = 1;
        }

        public void Dispose()
        {
            Local = string.Empty;
            if (mLanguage != null)
            {
                mLanguage.Clear();
                mLanguage = null;
            }
        }
        
        public void SetLocal(string localKey = "")
        {
            if (!string.IsNullOrEmpty(localKey))
            {
                SetLocalName(localKey);
            }
            string fileName = "local_".Append(Local, ".txt");
            if (languageAssetBundle != default)
            {
                TextAsset data = languageAssetBundle.LoadAsset<TextAsset>(fileName);
                string localsData = data.text;
                FillLanguagesData(ref localsData, AddLocalsLanguageData);
            }
        }

        public void SetLocalName(string localKey)
        {
            Local = localKey;
        }

        public void SetLocal<K>(Dictionary<K, string> data, string localKey = "")
        {
            if (!string.IsNullOrEmpty(localKey))
            {
                SetLocalName(localKey);
            }
            if (mLanguage == null)
            {
                mLanguage = new Dictionary<string, string>();
            }

            if (data != default)
            {
                FillLanguagesData(ref data, AddLocalsLanguageData);
            }
        }

        public string Language(string id, params string[] formats)
        {
            if (mLanguage == null)
            {
                if (mLanguage == null)
                {
                    mLanguage = new Dictionary<string, string>();
                }

                TextAsset asset = Resources.Load<TextAsset>("local_default");//如果本地化数据未初始化则先从默认的本地化文本获取数据
                if (asset != default)
                {
                    string data = asset.text;
                    FillLanguagesData(ref data, AddDefaultLanguageData);
                }
            }

            string result = IsContainsLanguageID(ref id) ? mLanguage[id] : id;

            int max = formats.Length;
            if ((result != id) && (max > 0))
            {
                result = string.Format(result, formats);
            }
            return result;
        }

        private void FillLanguagesData(ref string data, Action onAddLanguageData)
        {
            mTail = string.IsNullOrEmpty(Local) ? string.Empty : "_".Append(Local);

            string[] languagesData = data.Split('\n');
            int max = languagesData.Length;
            for (int i = 0; i < max; i++)
            {
                mKeyPair = languagesData[i].Split(mPairSpliter);

                if (IsInvalidPair(mKeyPair.Length))
                {
                    continue;
                }

                mKey = mKeyPair[mPairKeyIndex].Trim();
                mValue = mKeyPair[mPairValueIndex].Trim();
                onAddLanguageData?.Invoke();
            }
        }

        private void FillLanguagesData<K>(ref Dictionary<K, string> data, Action onAddLanguageData)
        {
            mTail = string.IsNullOrEmpty(Local) ? string.Empty : "_".Append(Local);

            int max = data.Count;
            Dictionary<K, string>.Enumerator enumerator = data.GetEnumerator();
            for (int i = 0; i < max; i++)
            {
                mKey = enumerator.Current.Key.ToString();
                mValue = enumerator.Current.Value;
                onAddLanguageData?.Invoke();
                enumerator.MoveNext();
            }
        }

        private void AddLocalsLanguageData()
        {
            if (!string.IsNullOrEmpty(mKey))
            {
                mLanguage[mKey] = mValue;
            }
        }

        private void AddDefaultLanguageData()
        {
            if (IsTailWithLocalSign(ref mKey, ref mTail))
            {
                mKey = mKey.Substring(0, mKey.Length - mTail.Length);
                mLanguage[mKey] = mValue;
            }
        }

        private bool IsInvalidPair(int len)
        {
            return len < 2;
        }

        private bool IsTailWithLocalSign(ref string key, ref string tail)
        {
            if (string.IsNullOrEmpty(tail))
            {
                return true;
            }
            return key.IndexOf(tail, StringComparison.Ordinal) != -1;
        }

        private bool IsContainsLanguageID(ref string id)
        {
            return mLanguage != null && mLanguage.ContainsKey(id);
        }
        
        public string Local { get; private set; }
    }

}