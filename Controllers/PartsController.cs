using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsController : MonoBehaviour
{
    [SerializeField] private GameObject _frontView;
    [SerializeField] private GameObject _sideView;

    [Header("# Blink Setting")]
    [SerializeField] private Material[] _materials;

    [SerializeField] private int _blinkCount;

    [SerializeField] private Material[] _instancedMaterials;

    [Header("# Character Trail Setting")]
    [SerializeField] private GameObject _trailPrefab;
    [SerializeField] private float _trailLifetime = 0.5f;
    [SerializeField] private float _trailSpawnInterval = 0.05f;

    private bool _isDashing = false;
    private float _trailTimer = 0f;

    private void Awake()
    {
        _instancedMaterials = new Material[_materials.Length];

        for (int i = 0; i < _materials.Length; i++)
        {
            _instancedMaterials[i] = Instantiate(_materials[i]);
        }

        SpriteRenderer[] frontSpriteRenderers = _frontView.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
        SpriteRenderer[] sideSpriteRenderers = _sideView.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);

        for (int i = 0; i < frontSpriteRenderers.Length; i++)
        {
            frontSpriteRenderers[i].material = _instancedMaterials[0];
        }

        for(int j = 0; j < sideSpriteRenderers.Length; j++)
        {
            sideSpriteRenderers[j].material = _instancedMaterials[1];
        }
    }

    private void Update()
    {
        if (_isDashing)
        {
            _trailTimer -= Time.deltaTime;
            if (_trailTimer <= 0f)
            {
                SpawnTrail();
                _trailTimer = _trailSpawnInterval;
            }
        }
    }

    #region Blink
    public void SetOriginMaterial()
    {
        foreach (Material material in _instancedMaterials)
        {
            Color originalColor = material.color;
            originalColor.a = 1;
            material.color = originalColor;
            material.EnableKeyword("_EMISSION");
        }
    }

    public IEnumerator SetBlink(float invincibleTime)
    {
        int completed = 0;
        int total = _instancedMaterials.Length;

        foreach (Material material in _instancedMaterials)
        {
            StartCoroutine(BlinkWithCallback(material, invincibleTime, () => completed++));
        }

        while (completed < total)
            yield return null;
    }

    IEnumerator BlinkWithCallback(Material material, float blinkDuration, Action onComplete)
    {
        yield return Blink(material, blinkDuration);
        onComplete?.Invoke();
    }

    IEnumerator Blink(Material material, float blinkDuration)
    {
        Color originalColor = material.color;
        Color transparentColor = originalColor;
        transparentColor.a = 0f;

        for (int i = 0; i < _blinkCount; i++)
        {
            material.DisableKeyword("_EMISSION");
            material.color = transparentColor;
            yield return new WaitForSeconds(blinkDuration / (_blinkCount * 2f));

            material.EnableKeyword("_EMISSION");
            material.color = originalColor;
            yield return new WaitForSeconds(blinkDuration / (_blinkCount * 2f));
        }

        material.EnableKeyword("_EMISSION");
        originalColor.a = 1f;
        material.color = originalColor;
    }
    #endregion

    #region Character Trail
    public void StartDashTrail()
    {
        _isDashing = true;
        _trailTimer = 0f;
    }

    public void StopDashTrail()
    {
        _isDashing = false;
    }

    void SpawnTrail()
    {
        GameObject trail = Managers.Resource.Instantiate("Player/CharacterTrail");
        trail.transform.SetPositionAndRotation(transform.position, transform.rotation);

        float direction = Mathf.Sign(transform.localScale.x);

        Vector3 baseScale = _trailPrefab.transform.localScale;
        trail.transform.localScale = new Vector3(baseScale.x * direction, baseScale.y, baseScale.z);

        Transform[] originalBones = _sideView.GetComponentsInChildren<Transform>();
        Transform[] trailBones = trail.GetComponentsInChildren<Transform>();

        Dictionary<string, Transform> originalMap = new();
        foreach (var bone in originalBones)
            originalMap[bone.name] = bone;

        foreach (var bone in trailBones)
        {
            if (originalMap.TryGetValue(bone.name, out Transform source))
            {
                bone.localPosition = source.localPosition;
                bone.localRotation = source.localRotation;
                bone.localScale = source.localScale;
            }
        }

        StartCoroutine(DestroyTrail(trail, _trailLifetime));

        StartCoroutine(FadeTrail(trail));
    }

    private IEnumerator DestroyTrail(GameObject trail, float lifeTime = 0)
    {
        yield return new WaitForSeconds(lifeTime);

        Managers.Resource.Destroy(trail);
    }

    private IEnumerator FadeTrail(GameObject trail)
    {
        SpriteRenderer[] renderers = trail.GetComponentsInChildren<SpriteRenderer>();

        foreach (var sr in renderers)
        {
            if (sr != null)
            {
                sr.material = new Material(sr.material);
            }
        }

        float timer = 0f;
        while (timer < _trailLifetime)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / _trailLifetime);
            foreach (var sr in renderers)
            {
                if (sr != null)
                {
                    Color c = sr.material.color;
                    c.a = alpha;
                    sr.material.color = c;
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }
    #endregion
}
