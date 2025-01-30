using System;
using UnityEngine;

public class PauseMultiplayerUI : MonoBehaviour, IShowWindowUI, IHideWindowUI
{
    private PongGameManager _pongGameManager;

    public void Initialize(PongGameManager pongGameManager)
    {
        _pongGameManager = pongGameManager;
        _pongGameManager.OnMultiplayerGamePaused += OnGamePaused;
        _pongGameManager.OnMultiplayerGameUnpaused += OnGameUnpaused;
    }

    private void OnGamePaused(object sender, EventArgs e)
    {
        if (!_pongGameManager.IsSingleplayer())
        Show();
    }

    private void OnGameUnpaused(object sender, EventArgs e)
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
