using UnityEngine;
using UnityEngine.UI;

public class MultiplayerMenuUI : MonoBehaviour, IShowWindowUI, IHideWindowUI
{
    [SerializeField] private Button _oneComputerGameButton;
    [SerializeField] private Button _oblineGameButton;
    [SerializeField] private Button _exitButton;

    private MainMenuUI _mainMenuUI;
    private OneComputerMultiplayerMenuUI _oneComputerMultiplayerMenuUI;
    
    public void Initialize(MainMenuUI mainMenuUI ,OneComputerMultiplayerMenuUI oneComputerMultiplayerMenuUI)
    {
        _mainMenuUI = mainMenuUI;
        _oneComputerMultiplayerMenuUI = oneComputerMultiplayerMenuUI;
    }

    private void Start()
    {
        _oneComputerGameButton.onClick.AddListener( () => { Hide(); _oneComputerMultiplayerMenuUI.Show();});
        _exitButton.onClick.AddListener( () => { Hide(); _mainMenuUI.Show(); });
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _oneComputerGameButton.GetComponent<ButtonWithPointerUI>().ManualSelect();
        _oneComputerGameButton.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
