using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance { get; private set; }

    [SerializeField] private ColumnTrigger[] columns;
    [SerializeField] private float[] rowY = new float[3];
    [SerializeField] private float placeDuration = 0.12f;
    [SerializeField] private float collapseDuration = 0.15f;

    [SerializeField] private int pointsColor0 = 50;
    [SerializeField] private int pointsColor1 = 100;
    [SerializeField] private int pointsColor2 = 200;
    [SerializeField] private int defaultPoints = 50;
    [SerializeField, Range(0f,1f)] private float colorTolerance = 0.01f;

    private Ball[,] grid = new Ball[3,3];

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public bool HasSpaceInColumn(int column) => FirstEmptyRow(column) != -1;

    public void TryPlaceInColumn(int column, Ball ball)
    {
        int row = FirstEmptyRow(column);
        if (row == -1) { GameManager.Instance.GameOver(); return; }

        var rb = ball.GetComponent<Rigidbody2D>();
        var col = ball.GetComponent<Collider2D>();
        if (rb) { rb.velocity = Vector2.zero; rb.angularVelocity = 0f; rb.isKinematic = true; }
        if (col) col.enabled = false;

        Vector3 target = new Vector3(columns[column].transform.position.x, rowY[row], ball.transform.position.z);

        ball.transform.DOKill();
        ball.transform.DOMove(target, placeDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            grid[column, row] = ball;

            var triplets = FindMatchTriplets();
            if (triplets.Count > 0)
            {
                ScoreManager.Instance.AddScore(CalcPointsForTriplets(triplets));
                RemoveFromGrid(triplets);
                AnimateTripletsSequentially(triplets, () =>
                {
                    CollapseAll(() =>
                    {
                        var more = FindMatchTriplets();
                        if (more.Count > 0)
                        {
                            ScoreManager.Instance.AddScore(CalcPointsForTriplets(more));
                            RemoveFromGrid(more);
                            AnimateTripletsSequentially(more, () => CollapseAll(CheckFullAndMaybeLose));
                        }
                        else
                        {
                            CheckFullAndMaybeLose();
                        }
                    });
                });
            }
            else
            {
                CheckFullAndMaybeLose();
            }
        });
    }

    private void CheckFullAndMaybeLose()
    {
        if (IsFull()) GameManager.Instance.GameOver();
    }

    private int FirstEmptyRow(int c)
    {
        for (int r = 0; r < 3; r++) if (grid[c,r] == null) return r;
        return -1;
    }

    private List<List<Ball>> FindMatchTriplets()
    {
        var res = new List<List<Ball>>();
        var used = new HashSet<Ball>();

        TryLine(res, used, grid[0,0], grid[1,0], grid[2,0]); 
        TryLine(res, used, grid[0,1], grid[1,1], grid[2,1]); 
        TryLine(res, used, grid[0,2], grid[1,2], grid[2,2]);

        TryLine(res, used, grid[0,0], grid[0,1], grid[0,2]); 
        TryLine(res, used, grid[1,0], grid[1,1], grid[1,2]); 
        TryLine(res, used, grid[2,0], grid[2,1], grid[2,2]); 

        TryLine(res, used, grid[0,0], grid[1,1], grid[2,2]); 
        TryLine(res, used, grid[0,2], grid[1,1], grid[2,0]); 

        return res;
    }

    private void TryLine(List<List<Ball>> acc, HashSet<Ball> used, Ball a, Ball b, Ball c)
    {
        if (!a || !b || !c) return;
        if (used.Contains(a) || used.Contains(b) || used.Contains(c)) return;
        var col = a.GetColor();
        if (b.GetColor() != col || c.GetColor() != col) return;
        acc.Add(new List<Ball>{ a, b, c });
        used.Add(a); used.Add(b); used.Add(c);
    }

    private void RemoveFromGrid(List<List<Ball>> triplets)
    {
        foreach (var t in triplets)
            for (int c = 0; c < 3; c++)
                for (int r = 0; r < 3; r++)
                    if (grid[c,r] && (grid[c,r] == t[0] || grid[c,r] == t[1] || grid[c,r] == t[2]))
                        grid[c,r] = null;
    }

    private void AnimateTripletsSequentially(List<List<Ball>> triplets, Action onAllDone)
    {
        int i = 0;
        void Next()
        {
            if (i >= triplets.Count) { onAllDone?.Invoke(); return; }
            var trio = triplets[i++];
            MatchAnimator.Instance.AnimateMatch(trio, Next);
        }
        Next();
    }

    private void CollapseAll(Action onComplete)
    {
        var seq = DOTween.Sequence();
        for (int c = 0; c < 3; c++)
        {
            var keep = new List<Ball>(3);
            for (int r = 0; r < 3; r++) if (grid[c,r] != null) keep.Add(grid[c,r]);
            for (int r = 0; r < 3; r++) grid[c,r] = null;

            for (int i = 0; i < keep.Count; i++)
            {
                var b = keep[i];
                grid[c,i] = b;
                Vector3 pos = new Vector3(columns[c].transform.position.x, rowY[i], b.transform.position.z);
                b.transform.DOKill();
                seq.Join(b.transform.DOMove(pos, collapseDuration).SetEase(Ease.InOutQuad));
            }
        }
        seq.OnComplete(() => onComplete?.Invoke());
    }

    private bool IsFull()
    {
        for (int c = 0; c < 3; c++) if (FirstEmptyRow(c) != -1) return false;
        return true;
    }

    private int CalcPointsForTriplets(List<List<Ball>> triplets)
    {
        int total = 0;
        foreach (var t in triplets)
        {
            var col = t[0].GetComponent<SpriteRenderer>().color;
            total += PointsForColor(col);
        }
        return total;
    }

    private int PointsForColor(Color color)
    {
        var palette = ColorSystem.Instance ? ColorSystem.Instance.AllColors : null;
        if (palette != null && palette.Length > 0)
        {
            if (palette.Length > 0 && Approximately(color, palette[0])) return pointsColor0;
            if (palette.Length > 1 && Approximately(color, palette[1])) return pointsColor1;
            if (palette.Length > 2 && Approximately(color, palette[2])) return pointsColor2;
        }
        return defaultPoints;
    }

    private bool Approximately(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) <= colorTolerance &&
               Mathf.Abs(a.g - b.g) <= colorTolerance &&
               Mathf.Abs(a.b - b.b) <= colorTolerance &&
               Mathf.Abs(a.a - b.a) <= colorTolerance;
    }
}
