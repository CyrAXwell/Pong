using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour, IMoveControllable
{
    private const string PLAYER_PREFS_BINDINGS = "PlayerPrefsBindings";
    public event EventHandler OnPauseAction;

    public enum Binding {
        Player1_MoveUp,
        Player1_MoveDown,
        Player2_MoveUp,
        Player2_MoveDown,
        Pause,
    }

    private PlayerInputActions _playerInputActions;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
            _playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));

        _playerInputActions.Enable();
        _playerInputActions.Player.Pause.performed += OnPauseActionPerformed;
    }

    private void OnPauseActionPerformed(InputAction.CallbackContext context)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMoveVector(Player player)
    {
        Vector2 inputVector;
        switch (player.GetIndex())
        {
            default:
                inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
                break;
            case 0:
                inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
                break;
            case 1:
                inputVector = _playerInputActions.Player2.Move.ReadValue<Vector2>();
                break;
        }
        
        return inputVector;
    }

    public string GetBindingText(Binding binding)
    {
        bool joystickConnected = IsJoystickConnected();
            
        switch(binding)
        {
            default:
            case Binding.Player1_MoveUp:
                if (joystickConnected)
                    return _playerInputActions.Player.Move.bindings[5].ToDisplayString();
                else
                    return _playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.Player1_MoveDown:
                if (joystickConnected)
                    return _playerInputActions.Player.Move.bindings[5].ToDisplayString();
                else
                    return _playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.Player2_MoveUp:
                return _playerInputActions.Player2.Move.bindings[1].ToDisplayString();
            case Binding.Player2_MoveDown:
                return _playerInputActions.Player2.Move.bindings[2].ToDisplayString();
            case Binding.Pause:
                if (joystickConnected)
                    return _playerInputActions.Player.Pause.bindings[1].ToDisplayString();
                else
                    return _playerInputActions.Player.Pause.bindings[0].ToDisplayString();
        }
    }

    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        _playerInputActions.Player.Disable();
        _playerInputActions.Player2.Disable();

        InputAction inputAction;
        int bindingIndex;

        bool joystickConnected = IsJoystickConnected();

        switch(binding)
        {
            default:
            case Binding.Player1_MoveUp:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = joystickConnected ? 5 : 1;
                break;
            case Binding.Player1_MoveDown:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = joystickConnected ? 5 : 2;
                break;
            case Binding.Player2_MoveUp:
                inputAction = _playerInputActions.Player2.Move;
                bindingIndex = 1;
                break;
            case Binding.Player2_MoveDown:
                inputAction = _playerInputActions.Player2.Move;
                bindingIndex = 2;
                break;
            case Binding.Pause:
                inputAction = _playerInputActions.Player.Pause;
                bindingIndex = joystickConnected ? 1 : 0;
                break;
        }
        
        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback => {
                callback.Dispose();
                _playerInputActions.Player.Enable();
                _playerInputActions.Player2.Enable();
                onActionRebound();

                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, _playerInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
            })
            .Start();
    }

    private bool IsJoystickConnected()
    {
        string[] joysticks = Input.GetJoystickNames();
        foreach (string joystick in joysticks)
        {
            if (joystick != "")
                return true;
        } 
        
        return false;
    }
}
