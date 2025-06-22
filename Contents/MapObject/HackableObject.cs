using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackableObject : MonoBehaviour
{
    [Header("# Component")]
    public Rigidbody2D _rigidBody;
    private Collider2D _switchCollider;

    [Header("# Setting")]
    [SerializeField]
    private bool _isHacked;
    public bool IsHacked { get { return _isHacked; } set { _isHacked = value; } }

    public float _moveSpeed;

    private Vector2 _inputVector;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HackableObject"))
        {
            _switchCollider = collision;
        }
    }

    private void Update()
    {
        _inputVector.x = Input.GetAxisRaw("Horizontal");
        _inputVector.y = Input.GetAxisRaw("Vertical");

        OpenTheGate();
    }

    private void FixedUpdate()
    {
        if (IsHacked)
        {
            _rigidBody.velocity = _inputVector * _moveSpeed;
        }
    }

    private void OpenTheGate()
    {
        if (!IsHacked)
            return;

        if (Input.GetKeyDown(KeyCode.E) && _switchCollider != null)
        {
            GateSwitch gateSwitch = _switchCollider.GetComponent<GateSwitch>();

            if (gateSwitch == null)
            {
                Debug.Log("null GateSwitch Component");
                return;
            }

            gateSwitch.OnTriggerGateSwitch();
            IsHacked = false;
        }
    }
}