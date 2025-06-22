using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, IPoolable
{
    [Header("# Component")]
    public Collider2D _collider;
    public Rigidbody2D _rigidBody;

    [Header("# Setting")]
    public float _magnetStrength;
    public float _accelerationTime;

    private Transform _magnetTransform;

    private float _magnetTime = 0f;
    private bool _isInRange;

    public void Init()
    {
        _collider.isTrigger = false;

        _isInRange = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GetCoin();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AutoCollector"))
        {
            _magnetTransform = collision.transform;

            _isInRange = collision.GetComponent<AutoCollect>()._onAutoCollect;

            if (_isInRange)
                _collider.isTrigger = true;
        }
    }

    private void FixedUpdate()
    {
        MoveCoin();
    }

    private void MoveCoin()
    {
        if (!_isInRange)
            return;

        _magnetTime += Time.fixedDeltaTime;

        Vector2 currentPos = _rigidBody.position;
        Vector2 targetPos = (Vector2)_magnetTransform.position + Vector2.up * 2f;

        Vector2 dir = targetPos - currentPos;
        float distance = dir.magnitude;

        if (distance < 1f)
        {
            GetCoin();
            return;
        }

        dir.Normalize();

        float t = Mathf.Clamp01(_magnetTime / _accelerationTime);
        float speed = _magnetStrength * t;

        Vector2 move = currentPos + speed * Time.fixedDeltaTime * dir;

        _rigidBody.MovePosition(move);
    }

    private void GetCoin()
    {
        if (_isInRange == true)
            _isInRange = false;

        Debug.Log("ÄÚÀÎÈ¹µæ!");

        Managers.Resource.Destroy(gameObject);
    }
}