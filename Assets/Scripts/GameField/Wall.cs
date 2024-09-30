using UnityEngine;

public class Wall : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Ball>(out Ball ball))
        {
            ball.OnWallBounce();
        }
    }
}
