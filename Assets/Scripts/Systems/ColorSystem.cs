using UnityEngine;

public class ColorSystem : MonoBehaviour
{
    public static ColorSystem Instance { get; private set; }

    [SerializeField] private Color[] allColors;
    public Color[] AllColors => allColors;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public Color GetRandomColor()
    {
        if (allColors == null || allColors.Length == 0) return Color.white;
        return allColors[Random.Range(0, allColors.Length)];
    }
}