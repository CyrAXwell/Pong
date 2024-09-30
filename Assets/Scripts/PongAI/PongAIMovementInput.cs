using UnityEngine;

public class PongAIMovementInput : IMoveControllable
{
    private Ball _ball;
    private IPongAIMovement _pongAIMovement;
    private PongAIDifficultySO _pongAIDifficultySO;

    public  PongAIMovementInput(Ball ball, PongAIDifficultySO pongAIDifficultySO, Player player, Player otherPlayer)
    {
        _ball = ball;
        _pongAIDifficultySO = pongAIDifficultySO;
        _pongAIMovement = new PongAILeading(_ball, player, otherPlayer);
    }

    public Vector2 GetMoveVector(Player player)
    {
        Vector2 inputVector = _pongAIMovement.GetMoveVector();
        inputVector *= _pongAIDifficultySO.SpeedMultiplier;

        return inputVector;
    }
}
