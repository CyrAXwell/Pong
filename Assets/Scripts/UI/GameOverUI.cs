using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour, IShowWindowUI, IHideWindowUI
{
    [SerializeField] private Button _playAgainButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private GameObject _leftPlayerWinTitle;
    [SerializeField] private GameObject _rightPlayerWinTitle;
    [SerializeField] private TMP_Text _totalScoreText;

    private PongGameManager _pongGameManager;
    private MainMenuUI _mainMenuUI;

    public void Initialize(PongGameManager pongGameManager, MainMenuUI mainMenuUI)
    {
        _pongGameManager = pongGameManager;
        _mainMenuUI = mainMenuUI;
        _pongGameManager.OnGameOver += OnGameOver;
        _pongGameManager.OnPlayerDisconnected += OnPlayerDisconnection;
    }

    private void Start()
    {
        _playAgainButton.onClick.AddListener( () => { 
            Hide(); 
            _mainMenuButton.GetComponent<ButtonWithPointerUI>().ManualSelect();
            _mainMenuButton.Select();
            _pongGameManager.OnPlayAgainButton(); 
        } );
        _mainMenuButton.onClick.AddListener( () => { Hide(); PongGameMultipayer.Instance.ShutDown(); _mainMenuUI.Show();  } );
    }

    private void OnGameOver(object sender, EventArgs e)
    {
        Show();
        UpdateVisual();
    }

    private void OnPlayerDisconnection(object sender, EventArgs e)
    {
        if (_pongGameManager.IsGameOver() && !NetworkManager.Singleton.IsServer)
            Hide();
    }

    private void UpdateVisual()
    {
        int leftScore = _pongGameManager.GetLeftPlayerScore();
        int rightScore = _pongGameManager.GetRightPlayerScore();
        _totalScoreText.text = leftScore.ToString() + ":" + rightScore.ToString();

        bool isLeftPlayerWin = leftScore > rightScore;
        _leftPlayerWinTitle.SetActive(isLeftPlayerWin);
        _rightPlayerWinTitle.SetActive(!isLeftPlayerWin);
    }

    public void Show()
    {
        gameObject.SetActive(true); 
        _playAgainButton.GetComponent<ButtonWithPointerUI>().ManualSelect();
        _playAgainButton.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false); 
    }

    private void OnDestroy()
    {
        _pongGameManager.OnGameOver -= OnGameOver;
    }

    
}
