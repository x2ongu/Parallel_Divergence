using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralEnemyData : MonoBehaviour
{
    [SerializeField]
    private Vector2 _spawnPoint;
    public Vector2 SpawnPoint { get { return _spawnPoint; } set { _spawnPoint = value; } }

    [SerializeField] private int _id;

    [SerializeField] private int _damage;
    [SerializeField] private int _maxHp;
    [SerializeField] private int _hp;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _attackRange;

    [SerializeField] private bool _isGroundEnemy;

    [SerializeField] private float _groundTraceDistanceX;
    [SerializeField] private float _groundTraceDistanceY;

    [SerializeField] private float _airealTraceRange;

    public float AirealHeight = 0f;

    [SerializeField] public bool IsFixed = false;
    [SerializeField] public bool SuperArmor = false;

    public int ID { get { return _id; } set { _id = value; } }
    public int Damage { get { return _damage; } set { _damage = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int Hp { get { return _hp; } set { _hp = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public float AttackSpeed { get { return _attackSpeed; } set { _attackSpeed = value; } }
    public float AttackRange { get { return _attackRange; } set { _attackRange = value; } }
    public bool IsGroundEnemy { get { return _isGroundEnemy; } set { _isGroundEnemy = value; } }
    public float GroundTraceDistanceX { get { return _groundTraceDistanceX; } set { _groundTraceDistanceX = value; } }
    public float GroundTraceDistanceY { get { return _groundTraceDistanceY; } set { _groundTraceDistanceY = value; } }
    public float AirealTraceRange { get { return _airealTraceRange; } set { _airealTraceRange = value; } }

    public void TakeDamage(int damage)
    {
        Hp -= damage;

        if (Hp <= 0)
        {
            Hp = 0;
        }
    }
}