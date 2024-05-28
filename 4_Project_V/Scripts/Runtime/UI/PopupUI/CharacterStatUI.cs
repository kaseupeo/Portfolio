using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class CharacterStatUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Transform originalParent;
    [SerializeField] private Transform levelParent;
    [SerializeField] private Transform realParent;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject slotVariantPrefab;
    
    private Stats _stats;
    private Stat _statPoint;

    private bool _isPointerOverUI;
    public bool IsPointerOverUI => _isPointerOverUI;

    private void Awake()
    {
        Managers.UI.CharacterStatUI = this;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        _statPoint = _stats.StatPoint;

        foreach (var stat in _stats.OriginalStatList)
        {
            var slot = Instantiate(slotPrefab, originalParent).GetComponent<StatSlotUI>();
            slot.Init(stat);
            slot.OnStatChanged += OnStatChanged;
        }

        foreach (var stat in _stats.LevelStatList)
        {
            var slot = Instantiate(slotVariantPrefab, levelParent).GetComponent<StatSlotUI>();
            slot.Init(stat);
        }
        
        foreach (var stat in _stats.RealStatList)
        {
            var slot = Instantiate(slotVariantPrefab, realParent).GetComponent<StatSlotUI>();
            slot.Init(stat, stat == _stats.HPStat || stat == _stats.MPStat);
        }
    }

    private void CreateStatSlot(Stat stat, GameObject prefab, Transform parent, bool isShowMaxStat = false)
    {
        var slot = Instantiate(prefab, parent).GetComponent<StatSlotUI>();
        slot.Init(stat, isShowMaxStat);
    }
    
    public void Show(Creature creature)
    {
        _stats = creature.Stats;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        _isPointerOverUI = false;
        gameObject.SetActive(false);
    }
    
    private void OnStatChanged(Stat stat, int point)
    {
        if (_statPoint.DefaultValue > 0)
        {
            stat.DefaultValue += point;
            _statPoint.DefaultValue--;
            
            _stats.ChangedStat();
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData) => _isPointerOverUI = true;
    public void OnPointerExit(PointerEventData eventData) => _isPointerOverUI = false;
}