using UnityEngine;
using System.Collections.Generic;

public class BallSpawner : MonoBehaviour
{
    public static BallSpawner Instance { get; private set; }

    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private int distinctColors = 3;
    [SerializeField] private int repeatCount = 3;
    
    [SerializeField] private Vector2[] spawnPoints;

    private Queue<Color> _spawnQueue = new Queue<Color>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        GenerateQueue();
    }

    private void GenerateQueue()
    {
        _spawnQueue.Clear();
        var palette = ColorSystem.Instance.AllColors;
        var list = new List<Color>();

        for (int i = 0; i < distinctColors && i < palette.Length; i++)
        for (int j = 0; j < repeatCount; j++)
            list.Add(palette[i]);

        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }

        foreach (var color in list)
            _spawnQueue.Enqueue(color);
    }

    public Rigidbody2D SpawnNextBall()
    {
        if (_spawnQueue.Count == 0)
            GenerateQueue();
        
        bool rightSpawnPos = Random.Range(0, 2) == 0;
        
        Vector2 posToSpawn = rightSpawnPos ? spawnPoints[1] : spawnPoints[0];

        var nextColor = _spawnQueue.Dequeue();
        var go = Instantiate(ballPrefab, posToSpawn, Quaternion.identity);
        var ball = go.GetComponent<Ball>();
        ball.SetColor(nextColor);
        return go.GetComponent<Rigidbody2D>();
    }
}