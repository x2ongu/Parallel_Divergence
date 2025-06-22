using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackIndicator : MonoBehaviour
{
    HashSet<Collider2D> m_targetHashSet = new HashSet<Collider2D>();
    public HashSet<Collider2D> TargetHashSet { get { return m_targetHashSet; } }

    [Header("# 공격자 타입"), SerializeField]
    public Define.AttackerType m_attackerType;

    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (m_attackerType)
        {
            case Define.AttackerType.Player:
                if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss") || collision.gameObject.CompareTag("Summoner") || collision.gameObject.CompareTag("DestructibleObject"))
                {
                    if (!m_targetHashSet.Contains(collision))
                        m_targetHashSet.Add(collision);
                }
                break;

            case Define.AttackerType.Enemy:
                if (collision.gameObject.CompareTag("Player"))
                    m_targetHashSet.Add(collision);
                break;

            default:

                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (m_attackerType)
        {
            case Define.AttackerType.Player:
                if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss") || collision.gameObject.CompareTag("Summoner") || collision.gameObject.CompareTag("DestructibleObject"))
                {
                    if (m_targetHashSet.Contains(collision))
                        m_targetHashSet.Remove(collision);
                }
                break;

            case Define.AttackerType.Enemy:
                if (collision.gameObject.CompareTag("Player"))
                    m_targetHashSet.Remove(collision);
                break;

            default:

                break;
        }
    }
}