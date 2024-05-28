public class LoadingUI : BaseUI
{
    public float LoadingAmount { get; set; }

    protected override void Awake()
    {
        base.Awake();

        Managers.UI.LoadingUI = this;
        
        images["FillImage"].fillAmount = 0;
        texts["LoadingText"].text = "0 / 100%";
    }

    private void Update()
    {
        images["FillImage"].fillAmount = LoadingAmount;
        texts["LoadingText"].text = $"{(int)(LoadingAmount * 100)} / 100%";
    }
}