using System;
using UnityEngine;

public class PongGameManager : MonoBehaviour
{
    public event EventHandler OnScoreChange;
    public event EventHandler OnStartGame;
    public event EventHandler OnGameOver;
    public event EventHandler OnEndTheGame;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;

    [SerializeField] private Player _playerPrafab;
    [SerializeField] private Transform[] _playerSpawnPointArray;
    [SerializeField] private Ball _ballPrefab;
    [SerializeField] private Transform _ballSpawnPoint;
    [SerializeField] private PlayerGoal _leftGoal;
    [SerializeField] private PlayerGoal _rightGoal;
    
    private GameInput _gameInput;
    private int _playerAmount;
    private Ball _ball;
    private int _leftPlayerScore;
    private int _rightPlayerScore;
    private int _targetGoal;
    private Player[] _players = new Player[2];
    private PongAIDifficultySO _difficulty;
    private bool _isGamePaused;
    private bool _gameIsStarted;

    public void Initialize(GameInput gameInput)
    {
        _gameInput = gameInput;
        _gameInput.OnPauseAction += OnGamePauseGameInputAction;
        _leftGoal.OnGoal += OnGoal;
        _rightGoal.OnGoal += OnGoal;
    }

    public void StartGame(int players, int targetGoal, PongAIDifficultySO difficulty)
    {
        _playerAmount = players;
        _targetGoal = targetGoal;
        _difficulty = difficulty;

        InitializeGame();
    }
    
    public void RestartGame()
    {   
        DestroyPlayers();
        InitializeGame();
    }

    public void EndTheGame()
    {
        Destroy(_ball.gameObject);
        DestroyPlayers();
        _gameIsStarted = false;
        
        OnEndTheGame?.Invoke(this, EventArgs.Empty);
    }

    public void DestroyPlayers()
    {
        foreach (Player player in _players)
            Destroy(player.gameObject);
    }

    public int GetLeftPlayerScore() => _leftPlayerScore;
    public int GetRightPlayerScore() => _rightPlayerScore;

    private void InitializeGame()
    {
        InitializeBall();
        InitializePlayers();
        ResetGameScore();
        _ball.CanMove();
        _gameIsStarted = true;

        OnStartGame?.Invoke(this, EventArgs.Empty);
    }

    private void InitializeBall()
    {
        _ball = Instantiate(_ballPrefab, _ballSpawnPoint.position, Quaternion.identity);
        _ball.Initialize();
    }

    private void InitializePlayers()
    {
        for (int i = 0; i < 2; i++)
        {
            _players[i] = Instantiate(_playerPrafab, _playerSpawnPointArray[i].position, Quaternion.identity);
            _players[i].Initialize(i);
        }

        for (int i = 0; i < _playerAmount; i++)
            _players[i].GetComponent<PlayerMovement>().Initialize(_players[i] , _gameInput, this);

        for (int i = _playerAmount; i < 2; i++)
        {
            PongAIMovementInput pongAIMovementInput = new PongAIMovementInput(_ball, _difficulty, _players[i], GetOtherPlayer(i));
            _players[i].GetComponent<PlayerMovement>().Initialize(_players[i] , pongAIMovementInput, this);
        }
    }

    private void OnGoal(object sender, PlayerGoal.OnGoalEventArgs e)
    {
        AddScore(e.goalSide);
        _ball.transform.position = _ballSpawnPoint.position;
        _ball.ResetMove(e.goalSide);

        OnScoreChange?.Invoke(this, EventArgs.Empty);
        if (_leftPlayerScore >= _targetGoal || _rightPlayerScore >= _targetGoal)
        {   
            _gameIsStarted = false;
            Destroy(_ball.gameObject);

            for (int i = 0; i < _players.Length; i++)
                _players[i].transform.position = _playerSpawnPointArray[i].position;


            OnGameOver?.Invoke(this, EventArgs.Empty);
        }
    }

    private void ResetGameScore()
    {
        _leftPlayerScore = 0;
        _rightPlayerScore = 0;
    }

    private void AddScore(GoalSide goalSide)
    {
        if (goalSide == GoalSide.rightGoal)
            _leftPlayerScore++;
        else
            _rightPlayerScore++;
    }

    private Player GetOtherPlayer(int index)
    {
        if (index == 0)
            return _players[1];
        else
            return _players[0];
    }

    private void OnGamePauseGameInputAction(object sender, EventArgs e)
    {
        if (_gameIsStarted)
            TogglePauseGame();
    }

    public void TogglePauseGame()
    {
        _isGamePaused = !_isGamePaused;
        if (_isGamePaused)
        {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnDestroy()
    {
        _leftGoal.OnGoal -= OnGoal;
        _rightGoal.OnGoal -= OnGoal;
    }
}
