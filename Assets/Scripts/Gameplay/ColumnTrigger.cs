using UnityEngine;

public class ColumnTrigger : MonoBehaviour
{
    [SerializeField] private int columnIndex;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out Ball b)) return;

        if (!MatchManager.Instance.HasSpaceInColumn(columnIndex))
        {
            GameManager.Instance.GameOver();
            return;
        }

        MatchManager.Instance.TryPlaceInColumn(columnIndex, b);
    }
}