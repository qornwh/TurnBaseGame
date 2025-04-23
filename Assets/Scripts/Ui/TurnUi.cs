using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnText;
    private event Action<int> TurnUpdateAction;
    
    void Awake()
    {
        TurnUpdateAction += TurnUpdate;
    }

    private void TurnUpdate(int turnCount)
    {
        turnText.SetText(turnCount.ToString());
    }

    public void OnTurnUpdateAction(int turnCount)
    {
        TurnUpdateAction?.Invoke(turnCount);
    }
}
