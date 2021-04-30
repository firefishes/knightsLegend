using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace ShipDock.Editors
{
    public class ABAssetCreater
    {
        private List<string> mNameNode;

        public ABAssetCreater(string nameSource)
        {
            mNameNode = new List<string>();

            ABNameSourse = nameSource;
        }

        public string GetABName()
        {
            string node;
            string[] splited = ABNameSourse?.Split(StringUtils.PATH_SYMBOL_CHAR);
            int max = splited.Length;
            for (int i = 0; i < max; i++)
            {
                node = splited[i];
                if (node.IndexOf(StringUtils.DOT, StringComparison.Ordinal) >= 0)
                {
                    return mNameNode.Joins(StringUtils.PATH_SYMBOL);
                }

                if (i <= 1)
                {
                    mNameNode.Add(node);
                }
                else
                {
                    return mNameNode.Joins(StringUtils.PATH_SYMBOL);
                }
            }
            return string.Empty;
        }

        public AssetImporter Importer { get; set; }
        public string ABNameSourse { get; private set; }
    }
}