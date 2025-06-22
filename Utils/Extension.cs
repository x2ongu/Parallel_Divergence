using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// this에 해당하는 컴포넌트의 메서드를 추가해 기존 메서드의 기능 확장을 위한 script
public static class Extension
{
    // 매개변수 this GameObject go -> this 사용으로 인한 확장 메서드(class 또한 정적으로 구현해야 함)
    // GameObject. 이후 나오는 GameObject.GetComponent() 기능처럼 GameObject.GetOrAddComponent()를 추가해 준다.
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