using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedCostumeButtonUI : MonoBehaviour
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
        label.text = Costume.Category.DisplayName;
        prevButton.onClick.AddListener(() => Managers.UI.OnPrevCostume.Invoke(Costume.Category));
        nextButton.onClick.AddListener(() => Managers.UI.OnNextCostume.Invoke(Costume.Category));
    }
}