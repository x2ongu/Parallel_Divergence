using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticObject : MonoBehaviour
{
    [Header("# Component")]
    public Rigidbody2D _rigidBody;
    Coroutine _coroutine;

    [Header("# Setting")]
    public AnimationCurve _animationCurve;

    public Vector2 _popupDirection;

    public float _moveDistance;
    public float _moveDuration;

    private Vector2 _startPos;
    private Vector2 _targetPos;

    private float _timer;
    private float _percentage;

    private bool _isPopup = false;
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
        {
            target.SetParent(null);
        }
    }

    private void FixedUpdate() { MoveObject(); }

    public void PopUpObject()
    {
        Debug.Log("popup!!");
        if (_coroutine != null || _isMoving)
            return;

        if (_isPopup)
            SetObjectForMovement(-_popupDirection);
        else
            SetObjectForMovement(_popupDirection);
    }

    private void SetObjectForMovement(Vector2 dir)
    {
        _startPos = _rigidBody.position;
        Vector2 moveVec = dir.normalized * _moveDistance;
        _targetPos = _startPos + moveVec;

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
            Vector3 finalPos = new Vector3(_targetPos.x, _targetPos.y, transform.position.z);
            transform.position = finalPos;
            _isPopup = !_isPopup;
            _isMoving = false;
            Debug.Log(transform.GetChild(0).position);
            return;
        }

        Vector2 nextPos2D = Vector2.Lerp(_startPos, _targetPos, _animationCurve.Evaluate(_percentage));
        Vector3 nextPos = new(nextPos2D.x, nextPos2D.y, transform.position.z);
        transform.position = nextPos;
    }
}