using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour, IShowWindowUI, IHideWindowUI
{
    [SerializeField] private Button _createPrivateButton;
    [SerializeField] private Button _createPublicButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private TMP_InputField _lobbyNameInputField;

    private PongGameManager _pongGameManager;
    private LobbyUI _lobbyUI;

    public void Initialize(PongGameManager pongGameManager, LobbyUI lobbyUI)
    {
        _pongGameManager = pongGameManager;
        _lobbyUI = lobbyUI;
    }

    private void Start()
    {
        _createPrivateButton.onClick.AddListener( () => { Hide(); PongGameLobby.Instance.CreateLobby(_lobbyNameInputField.text, true); });
        _createPublicButton.onClick.AddListener( () => { Hide(); PongGameLobby.Instance.CreateLobby(_lobbyNameInputField.text, false); });
        _closeButton.onClick.AddListener( () => { Hide(); _lobbyUI.Show(); });
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _createPrivateButton.GetComponent<ButtonWithPointerUI>().ManualSelect();
        _createPrivateButton.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
