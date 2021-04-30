using System;
using UnityEngine;

namespace ShipDock.Datas
{
    public class ClientData<DeviceT, ClientT> where DeviceT : DeviceLocalInfo, new() where ClientT : ClientLocalInfo, new()
    {
        public const string DEVICE_INFO = "DeviceInfo";
        public const string LAST_ACCOUNT_ID = "LastAccountID";
        //public const string ACCOUNT_ID = "AccountID";
        public const string PLAYER_INFO = "PlayerInfo";

        public bool IsInited { get; private set; }
        public DeviceT DeviceInfo { get; private set; }
        public ClientT ClientInfo { get; private set; }

        public ClientData() { }

        public void Init()
        {
            if (IsInited)
            {
                return;
            }
            else { }

            IsInited = true;

            InitClientInfo();
            InitDeviceInfo();
        }

        private void InitDeviceInfo()
        {
            string deviceInfoKey = GetDeviceInfoKey();
            string infoRaw = GetLocalStringData(deviceInfoKey);
            if (string.IsNullOrEmpty(infoRaw))
            {
                DeviceInfo = new DeviceT();
            }
            else
            {
                DeviceInfo = JsonUtility.FromJson<DeviceT>(infoRaw);
                Debug.Log("ClientInfo " + DeviceInfo);
                Debug.Log(string.Format("Last device info init success, account id is {0}", infoRaw));
            }
        }

        private void InitClientInfo()
        {
            string lastAccountID = GetLocalStringData(LAST_ACCOUNT_ID);
            if (string.IsNullOrEmpty(lastAccountID))
            {
                ClientInfo = new ClientT();
                Debug.Log("New client info init success");
            }
            else
            {
                InitClientInfoFromLast(ref lastAccountID);
                Debug.Log(string.Format("Last client info init success, account id is {0}", lastAccountID));
            }
        }

        private void InitClientInfoFromLast(ref string lastAccountID)
        {
            UpdateLocalData(LAST_ACCOUNT_ID, lastAccountID);

            if (ClientInfo != default)
            {
                if (ClientInfo.account_id != lastAccountID)
                {
                    ClientInfo.CheckInfoPatch();

                    string oldInfoKey = GetClientInfoKey();
                    string infoRaw = JsonUtility.ToJson(ClientInfo);
                    UpdateLocalData(oldInfoKey, infoRaw);
                    ClientInfo = default;

                    GetClientInfoByAccountID(ref lastAccountID);
                }
                else { }
            }
            else
            {
                GetClientInfoByAccountID(ref lastAccountID);
            }
        }

        private void GetClientInfoByAccountID(ref string lastAccountID)
        {
            string infoKey = PLAYER_INFO.Append(lastAccountID);
            string infoRaw = GetLocalStringData(infoKey);

            ClientInfo = JsonUtility.FromJson<ClientT>(infoRaw);
            Debug.Log("ClientInfo " + ClientInfo);
            Debug.Log(string.Format("Last client info init success, account id is {0}", infoRaw));

            if (ClientInfo == default)
            {
                ClientInfo = new ClientT()
                {
                    account_id = lastAccountID,
                };
                ClientInfo.CheckInfoPatch();
            }
            else { }
        }

        /// <summary>
        /// 更新本地字符串数据
        /// </summary>
        public void UpdateLocalData(string keyName, string data)
        {
            if (!string.IsNullOrEmpty(keyName))
            {
                //if (HasKey(keyName) && (data == PlayerPrefs.GetString(keyName)))
                //{
                //    Debug.Log("Update local info, key is null");
                //    return;
                //}
                //else { }

                PlayerPrefs.SetString(keyName, data);

                Debug.Log(string.Format("Update local info.. {0} >> {1}", keyName, data));
            }
            else
            {
                Debug.Log("Update local info, key is null");
            }
        }

        /// <summary>
        /// 获取本地字符串类型的数据
        /// </summary>
        public string GetLocalStringData(string keyName)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(keyName) && HasKey(keyName))
            {
                result = PlayerPrefs.GetString(keyName);
            }
            else { }

            return result;
        }

        public void DelLocalStringData(string keyName)
        {
            PlayerPrefs.DeleteKey(keyName);
        }

        /// <summary>
        /// 本地是否存在该字段
        /// </summary>
        public bool HasKey(string keyName)
        {
            return PlayerPrefs.HasKey(keyName);
        }

        public void FlushInfos()
        {
            FlushDeviceInfo();
            FlushClientInfo();
        }

        public void FlushDeviceInfo()
        {
            string json = JsonUtility.ToJson(DeviceInfo);
            string deviceInfoKey = GetDeviceInfoKey();
            UpdateLocalData(deviceInfoKey, json);
        }

        public void FlushClientInfo()
        {
            string json = JsonUtility.ToJson(ClientInfo);
            string clientInfoKey = GetClientInfoKey();
            UpdateLocalData(clientInfoKey, json);
            UpdateLocalData(LAST_ACCOUNT_ID, ClientInfo.account_id);
        }

        public void DeleteClientInfo()
        {
            string clientInfoKey = GetClientInfoKey();
            DelLocalStringData(clientInfoKey);
            UpdateLocalData(LAST_ACCOUNT_ID, string.Empty);
        }

        public void CreateNewClient()
        {
            FlushInfos();
            DelLocalStringData(LAST_ACCOUNT_ID);
            InitClientInfo();
        }

        private string GetDeviceInfoKey()
        {
            return DEVICE_INFO.Append(ClientInfo.account_id);
        }

        private string GetClientInfoKey()
        {
            return PLAYER_INFO.Append(ClientInfo.account_id);
        }
    }
}
