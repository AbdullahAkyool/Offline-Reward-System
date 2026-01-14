using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private Button mainButton;

    [SerializeField] private GameObject mainButtonUsedState;
    [SerializeField] private GameObject mainButtonUnusedState;

    [SerializeField] private Image mainButtonUnusedImage;
    [SerializeField] private Color disabledUnusedColor = Color.gray;

    private UnityAction mainButtonEvent;
    private bool isInteractable = true;
    private Color originalUnusedColor;

    void Awake()
    {
        mainButton = GetComponent<Button>();
        if (mainButtonUnusedImage != null)
        {
            originalUnusedColor = mainButtonUnusedImage.color;
        }

        mainButton.onClick.AddListener(HandleClick);

        SetButtonState(true);
        SetInteractable(true);
    }

    private void HandleClick()
    {
        if (!isInteractable)
        {
            return;
        }
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

    public void SetInteractable(bool canUse)
    {
        isInteractable = canUse;
        if (mainButton != null)
        {
            mainButton.interactable = canUse;
        }

        if (mainButtonUnusedImage == null)
        {
            return;
        }

        if (canUse)
        {
            mainButtonUnusedImage.color = originalUnusedColor;
        }
        else
        {
            SetButtonState(true);
            mainButtonUnusedImage.color = disabledUnusedColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isInteractable)
        {
            return;
        }
        SetButtonState(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isInteractable)
        {
            return;
        }
        SetButtonState(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isInteractable)
        {
            return;
        }
        SetButtonState(true);
    }
}
