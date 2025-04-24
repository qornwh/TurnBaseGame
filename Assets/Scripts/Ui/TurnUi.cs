using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TurnUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnCountText;
    [SerializeField] private TextMeshProUGUI turnMsgText;
    [SerializeField] private GameObject victoryMsg;
    [SerializeField] private GameObject defeatMsg;
    private event Action TurnUpdateAction;
    private event Action SubTurnUpdateAction;
    
    void Awake()
    {
        TurnUpdateAction += TurnUpdate;
        SubTurnUpdateAction += SubTurnUpdate;
    }

    private void TurnUpdate()
    {
        var gm = GameInstance.GetInstance().GameManager;
        int turnCount = gm.TurnSystem.GetCurrentTurnCount();
        turnCountText.SetText(turnCount.ToString());
    }

    private void SubTurnUpdate()
    {
        var gm = GameInstance.GetInstance().GameManager;
        var ts = gm.TurnSystem;

        switch (ts.GetCurrentPhase())
        {
            case TurnPhase.Start:
                turnMsgText.SetText("턴 시작");
                break;
            case TurnPhase.Move :
                turnMsgText.SetText("캐릭터를 움직여 주십시오");
                break;
            case TurnPhase.Skill:
                turnMsgText.SetText("캐릭터의 스킬을 선택해 주십시오");
                break;
            case TurnPhase.SkillExecute:
                turnMsgText.SetText("캐릭터가 스킬을 플레이 합니다");
                break;
            case TurnPhase.End:
            {
                turnMsgText.SetText("게임이 종료 되었습니다");
                if (gm.IsWin())
                {
                    victoryMsg.SetActive(true);
                }
                else
                {
                    defeatMsg.SetActive(true);
                }
            }
                break;
            default:
                turnMsgText.SetText("");
                break;
        }
    }

    public void OnTurnUpdate()
    {
        TurnUpdateAction?.Invoke();
    }

    public void OnSubTurnUpdate()
    {
        SubTurnUpdateAction?.Invoke();
    }
}
