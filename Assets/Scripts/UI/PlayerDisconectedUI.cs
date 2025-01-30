using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDisconectedUI : MonoBehaviour, IShowWindowUI, IHideWindowUI
{
    [SerializeField] private Button _mainMenuButton;

    private PongGameManager _pongGameManager;
    private MainMenuUI _mainMenuUI;

    public void Initialize(PongGameManager pongGameManager, MainMenuUI mainMenuUI)
    {
        _pongGameManager = pongGameManager;
        _mainMenuUI = mainMenuUI;

        _pongGameManager.OnPlayerDisconnected += OnPlayerDisconnection;
    }

    private void Start()
    {
        _mainMenuButton.onClick.AddListener( () => { Hide(); PongGameMultipayer.Instance.ShutDown(); _mainMenuUI.Show(); } );// SceneManager.LoadScene(0);
    }

    private void OnPlayerDisconnection(object sender, EventArgs e)
    {
        if ((!_pongGameManager.IsGameOver() || !NetworkManager.Singleton.IsServer) && !PongGameMultipayer.Instance.IsConnecting())
            Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _mainMenuButton.GetComponent<ButtonWithPointerUI>().ManualSelect();
        _mainMenuButton.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
