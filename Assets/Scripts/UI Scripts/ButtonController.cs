using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    private Button mainButton;

    [SerializeField] private GameObject mainButtonUsedState;
    [SerializeField] private GameObject mainButtonUnusedState;

    private UnityAction mainButtonEvent;

    void Awake()
    {
        mainButton = GetComponent<Button>();

        mainButton.onClick.AddListener(HandleClick);

        SetButtonState(true);
    }

    private void HandleClick()
    {
        mainButtonEvent?.Invoke();
    }

    public void AddListener(UnityAction unityAction)
    {
        mainButtonEvent += unityAction;
    }

    public void RemoveListener(UnityAction unityAction)
    {
        mainButtonEvent -= unityAction;
    }

    public void SetButtonState(bool state)
    {
        if (state)
        {
            mainButtonUnusedState.SetActive(true);
            mainButtonUsedState.SetActive(false);
        }
        else
        {
            mainButtonUnusedState.SetActive(false);
            mainButtonUsedState.SetActive(true);
        }
    }
}
