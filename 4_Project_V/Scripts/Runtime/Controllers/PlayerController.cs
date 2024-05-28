using System;
using System.Linq;
using UnityEngine;

public class PlayerController : BaseController
{
    [SerializeField] private GameObject marker;

    private Creature _creature;
    private GameObject _marker;

    private void Awake()
    {
        _creature = GetComponent<Creature>();
        Managers.Game.Player = _creature;
    }

    private void Start()
    {
        _creature.SkillSystem.onSkillTargetSelectionCompleted += ReserveSkill;
    }

    private void OnEnable()
    {
        Managers.Input.OnLeftClicked += SelectTarget;
        Managers.Input.OnLeftClicked += Attack;
        Managers.Input.OnRightClicked += MoveToPosition;
        Managers.Input.ChangedWeapon.started += _ => _creature.EquipmentSystem.ChangeEquipWeapon();
        
        Managers.Input.UIPressActionDic[Define.InterfaceElements.Skill].started += _ =>
        {
            var skillTreeUI = Managers.UI.SkillTreeView;

            if (!skillTreeUI.gameObject.activeSelf)
                skillTreeUI.Show(_creature, _creature.SkillSystem.DefaultSkillTree);
            else
                skillTreeUI.Hide();
        };
        Managers.Input.UIPressActionDic[Define.InterfaceElements.Stat].started += _ =>
        {
            var characterStatUI = Managers.UI.CharacterStatUI;

            if (!characterStatUI.gameObject.activeSelf)
                characterStatUI.Show(_creature);
            else
                characterStatUI.Hide();
        };
        Managers.Input.UIPressActionDic[Define.InterfaceElements.Inventory].started += _ =>
        {
            var inventoryUI = Managers.UI.InventoryUI;

            if (!inventoryUI.gameObject.activeSelf)
                inventoryUI.Show(_creature);
            else
                inventoryUI.Hide();
        };
        Managers.Input.UIPressActionDic[Define.InterfaceElements.Quest].started += _ =>
        {
            var questUI = Managers.UI.QuestUI;

            if (!questUI.gameObject.activeSelf)
                questUI.Show(_creature);
            else
                questUI.Hide();
        };
        Managers.Input.UIPressActionDic[Define.InterfaceElements.MainMenu].started += _ =>
        {
            var mainMenuUI = Managers.UI.MainMenuUI;
            if (!mainMenuUI.gameObject.activeSelf)
                mainMenuUI.Show(_creature);
            else mainMenuUI.Hide();

        };
        Managers.Input.Interaction.started += _ =>
        {
            var shopUI = Managers.UI.ShopPopupUI;

            Collider[] results = new Collider[1];
            var size = Physics.OverlapSphereNonAlloc(transform.position, 2, results, LayerMask.NameToLayer("NPC"));

            if (size == 1 && !shopUI.gameObject.activeSelf)
                shopUI.Show(_creature);
            else
                shopUI.Hide();
        };
    }

    private void OnDisable()
    {
        Managers.Input.OnLeftClicked -= SelectTarget;
        Managers.Input.OnLeftClicked -= Attack;
        Managers.Input.OnRightClicked -= MoveToPosition;
    }

    private void MoveToPosition(Vector2 mousePosition)
    {
        var ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            _creature.Movement.Destination = hit.point;
            _creature.SkillSystem.CancelReservedSkill();

            if (!_marker)
            {
                var go = Instantiate(marker);
                _marker = go;
            }

            _marker.gameObject.SetActive(true);
            _marker.transform.position = hit.point;
        }
    }

    private void ReserveSkill(SkillSystem skillSystem, Skill skill, TargetSearcher targetSearcher, TargetSelectionResult result)
    {
        if (result.ResultMessage != Define.SearchResultMessage.OutOfRange || !skill.IsInState<SearchingTargetState>())
            return;

        _creature.SkillSystem.ReserveSkill(skill);

        var selectionResult = skill.TargetSelectionResult;

        if (selectionResult.SelectedTarget)
            _creature.Movement.TraceTarget = selectionResult.SelectedTarget.transform;
        else
            _creature.Movement.Destination = selectionResult.SelectedPosition;
    }
    
    private void SelectTarget(Vector2 mousePosition)
    {
        var ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
        {
            var target = hit.transform.GetComponent<Creature>();

            if (target)
            {
                // TODO : 
            }
        }
    }
    
    private void Attack(Vector2 mousePosition)
    {
        if (Managers.UI.IsPointerOverUI || _creature.SkillSystem.BasicSkill == null)
            return;
        
        var ray = Camera.main.ScreenPointToRay(mousePosition);

        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, LayerMask.GetMask("Ground"))) 
            return;

        if (_creature.SkillSystem.BasicSkill.IsUseable)
        {
            _creature.Movement.LookAtImmediate(hit.point);
            _creature.SkillSystem.BasicSkill.Use();
        }
    }
}