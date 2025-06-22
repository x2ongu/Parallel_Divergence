using System;
using System.Collections.Generic;
using UnityEngine;

public class StickyBomb : MonoBehaviour, IPoolable
{
    public static List<StickyBomb> AllBombs = new List<StickyBomb>();
    [HideInInspector]
    public Collider2D _attachedObject;

    [Header("# Component")]
    public Rigidbody2D _rigidBody;

    [Header("# Setting")]
    public LayerMask _hitLayers;

    public Vector2 _throwDirection;
    public float _throwPower;
    public float _throwRotationCount;
    public float _explodeRadius;

    public bool AttachedExplosion { get { return _attachedExplosion; } set { _attachedExplosion = value; } }
    private bool _attachedExplosion = false;

    [Header("# Light")]
    public GameObject _redLight;

    public float _lightRepeatTime;

    public void Init()
    {
        _rigidBody.bodyType = RigidbodyType2D.Dynamic;

        _rigidBody.angularVelocity = 360f * _throwRotationCount;

        _redLight.SetActive(false);

        CancelInvoke(nameof(TurnOnAndOffLight));

        Debug.Log("Á¡ÂøÆøÅº INIT");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_attachedObject != null)
            return;

        if (collision.CompareTag("Ground") || collision.CompareTag("DestructibleObject") || collision.CompareTag("MagneticObject") || collision.CompareTag("MovingPlatform"))
        {
            _attachedObject = collision;
            _rigidBody.velocity = Vector2.zero;
            _rigidBody.angularVelocity = 0f;
            _rigidBody.bodyType = collision.CompareTag("Ground") ? RigidbodyType2D.Static : RigidbodyType2D.Kinematic;

            InvokeRepeating(nameof(TurnOnAndOffLight), 0f, _lightRepeatTime);

            if (!collision.CompareTag("Ground"))
                transform.SetParent(collision.transform);
        }
        else if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            this.ExplodeBomb();
        }
    }

    private void Update()
    {
        DestroyAttachedObject();
    }

    public void ThrowBomb(float playerDir)
    {
        AllBombs.Add(this);

        Vector2 throwDir = new(playerDir * _throwDirection.x, _throwDirection.y);

        _rigidBody.AddForce(throwDir * _throwPower, ForceMode2D.Impulse);
    }

    public void ExplodeBomb()
    {
        Managers.Resource.Instantiate("Effect/StickyBomb_Explosion").transform.SetPositionAndRotation(transform.position, Quaternion.identity);

        Collider2D[] enemys = Physics2D.OverlapCircleAll(transform.position, _explodeRadius, _hitLayers);
        if(enemys.Length != 0)
        {
            for (int i = 0; i < enemys.Length; i++)
            {
                if (enemys[i].CompareTag("Enemy"))
                {
                    enemys[i].GetComponent<General_Base>().TakeDamage(10, transform.position.x);
                }
                else if (enemys[i].CompareTag("Boss"))
                {
                    enemys[i].GetComponent<Boss_Base>().TakeDamage(10);
                }
                else if (enemys[i].CompareTag("Summoner"))
                {
                    enemys[i].GetComponent<Boss_Elysia_Drone>().Dead();
                }
                else if (enemys[i].CompareTag("InteractableObject"))
                {
                    GateSwitch gateSwitch = enemys[i].GetComponent<GateSwitch>();

                    if (gateSwitch != null)
                        gateSwitch.OnTriggerGateSwitch();
                }
            }
        }

        Managers.Game.GetPlayer()._currentBoomCount--;
        Managers.Game.GetCamera()._impulseSource.GenerateImpulse();

        _attachedObject = null;
        AllBombs.Remove(this);
        Managers.Resource.Destroy(gameObject);
    }

    private void DestroyAttachedObject()
    {
        if (_attachedObject == null)
            return;

        if (!_attachedObject.enabled)
        {
            this.ExplodeBomb();
        }
    }

    private void TurnOnAndOffLight()
    {
        if (_redLight.activeSelf)
            _redLight.SetActive(false);
        else
            _redLight.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explodeRadius);
    }
}