using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;

   void Awake() { Init(); }
    private void OnEnable() { OnEnableInit(); }

    protected virtual void Init()
    {
        // EventSystem이 없다면 UI가 작동하지 않으므로 EventSystem Type의 Object를 찾아보고 없다면 Prefab으로 만들어 둔 @EventSystem을 생성
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";


    }

    protected virtual void OnEnableInit()
    {
        StartCoroutine(Managers.Game.FadeInCoroutine());
    }

    public abstract void Clear();

    public IEnumerator LoadSceneWithEffect(Define.Scene type)
    {
        yield return StartCoroutine(Managers.Game.FadeOutCoroutine());

        Managers.Clear();
        Managers.Scene.LoadScene(type);
    }
}