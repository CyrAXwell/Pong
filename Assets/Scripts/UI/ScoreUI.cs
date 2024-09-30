using System;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour , IShowWindowUI, IHideWindowUI
{
    [SerializeField] private TMP_Text _leftPlayerScoreText;
    [SerializeField] private TMP_Text _rightPlayerScoreText;

    private PongGameManager _gameManager;

    public void Initialize(PongGameManager gameManager)
    {
        _gameManager = gameManager;
        _gameManager.OnScoreChange += OnScoreChange;
        _gameManager.OnStartGame += OnStartGame;
        _gameManager.OnGameOver += OnGameOver;
        _gameManager.OnEndTheGame += OnGameOver;

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
        _leftPlayerScoreText.text = _gameManager.GetLeftPlayerScore().ToString();
        _rightPlayerScoreText.text = _gameManager.GetRightPlayerScore().ToString();
    }
}
