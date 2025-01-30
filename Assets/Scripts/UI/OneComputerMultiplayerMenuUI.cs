using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OneComputerMultiplayerMenuUI : MonoBehaviour, IShowWindowUI, IHideWindowUI
{
    const int PLAYERS_AMOUNT = 2;

    [SerializeField] private Button _goalChangeButton;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private TMP_Text _goalText;
    [SerializeField] private TMP_Text _player1Controls;
    [SerializeField] private TMP_Text _player2Controls;
    
    private PongGameManager _pongGameManager;
    private MultiplayerMenuUI _multiplayerMenuUI;
    private IEnumerator _goalEnumerator;
    private GameInput _gameInput;

    public void Initialize(PongGameManager pongGameManager, MultiplayerMenuUI multiplayerMenuUI, GameInput gameInput)
    {
        _pongGameManager = pongGameManager;
        _multiplayerMenuUI = multiplayerMenuUI;
        _gameInput = gameInput;
    }

    private void Start()
    {
        _goalChangeButton.onClick.AddListener( () => { ChangeButtonState(_goalEnumerator); } );
        IEnumerator difficulty = _pongGameManager.GetLevelSettingSO().Difficulty.GetEnumerator();
        difficulty.MoveNext();
        bool isCPUPlayer = false;
        _startGameButton.onClick.AddListener( () => {  
            _pongGameManager.StartGame(isCPUPlayer, (int)_goalEnumerator.Current, (PongAIDifficultySO)difficulty.Current); 
            Hide(); });
        _exitButton.onClick.AddListener( () => { Hide(); _multiplayerMenuUI.Show();});

        _goalEnumerator = _pongGameManager.GetLevelSettingSO().Goals.GetEnumerator();
        _goalEnumerator.MoveNext();
        UpdateVisual();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _startGameButton.GetComponent<ButtonWithPointerUI>().ManualSelect();
        _startGameButton.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ChangeButtonState(IEnumerator enumerator)
    {
        if (!enumerator.MoveNext())
        {
            enumerator.Reset();
            enumerator.MoveNext();
        }
        
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        int goal = (int)_goalEnumerator.Current;
        _goalText.text = goal == 0 ? "-" : goal.ToString();

        _player1Controls.text = "PLAYER 1: " + _gameInput.GetBindingText(GameInput.Binding.Player1_MoveUp).ToUpper() 
            + ", " + _gameInput.GetBindingText(GameInput.Binding.Player1_MoveDown).ToUpper();
        _player2Controls.text = "PLAYER 2: " + _gameInput.GetBindingText(GameInput.Binding.Player2_MoveUp).ToUpper()
            + ", " + _gameInput.GetBindingText(GameInput.Binding.Player2_MoveDown).ToUpper();
    }
}
