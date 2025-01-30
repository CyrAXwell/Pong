using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngineRandom = UnityEngine.Random;

public class Ball : NetworkBehaviour
{
    const float MAX_ANGLE_REFLECT = Mathf.PI / 3;
    public event EventHandler OnStartMoving;

    const string PLAYER_LAYER = "Player";
    const string NOTHING_LAYER = "Nothing";

    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _speedIncrease;

    private float _speed;
    private Vector2 _moveDirection;
    private bool _canMove;
    private Rigidbody _rb;
    private bool _inWall;

    public override void OnNetworkSpawn()
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void CanMove()
    {   
        _speed = _minSpeed;
        float angle = UnityEngineRandom.Range(-MAX_ANGLE_REFLECT, MAX_ANGLE_REFLECT);
        float playerSide = UnityEngineRandom.value < 0.5f ? -1f : 1f;
        _moveDirection = new Vector2(playerSide * Mathf.Cos(angle), Mathf.Sin(angle));

        _canMove = true;
        OnStartMoving?.Invoke(this, EventArgs.Empty);
    }

    public void ResetMove(GoalSide goalSide)
    {
        ResetMoveClientRpc();

        _speed =_minSpeed;

        float angle = UnityEngineRandom.Range(-MAX_ANGLE_REFLECT, MAX_ANGLE_REFLECT);
        _moveDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        if (goalSide == GoalSide.leftGoal)
            _moveDirection.x *= -1;
        
        _canMove = true;
        OnStartMoving?.Invoke(this, EventArgs.Empty);
    }

    [ClientRpc]
    private void ResetMoveClientRpc()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.Goal, 1f);
    }

    private void FixedUpdate()
    {
        if (_canMove)
            Move();
    }

    private void Update()
    {
        float collideDistance = 6f;
        if ( Mathf.Abs(transform.position.x) < collideDistance && _rb.excludeLayers != LayerMask.GetMask(NOTHING_LAYER))
            _rb.excludeLayers = LayerMask.GetMask(NOTHING_LAYER);
    }

    private void Move()
    {
        _rb.velocity = new Vector3(_moveDirection.x, 0 , _moveDirection.y) * _speed * Time.fixedDeltaTime;
    }

    public void OnWallBounce()
    {
        _moveDirection.y *= -1; 
        WallBounceClientRpc();
    }

    [ClientRpc]
    private void WallBounceClientRpc()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.WallBounce);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerBounceServerRpc(Vector3 playerPosition, float playerLoaclScaleZ)
    {
        _rb.excludeLayers = LayerMask.GetMask(PLAYER_LAYER);

        if (Mathf.Abs(playerPosition.x) - Mathf.Abs(transform.position.x) >  0)
        {
            float dir = _moveDirection.x;
            float angle = -MAX_ANGLE_REFLECT * (playerPosition.z - transform.position.z) / (playerLoaclScaleZ / 2 + transform.localScale.z / 2);
            _moveDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            if (dir > 0)
                _moveDirection.x *= -1;
            
            _speed = _speed < _maxSpeed ? _speed + _speedIncrease : _maxSpeed;

            if(_inWall)
                _moveDirection.y *= -1;

            PlayerBounceClientRpc();
        }
    }

    [ClientRpc]
    private void PlayerBounceClientRpc()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.PlayerBounce);
    }

    public Vector2 GetMoveDirection () =>  _moveDirection;
    public float GetBallRadius () => transform.localScale.z / 2f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Wall>(out Wall wall))
            _inWall = true;
        
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            if (IsServer)
                PlayerBounceServerRpc(player.transform.position, player.transform.localScale.z);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Wall>(out Wall wall))
            _inWall = false;
    }
}
