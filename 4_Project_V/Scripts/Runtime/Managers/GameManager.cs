using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameManager
{
    private Define.ScreenMode _screenMode;
    private Define.GameSpeed _gameSpeed;

    public Define.ScreenMode ScreenMode
    {
        get => _screenMode;
        set
        {
            _screenMode = value;
            switch (_screenMode)
            {
                case Define.ScreenMode.FullScreen:
                    Screen.fullScreen = true;
                    break;
                case Define.ScreenMode.Windowed:
                    Screen.fullScreen = false;
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
            }
        }
    }

    public Define.GameSpeed GameSpeed
    {
        get => _gameSpeed;
        set
        {
            _gameSpeed = value;
            Time.timeScale = (int)_gameSpeed;
        }
    }

    private Inventory _inventory;
    public Creature Player { get; set; }

    public Inventory Inventory
    {
        get
        {
            if (_inventory == null)
                Object.FindObjectOfType<Inventory>().Init();
            
            return _inventory;
        }
        set => _inventory = value;
    }

    public Shop Shop { get; set; }

    public Action<Equipment> OnWeaponEquiped;

    public void Init()
    {
        _gameSpeed = Define.GameSpeed.Speed1X;
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif

    }
}