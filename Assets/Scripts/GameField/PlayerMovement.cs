using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;

    private Rigidbody _rb;
    private IMoveControllable _gameInput;
    private Player _player;
    private Vector2 _inputVector;
    private bool _canMove;
    private PongGameManager _pongGameManager;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Player player, IMoveControllable gameInput, PongGameManager pongGameManager)
    {
        _gameInput = gameInput;
        _player = player;
        _pongGameManager = pongGameManager;
        _pongGameManager.OnStartGame += OnStartGame;
        _pongGameManager.OnGameOver += OnGameOver;
    }

    private void OnGameOver(object sender, EventArgs e)
    {
        _canMove = false;
        _rb.velocity =Vector3.zero;
    }

    private void OnStartGame(object sender, EventArgs e)
    {
        _canMove = true;
    }

    private void FixedUpdate()
    {
        if (_canMove)
            Move();
    }

    private void Update()
    {
        _inputVector = _gameInput.GetMoveVector(_player);
    }

    private void Move()
    {
        _rb.velocity = new Vector3(0, 0, _inputVector.y) * speed * Time.fixedDeltaTime;
    }

    private void OnDestroy()
    {
        _pongGameManager.OnStartGame -= OnStartGame;
        _pongGameManager.OnGameOver -= OnGameOver;
    }
}
