using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarPoint : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private const float alphaDelta = 0.0035f;
    private const float alphaMinLimit = 0.0f;
    private const float alphaMaxLimit = 1.0f;
    
    private float alpha = alphaMaxLimit;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }   
    void Update()
    {
        var color = spriteRenderer.color;
        Debug.Log(color);
        spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
        alpha -= alphaDelta;
        if (alpha <= alphaMinLimit)
            alpha = alphaMaxLimit;
    }
}
