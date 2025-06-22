using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    [SerializeField]
    GameObject m_destructibleObjectPieces;

    [Header("# Destroy With Attack Setting")]
    public bool _canDestroyWithAttack;
    public int _attackCount;

    public void HitDestructibleObject(bool isBash)
    {
        if (isBash)
        {
            DestroyObject();
            return;
        }

        if (_canDestroyWithAttack)
        {
            StartCoroutine(ReactObject());
            _attackCount--;

            if (_attackCount <= 0)
                DestroyObject();

            return;
        }
        else
        {
            StartCoroutine(ReactObject());
            return;
        }
    }

    private void DestroyObject()
    {
        GameObject obj = Managers.Resource.Instantiate(m_destructibleObjectPieces);
        obj.transform.SetPositionAndRotation(transform.position, transform.rotation);
        obj.GetOrAddComponent<DestructibleObjectPieces>().Brust();

        Managers.Resource.Destroy(gameObject);
    }

    private IEnumerator ReactObject(float duration = 0.2f, float magnitude = 0.1f)
    {
        Vector3 originalPos = transform.localPosition;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float x = Random.Range(-2f, 2f) * magnitude;
            transform.localPosition = originalPos + new Vector3(x, 0, 0);
            
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}