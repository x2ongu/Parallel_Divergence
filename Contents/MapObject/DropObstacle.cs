using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObstacle : MonoBehaviour
{
    [Header("# Setting")]
    [SerializeField] private AnimationCurve _animationCurve;
    private Collider2D _collider;
    private Rigidbody2D _rigidbody;

    [SerializeField] private Transform _rayPositionLeft;
    [SerializeField] private Transform _rayPositionRight;

    private Vector2 _originPosition;

    [SerializeField] private float _actDuration;
    [SerializeField] private float _returnDuration;
    private float _distance;

    [SerializeField] private bool _doAct;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player == null)
                return;

            player.OnHit(2, transform);
        }
        else if(collision.CompareTag("Enemy"))
        {
            General_Base enemy = collision.GetComponent<General_Base>();
            if (enemy == null)
                return;

            enemy.TakeDamage(-1, transform.position.x);
        }
    }

    private void Start()
    {
        _originPosition = transform.position;
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();

        _doAct = true;
        _collider.isTrigger = false;
    }

    private void Update() { FindPlayer(); }

    private void FindPlayer()
    {
        if (_doAct == false)
            return;

        RaycastHit2D ground = Physics2D.Raycast(_rayPositionLeft.position, Vector2.down, 100f, 1 << (int)Define.Layer.Ground);

        if (ground.collider == null)
            return;

        _distance = ground.distance;

        RaycastHit2D left = Physics2D.Raycast(_rayPositionLeft.position, Vector2.down, _distance, 1 << (int)Define.Layer.Player);
        RaycastHit2D right = Physics2D.Raycast(_rayPositionRight.position, Vector2.down, _distance, 1 << (int)Define.Layer.Player);

        if (left || right)
        {
            StartCoroutine(DoAct());
        }
    }

    private IEnumerator DoAct()
    {
        _doAct = false;
        _collider.isTrigger = true;

        float timer = 0;
        Vector2 startPos = _rigidbody.position;

        while (timer < _actDuration)
        {
            timer += Time.fixedDeltaTime;

            float t = Mathf.Clamp01(timer / _actDuration);
            float curve = _animationCurve.Evaluate(t);

            Vector2 offset = _distance * curve * Vector2.down;

            _rigidbody.MovePosition(startPos + offset);

            yield return null;
        }

        StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        _collider.isTrigger = false;

        yield return new WaitForSeconds(1f);

        Vector2 startPos = _rigidbody.position;
        float targetPosY = _originPosition.y - startPos.y;

        float timer = 0;
        while (timer < _returnDuration)
        {
            timer += Time.fixedDeltaTime;

            float t = Mathf.Clamp01(timer / _returnDuration);
            float curve = targetPosY * t;

            Vector2 offset = Vector2.up * curve;

            _rigidbody.MovePosition(startPos + offset);

            yield return null;
        }

        _doAct = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_rayPositionLeft.position, Vector2.down * _distance);
        Gizmos.DrawRay(_rayPositionRight.position, Vector2.down * _distance);
    }
}
