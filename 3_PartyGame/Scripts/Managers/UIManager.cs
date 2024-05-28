using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    public Dictionary<PanelType, PanelUI> PanelDic { get; set; } = new();

    public void OpenPanel(PanelType panelType)
    {
        Debug.Log($"<color=cyan>패널</color> - {panelType}");

        foreach (var panel in PanelDic.Values) 
            panel.gameObject.SetActive(panelType == panel.PanelType);
    }
}