using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitPopup : PopupUI
{
    protected override void Awake()
    {
        base.Awake();
        buttons["CancelButton"].onClick.AddListener(()=> Managers.Instance.Quit());
    }

}
