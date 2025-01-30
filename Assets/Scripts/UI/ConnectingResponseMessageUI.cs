using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectingResponseMessageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private Button _closeButton;

    private LobbyUI _lobbyUI;

    public void Initialize(LobbyUI lobbyUI)
    {
        _lobbyUI = lobbyUI;
        PongGameMultipayer.Instance.OnFailedToJoinGame += OnFailedToJoinGame;
    }

    private void Start()
    {
        _closeButton.onClick.AddListener( () => { Hide(); _lobbyUI.Show(); });
    }

    private void OnFailedToJoinGame(object sender, EventArgs e)
    {
        if (!PongGameMultipayer.Instance.IsConnecting())
            return;

        Show();
        _messageText.text = NetworkManager.Singleton.DisconnectReason.ToUpper();

        if (_messageText.text == "")
        {
            string message = "Failed to connect";
            _messageText.text = message.ToUpper();
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _closeButton.GetComponent<ButtonWithPointerUI>().ManualSelect();
        _closeButton.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
