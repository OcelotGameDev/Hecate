using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public enum SchemeType
{
    Keyboard, 
    Gamepad,
}

public class ControllerChooser : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;

    private static SchemeType _currentScheme = SchemeType.Keyboard;
    public static SchemeType CurrentScheme => _currentScheme;

    public static Action<SchemeType> OnSchemeChanged;
    
    private void Awake()
    {
        ChangeScheme(_playerInput.currentControlScheme);
    }

    private void OnEnable()
    {
        InputUser.onChange += OnChange;
    }

    private void OnDisable()
    {
        InputUser.onChange -= OnChange; 
    }
    
    private void OnChange(InputUser user, InputUserChange inputChange, InputDevice device)
    {
        ChangeScheme(_playerInput.currentControlScheme);
    }

    private void ChangeScheme(string schemeName)
    {
        _currentScheme = schemeName switch
        {
            "Keyboard" => SchemeType.Keyboard,
            "GamePad" => SchemeType.Gamepad,
            _ => _currentScheme
        };
        
        OnSchemeChanged?.Invoke(_currentScheme);
    }    
    
    private void OnValidate()
    {
        if (!_playerInput) _playerInput = this.GetComponent<PlayerInput>();
    }
}