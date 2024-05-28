using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    public Costume Costume { get; set; }

    private void Awake()
    {

    }

    public void Init()
    {
        label.text = Costume.Type.ToString();
        prevButton.onClick.AddListener(Costume.PrevCostume);
        nextButton.onClick.AddListener(Costume.NextCostume);
    }
}