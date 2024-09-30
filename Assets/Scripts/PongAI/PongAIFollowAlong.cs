using UnityEngine;

public class PongAIFollowAlong : IPongAIMovement
{
    private const float MOVE_DEAD_ZONE = 0.2f;

    private Ball _ball;
    private Player _player;

    public PongAIFollowAlong(Ball ball, Player player, Player otherPlayer)
    {
        _ball = ball;
        _player = player;
    }

    public Vector2 GetMoveVector()
    {
        Vector3 playerPosition = _player.transform.position;
        Vector3 targetPosition = _ball.transform.position;

        Vector2 moveVector = Vector2.zero;
        if (playerPosition.z > targetPosition.z && (playerPosition.z - targetPosition.z > MOVE_DEAD_ZONE))
            moveVector = new Vector2(0, -1f);
        else if (playerPosition.z < targetPosition.z && (playerPosition.z - targetPosition.z < MOVE_DEAD_ZONE))
            moveVector = new Vector2(0, 1f);

        return  moveVector;
    }
}
