using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    [SerializeField]
    private RectTransform canvas;

    [Header("Main")]
    // Default Image
    [SerializeField] private Image mainImage;
    // Default Image의 Fill
    [SerializeField] private Image mainImageFill;
    // Charge에 쓰이는 Fill
    [SerializeField] private Image fillImage;

    [Header("Border")]  
    [SerializeField] private RectTransform leftBorder;
    [SerializeField] private RectTransform rightBorder;

    private float _radius;
    private float _angle = 360f;
    private float _fillAmount;

    public float Radius
    {
        get => _radius;
        set
        {
            _radius = Mathf.Max(value, 0f);
            // 기본 Scale 0.01 * 2 * radius = 0.01 * 2r = 지름
            canvas.localScale = Vector2.one * 0.02f * _radius;
        }
    }

    public float Angle
    {
        get => _angle;
        set
        {
            _angle = Mathf.Clamp(value, 0f, 360f);
            mainImage.fillAmount = _angle / 360f;
            mainImageFill.fillAmount = mainImage.fillAmount; 
            fillImage.fillAmount = mainImage.fillAmount;

            canvas.transform.eulerAngles = new Vector3(90f, -_angle * 0.5f, 0f);

            if (Mathf.Approximately(mainImage.fillAmount, 1f))
            {
                leftBorder.gameObject.SetActive(false);
                rightBorder.gameObject.SetActive(false);
            }
            else
            {
                leftBorder.gameObject.SetActive(true);
                rightBorder.gameObject.SetActive(true);
                rightBorder.transform.localEulerAngles = new Vector3(0f, 0f, 180f - _angle);
            }
        }
    }

    public float FillAmount
    {
        get => _fillAmount;
        set
        {
            _fillAmount = Mathf.Clamp01(value);
            fillImage.transform.localScale = Vector3.one * _fillAmount;
        }
    }

    public Transform TraceTarget
    {
        get => transform.parent;
        set
        {
            transform.parent = value;
            transform.localPosition = new Vector3(0f, 0.01f, 0f);
            transform.localRotation = Quaternion.identity;
        }
    }

    public void Init(float angle, float radius, float fillAmount = 0f, Transform traceTarget = null)
    {
        Angle = angle;
        Radius = radius;
        TraceTarget = traceTarget;
        FillAmount = fillAmount;

        if (traceTarget == null) 
            Managers.Input.OnMousePosition += TraceCursor;
    }

    // private void OnEnable()
    // {
    //     Managers.Input.OnMousePosition -= TraceCursor;
    //
    //     if (TraceTarget == null) 
    //         Managers.Input.OnMousePosition += TraceCursor;
    // }

    private void Update()
    {
        Managers.Input.OnMousePosition -= TraceCursor;
        
        if (TraceTarget == null) 
            Managers.Input.OnMousePosition += TraceCursor;
    }

    private void LateUpdate()
    {
        if (Mathf.Approximately(_angle, 360f))
            transform.rotation = Quaternion.identity;
    }

    private void TraceCursor()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
            transform.position = hitInfo.point + new Vector3(0f, 0.01f);
    }

    private void TraceCursor(Vector2 mousePosition)
    {
        var ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
            transform.position = hitInfo.point + new Vector3(0f, 0.1f);
    }

    public void Clear()
    {
        Angle = 360f;
        Radius = 0f;
        TraceTarget = null;
        FillAmount = 0f;

        Managers.Input.OnMousePosition -= TraceCursor;
    }
    
    // private void OnDisable()
    // { 
    //     Managers.Input.OnMousePosition -= TraceCursor;
    // }
}
