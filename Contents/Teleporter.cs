using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private bool _canInteract = true;

    [Header("# Move Scene Setting")]
    [SerializeField] Define.Scene _nextScene;
    [SerializeField] Vector2 _nextScenePos;
    [SerializeField] private bool _changeScene = false;

    [Header("# Same Scene Setting")]
    [SerializeField] private Transform _targetTransform;

    private Animator _anim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_canInteract)
            return;

        if (collision.CompareTag("Player"))
        {
            StartCoroutine(Teleport());
        }
    }

    public void DoTeleport()
    {
        _anim = GetComponentInChildren<Animator>();
        _anim.Play("Open");

        if (!_changeScene)
            StartCoroutine(Teleport());
        else
            StartCoroutine(TeleportWithScene());
    }

    private Transform SetTeleport()
    {
        SavePoint savePoint = _targetTransform.GetComponent<SavePoint>();

        if (savePoint != null)
        {
            savePoint.SetSavePoint();
        }

        return _targetTransform;
    }

    private IEnumerator Teleport()
    {
        PlayerController player = Managers.Game.GetPlayer();
        player.CanMove = false;

        if (_canInteract)
            yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(Managers.Game.FadeOutCoroutine());

        Vector2 pos = SetTeleport().position;
        player.transform.position = pos;
        player.CanMove = true;

        yield return new WaitForSeconds(1f);


        yield return StartCoroutine(Managers.Game.FadeInCoroutine());
    }

    private IEnumerator TeleportWithScene()
    {
        PlayerController player = Managers.Game.GetPlayer();
        player.CanMove = false;

        Managers.Scene.TeleportPos = _nextScenePos;
        Managers.Scene.IsTeleport = true;

        player._playerData.SaveSceneIndex = (int)_nextScene;
        player._playerData.SavePositionX = _nextScenePos.x;
        player._playerData.SavePositionY = _nextScenePos.y;

        if (_canInteract)
            yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(Managers.Scene.CurrentScene.LoadSceneWithEffect(_nextScene));
    }
}