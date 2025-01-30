using System;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour , IShowWindowUI, IHideWindowUI
{
    [SerializeField] private TMP_Text _leftPlayerScoreText;
    [SerializeField] private TMP_Text _rightPlayerScoreText;

    private PongGameManager _pongGameManager;

    public void Initialize(PongGameManager pongGameManager)
    {
        _pongGameManager = pongGameManager;
        _pongGameManager.OnScoreChange += OnScoreChange;
        _pongGameManager.OnStartGame += OnStartGame;
        _pongGameManager.OnGameOver += OnGameOver;
        _pongGameManager.OnPlayerDisconnected += OnGameOver;
        _pongGameManager.OnShotDown += OnGameOver;

    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnStartGame(object sender, EventArgs e)
    {
        Show();
        UpdateVisual();
    }

    private void OnScoreChange(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void OnGameOver(object sender, EventArgs e)
    {
        Hide();
    }

    private void UpdateVisual()
    {
        _leftPlayerScoreText.text = _pongGameManager.GetLeftPlayerScore().ToString();
        _rightPlayerScoreText.text = _pongGameManager.GetRightPlayerScore().ToString();
    }
    
}
