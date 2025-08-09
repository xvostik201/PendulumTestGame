using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Pendulun pendulum;
    [SerializeField] private BallSpawner spawner;

    [Header("Respawn delay")]
    [SerializeField] private float respawnDelay = 2f;
    
    [Header("Aduio")]
    [SerializeField] private AudioCue sfx;

    private bool _canShoot;

    private void OnEnable()
    {
        pendulum.OnBallAttached += OnBallAttached;
    }

    private void OnDisable()
    {
        pendulum.OnBallAttached -= OnBallAttached;
    }

    private void Start()
    {
        var firstRb = spawner.SpawnNextBall();
        pendulum.SetBallToJoint(firstRb);
    }

    private void OnBallAttached(Ball ball)
    {
        _canShoot = true;
    }

    private void Update()
    {
        if (_canShoot && Input.GetMouseButtonDown(0))
        {
            _canShoot = false;
            pendulum.ReleaseBall();
            StartCoroutine(RespawnNextBall());
            sfx.Play();
        }
    }

    private IEnumerator RespawnNextBall()
    {
        yield return new WaitForSeconds(respawnDelay);
        var nextRb = spawner.SpawnNextBall();
        pendulum.SetBallToJoint(nextRb);
    }
}