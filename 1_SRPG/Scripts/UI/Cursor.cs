using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ShowCursor()
    {
        _spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void HideCursor()
    {
        _spriteRenderer.color = new Color(1, 1, 1, 0);
    }
}
