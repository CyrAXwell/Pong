using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour, IShowWindowUI, IHideWindowUI
{
    [SerializeField] private Button _singleplayerButton;
    [SerializeField] private Button _multiplayerButton;
    [SerializeField] private Button _optionsButton;

    private SingleplayerMenuUI _singleplayerMenuUI;
    private MultiplayerMenuUI _multiplayerMenuUI;
    private OptionsUI _optionsUI;

    public void Initialize(SingleplayerMenuUI singleplayerMenuUI, MultiplayerMenuUI multiplayerMenuUI, OptionsUI optionsUI)
    {
        _singleplayerMenuUI = singleplayerMenuUI;
        _multiplayerMenuUI = multiplayerMenuUI;
        _optionsUI = optionsUI;

        Show();
    }

    private void Start()
    {
        _singleplayerButton.onClick.AddListener(() => { Hide(); _singleplayerMenuUI.Show(); });
        _multiplayerButton.onClick.AddListener(() => { Hide(); _multiplayerMenuUI.Show(); });
        _optionsButton.onClick.AddListener(() => { Hide(); _optionsUI.Show(); });
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
        _singleplayerButton.GetComponent<ButtonWithPointerUI>().ManualSelect();
        _singleplayerButton.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
