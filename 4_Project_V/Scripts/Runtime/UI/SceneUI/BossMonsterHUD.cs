using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossMonsterHUD : SceneUI
{
    Transform rect;
    Image fillamountImage;
    TMP_Text bossName;
    TMP_Text hpText;
    private Creature targetCreature;
    float maxHP;
    float curHP;
    protected override void Awake()
    {
        base.Awake();
        rect = transforms["BossMonsterHUD"].transform;
        fillamountImage = images["HPFillAmountImage"];
        hpText = texts["HPText"];
        bossName = texts["BossNameText"];
        Managers.UI.BossMonsterHUD = this;
        gameObject.SetActive(false);
       
    }
    private void Start()
    {
        
    }
    private void OnEnable()
    {

    }
    private void OnDisable()
    {
        SetData(null);
    }
    public void SetData(Creature target)
    {
        targetCreature = target;
        if (target == null) return;
        maxHP = targetCreature.Stats.HPStat.Value;
        Debug.Log($"maxHP : {maxHP}");
        bossName.text = targetCreature.name;
        
    }
    private void Update()
    {
        if (targetCreature == null) return;
        curHP = targetCreature.Stats.HPStat.Value;
        var ratio = curHP / maxHP;
        hpText.text = $"{ratio * (1 * 100)}%";
        fillamountImage.fillAmount = targetCreature.Stats.HPStat.Value / maxHP;
    }
    

}
