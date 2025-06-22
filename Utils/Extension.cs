using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// this�� �ش��ϴ� ������Ʈ�� �޼��带 �߰��� ���� �޼����� ��� Ȯ���� ���� script
public static class Extension
{
    // �Ű����� this GameObject go -> this ������� ���� Ȯ�� �޼���(class ���� �������� �����ؾ� ��)
    // GameObject. ���� ������ GameObject.GetComponent() ���ó�� GameObject.GetOrAddComponent()�� �߰��� �ش�.
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }

    public static void BindEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_Base.BindEvent(go, action, type);
    }

    public static bool IsValid(this GameObject go) { return go != null && go.activeSelf; }
}