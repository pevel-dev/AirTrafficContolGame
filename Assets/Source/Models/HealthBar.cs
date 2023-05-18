using UnityEngine;


public class HealthBar : MonoBehaviour
{
    private float _maxHealth;
    private float _health;
    private Renderer _material;
    private MaterialPropertyBlock _property;
    private static readonly int Arc1 = Shader.PropertyToID("_Arc1");
    private SpriteRenderer _healBarRenderer;

    public void Initialize(float seconds)
    {
        _health = seconds;
        _maxHealth = seconds;
        _material = GetComponent<Renderer>();
        _property = new MaterialPropertyBlock();
        _healBarRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        _health -= Time.deltaTime;
        if (_health < 0.1) 
            return;
        
        _healBarRenderer.color = Color.Lerp(Color.red, Color.blue, _health/_maxHealth);
        _property.SetInt(Arc1, 360 - (int)((_health / _maxHealth) * 360));
        _material.SetPropertyBlock(_property);
    }

    public bool Status()
    {
        return _health >= 0;
    }
}
