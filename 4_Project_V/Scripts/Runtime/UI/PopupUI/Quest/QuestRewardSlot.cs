using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestRewardSlot : MonoBehaviour
{
    [field: SerializeField]
    public Image Icon { get; set; }
    [field: SerializeField]
    public TextMeshProUGUI Quantity { get; set; }

    public void Clear()
    {
        Icon.sprite = null;
        Icon.color = new Color(1, 1, 1, 0);
        Quantity.text = String.Empty;
    }
}