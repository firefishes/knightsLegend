using ShipDock.Applications;
using ShipDock.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ShipDock.Editors
{
    public class ResDataVersionEditorCreater
    {
        public bool isUpdateVersion;
        public bool isSyncClientVersions;
        public bool isUpdateResVersion;
        public bool isSyncAppVersion;
        public string tempOutputPath;
        /// <summary>即将创建资源包的名称列表</summary>
        public List<string> ABNamesWillBuild;
        /// <summary>远程资源服务器网关</summary>
        public string resRemoteGateWay;

        /// <summary>
        /// 创建资源版本
        /// </summary>
        /// <param name="abNames">用于创建资源包的名称列表</param>
        /// <param name="resGateway">远程资源服务器网关</param>
        /// <param name="isIgnoreRemote">是否忽略基于线上的版本创建新的版本</param>
        public void CreateResDataVersion(bool isIgnoreRemote = false)
        {
            if (!isIgnoreRemote && string.IsNullOrEmpty(resRemoteGateWay))
            {
                Debug.LogError("Remote gateway do not allow empty when non neglect remote versions.");
                return;
            }
            else
            {
                if (isIgnoreRemote)
                {
                    OnGetRemoteVersion(false, default);
                }
                else
                {
                    Loader.Loader loader = new Loader.Loader();
                    loader.CompleteEvent.AddListener(OnGetRemoteVersion);
                    loader.Load(resRemoteGateWay.Append(ResDataVersion.FILE_RES_DATA_VERSIONS_NAME));
                }
            }
        }

        private void OnGetRemoteVersion(bool flag, Loader.Loader ld)
        {
            ResDataVersion remoteVers = default;
            if (flag)
            {
                string data = ld.TextData;
                remoteVers = JsonUtility.FromJson<ResDataVersion>(data);

                if (remoteVers == default)
                {
                    Debug.Log("Do not exists remote versions.");
                }
                ld.Dispose();
            }
            else
            {
                Debug.Log("Do not get remote versions.");
            }
            BuildVersionConfig(ref remoteVers);
        }

        /// <summary>
        /// 创建资源版本
        /// </summary>
        /// <param name="remoteVers"></param>
        private void BuildVersionConfig(ref ResDataVersion remoteVers)
        {
            string versions;// = FileOperater.ReadUTF8Text(AppPaths.ABBuildOutputRoot.Append(ResDataVersion.FILE_RES_DATA_VERSIONS_NAME));
            ResDataVersion resDataVersion;// = JsonUtility.FromJson<ResDataVersion>(versions);
            
            if (remoteVers == default)
            {
                versions = FileOperater.ReadUTF8Text(AppPaths.ABBuildOutputRoot.Append(ResDataVersion.FILE_RES_DATA_VERSIONS_NAME));
                resDataVersion = JsonUtility.FromJson<ResDataVersion>(versions);
                if (resDataVersion == default)
                {
                    resDataVersion = new ResDataVersion
                    {
                        app_version = Application.version,
                        res_version = remoteVers != default ? remoteVers.res_version : 0,//根据是否存在线上版本同步资源号
                    };
                }
            }
            else
            {
                resDataVersion = new ResDataVersion();
                resDataVersion.CloneVersionsFrom(ref remoteVers);
            }

            RemoveInvalidABName();

            string[] abNamesValue = ABNamesWillBuild != default ? ABNamesWillBuild.ToArray() : new string[0];
            resDataVersion.CreateNewResVersion(ref resRemoteGateWay, isUpdateVersion, isUpdateResVersion, isSyncAppVersion, ref remoteVers, ref abNamesValue);
            resDataVersion.Refresh();

            if (isSyncClientVersions)
            {
                List<ScriptableObject> list = default;
                ShipDockEditorUtils.FindAssetInEditorProject(ref list, "t:ScriptableObject", @"Assets\Prefabs");
                ClientResVersion clientRes = (ClientResVersion)list[0];
                clientRes.Versions.CloneVersionsFrom(ref resDataVersion);
                clientRes.SetChanges(resDataVersion.ResChanges);
            }

            versions = JsonUtility.ToJson(resDataVersion);
            //FileOperater.WriteBytes(versions, AppPaths.ABBuildOutput);//临时目录里的正式文件
            FileOperater.WriteUTF8Text(versions, AppPaths.ABBuildOutputRoot.Append(ResDataVersion.FILE_RES_DATA_VERSIONS_NAME.Append(".json~")));//仅用于查看
            FileOperater.WriteBytes(versions, AppPaths.ABBuildOutputRoot.Append(ResDataVersion.FILE_RES_DATA_VERSIONS_NAME));//位于资源主目录里的正式文件

            resDataVersion.Clean();
            remoteVers?.Clean();
        }

        private void RemoveInvalidABName()
        {
            string path;
            List<string> invalids = new List<string>();
            int max = ABNamesWillBuild != default ? ABNamesWillBuild.Count : 0;
            for (int i = 0; i < max; i++)
            {
                path = AppPaths.ABBuildOutputRoot.Append(ABNamesWillBuild[i]);
                if (File.Exists(path))
                {
                    invalids.Add(path);
                    Debug.Log("Version will build : " + path);
                }
            }
            max = invalids.Count;
            for (int i = 0; i < max; i++)
            {
                ABNamesWillBuild.Remove(invalids[i]);
            }
            invalids.Clear();
        }
    }
}