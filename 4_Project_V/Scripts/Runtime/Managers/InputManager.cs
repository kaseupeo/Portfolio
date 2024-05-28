using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Elements = Define.InterfaceElements;

public class InputManager
{
    private PlayerControls _controls;
    
    public Action<int> CameraAction;
    public Action<float> ScrollAction;
    public delegate void MouseHandler(Vector2 mousePosition);
    public event MouseHandler OnMousePosition;
    public event MouseHandler OnLeftClicked;
    public event MouseHandler OnRightClicked;

    public InputAction ChangedWeapon { get; set; }
    public Dictionary<int, InputAction> SkillAction { get; set; } = new();
    public Dictionary<int, InputAction> ItemAction { get; set; } = new();

    public InputAction RightMousePressAction { get; set; }
    public InputAction LeftMousePressAction { get; set; }
    public Dictionary<Elements, InputAction> UIPressActionDic { get; set; } = new();

    public InputAction Interaction { get; set; }

    public void Init()
    {
        _controls = new PlayerControls();
        _controls.Enable();
        
        BindMove();
        BindChangedWeapon();
        BindSkill();
        BindItem();
        BindUI();
        BindInteraction();
    }

    private void BindMove()
    {
        RightMousePressAction = _controls.PC.Move;
        LeftMousePressAction = _controls.PC.Targeting;
        
        // 캐릭터 이동
        _controls.PC.Move.performed += context => OnRightClicked?.Invoke(context.ReadValue<Vector2>());
        _controls.PC.MousePosition.performed += context => OnMousePosition?.Invoke(context.ReadValue<Vector2>());
        _controls.PC.Targeting.performed += context => OnLeftClicked?.Invoke(context.ReadValue<Vector2>());

        // 카메라 회전
        _controls.PC.CounterclockwiseRotationCamera.performed += _ => CameraAction.Invoke(-90);
        _controls.PC.ClockwiseRotationCamera.performed += _ => CameraAction.Invoke(90);
        
        // 카메라 줌 인/아웃
        _controls.PC.Zoom.performed += context => ScrollAction?.Invoke(context.ReadValue<float>());
    }

    private void BindChangedWeapon()
    {
        ChangedWeapon = _controls.PC.ChangeWeapon;
    }

    private void BindSkill()
    {
        SkillAction[1] = _controls.PC.SkillQuickSlot_1;
        SkillAction[2] = _controls.PC.SkillQuickSlot_2;
        SkillAction[3] = _controls.PC.SkillQuickSlot_3;
        SkillAction[4] = _controls.PC.SkillQuickSlot_4;
        SkillAction[5] = _controls.PC.SkillQuickSlot_5;
        SkillAction[6] = _controls.PC.SkillQuickSlot_6;
    }

    private void BindItem()
    {
        ItemAction[1] = _controls.PC.ItemQuickSlot_1;
        ItemAction[2] = _controls.PC.ItemQuickSlot_2;
        ItemAction[3] = _controls.PC.ItemQuickSlot_3;
        ItemAction[4] = _controls.PC.ItemQuickSlot_4;
        ItemAction[5] = _controls.PC.ItemQuickSlot_5;
        ItemAction[6] = _controls.PC.ItemQuickSlot_6;
    }

    private void BindUI()
    {
        UIPressActionDic[Elements.Map] = _controls.PC.Map;
        UIPressActionDic[Elements.Inventory] = _controls.PC.Inventory;
        UIPressActionDic[Elements.Quest] = _controls.PC.Quest;
        UIPressActionDic[Elements.Skill] = _controls.PC.Skill;
        UIPressActionDic[Elements.Stat] = _controls.PC.Stat;
        UIPressActionDic[Elements.MainMenu] = _controls.PC.MainMenu;
    }

    private void BindInteraction()
    {
        Interaction = _controls.PC.Interaction;
    }
    
    public void Clear()
    {
        _controls.Disable();
    }
}