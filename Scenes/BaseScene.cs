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
        // EventSystem�� ���ٸ� UI�� �۵����� �����Ƿ� EventSystem Type�� Object�� ã�ƺ��� ���ٸ� Prefab���� ����� �� @EventSystem�� ����
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