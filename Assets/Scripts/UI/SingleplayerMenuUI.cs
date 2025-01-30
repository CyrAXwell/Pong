using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleplayerMenuUI : MonoBehaviour, IShowWindowUI, IHideWindowUI
{    
    const int PLAYERS_AMOUNT = 1;

    [SerializeField] private Button _difficultChangeButton;
    [SerializeField] private Button _goalChangeButton;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private TMP_Text _difficultText;
    [SerializeField] private TMP_Text _goalText;

    private PongGameManager _pongGameManager;
    private IEnumerator _goalEnumerator;
    private IEnumerator _difficultEnumerator;
    private MainMenuUI _mainMenuUI;
    
    public void Initialize(PongGameManager pongGameManager, MainMenuUI mainMenuUI)
    {
        _pongGameManager = pongGameManager;
        _mainMenuUI = mainMenuUI;
    }

    private void Start()
    {
        _difficultChangeButton.onClick.AddListener( () => { ChangeButtonState(_difficultEnumerator); });
        _goalChangeButton.onClick.AddListener( () => { ChangeButtonState(_goalEnumerator); });
        bool isCPUPlayer = true;
        _startGameButton.onClick.AddListener( () => 
        { 
            _pongGameManager.StartGame(isCPUPlayer, (int)_goalEnumerator.Current, (PongAIDifficultySO)_difficultEnumerator.Current); 
            Hide(); 
        });
        _exitButton.onClick.AddListener( () => { Hide(); _mainMenuUI.Show();});

        _difficultEnumerator = _pongGameManager.GetLevelSettingSO().Difficulty.GetEnumerator();
        _goalEnumerator = _pongGameManager.GetLevelSettingSO().Goals.GetEnumerator();

        _difficultEnumerator.MoveNext();
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
        PongAIDifficultySO difficultySO = (PongAIDifficultySO)_difficultEnumerator.Current;
        _difficultText.text = difficultySO.Difficulty.ToString().ToUpper();

        int goal = (int)_goalEnumerator.Current;
        _goalText.text = goal == 0 ? "-" : goal.ToString();
    }
}
