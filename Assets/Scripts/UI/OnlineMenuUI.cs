using UnityEngine;
using UnityEngine.UI;

public class OnlineMenuUI : MonoBehaviour, IShowWindowUI, IHideWindowUI
{
    [SerializeField] private Button _hostGameButton;
    [SerializeField] private Button _clientGameButton;
    [SerializeField] private Button _exitButton;

    private MultiplayerMenuUI _multiplayerMenuUI;
    
    public void Initialize(MultiplayerMenuUI multiplayerMenuUI)
    {
        _multiplayerMenuUI = multiplayerMenuUI;
    }

    private void Start()
    {
        _hostGameButton.onClick.AddListener( () => {Hide(); PongGameMultipayer.Instance.StartHostGame(); });
        _clientGameButton.onClick.AddListener( () => {Hide(); PongGameMultipayer.Instance.StartClientGame(); });
        _exitButton.onClick.AddListener( () => { Hide(); _multiplayerMenuUI.Show(); });
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _hostGameButton.GetComponent<ButtonWithPointerUI>().ManualSelect();
        _hostGameButton.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
