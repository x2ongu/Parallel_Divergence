using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObjectPieces : MonoBehaviour
{
    private Rigidbody2D[] _objects;

    private float _minSpeed = 5f;
    private float _maxSpeed = 10f;

    private void OnEnable()
    {
        _objects = GetComponentsInChildren<Rigidbody2D>();

        foreach (var obj in _objects)
        {
            obj.transform.position = Vector3.zero;
        }
    }

    public void Brust()
    {
        _objects = GetComponentsInChildren<Rigidbody2D>();

        foreach (var obj in _objects)
        {
            Vector2 randomVec = new(Random.Range(-0.7f, 0.7f), 1);
            float speed = Random.Range(_minSpeed, _maxSpeed);

            obj.AddForce(2f * speed * randomVec, ForceMode2D.Impulse);
        }

        StartCoroutine(Pooling());
    }

    IEnumerator Pooling()
    {
        yield return new WaitForSeconds(1f);

        transform.position = Vector3.zero;

        Managers.Resource.Destroy(gameObject);
    }
}