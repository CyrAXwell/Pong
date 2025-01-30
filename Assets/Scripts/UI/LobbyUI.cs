using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour, IShowWindowUI, IHideWindowUI
{
    [SerializeField] private Button _createLobbyButton;
    [SerializeField] private Button _quickJoinButton;
    [SerializeField] private Button _joinCodeButton;
    [SerializeField] private TMP_InputField _joinCodeInputField;
    [SerializeField] private TMP_InputField _playerNameInputField;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Transform _lobbyContainer;
    [SerializeField] private Transform _lobbyTemplate;

    private PongGameManager _pongGameManager;
    private MultiplayerMenuUI _multiplayerMenuUI;
    private LobbyCreateUI _lobbyCreateUI;

    public void Initialize(PongGameManager pongGameManager, MultiplayerMenuUI multiplayerMenuUI, LobbyCreateUI lobbyCreateUI)
    {
        _pongGameManager = pongGameManager;
        _multiplayerMenuUI = multiplayerMenuUI;
        _lobbyCreateUI = lobbyCreateUI;
        PongGameLobby.Instance.OnLobbyListChanged += OnLobbyListChanged;
    }

    private void Start()
    {
        _createLobbyButton.onClick.AddListener( () => { Hide(); _lobbyCreateUI.Show(); });
        _quickJoinButton.onClick.AddListener( () => { Hide(); PongGameLobby.Instance.QuickJoin(); });
        _joinCodeButton.onClick.AddListener( () => { Hide(); PongGameLobby.Instance.JoinWithCode(_joinCodeInputField.text); });
        _exitButton.onClick.AddListener( () => { Hide(); PongGameLobby.Instance.LeaveLobby(); _multiplayerMenuUI.Show(); });
        _playerNameInputField.onValueChanged.AddListener( (string newString) => { PongGameMultipayer.Instance.SetPlayerName(newString); });
    }

    private void OnLobbyListChanged(object sender, PongGameLobby.OnLobbyLostChangedEventArgs e)
    {
        if (gameObject.activeInHierarchy)
            UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        string selectedLobbyid = "";
        foreach (Transform child in _lobbyContainer)
        { 
            if (child == _lobbyTemplate) continue;

            if (child.GetComponent<LobbyListSingleUI>().IsSelected())
                selectedLobbyid = child.GetComponent<LobbyListSingleUI>().GetLobbyId();

            Destroy(child.gameObject);
        }

        bool selectetLobbyIsDestroy = true;
        foreach (Lobby lobby in lobbyList)
        {
            
            if (!(lobby.Data != null && lobby.Data.ContainsKey(PongGameLobby.KEY_RELAY_JOIN_CODE)))
                continue;

            Transform lobbyTransform = Instantiate(_lobbyTemplate, _lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby, this);

            if (selectedLobbyid != "" && selectedLobbyid == lobby.Id)
            { 
                lobbyTransform.GetComponent<ButtonWithPointerUI>().ManualSelect();
                lobbyTransform.GetComponent<Button>().Select();
                selectetLobbyIsDestroy = false;
            }
        }

        if (selectedLobbyid != "" && selectetLobbyIsDestroy)
        {
            _createLobbyButton.GetComponent<ButtonWithPointerUI>().ManualSelect();
            _createLobbyButton.Select();
        }

    }

    public void Show()
    {
        PongGameLobby.Instance.OnShowLobbyUI();
        _lobbyTemplate.gameObject.SetActive(false);

        gameObject.SetActive(true);
        _createLobbyButton.GetComponent<ButtonWithPointerUI>().ManualSelect();
        _createLobbyButton.Select();

        _playerNameInputField.text = PongGameMultipayer.Instance.GetPlayerName();
        UpdateLobbyList(new List<Lobby>());
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
