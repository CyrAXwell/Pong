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

    public void Initialize(PongGameManager pongGameManager, MainMenuUI mainMenuUI)
    {
        _pongGameManager = pongGameManager;
        _mainMenuUI = mainMenuUI;

        _pongGameManager.OnGamePaused += OnGamePaused;
        _pongGameManager.OnGameUnpaused += OnGameUnpaused; 
    }

    private void Start()
    {
        _resumeButton.onClick.AddListener( () => { _pongGameManager.TogglePauseGame(); } );
        _soundButton.onClick.AddListener( () => { AudioManager.Instance.ChangeVolume(); UpdateVisual(); });
        _mainMenuButton.onClick.AddListener( () => { _pongGameManager.EndTheGame(); _pongGameManager.TogglePauseGame(); _mainMenuUI.Show();}); 
    }

    private void OnGameUnpaused(object sender, EventArgs e)
    {
        _mainMenuButton.GetComponent<ButtonWithPointerUI>().ManualSelect();
        _mainMenuButton.Select();
        Hide();
    }

    private void OnGamePaused(object sender, EventArgs e)
    {
        Show();
        UpdateVisual();
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
        _pongGameManager.OnGamePaused -= OnGamePaused;
        _pongGameManager.OnGameUnpaused -= OnGameUnpaused;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
