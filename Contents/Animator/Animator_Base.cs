using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Animator_Base : MonoBehaviour
{
    [SerializeField] protected GameObject _frontView;
    [SerializeField] protected GameObject _sideView;

    protected Animator _anim;

    private void OnEnable() { Init(); }

    public virtual void Init()
    {
        _anim = GetComponent<Animator>();
    }

    public void ViewController(bool isFront)
    {
        if (_frontView == null || _sideView == null)
            return;

        _frontView.SetActive(isFront);
        _sideView.SetActive(!isFront);
    }

    protected void CheckAnimationIdle()
    {
        if (_frontView == null || _sideView == null)
            return;

        bool isIdle = _anim.GetCurrentAnimatorStateInfo(0).IsName("Idle");

        _frontView.SetActive(isIdle);
        _sideView.SetActive(!isIdle);
    }
}
