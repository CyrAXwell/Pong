using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour, IShowWindowUI, IHideWindowUI
{
    [SerializeField] private Button _soundButton;
    [SerializeField] private Button _player1MoveUpButton;
    [SerializeField] private Button _player1MoveDownButton;
    [SerializeField] private Button _player2MoveUpButton;
    [SerializeField] private Button _player2MoveDownButton;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private TMP_Text _soundText;
    [SerializeField] private TMP_Text _player1MoveUpText;
    [SerializeField] private TMP_Text _player1MoveDownText;
    [SerializeField] private TMP_Text _player2MoveUpText;
    [SerializeField] private TMP_Text _player2MoveDownText;
    [SerializeField] private TMP_Text _pauseText;
    [SerializeField] private GameObject PresstoRebindKeyPanel;

    private GameInput _gameInput;
    private MainMenuUI _mainMenuUI;

    public void Initialize(GameInput gameInput, MainMenuUI mainMenuUI)
    {
        _gameInput = gameInput;
        _mainMenuUI = mainMenuUI;
    }

    private void Start()
    {
        _soundButton.onClick.AddListener( () => { AudioManager.Instance.ChangeVolume(); UpdateVisual();});
        _player1MoveUpButton.onClick.AddListener( () => { RebindBinding(GameInput.Binding.Player1_MoveUp); });
        _player1MoveDownButton.onClick.AddListener( () => { RebindBinding(GameInput.Binding.Player1_MoveDown); });
        _player2MoveUpButton.onClick.AddListener( () => { RebindBinding(GameInput.Binding.Player2_MoveUp); });
        _player2MoveDownButton.onClick.AddListener( () => { RebindBinding(GameInput.Binding.Player2_MoveDown); });
        _pauseButton.onClick.AddListener( () => { RebindBinding(GameInput.Binding.Pause); });
        _exitButton.onClick.AddListener( () => { Hide(); _mainMenuUI.Show(); }); 
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _soundButton.GetComponent<ButtonWithPointerUI>().ManualSelect();
        _soundButton.Select();
        UpdateVisual();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void UpdateVisual()
    {
        _soundText.text = "SOUND: " + AudioManager.Instance.GetVolume();
        _player1MoveUpText.text = _gameInput.GetBindingText(GameInput.Binding.Player1_MoveUp).ToUpper();
        _player1MoveDownText.text = _gameInput.GetBindingText(GameInput.Binding.Player1_MoveDown).ToUpper();
        _player2MoveUpText.text = _gameInput.GetBindingText(GameInput.Binding.Player2_MoveUp).ToUpper();
        _player2MoveDownText.text = _gameInput.GetBindingText(GameInput.Binding.Player2_MoveDown).ToUpper();
        _pauseText.text = "PAUSE: " + _gameInput.GetBindingText(GameInput.Binding.Pause).ToUpper();
    }

    private void RebindBinding(GameInput.Binding binding) {
        Hide();
        
        ShowPressToRebindKey();
        _gameInput.RebindBinding(binding, () => {
            HidePressToRebindKey();
            Show();
        });
    }

    private void ShowPressToRebindKey()
    {
        PresstoRebindKeyPanel.SetActive(true);
    }

    private void HidePressToRebindKey()
    {
        PresstoRebindKeyPanel.SetActive(false);
    }
}
