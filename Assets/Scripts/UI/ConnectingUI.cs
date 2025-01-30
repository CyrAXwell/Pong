using System;
using UnityEngine;

public class ConnectingUI : MonoBehaviour, IShowWindowUI, IHideWindowUI
{
    private PongGameManager _pongGameManager;

    public void Initialize(PongGameManager pongGameManager)
    {
        _pongGameManager = pongGameManager;
        PongGameMultipayer.Instance.OnTryingToJoinGame += OnTryingToJoinGame;
        PongGameMultipayer.Instance.OnFailedToJoinGame += OnFailedToJoinGame;
        _pongGameManager.OnJoinGame += OnJoinGame;
    }

    private void OnJoinGame(object sender, EventArgs e)
    {
        Hide();
    }

    private void OnTryingToJoinGame(object sender, EventArgs e)
    {
        Show();
    }

    private void OnFailedToJoinGame(object sender, EventArgs e)
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
