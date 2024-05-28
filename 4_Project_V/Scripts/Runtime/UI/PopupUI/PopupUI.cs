using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PopupUI : BaseUI
{
    
    protected override void Awake()
    {
        base.Awake();
      
    }

    public void CloseUI()
    {
        Managers.UI.ClosePopupUI();
    }
 
  
}