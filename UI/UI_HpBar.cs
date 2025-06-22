using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UI_HpBar : UI_Base
{
    [SerializeField] Boss_Base _boss;

    enum Texts
    {
        Text_Hp
    }

    enum Sliders
    {
        Slider_Hp
    }

    private void Update()
    {
        float ratio = _boss._currentHp / (float)_boss._maxHp;
        SetHPRatio(ratio);
    }

    public override void Init()
    {
        BindText(typeof(Texts));
        BindSlider(typeof(Sliders));
    }
    public void SetHPRatio(float ratio)
    {
        GetSlider((int)Sliders.Slider_Hp).value = ratio;
        string text = _boss._currentHp.ToString() + " / " + _boss._maxHp.ToString();
        GetText((int)Texts.Text_Hp).text = text;
    }
}