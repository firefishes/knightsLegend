using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using ShipDock.Tools;

namespace ShipDock.Applications
{
    [RequireComponent(typeof(ScrollRect))]
    public class LoopScrollView : MonoBehaviour
    {
        private const int CACHE_COUNT = 4;
        private const int INVALIID_START_INDEX = -1;

        [SerializeField]
        private Rect m_ContentAnchorH = new Rect(0f, 0f, 0f, 1f);
        [SerializeField]
        private Rect m_ContentAnchorV = new Rect(0.5f, 1f, 0.5f, 1f);
        [SerializeField]
        private Vector2 m_ItemPivot = new Vector2(0.5f, 1f);
        [SerializeField]
        private Vector2 m_ItemAnchorMin = new Vector2(0.5f, 1f);
        [SerializeField]
        private Vector2 m_ItemAnchorMax = new Vector2(0.5f, 1f);

        private int mMaxCount;
        private int mDataCount;
        private int mCreateCount;
        private int mStartIndex;
        private float mItemX;
        private float mItemY;
        private float mContentH;
        private float mContentW;
        private float mCellPadding;
        private Vector2 mCellSize;
        private Vector2 mScrollRectSize;
        private ScrollRect mScrollRect;
        private RectTransform mContentRectTrans;
        private LoopScrollItem mUpdateItem;
        private LoopScrollItem mPrefabItem;
        private LoopScrollItem mChangingItem;
        private List<LoopScrollItem> mItemList;
        private Queue<LoopScrollItem> mItemQueue;
        private Dictionary<LoopScrollItem, int> mItemIndexDic;
        private Action<LoopScrollItem, int> mUpdateCell;
        private List<ILoopScrollItemData> mItemData;

        private void OnDestroy()
        {
            mScrollRect = default;
            mContentRectTrans = default;
            mUpdateCell = default;
            mUpdateItem = default;
            mPrefabItem = default;
            mChangingItem = default;

            Utils.Reclaim(ref mItemData);
            Utils.Reclaim(ref mItemList);
            Utils.Reclaim(ref mItemQueue);
            Utils.Reclaim(ref mItemIndexDic);
        }

        public void InitView(ref List<ILoopScrollItemData> infos, LoopScrollItem itemRenderer, float padding = 0f, Action<LoopScrollItem, int> onUpdateCell = default)
        {
            mItemData = infos;
            mUpdateCell += UpdateItemInfo;
            if (onUpdateCell != default)
            {
                mUpdateCell += onUpdateCell;
            }
            Show(infos.Count, itemRenderer, padding);
        }

        private void UpdateItemInfo(LoopScrollItem item, int index)
        {
            ILoopScrollItemData info = mItemData[index];
            info.FillInfoToItem(ref item);
        }

        private void Show(int dataCount, LoopScrollItem item, float padding = 0f)
        {
            mDataCount = dataCount;
            mPrefabItem = item;
            mCellPadding = padding;

            mItemList = new List<LoopScrollItem>();
            mItemQueue = new Queue<LoopScrollItem>();
            mItemIndexDic = new Dictionary<LoopScrollItem, int>();
            mScrollRect = GetComponent<ScrollRect>();
            mContentRectTrans = mScrollRect.content;
            mScrollRectSize = mScrollRect.GetComponent<RectTransform>().sizeDelta;
            mCellSize = item.GetComponent<RectTransform>().sizeDelta;

            mStartIndex = 0;
            mCreateCount = 0;
            mMaxCount = GetMaxCount();

            if (mScrollRect.horizontal)
            {
                mContentRectTrans.anchorMin = new Vector2(m_ContentAnchorH.x, m_ContentAnchorH.y);
                mContentRectTrans.anchorMax = new Vector2(m_ContentAnchorH.width, m_ContentAnchorH.height);
            }
            else
            {
                mContentRectTrans.anchorMin = new Vector2(m_ContentAnchorV.x, m_ContentAnchorV.y);
                mContentRectTrans.anchorMax = new Vector2(m_ContentAnchorV.width, m_ContentAnchorV.height);
            }
            mScrollRect.onValueChanged.RemoveAllListeners();
            mScrollRect.onValueChanged.AddListener(OnValueChanged);
            ResetSize(dataCount);
        }

        public void ResetSize(int dataCount)
        {
            mDataCount = dataCount;
            mContentRectTrans.sizeDelta = GetContentSize();

            LoopScrollItem item;
            for (int i = mItemList.Count - 1; i >= 0; i--)
            {
                item = mItemList[i];
                RecoverItem(item);
            }
            
            mCreateCount = Mathf.Min(dataCount, mMaxCount);
            for (int i = 0; i < mCreateCount; i++)
            {
                CreateItem(i);
            }

            mStartIndex = -1;
            mContentRectTrans.anchoredPosition = Vector3.zero;
            OnValueChanged(Vector2.zero);
        }

        public void UpdateList()
        {
            for (int i = 0; i < mItemList.Count; i++)
            {
                mUpdateItem = mItemList[i];
                int index = mItemIndexDic[mUpdateItem];
                mUpdateCell(mUpdateItem, index);
            }
        }

        private void CreateItem(int index)
        {
            LoopScrollItem item;
            if (mItemQueue.Count > 0)
            {
                item = mItemQueue.Dequeue();
                mItemIndexDic[item] = index;
                item.gameObject.SetActive(true);
            }
            else
            {
                item = Instantiate(mPrefabItem);
                mItemIndexDic.Add(item, index);
                item.transform.SetParent(mContentRectTrans.transform);

                RectTransform rect = item.GetComponent<RectTransform>();
                rect.pivot = m_ItemPivot;
                rect.anchorMin = m_ItemAnchorMin;
                rect.anchorMax = m_ItemAnchorMax;
            }
            mItemList.Add(item);
            item.transform.localPosition = GetPosition(index);
            mUpdateCell(item, index);
        }

        private void RecoverItem(LoopScrollItem item)
        {
            item.gameObject.SetActive(false);
            mItemList.Remove(item);
            mItemQueue.Enqueue(item);
            mItemIndexDic[item] = INVALIID_START_INDEX;
        }

        private void OnValueChanged(Vector2 vec)
        {
            int curmStartIndex = GetmStartIndex();
            if ((mStartIndex != curmStartIndex) && (curmStartIndex > INVALIID_START_INDEX))
            {
                mStartIndex = curmStartIndex;

                for (int i = mItemList.Count - 1; i >= 0; i--)
                {
                    mChangingItem = mItemList[i];
                    int index = mItemIndexDic[mChangingItem];
                    if (index < mStartIndex || index > (mStartIndex + mCreateCount - 1))
                    {
                        RecoverItem(mChangingItem);
                    }
                }

                for (int i = mStartIndex; i < mStartIndex + mCreateCount; i++)
                {
                    if (i >= mDataCount)
                    {
                        break;
                    }

                    bool isExist = false;
                    for (int j = 0; j < mItemList.Count; j++)
                    {
                        mChangingItem = mItemList[j];
                        int index = mItemIndexDic[mChangingItem];
                        if (index == i)
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if (isExist)
                    {
                        continue;
                    }

                    CreateItem(i);
                }
            }
        }

        private int GetMaxCount()
        {
            if (mScrollRect.horizontal)
            {
                return Mathf.CeilToInt(mScrollRectSize.x / (mCellSize.x + mCellPadding)) + CACHE_COUNT;
            }
            else
            {
                return Mathf.CeilToInt(mScrollRectSize.y / (mCellSize.y + mCellPadding)) + CACHE_COUNT;
            }
        }

        private int GetmStartIndex()
        {
            if (mScrollRect.horizontal)
            {
                return Mathf.FloorToInt(-mContentRectTrans.anchoredPosition.x / (mCellSize.x + mCellPadding));
            }
            else
            {
                return Mathf.FloorToInt(mContentRectTrans.anchoredPosition.y / (mCellSize.y + mCellPadding));
            }
        }

        private Vector3 GetPosition(int index)
        {
            if (mScrollRect.horizontal)
            {
                mItemX = index * (mCellSize.x + mCellPadding);
                return new Vector3(mItemX, 0, 0);
            }
            else
            {
                mItemY = index * -(mCellSize.y + mCellPadding);
                return new Vector3(0, mItemY, 0);
            }
        }

        private Vector2 GetContentSize()
        {
            if (mScrollRect.horizontal)
            {
                mContentW = mCellSize.x * mDataCount + mCellPadding * (mDataCount - 1);
                mContentH = mContentRectTrans.sizeDelta.y;
            }
            else
            {
                mContentW = mContentRectTrans.sizeDelta.x;
                mContentH = mCellSize.y * mDataCount + mCellPadding * (mDataCount - 1);
            }
            return new Vector2(mContentW, mContentH);
        }
    }

}