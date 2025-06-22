using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("# Component")]
    public Rigidbody2D _rigidBody;

    [Header("# Setting")]
    public AnimationCurve _animationCurve;

    public float _moveDistance;
    public float _moveDuration;

    public bool _isHorizontal;
    public bool _switchOn;

    private Vector3 _startPos;
    private Vector3 _targetPos;
    private Vector3 _direction;

    private float _timer;
    private float _percentage;
    private float _dir = 1f;

    private bool _isMoving = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            StartCoroutine(Delayedparent(collision.transform));
        }
    }

    private IEnumerator Delayedparent(Transform target)
    {
        yield return null;

        PlayerMovement player = target.GetComponent<PlayerMovement>();

        if (player.OnGroundObject)
        {
            player.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            StartCoroutine(DelayedUnparent(collision.transform));
        }
    }

    private IEnumerator DelayedUnparent(Transform target)
    {
        yield return null;

        if (target != null)
            target.SetParent(null);
    }

    private void Start()
    {
        if (_switchOn)
            InvokeMoveingPlatform();
    }

    private void FixedUpdate() { MoveObject(); }

    public void InvokeMoveingPlatform()
    {
        InvokeRepeating(nameof(SetObjectForMovement), 0f, _moveDuration + 1f);
    }

    private void SetObjectForMovement()
    {
        _startPos = transform.position;

        if (_isHorizontal)
            _direction = transform.right.normalized * _dir;
        else
            _direction = transform.up.normalized * _dir;

        _dir *= -1f;
        _targetPos = _startPos + (_direction * _moveDistance);

        _timer = 0f;
        _isMoving = true;

        _rigidBody.bodyType = RigidbodyType2D.Kinematic;
    }

    private void MoveObject()
    {
        if (!_isMoving)
            return;

        _timer += Time.fixedDeltaTime;
        _percentage = _timer / _moveDuration;

        if (_percentage >= 1f)
        {
            _targetPos.z = _startPos.z;
            transform.position = _targetPos;
            _isMoving = false;
            return;
        }

        Vector3 nextPos = Vector3.Lerp(_startPos, _targetPos, _animationCurve.Evaluate(_percentage));
        nextPos.z = _startPos.z; // z À¯Áö
        transform.position = nextPos;
    }
}