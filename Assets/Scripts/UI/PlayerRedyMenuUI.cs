
using System;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRedyMenuUI : MonoBehaviour, IShowWindowUI, IHideWindowUI
{
    public event EventHandler OnPlayerReady;
    public event EventHandler OnPlayerNotReady;
    public event EventHandler OnTargetGoalChanged;

    [SerializeField] private Button _readyButton;
    [SerializeField] private Button _notReadyButton;
    [SerializeField] private Button _goalChangeButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private TMP_Text _goalText;
    [SerializeField] private TMP_Text _waitingText;
    [SerializeField] private TMP_Text _lobbyNameText;
    [SerializeField] private TMP_Text _lobbyCodeText;
    [SerializeField] private TMP_Text[] _playersNamesTextArray;

    private IEnumerator _goalEnumerator;
    private PongGameManager _pongGameManager;
    private LobbyUI _lobbyUI;

    public void Initialize(PongGameManager pongGameManager, LobbyUI lobbyUI)
    {
        _pongGameManager = pongGameManager;
        _lobbyUI = lobbyUI;

        _pongGameManager.OnPlayerDisconnected += OnPlayerDisconnection;
        PongGameMultipayer.Instance.OnPlayerDataNetworkListChanged += OnPlayerDataNetworkListChange;
    }

    private void Start()
    {
        _readyButton.onClick.AddListener( () => { 
            OnPlayerReady?.Invoke(this, EventArgs.Empty); 
            ShowButton(_notReadyButton); 
            _readyButton.gameObject.SetActive(false); 
            _waitingText.gameObject.SetActive(true); 
            _goalChangeButton.interactable = false;
        });
        _notReadyButton.onClick.AddListener( () => { 
            OnPlayerNotReady?.Invoke(this, EventArgs.Empty); 
            ShowButton(_readyButton); 
            _notReadyButton.gameObject.SetActive(false); 
            _waitingText.gameObject.SetActive(false); 
            _goalChangeButton.interactable = true;
        });
        _goalChangeButton.onClick.AddListener( () => { ChangeButtonState(_goalEnumerator); });
        _exitButton.onClick.AddListener( () => { Hide(); PongGameMultipayer.Instance.ShutDown(); _lobbyUI.Show(); });

        _goalEnumerator = _pongGameManager.GetLevelSettingSO().Goals.GetEnumerator();

        _goalEnumerator.MoveNext();
        
        UpdateVisual();
    }

    private void OnPlayerDisconnection(object sender, EventArgs e)
    {
        if (!_pongGameManager.IsWaitingPlayersState())
        {
            return;
        }

        Hide();
    }

    private void OnPlayerDataNetworkListChange(object sender, EventArgs e)
    {
        for (int i = 0; i < _playersNamesTextArray.Count(); i++)
        {
            _playersNamesTextArray[i].text = PongGameMultipayer.Instance.GetPlayerDataNameByPlayerIndex(i);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);

        _notReadyButton.gameObject.SetActive(false);
        _waitingText.gameObject.SetActive(false); 
        _goalChangeButton.interactable = true;
        ShowButton(_readyButton); 

        UpdateVisual();
    }

    public void Hide()
    {
        gameObject.SetActive(false);        
    }

    public int GetTargetGoal()
    {
        return (int)_goalEnumerator.Current;
    }

    private void ShowButton(Button button)
    {
        button.gameObject.SetActive(true);
        button.GetComponent<ButtonWithPointerUI>().ManualSelect();
        button.Select();
    }

    private void ChangeButtonState(IEnumerator enumerator)
    {
        if (!enumerator.MoveNext())
        {
            enumerator.Reset();
            enumerator.MoveNext();
        }
        OnTargetGoalChanged?.Invoke(this, EventArgs.Empty);
        //UpdateVisual();
    }

    public void UpdateGoatText(int goal)
    {
        _goalText.text = goal == 0 ? "-" : goal.ToString();
    }

    private void UpdateVisual()
    {
        _goalText.text = _pongGameManager.GetTargetGoal().ToString();

        Lobby lobby = PongGameLobby.Instance.GetLobby();
        _lobbyNameText.text = "LOBBY NAME: " + lobby.Name;
        _lobbyCodeText.text = "LOBBY CODE: " + lobby.LobbyCode;
        // int goal = (int)_goalEnumerator.Current;
        // _goalText.text = goal == 0 ? "-" : goal.ToString();
    }
}
