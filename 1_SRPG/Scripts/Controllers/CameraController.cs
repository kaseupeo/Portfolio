using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    private Camera _camera;
    private PlayerActions _playerActions;
    private Vector2 _cameraPosition;
    private Define.CameraMode _cameraMode;
    
    [SerializeField] private float cameraMoveSpeed;
    [SerializeField] private float cameraMoveArea;
    
    private void Awake()
    {
        _playerActions = new PlayerActions();
    }
    
    private void Start()
    {
        _camera = Camera.main;
        _playerActions.Camera.MousePosition.performed += cameraPos => _cameraPosition = CameraMoveByMouse(cameraPos.ReadValue<Vector2>());
        _playerActions.Camera.KeyBoardPosition.performed += cameraPos => _cameraPosition = CameraMoveByKeyboard(cameraPos.ReadValue<Vector2>());
        _playerActions.Camera.CharacterPosition.performed += _ => _camera.transform.position = FocusSelectedCharacter();
        _playerActions.Camera.MouseScroll.performed += scroll => CameraZoom(scroll.ReadValue<float>() * 0.0002f);
    }
    
    private void LateUpdate()
    {
        _camera.transform.Translate(_cameraPosition * (Time.deltaTime * cameraMoveSpeed));

        if (_cameraMode == Managers.Game.CameraMode)
            return;

        switch (Managers.Game.CameraMode)
        {
            case Define.CameraMode.Both:
                _playerActions.Camera.MousePosition.Enable();
                _playerActions.Camera.KeyBoardPosition.Enable();
                _cameraMode = Define.CameraMode.Both;
                break;
            case Define.CameraMode.Mouse:
                _playerActions.Camera.MousePosition.Enable();
                _playerActions.Camera.KeyBoardPosition.Disable();
                _cameraMode = Define.CameraMode.Mouse;
                break;
            case Define.CameraMode.Keyboard:
                _playerActions.Camera.MousePosition.Disable();
                _playerActions.Camera.KeyBoardPosition.Enable();
                _cameraMode = Define.CameraMode.Keyboard;
                break;
        }
    }
    
    private Vector2 CameraMoveByMouse(Vector2 cameraPos)
    {
        Vector2 position = Vector2.zero;

        if (!Managers.Game.IsFullScreenMode && (!(cameraPos.x > 0) || !(cameraPos.y > 0) || !(cameraPos.x < Screen.width) ||
             !(cameraPos.y < Screen.height)))
            return Vector2.zero;

        float xRate = cameraPos.x / Screen.width;
        float yRate = cameraPos.y / Screen.height;

        if (xRate < cameraMoveArea)
            position += Vector2.left;
        if (xRate > 1 - cameraMoveArea) 
            position += Vector2.right;
        if (yRate < cameraMoveArea)
            position += Vector2.down;
        if (yRate > 1 - cameraMoveArea) 
            position += Vector2.up;

        return position;
    }

    private Vector2 CameraMoveByKeyboard(Vector2 cameraPos)
    {
        return cameraPos;
    }

    private Vector3 FocusSelectedCharacter()
    {
        if (Managers.Game.SelectedCharacter == null)
        {
            return _camera.transform.position;
        }
        var focusPos = Managers.Game.SelectedCharacter.transform.position;
        return new Vector3(focusPos.x, focusPos.y, -10);
    }

    private void CameraZoom(float scroll)
    {
        float zoomY = 0.1f;
        if (scroll > 0)
        {
            _camera.orthographicSize += -zoomY;
        }
        else if (scroll < 0)
        {
            _camera.orthographicSize += zoomY;
        }
        
        if (_camera.orthographicSize < 1)
        {
            _camera.orthographicSize = 1;
        }
        else if (_camera.orthographicSize > 10)
        {
            _camera.orthographicSize = 10;
        }
    }
    
    private void OnEnable()
    {
        _playerActions.Enable();
    }

    private void OnDisable()
    {
        _playerActions.Disable();
    }
}