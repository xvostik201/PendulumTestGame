using UnityEngine;
using System;

[RequireComponent(typeof(LineRenderer), typeof(SpringJoint2D))]
public class Pendulun : MonoBehaviour
{
    public event Action<Ball> OnBallAttached;

    private LineRenderer _lineRenderer;
    private SpringJoint2D _springJoint;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _springJoint = GetComponent<SpringJoint2D>();
    }

    private void LateUpdate()
    {
        if (_springJoint.connectedBody == null)
            return;

        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _springJoint.connectedBody.transform.position);
    }


    public void SetBallToJoint(Rigidbody2D ballRb)
    {
        _springJoint.connectedBody = ballRb;
        _lineRenderer.enabled = true;
        if (ballRb.TryGetComponent(out Ball ball))
            OnBallAttached?.Invoke(ball);
    }

    public void ReleaseBall()
    {
        _springJoint.connectedBody = null;
        _lineRenderer.enabled = false;
    }
}