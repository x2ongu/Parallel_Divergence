using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SkinChanger : MonoBehaviour
{
    SpriteLibrary m_sprLibrary;

    public SpriteLibraryAsset m_base;
    public SpriteLibraryAsset m_skin;

    public bool m_skinChange;
    bool m_bool;

    private void Awake()
    {
        m_skinChange = false;
        m_bool = false;

        m_sprLibrary = GetComponent<SpriteLibrary>();
        m_sprLibrary.spriteLibraryAsset = m_base;
    }

    private void Update()
    {
        if (m_bool != m_skinChange)
            SkinChange(m_skinChange);
    }

    void SkinChange(bool change)
    {
        if (change)
        {
            Debug.Log("��Ų ����");
            m_sprLibrary.spriteLibraryAsset = m_skin;
        }
        else
        {
            Debug.Log("�⺻ ��Ų");
            m_sprLibrary.spriteLibraryAsset = m_base;
        }

        SpriteResolver[] resolvers = GetComponentsInChildren<SpriteResolver>();
        Debug.Log("Resolver ���� : " + resolvers.Length);

        foreach (var resolver in resolvers)
        {
            Debug.Log($"Resolver {resolver.name} : {resolver.GetCategory()} / {resolver.GetLabel()}");
            resolver.ResolveSpriteToSpriteRenderer();
        }

        m_bool = change;
    }
}
