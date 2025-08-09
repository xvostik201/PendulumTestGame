using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Ball : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Color _color;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (_color == default)
            SetColor(ColorSystem.Instance.GetRandomColor());
    }

    public void SetColor(Color c)
    {
        _color = c;
        _spriteRenderer.color = c;
    }

    public Color GetColor()
    {
        return _color;
    }
}