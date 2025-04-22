using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubStateUi : MonoBehaviour
{
    private int data;
    private string title;

    [SerializeField] private TextMeshProUGUI text;

    public void Init(string title, int data)
    {
        this.title = title;
        this.data = data;
        UpdateState(data);
    }

    public void UpdateState(int data)
    {
        this.data = data;
        text.text = title + ':' + data.ToString();
    }
}
