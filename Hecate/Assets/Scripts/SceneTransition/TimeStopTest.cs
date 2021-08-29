using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeStopTest : MonoBehaviour
{
    private PlayerActions _actions;

    private Coroutine _coroutine = null;

    private void Awake()
    {
        _actions = new PlayerActions();
        _actions.Default.Enable();
    }

    private void OnEnable()
    {
        _actions.Default.Jump.performed += StopPlayTime;
    }

    private void OnDisable()
    {
        _actions.Default.Jump.performed -= StopPlayTime;
    }

    private void StopPlayTime(InputAction.CallbackContext context)
    {
        Time.timeScale = Time.timeScale <= 0.5f ? 1f : 0f;

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(Test());
    }

    private IEnumerator Test()
    {
        while (Time.timeScale < 0.5f)
        {
            yield return null;
            Debug.Log("Running");
        }
    }
}
