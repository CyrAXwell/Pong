using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TMP_Text _lobbyNameText;

    private Lobby _lobby;
    private bool _isSelected;

    public bool IsSelected() => _isSelected;
    public string GetLobbyId() => _lobby.Id;

    public void SetLobby(Lobby lobby, LobbyUI lobbyUI)
    {
        _lobby = lobby;
        _lobbyNameText.text = _lobby.Name;

        GetComponent<Button>().onClick.AddListener(() => { 
            lobbyUI.Hide(); 
            PongGameLobby.Instance.JoinWithId(_lobby.Id); 
        } );
    }

    public void OnSelect(BaseEventData eventData)
    {
        _isSelected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _isSelected = false;
    }
}
