using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    public Text messageText;
    public Button okButton;
    public Button noButton;
    public GameObject messageBoxPanel;

    public enum MessageBoxType
    {
        OK,
        OKCancel
    }

    public Action OnOkClicked;
    public Action OnNoClicked;

    private void Start()
    {
        okButton.onClick.AddListener(OnOkButtonClicked);
        noButton.onClick.AddListener(OnNoButtonClicked);
    }

    public void Show(string message, MessageBoxType type)
    {
        messageText.text = message;
        messageBoxPanel.SetActive(true);

        switch (type)
        {
            case MessageBoxType.OK:
                okButton.gameObject.SetActive(true);
                noButton.gameObject.SetActive(false);
                break;
            case MessageBoxType.OKCancel:
                okButton.gameObject.SetActive(true);
                noButton.gameObject.SetActive(true);
                break;
        }
    }

    public void Hide()
    {
        messageBoxPanel.SetActive(false);
    }

    private void OnOkButtonClicked()
    {
        OnOkClicked?.Invoke();
        Hide();
    }

    private void OnNoButtonClicked()
    {
        OnNoClicked?.Invoke();
        Hide();
    }
}
