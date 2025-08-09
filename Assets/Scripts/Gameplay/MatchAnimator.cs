using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MatchAnimator : MonoBehaviour
{
    public static MatchAnimator Instance { get; private set; }

    [SerializeField] private ParticleSystem particlePrefab;
    [SerializeField] private float moveDuration = 0.35f;
    [SerializeField] private float scaleDuration = 0.25f;
    [SerializeField] private AudioCue sfx;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AnimateMatch(List<Ball> matched, Action onComplete)
    {
        if (matched == null || matched.Count < 3) { onComplete?.Invoke(); return; }

        var a = matched[0];
        var b = matched[1]; 
        var c = matched[2];

        var centerPos = b.transform.position;
        var color = b.GetComponent<SpriteRenderer>().color;

        DisablePhysics(a);
        DisablePhysics(b);
        DisablePhysics(c);

        a.transform.DOKill();
        b.transform.DOKill();
        c.transform.DOKill();

        var seq = DOTween.Sequence();

        seq.Join(a.transform.DOMove(centerPos, moveDuration).SetEase(Ease.InOutQuad));
        seq.Join(c.transform.DOMove(centerPos, moveDuration).SetEase(Ease.InOutQuad));

        seq.Append(a.transform.DOScale(Vector3.zero, scaleDuration).SetEase(Ease.InBack));
        seq.Join  (b.transform.DOScale(Vector3.zero, scaleDuration).SetEase(Ease.InBack));
        seq.Join  (c.transform.DOScale(Vector3.zero, scaleDuration).SetEase(Ease.InBack));

        seq.OnComplete(() =>
        {
            var ps = Instantiate(particlePrefab, centerPos, Quaternion.identity);
            var main = ps.main;
            main.startColor = color;
            ps.Play();
            Destroy(ps.gameObject, main.duration + main.startLifetime.constantMax);

            Destroy(a.gameObject);
            Destroy(b.gameObject);
            Destroy(c.gameObject);

            onComplete?.Invoke();
            
            sfx.Play();
        });
    }

    private void DisablePhysics(Ball b)
    {
        var col = b.GetComponent<Collider2D>();
        var rb  = b.GetComponent<Rigidbody2D>();
        if (col) col.enabled = false;
        if (rb) { rb.velocity = Vector2.zero; rb.angularVelocity = 0f; rb.isKinematic = true; }
    }
}
