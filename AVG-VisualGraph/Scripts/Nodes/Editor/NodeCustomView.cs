using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LJ.VisualAVG;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using VisualGraphInEditor;
using System;
using System.Linq;
using UnityEditor.UIElements;
using VisualGraphRuntime;
using Random = UnityEngine.Random;

#region Node_Wait
[CustomNodeView((typeof(Node_Wait)))]
public class NodeWaitNodeView : VisualGraphNodeView
{
    public override bool ShowNodeProperties => false;

    public override void DrawNode()
    {
        FloatField textField = new FloatField("等待时间");
        textField.value = (node as Node_Wait).waitTimer;
        mainContainer.Add(textField);
        textField.RegisterValueChangedCallback((data) => (this.node as Node_Wait).waitTimer = data.newValue);
    }
}
#endregion

#region Node_SayHide
[CustomNodeView((typeof(Node_SayHide)))]
public class NodeSayHideView : VisualGraphNodeView
{
    public override Vector2 default_size => new Vector2(150, 150);
    public override bool ShowNodeProperties => false;
}

#endregion

#region Node_SayOther
[CustomNodeView((typeof(Node_SayOther)))]
public class Node_SayOther_View : VisualGraphNodeView
{
    public override bool ShowNodeProperties => false;

    PopupField<string> popupicon;
    Node_SayOther nodesay;
    public override void InitNode(VisualGraphNode graphNode)
    {
        base.InitNode(graphNode);
        nodesay = (node as Node_SayOther);
    }
    public override void DrawNode()
    {
        NodeCustomHelper.DrawTextField(mainContainer, " Msg", nodesay.msg, onChangeMsg: (data) => nodesay.msg = data);
        NodeCustomHelper.DrawObjectField<AudioClip>(mainContainer, "配音", nodesay.msgAudio, onChangeObj: (data) => nodesay.msgAudio = data);

        NodeCustomHelper.DrawPopupItem(mainContainer, "姓名", new List<string>(nodesay.listOtherRoleName), ref nodesay.roleName, (data) =>
        {
            nodesay.roleName = data;
            if (nodesay.showIcon && popupicon != null)
            {
                mainContainer.Remove(popupicon);
                AddPopupIcon();
            }
        });

        Toggle toggle = new Toggle("是否展示头像");
        toggle.value = nodesay.showIcon;
        toggle.labelElement.style.minWidth = 100;
        mainContainer.Add(toggle);
        toggle.RegisterValueChangedCallback(data =>
        {
            nodesay.showIcon = data.newValue;
            if (nodesay.showIcon)
            {
                if (popupicon == null)
                    AddPopupIcon();
            }
            else
            {
                if (popupicon != null)
                {
                    mainContainer.Remove(popupicon);
                    nodesay.spName = string.Empty;
                    popupicon = null;
                }
            }
        });
        if (toggle.value)
            AddPopupIcon();
        void AddPopupIcon()
        {
            popupicon = NodeCustomHelper.DrawPopupItem(mainContainer, "头像", new List<string>(nodesay.listSpNames), ref nodesay.spName, (data) => nodesay.spName = data);
        }
    }
}

#endregion

#region Node_SayAside

[CustomNodeView((typeof(Node_SayAside)))]
public class Node_SayAside_View : VisualGraphNodeView
{
    public override bool ShowNodeProperties => false;

    Node_SayAside sayAside;
    public override void InitNode(VisualGraphNode graphNode)
    {
        base.InitNode(graphNode);
        sayAside = node as Node_SayAside;
    }

    public override void DrawNode()
    {

        VisualElement element = new VisualElement();
        Button btnadd = new Button();
        btnadd.style.width = 115;
        btnadd.style.height = 25;
        btnadd.text = "㊉";
        btnadd.style.backgroundColor = new Color(0, 0.35f, 0);
        btnadd.RegisterCallback<ClickEvent>(data =>
        {
            var item = new Node_SayAside.AsideData();
            sayAside.Datas.Add(item);
            AddItem(item);
        });


        Button btndesc = new Button();
        btndesc.style.width = 115;
        btndesc.style.height = 25;
        btndesc.text = "㊀";
        btndesc.style.marginTop = -28;
        btndesc.style.marginLeft = 122;
        btndesc.style.backgroundColor = new Color(0.35f, 0f, 0);
        btndesc.RegisterCallback<ClickEvent>(data =>
        {
            if (sayAside.Datas.Count > 0)
            {
                sayAside.Datas.RemoveAt(sayAside.Datas.Count - 1);
                for (int i = 0; i < 3; i++) //移除后面两个就可以
                    mainContainer.Remove(mainContainer.Children().Last());
            }
        });

        element.Add(btnadd);
        element.Add(btndesc);
        mainContainer.Add(element);

        if (sayAside.Datas != null && sayAside.Datas.Count > 0)
        {
            foreach (var item in sayAside.Datas)
                AddItem(item);
        }
    }
    private void AddItem(Node_SayAside.AsideData item)
    {
        NodeCustomHelper.DrawTextField(mainContainer, " Msg", item.msg, onChangeMsg: (data) => item.msg = data);
        NodeCustomHelper.DrawObjectField<AudioClip>(mainContainer, "配音", item.msgAudio, onChangeObj: (data) => item.msgAudio = data);

        NodeCustomHelper.DrawLable(mainContainer, NodeCustomHelper.line, color: new Color(Random.value, Random.value, Random.value));
    }
}
#endregion

#region Node_SayRole
[CustomNodeView((typeof(Node_SayRole)))]
public class Node_SayRole_View : VisualGraphNodeView
{
    public override bool ShowNodeProperties => false;

    Node_SayRole sayRole;
    public override void InitNode(VisualGraphNode graphNode)
    {
        base.InitNode(graphNode);
        sayRole = node as Node_SayRole;
    }

    public override void DrawNode()
    {
        NodeCustomHelper.DrawTextField(mainContainer, " Msg", sayRole.msg, (data) => sayRole.msg = data);
        NodeCustomHelper.DrawObjectField<AudioClip>(mainContainer, "配音", sayRole.msgAudio, onChangeObj: (data) => sayRole.msgAudio = data);

        List<string> options = new List<string>(sayRole.listRolesName);
        NodeCustomHelper.DrawPopupItem(mainContainer, "名字", new List<string>(sayRole.listRolesName), ref sayRole.roleName, (data) =>
        {
            sayRole.roleName = data;
            for (int i = 0; i < 4; i++)  //移除后面四个重新添加
            {
                mainContainer.Remove(mainContainer.Children().Last());
            }
            AddTalkEmoji();
        });
        AddTalkEmoji();


        void AddTalkEmoji()
        {
            List<string> optionsemos = new List<string>(sayRole.listRoleEmojis);
            NodeCustomHelper.DrawPopupItem(mainContainer, "进入表情", optionsemos, ref sayRole.enterTalkEmoji, (data) => sayRole.enterTalkEmoji = data);
            NodeCustomHelper.DrawPopupItem(mainContainer, "说话表情", optionsemos, ref sayRole.talkEmoji, (data) => sayRole.talkEmoji = data);
            NodeCustomHelper.DrawPopupItem(mainContainer, "退出表情", optionsemos, ref sayRole.exitTalkEmoji, (data) => sayRole.exitTalkEmoji = data);
            NodeCustomHelper.DrawPopupItem(mainContainer, "说完表情", optionsemos, ref sayRole.talkEndEmoji, (data) => sayRole.talkEndEmoji = data);
        }
    }
}

#endregion

#region Node_SayMe
[CustomNodeView((typeof(Node_SayMe)))]
public class Node_SayMe_View : VisualGraphNodeView
{
    public override bool ShowNodeProperties => false;
    public override void DrawNode()
    {
        Node_SayMe sayMe = node as Node_SayMe;
        NodeCustomHelper.DrawTextField(mainContainer, " Msg", sayMe.msg, (data) => sayMe.msg = data);
        NodeCustomHelper.DrawObjectField<AudioClip>(mainContainer, "配音", sayMe.msgAudio, onChangeObj: (data) => sayMe.msgAudio = data);
        NodeCustomHelper.DrawLable(mainContainer, NodeCustomHelper.line, color: new Color(Random.value, Random.value, Random.value));

        List<string> options = new List<string>(sayMe.listRolesName);
        NodeCustomHelper.DrawPopupItem(mainContainer, "名字", new List<string>(sayMe.listRolesName), ref sayMe.targetRoleName, (data) =>
        {
            sayMe.targetRoleName = data;
            for (int i = 0; i < 4; i++)  //移除后面四个重新添加
            {
                mainContainer.Remove(mainContainer.Children().Last());
            }
            AddTalkEmoji();
        });
        AddTalkEmoji();

        void AddTalkEmoji()
        {
            List<string> optionsemos = new List<string>(sayMe.listRoleEmojis);
            NodeCustomHelper.DrawPopupItem(mainContainer, "进入表情", optionsemos, ref sayMe.enterTalkEmoji, (data) => sayMe.enterTalkEmoji = data);
            NodeCustomHelper.DrawPopupItem(mainContainer, "说话表情", optionsemos, ref sayMe.talkEmoji, (data) => sayMe.talkEmoji = data);
            NodeCustomHelper.DrawPopupItem(mainContainer, "退出表情", optionsemos, ref sayMe.exitTalkEmoji, (data) => sayMe.exitTalkEmoji = data);
            NodeCustomHelper.DrawPopupItem(mainContainer, "说完表情", optionsemos, ref sayMe.talkEndEmoji, (data) => sayMe.talkEndEmoji = data);
        }
    }
}
#endregion

#region Node_PotShow
[CustomNodeView((typeof(Node_PotShow)))]
public class Node_PotShowView : VisualGraphNodeView
{
    public override bool ShowNodeProperties => false;

    public override void DrawNode()
    {
        Node_PotShow potShow = node as Node_PotShow;
        NodeCustomHelper.DrawPopupItem(mainContainer, "姓名", new List<string>(potShow.listRoles), ref potShow.potname, (data) => { potShow.potname = data; });

        NodeCustomHelper.DrawEnumFidld(mainContainer, "效果", potShow.effectType,onChangeValue:(data)=>potShow.effectType=(AVGHelper.FXType)data);
        NodeCustomHelper.DrawEnumFidld(mainContainer, "位置", potShow.posType,onChangeValue:(data)=>potShow.posType = (AVGHelper.PosType)data);
        NodeCustomHelper.DrawEnumFidld(mainContainer, "距离", potShow.disType,onChangeValue:(data)=>potShow.disType = (AVGHelper.DistanceType)data);
    }
}
#endregion

#region Node_PotHide
[CustomNodeView((typeof(Node_PotHide)))]
public class Node_PotHide_View : VisualGraphNodeView
{
    public override bool ShowNodeProperties => false;

    public override void DrawNode()
    {
        Node_PotHide pothide = node as Node_PotHide;
        NodeCustomHelper.DrawPopupItem(mainContainer, "姓名", new List<string>(pothide.listRoles), ref pothide.potname, (data) => { pothide.potname = data; });

        NodeCustomHelper.DrawEnumFidld(mainContainer, "效果", pothide.effectType, onChangeValue: (data) => pothide.effectType = (AVGHelper.FXType)data);
    }
}
#endregion

public class NodeCustomHelper
{
    public const string line = "——————————————————————————————————————————————";

    /// <summary>
    /// 绘画一个 文本输入
    /// </summary>
    /// <param name="mainContainer">容器</param>
    /// <param name="lableName">输入框名字</param>
    /// <param name="baseInputValue">初始值</param>
    /// <param name="onChangeMsg">输入改变事件</param>
    /// <returns></returns>
    public static TextField DrawTextField(VisualElement mainContainer, string lableName, string baseInputValue, System.Action<string> onChangeMsg)
    {
        Label label = new Label(lableName);
        mainContainer.Add(label);
        TextField msg = new TextField();
        msg.multiline = true;
        msg.value = baseInputValue;
        msg.style.minHeight = 40f;
        msg.style.whiteSpace = WhiteSpace.Normal;//换行
        mainContainer.Add(msg);
        msg.RegisterValueChangedCallback(data => onChangeMsg(data.newValue));
        return msg;
    }
    public static ObjectField DrawObjectField<T>(VisualElement mainContainer, string lableName, T t, System.Action<T> onChangeObj) where T : UnityEngine.Object
    {
        ObjectField objectField = new ObjectField(lableName);
        objectField.value = t;
        objectField.labelElement.style.minWidth = 100;
        objectField.objectType = typeof(T);
        objectField.allowSceneObjects = false;
        mainContainer.Add(objectField);
        objectField.RegisterValueChangedCallback(data => onChangeObj((T)data.newValue));
        return objectField;
    }
    public static PopupField<string> DrawPopupItem(VisualElement mainContainer, string lableName, List<string> options, ref string strValue, System.Action<string> onChangeValue)
    {
        int index = 0;
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i] == strValue)
            {
                index = i;
                break;
            }
        }

        PopupField<string> popup = new PopupField<string>(lableName, options, index);
        strValue = popup.value;
        popup.labelElement.style.minWidth = 100;
        mainContainer.Add(popup);
        popup.RegisterValueChangedCallback(data =>
        {
            onChangeValue(data.newValue);
        });
        return popup;
    }
    public static Label DrawLable(VisualElement mainContainer, string lableName, Color? color = null, float minHeight = 20)
    {
        Label label = new Label(lableName);
        label.style.height = minHeight;
        if (color != null)
            label.style.color = (Color)color;
        mainContainer.Add(label);
        return label;
    }

    public static EnumField DrawEnumFidld(VisualElement mainContainer, string lableName,Enum baseEnum,System.Action<Enum> onChangeValue)
    {
        EnumField enumField = new EnumField(lableName, baseEnum);
        enumField.RegisterValueChangedCallback((data) =>
        {
            onChangeValue(data.newValue);
        });
        enumField.labelElement.style.minWidth = 100;
        mainContainer.Add(enumField);
        return enumField;
    }
}