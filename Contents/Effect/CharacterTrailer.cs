using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTrailer: MonoBehaviour
{
    [Header("# Character Trail Setting")]
    [SerializeField] private GameObject _sideView;
    [SerializeField] private GameObject _trailPrefab;

    [SerializeField] private float _trailLifetime = 0.5f;
    [SerializeField] private float _trailSpawnInterval = 0.05f;

    private bool _canTrail = false;
    private float _trailTimer = 0f;

    private void Update()
    {
        if (_canTrail)
        {
            _trailTimer -= Time.deltaTime;
            if (_trailTimer <= 0f)
            {
                SpawnTrail();
                _trailTimer = _trailSpawnInterval;
            }
        }
    }

    public void StartDashTrail()
    {
        _canTrail = true;
        _trailTimer = 0f;
    }

    public void StopDashTrail()
    {
        _canTrail = false;
    }

    void SpawnTrail()
    {
        GameObject trail = Managers.Resource.Instantiate(_trailPrefab);
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
}
