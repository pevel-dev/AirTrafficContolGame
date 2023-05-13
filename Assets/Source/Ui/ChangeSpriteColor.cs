using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpriteColor : MonoBehaviour
{
    public Color newColor;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = newColor;
    }
}
