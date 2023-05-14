using System;
using System.Collections;
using System.Collections.Generic;
using Source.Models;
using UnityEngine;


public class HealthBar : MonoBehaviour
{
    private float _maxHealth;
    private float _health;
    private Renderer _material;
    private MaterialPropertyBlock _property;
    private static readonly int Arc1 = Shader.PropertyToID("_Arc1");


    public void Initialize(float seconds)
    {
        _health = seconds;
        _maxHealth = seconds;
        _material = this.GetComponent<Renderer>();
        _property = new MaterialPropertyBlock();
    }

    public void Update()
    {
        _health -= Time.deltaTime;
        if (_health < 0.1) return;
        _property.SetInt(Arc1, 360 - (int)((_health / _maxHealth) * 360));
        _material.SetPropertyBlock(_property);
    }

    public bool Status()
    {
        return _health >= 0;
    }
}
