using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General_Ground : General_Base
{
    public override void Init()
    {
        base.Init();
        _type = Define.EnemyType.Ground;
    }

    public override void Move()
    {
        float movement = _destination.x - transform.position.x;
        Vector2 newVelocity = (movement * Vector2.right).normalized * EnemyData.MoveSpeed;

        newVelocity.y = 0f;

        RB.velocity = newVelocity;
    }

    public override void Attack() { }

    public override void InputInHacking()
    {
        if (!IsHacked)
            return;

        _inputVector.x = Input.GetAxisRaw("Horizontal");

        float dir;
        Vector2 scale = transform.localScale;

        if (_inputVector.x != 0)
        {
            if(_inputVector.x == -1)
            {
                dir = -1;
            }
            else
            {
                dir = 1;
            }

            scale.x = dir;
            transform.localScale = scale;

            StateManager.Anim.Play("Move");
        }
        else
            StateManager.Anim.Play("Idle");
    }
}
