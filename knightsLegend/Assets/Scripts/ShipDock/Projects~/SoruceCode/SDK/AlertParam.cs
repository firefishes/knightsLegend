using UnityEngine.Events;

namespace ShipDock.SDK
{
    /// <summary>
    ///
    /// 系统级提示UI的参数
    /// 
    /// Created by 田亚宗 on 2019/09/20.
    ///
    /// </summary>
    public sealed class AlertParam : IAlertParam
    {
        public string Title { get; private set; }

        public string Content { get; private set; }

        public string LeftButtonContent { get; private set; }

        public string RightButtonContent { get; private set; }

        public AlerStyle LeftButtonStyle { get; private set; }

        public AlerStyle RightButtonStyle { get; private set; }

        public UnityAction<string> CallBack { get; private set; }

        public UnityAction<string> OnShow { get; private set; }

        public AlertParam(string title, string content, string btnText = "确定")
            : this(title, content, btnText, "", AlerStyle.UIAlertActionStyleDefault, AlerStyle.UIAlertActionStyleDefault, s => { }, null)
        {

        }

        public AlertParam(string title, string content, string lbtnText, string rbtnText, UnityAction<string> callback) :
            this(title, content, lbtnText, rbtnText, AlerStyle.UIAlertActionStyleDefault, AlerStyle.UIAlertActionStyleDefault, callback, null)
        {
        }


        public AlertParam(string title, string content, UnityAction<string> callback)
            : this(title, content, "取消", "确定", AlerStyle.UIAlertActionStyleDefault, AlerStyle.UIAlertActionStyleDefault, callback, null)
        {
        }

        public AlertParam(string title, string content, string leftBtnContent, string rightBtnConent, AlerStyle leftStyle, AlerStyle rightStyle, UnityAction<string> callback, UnityAction<string> onShow)
        {
            Title = title;
            Content = content;
            CallBack = callback;
            OnShow = onShow;
            LeftButtonContent = leftBtnContent;
            RightButtonContent = rightBtnConent;
            LeftButtonStyle = leftStyle;
            RightButtonStyle = rightStyle;
        }

        public void SetButtonText(string left, string right)
        {
            LeftButtonContent = left;
            RightButtonContent = right;
        }

        public void SetButtonStyle(AlerStyle left, AlerStyle right)
        {
            LeftButtonStyle = left;
            RightButtonStyle = right;
        }

        public void SetAlertAction(UnityAction<string> callback, UnityAction<string> onshow = null)
        {
            CallBack = callback;
            OnShow = onshow;
        }
    }
}

