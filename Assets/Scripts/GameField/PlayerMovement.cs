using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float speed;

    private Rigidbody _rb;
    private IMoveControllable _gameInput;
    private Player _player;
    private Vector2 _inputVector;
    private bool _canMove;
    private PongGameManager _pongGameManager;
    private Vector3 _spawnPosition;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
        _rb.isKinematic = false;
    }

    public void Initialize(IMoveControllable gameInput, PongGameManager pongGameManager, Vector3 spawnPosition)
    {
        _gameInput = gameInput;
        _pongGameManager = pongGameManager;
        _spawnPosition = spawnPosition;
        //SetPlayerSpawnPositionServerRpc(_spawnPosition);
        transform.position = _spawnPosition;

        _pongGameManager.OnStartGame += OnStartGame;
        _pongGameManager.OnGameOver += OnGameOver;
        _pongGameManager.OnPlayerDisconnected += OnGameOver;
        _pongGameManager.OnResetGame += OnResetGame;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerSpawnPositionServerRpc(Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
    }

    private void OnGameOver(object sender, EventArgs e)
    {
        _canMove = false;
        StopMoveServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StopMoveServerRpc()
    {
        _rb.velocity = Vector3.zero;
    }

    private void OnStartGame(object sender, EventArgs e)
    {
        _canMove = true;
    }

    private void Update()
    {
        if (!IsOwner)
            return;
        if (_canMove)
        {
            Move();
            //MoveServerAuth();
        }
    }

    private void MoveServerAuth()
    {
        _inputVector = _gameInput.GetMoveVector(_player);
        MoveServerRpc(_inputVector);
    }

    [ServerRpc(RequireOwnership = false)]
    private void MoveServerRpc(Vector2 inputVector)
    {
        _rb.velocity = new Vector3(0, 0, inputVector.y) * speed * Time.fixedDeltaTime;
    }
    

    private void Move()
    {
        _inputVector = _gameInput.GetMoveVector(_player);
        _rb.velocity = new Vector3(0, 0, _inputVector.y) * speed * Time.fixedDeltaTime;
    }

    private void OnResetGame(object sender, EventArgs e)
    {
        if (!IsOwner)
            return;
        transform.position = _spawnPosition;
    }

    public override void OnDestroy()
    {
        if (!IsOwner)
            return;
        _pongGameManager.OnStartGame -= OnStartGame;
        _pongGameManager.OnGameOver -= OnGameOver;
        _pongGameManager.OnPlayerDisconnected -= OnGameOver;
        _pongGameManager.OnResetGame -= OnResetGame;
    }
}
