using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour, IShowWindowUI, IHideWindowUI
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _soundButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private TMP_Text _soundText;

    private PongGameManager _pongGameManager;
    private MainMenuUI _mainMenuUI;

    private bool _isExitGame;

    public void Initialize(PongGameManager pongGameManager, MainMenuUI mainMenuUI)
    {
        _pongGameManager = pongGameManager;
        _mainMenuUI = mainMenuUI;

        _pongGameManager.OnLocalGamePaused += OnGamePaused;
        _pongGameManager.OnLocalGameUnpaused += OnGameUnpaused; 
        _pongGameManager.OnPlayerDisconnected += OnPlayerDisconnection;
    }

    private void Start()
    {
        _resumeButton.onClick.AddListener( () => { _pongGameManager.TogglePauseGame(); } );
        _soundButton.onClick.AddListener( () => { AudioManager.Instance.ChangeVolume(); UpdateVisual(); });
        _mainMenuButton.onClick.AddListener( () => { 
            _isExitGame = true;
            _pongGameManager.TogglePauseGame(); 
        }); 
    }

    private void OnGameUnpaused(object sender, EventArgs e)
    {
        _mainMenuButton.GetComponent<ButtonWithPointerUI>().ManualSelect();
        _mainMenuButton.Select();
        Hide();

        if (_isExitGame)
        {
            _isExitGame = false;
            PongGameMultipayer.Instance.ShutDown();
            _mainMenuUI.Show();
        }
    }

    private void OnGamePaused(object sender, EventArgs e)
    {
        Show();
        UpdateVisual();
    }

    private void OnPlayerDisconnection(object sender, EventArgs e)
    {
        Hide();
    }

    private void UpdateVisual()
    {
        _soundText.text = "SOUND: " + AudioManager.Instance.GetVolume();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _resumeButton.GetComponent<ButtonWithPointerUI>().ManualSelect();
        _resumeButton.Select();
    }

    private void OnDestroy()
    {
        _pongGameManager.OnLocalGamePaused -= OnGamePaused;
        _pongGameManager.OnLocalGameUnpaused -= OnGameUnpaused;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
