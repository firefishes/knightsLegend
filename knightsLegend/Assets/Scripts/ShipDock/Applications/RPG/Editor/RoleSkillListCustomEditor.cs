using ShipDock.Tools;
using UnityEditor;
using UnityEngine;

namespace ShipDock.Applications
{

    [CustomEditor(typeof(RoleSkillList))]
    public class RoleSkillListCustomEditor : Editor
    {
        private string mRaw;
        private int mMode;
        private JSONObject mJSON;
        private JSONObject mSInfos;
        private JSONObject mMInfos;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            RoleSkillList info = (RoleSkillList)target;
            SkillInfo[] skillInfos = info.skills.infos;

            if(info.raw != default)
            {
                if (string.IsNullOrEmpty(mRaw))
                {
                    mRaw = System.Text.Encoding.UTF8.GetString(info.raw.bytes);
                }
                else
                {
                    if (GUILayout.Button("TO JSON"))
                    {
                        mMode = 1;
                        mJSON = JSONObject.Create(JSONObject.Type.OBJECT);
                        mJSON.AddField("skills", JSONObject.Create(JSONObject.Type.OBJECT));
                        mJSON.AddField("skillMotions", JSONObject.Create(JSONObject.Type.OBJECT));

                        JSONObject skills = mJSON.GetField("skills");
                        JSONObject motions = mJSON.GetField("skillMotions");

                        skills.AddField("infos", JSONObject.Create(JSONObject.Type.ARRAY));
                        motions.AddField("infos", JSONObject.Create(JSONObject.Type.ARRAY));

                        mSInfos = skills.GetField("infos");
                        mMInfos = motions.GetField("infos");
                    }
                    if (GUILayout.Button("FROM JSON"))
                    {
                        mMode = 2;
                        mJSON = JSONObject.Create(mRaw);

                        JSONObject skills = mJSON["skills"];
                        JSONObject motions = mJSON["skillMotions"];

                        mSInfos = skills["infos"];
                        mMInfos = motions["infos"];
                    }
                    if (GUILayout.Button("CLEAR"))
                    {
                        mRaw = string.Empty;
                        info.raw = default;
                    }
                }
            }
            else
            {
                mMode = 0;
                if (GUILayout.Button("RESET ALL"))
                {
                    Utils.Reclaim(ref info.skills.infos, false, true);
                    Utils.Reclaim(ref info.skillMotions.infos, false, true);

                    info.skills.infos = new SkillInfo[0];
                    info.skillMotions.infos = new MotionSceneInfo[0];
                }
            }
            CheckSkillInfos(ref info, ref skillInfos);
            CheckMotions(ref info, ref skillInfos);
            
            mMode = 0;

        }

        private void CheckSkillInfos(ref RoleSkillList info, ref SkillInfo[] skillInfos)
        {
            ValueSubgroup vs;
            JSONObject item = default;
            if(mSInfos == default)
            {
                mMode = 0;
            }
            switch (mMode)
            {
                case 2:
                    info.skills.infos = new SkillInfo[mSInfos.list.Count];
                    skillInfos = info.skills.infos;
                    break;
            }
            int max = skillInfos.Length;
            for (int i = 0; i < max; i++)
            {
                if(skillInfos[i] != default)
                {
                    EditorGUILayout.HelpBox("技能 ".Append(skillInfos[i].name.ToString(), " -- ", skillInfos[i].skillName), MessageType.Info);
                }

                switch (mMode)
                {
                    case 1:
                        item = JSONObject.Create(JSONObject.Type.OBJECT);
                        item.AddField("name", skillInfos[i].name);
                        item.AddField("skillName", skillInfos[i].skillName);
                        item.AddField("skillParams", JSONObject.Create(JSONObject.Type.ARRAY));
                        break;
                    case 2:
                        skillInfos[i] = new SkillInfo();
                        item = mSInfos[i];
                        skillInfos[i].name = item.GetIntValue("name");
                        skillInfos[i].skillName = item.GetStringValue("skillName");
                        break;
                }
                switch (mMode)
                {
                    case 2:
                        item = mSInfos[i]["skillParams"];
                        skillInfos[i].skillParams = new ValueSubgroup[item.list.Count];
                        break;
                }
                int n = skillInfos != default && 
                        skillInfos.Length > 0 && 
                        skillInfos[i].skillParams != default ? 
                            skillInfos[i].skillParams.Length : 0;
                for (int j = 0; j < n; j++)
                {
                    var v = skillInfos[i];
                    switch (mMode)
                    {
                        case 2:
                            v.skillParams[j] = new ValueSubgroup();
                            break;
                    }
                    vs = v.skillParams[j];
                    vs.valueTypeInEditor = (ValueItemType)EditorGUILayout.EnumPopup(vs.valueTypeInEditor);
                    vs.valueType = (int)vs.valueTypeInEditor;
                    vs.valueInEditor = vs.valueInEditor == default ? string.Empty : vs.valueInEditor;

                    switch (mMode)
                    {
                        case 1:
                            JSONObject s = JSONObject.Create(JSONObject.Type.OBJECT);
                            s.AddField("valueTypeInEditor", vs.valueType);
                            s.AddField("valueInEditor", vs.valueInEditor);
                            s.AddField("keyField", vs.keyField);
                            s.AddField("valueType", vs.valueType);
                            s.AddField("str", vs.str);
                            s.AddField("floatValue", vs.floatValue);
                            s.AddField("triggerValue", vs.triggerValue);
                            item["skillParams"].Add(s);
                            break;
                        case 2:
                            JSONObject ssi = item[j];
                            vs.valueTypeInEditor = (ValueItemType)ssi.GetIntValue("valueTypeInEditor");
                            vs.valueInEditor = ssi.GetStringValue("valueInEditor");
                            vs.keyField = ssi.GetStringValue("keyField");
                            vs.valueType = ssi.GetIntValue("valueType");
                            vs.str = ssi.GetStringValue("str");
                            vs.floatValue = ssi.GetFloatValue("floatValue");
                            vs.triggerValue = ssi.GetBoolValue("triggerValue");
                            skillInfos[i].skillParams[j] = vs;
                            break;
                    }

                    switch (vs.valueTypeInEditor)
                    {
                        case ValueItemType.STRING:
                            vs.str = vs.valueInEditor;
                            break;
                        case ValueItemType.INT:
                        case ValueItemType.FLOAT:
                            vs.floatValue = float.Parse(string.IsNullOrEmpty(vs.valueInEditor) ? "0" : vs.valueInEditor);
                            break;
                        case ValueItemType.DOUBLE:
                            vs.doubleValue = double.Parse(string.IsNullOrEmpty(vs.valueInEditor) ? "0" : vs.valueInEditor);
                            break;
                    }

                    EditorGUILayout.TextField("参数名 ".Append(vs.keyField));
                    if (vs.valueType == ValueItem.BOOL)
                    {
                        vs.triggerValue = EditorGUILayout.Toggle("布尔值", vs.triggerValue);
                    }
                    else
                    {
                        EditorGUILayout.TextField("其他值", vs.valueInEditor);
                    }
                    v.skillParams[j] = vs;
                    skillInfos[i] = v;
                }

                switch (mMode)
                {
                    case 1:
                        mSInfos.Add(item);
                        break;
                }
                info.skills.infos = skillInfos;
            }
        }

        private void CheckMotions(ref RoleSkillList info, ref SkillInfo[] skillInfos)
        {
            switch (mMode)
            {
                case 2:
                    info.skillMotions.infos = new MotionSceneInfo[mMInfos.list.Count];
                    break;
            }
            JSONObject item = default;
            MotionSceneInfo[] motionSceneInfos = info.skillMotions.infos;
            int max = motionSceneInfos.Length;
            for (int i = 0; i < max; i++)
            {
                var motionInfo = motionSceneInfos[i];
                if (mMode == 0)
                {
                    EditorGUILayout.HelpBox("技能动画 ".Append(motionInfo.ID.ToString(), "--", motionInfo.skillName), MessageType.Info);
                    motionInfo.ID = EditorGUILayout.IntField("动画ID ", motionInfo.ID);
                    motionInfo.isCombo = EditorGUILayout.Toggle("是否连续动画", motionInfo.isCombo);
                }

                switch (mMode)
                {
                    case 1:
                        item = JSONObject.Create(JSONObject.Type.OBJECT);
                        item.AddField("ID", motionInfo.ID);
                        item.AddField("skillName", skillInfos[i].skillName);
                        item.AddField("isCombo", motionInfo.isCombo);
                        item.AddField("indexsForID", JSONObject.Create(JSONObject.Type.ARRAY));
                        item.AddField("checkComboTime", motionInfo.checkComboTime);
                        break;
                    case 2:
                        item = mMInfos[i];
                        motionSceneInfos[i] = new MotionSceneInfo();
                        motionInfo = motionSceneInfos[i];
                        motionInfo.ID = item.GetIntValue("ID");
                        motionInfo.skillName = item.GetStringValue("skillName");
                        motionInfo.isCombo = item.GetBoolValue("isCombo");
                        motionInfo.checkComboTime = item.GetFloatValue("checkComboTime");
                        motionInfo.indexsForID = new int[mMInfos[i].GetField("indexsForID").Count];
                        break;
                }
                if (motionInfo.isCombo)
                {
                    int n = motionInfo.indexsForID.Length;
                    for (int j = 0; j < n; j++)
                    {
                        if(mMode == 0)
                        {
                            if (motionInfo.indexsForID[j] < skillInfos.Length)
                            {
                                EditorGUILayout.TextField("技能信息关联 ".Append(j.ToString()), skillInfos[motionInfo.indexsForID[j]].skillName);
                            }
                            motionInfo.indexsForID[j] = EditorGUILayout.IntField("技能信息索引：", motionInfo.indexsForID[j]);
                        }

                        switch (mMode)
                        {
                            case 1:
                                item["indexsForID"].Add(motionInfo.indexsForID[j]);
                                break;
                            case 2:
                                item = mMInfos[i].GetField("indexsForID");
                                motionInfo.indexsForID[j] = (int)(item[j].n);
                                break;
                        }
                    }
                    motionInfo.checkComboTime = EditorGUILayout.FloatField("连续动画检测时间间隔：", motionInfo.checkComboTime);
                }
                else
                {
                    if (mMode == 0)
                    {
                        if (motionInfo.indexsForID[0] < skillInfos.Length)
                        {
                            EditorGUILayout.TextField("技能信息关联", skillInfos[motionInfo.indexsForID[0]].skillName);
                        }
                        motionInfo.indexsForID[0] = EditorGUILayout.IntField("技能信息索引：", motionInfo.indexsForID[0]);
                    }

                    switch (mMode)
                    {
                        case 1:
                            item["indexsForID"].Add(motionInfo.indexsForID[0]);
                            break;
                        case 2:
                            item = mMInfos[i]["indexsForID"];
                            motionInfo.indexsForID[0] = (int)item[0].n;
                            break;
                    }
                }
                switch (mMode)
                {
                    case 1:
                        mMInfos.Add(item);
                        break;
                }
            }
        }
    }

}