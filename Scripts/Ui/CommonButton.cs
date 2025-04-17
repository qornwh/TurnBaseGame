using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class CommonButton : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI buttonText;
    [SerializeField] public Button button;

    public void Init(string text, Action onClickAction)
    {
        buttonText.text = text;
        button.onClick.RemoveAllListeners(); // Clear previous listeners
        if (onClickAction != null)
            button.onClick.AddListener(() => onClickAction?.Invoke());
    }
}