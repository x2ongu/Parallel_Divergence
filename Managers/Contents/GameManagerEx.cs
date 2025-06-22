using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerEx
{
    PlayerController _player;
    CameraController _camera;

    GameObject _panel;

    public PlayerController GetPlayer()
    {
        if (_player == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                player = Managers.Resource.Instantiate("Player/Player");
                player.transform.position = Vector3.zero;
            }

            _player = player.GetComponent<PlayerController>();
        }

        return _player;
    }

    public CameraController GetCamera()
    {
        if (_camera == null)
        {
            GameObject camera = GameObject.FindObjectOfType<CameraController>().gameObject;

            if (camera == null)
            {
                camera = Managers.Resource.Instantiate("Camera");
                camera.transform.position = Vector3.zero;
            }

            _camera = camera.GetComponent<CameraController>();
        }

        return _camera;
    }

    public void Freeze(GameObject obj)
    {
        var anim = obj.GetComponent<Animator>();
        if (anim != null) anim.speed = 0;

        var rb2d = obj.GetComponent<Rigidbody2D>();
        if (rb2d != null) rb2d.simulated = false;

        var rb = obj.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    public void Unfreeze(GameObject obj)
    {
        var anim = obj.GetComponent<Animator>();
        if (anim != null) anim.speed = 1;

        var rb2d = obj.GetComponent<Rigidbody2D>();
        if (rb2d != null) rb2d.simulated = true;

        var rb = obj.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;
    }

    public IEnumerator FadeOutCoroutine()
    {
        float elapsedTime = 0f;
        float fadeTime = 0.7f;

        if (_panel == null)
        {
            _panel = Managers.Resource.Instantiate("UI/FadeInOut");

            _panel.transform.SetParent(null);
            GameObject.DontDestroyOnLoad(_panel);
        }

        _panel.SetActive(true);
        _panel.GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        yield return new WaitForSeconds(0.5f);

        while (elapsedTime <= fadeTime)
        {
            _panel.GetComponentInChildren<CanvasRenderer>().SetAlpha(Mathf.Lerp(0f, 1f, elapsedTime / fadeTime));

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        _panel.GetComponentInChildren<CanvasRenderer>().SetAlpha(1);
    }

    public IEnumerator FadeInCoroutine()
    {
        float elapsedTime = 0f;
        float fadeTime = 0.7f;

        if (_panel == null)
        {
            _panel = Managers.Resource.Instantiate("UI/FadeInOut");

            _panel.transform.SetParent(null);
            GameObject.DontDestroyOnLoad(_panel);
        }

        _panel.SetActive(true);
        _panel.GetComponentInChildren<CanvasRenderer>().SetAlpha(1);
        yield return new WaitForSeconds(0.5f);

        while (elapsedTime <= fadeTime)
        {
            _panel.GetComponentInChildren<CanvasRenderer>().SetAlpha(Mathf.Lerp(1f, 0f, elapsedTime / fadeTime));

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _panel.SetActive(false);
    }

    public void Clear()
    {
        _player = null;
        _camera = null;
    }
}