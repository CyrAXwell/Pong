using System;
using System.Linq;
using UnityEngine;
using UnityEngineRandom = UnityEngine.Random;

public class PongAILeading : IPongAIMovement
{
    private Ball _ball;
    private Player _player;
    private Player _otherPlayer;
    private Vector3 _targetPosition;
    private bool _canMove;
    private bool _hasMoveVector;
    private Vector2 _moveVector;


    public PongAILeading(Ball ball, Player player, Player otherPlayer)
    {
        _ball = ball;
        _player = player;
        _otherPlayer = otherPlayer;
        _ball.OnStartMoving += OnBallStartMoving;
        _otherPlayer.OnBallCollide += OnBallCollideOtherPlayer;
    }

    private void OnBallStartMoving(object sender, EventArgs e)
    {
        if (_ball.GetMoveDirection().x * _player.transform.position.x > 0)
        {
            GetTargetPosition();
            _canMove = true;
            _hasMoveVector = false;
        }
    }

    private void OnBallCollideOtherPlayer(object sender, EventArgs e)
    {
        GetTargetPosition();
        _canMove = true;
        _hasMoveVector = false;
    }
    
    private Vector3 GetRaycastOriginOffset(RaycastHit hit, Vector3 direction)
    {
        Vector2 dir = new Vector2(direction.x, direction.z);
        Vector2 normal = new Vector2(hit.normal.x, hit.normal.z);
        float distance = _ball.GetBallRadius() / Mathf.Sin(Mathf.Deg2Rad * (90 - Vector2.Angle(- dir, normal)));

        return distance * direction.normalized;
    }

    public Vector2 GetMoveVector()
    {
        if (_canMove)
        {
            if(!_hasMoveVector)
            {
                if (_player.transform.position.z > _targetPosition.z)
                    _moveVector = new Vector2(0, -1f);
                else
                    _moveVector = new Vector2(0, 1f);
                
                _hasMoveVector = true;
            }   

            if (_moveVector.y < 0 && _player.transform.position.z < _targetPosition.z || _moveVector.y > 0 && _player.transform.position.z > _targetPosition.z)
            {
                _canMove = false;
            }
            
            return  _moveVector;
        }
        else
        {
            return  Vector2.zero;
        }
    }

    private void GetTargetPosition()
    {
        Vector3 origin = _ball.transform.position;
        Vector3 direction = new Vector3(_ball.GetMoveDirection().x, 0, _ball.GetMoveDirection().y);

        int maxRaycastCount = 10;
        while(maxRaycastCount > 0)
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(origin, direction);

            RaycastHit wallHit = hits.Where(hit => hit.collider.TryGetComponent<Wall>(out Wall wall)).FirstOrDefault();
            RaycastHit goalHit = hits.Where(hit => hit.collider.TryGetComponent<PlayerGoal>(out PlayerGoal goal)).FirstOrDefault();

            if ( (wallHit.collider != null && goalHit.collider != null && Mathf.Abs(wallHit.point.x) < Mathf.Abs(goalHit.point.x)) || wallHit.collider != null && goalHit.collider == null)
            {
                Debug.DrawRay(origin, direction * wallHit.distance - GetRaycastOriginOffset(wallHit, direction), Color.yellow, 100f);
                origin = wallHit.point - GetRaycastOriginOffset(wallHit, direction);
                direction.z *= -1;

            }
            else if (goalHit.collider != null)
            {
                _targetPosition = goalHit.point;
                _targetPosition.z += UnityEngineRandom.Range(-_player.transform.localScale.z / 2, _player.transform.localScale.z / 2);

                Debug.DrawRay(origin, direction * goalHit.distance - GetRaycastOriginOffset(goalHit, direction), Color.red, 100f);
                break;
            }
            maxRaycastCount --;
        }
    }
    
    
}
