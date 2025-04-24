using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum TurnPhase
{
  Start,
  Move,
  MoveEnd,
  Skill,
  SkillExecute,
  SkillEnd,
  End
}

public class TurnSystem
{
  public event Action OnTurnStart;
  public event Action<int> OnMovePhaseStart;
  public event Action OnMovePhaseEnd;
  public event Action<int> OnSkillPhaseStart;
  public event Action<int> OnSkillExecute;
  public event Action OnSkillPhaseEnd;
  public event Action OnTurnEnd;

  public Func<bool> IsSkillEnd;

  private int currentTurnIdx;
  private List<int> playerIds;
  private bool isForwardOrder;
  private TurnPhase currentPhase;
  private int currentTurnCount;

  public void Init(List<int> playerIds)
  {
    this.currentTurnIdx = 0;
    this.playerIds = playerIds;
    this.isForwardOrder = true;
    this.currentPhase = TurnPhase.Start;
  }

  public void StartTurn()
  {
    currentTurnCount++;
    currentPhase = TurnPhase.Start;
    OnTurnStart?.Invoke();
    StartMovePhase();
  }

  public void StartMovePhase()
  {
    currentPhase = TurnPhase.Move;
    OnMovePhaseStart?.Invoke(playerIds[currentTurnIdx]);
  }

  public void EndMovePhase()
  {
    currentPhase = TurnPhase.MoveEnd;
    OnMovePhaseEnd?.Invoke();
    StartSkillPhase();
  }

  public void StartSkillPhase()
  {
    currentPhase = TurnPhase.Skill;
    OnSkillPhaseStart?.Invoke(playerIds[currentTurnIdx]);
  }

  public void SkillExecute()
  {
    currentPhase = TurnPhase.SkillExecute;
    OnSkillExecute?.Invoke(playerIds[currentTurnIdx]);
  }

  public void EndSkillPhase()
  {
    currentPhase = TurnPhase.SkillEnd;
    OnSkillPhaseEnd?.Invoke();
    StartTurn();
  }

  public void TurnEndPhase()
  {
    currentPhase = TurnPhase.End;
    OnTurnEnd?.Invoke();
  }

  public void EndTurn()
  {
    if (currentPhase == TurnPhase.Move)
    {
      // 다음 플레이어의 턴 시작
      MoveToNextPlayer();
    }
    else if (currentPhase == TurnPhase.Skill)
    {
      // 다음 플레이어 스킬 시작
      SkillToNextPlayer();
    }
    else if (currentPhase == TurnPhase.SkillExecute)
    {
      // 다음 플레이어 스킬 시작
      SkillExecuteToNextPlayer();
    }
  }

  private void UpdateTurn()
  {
    if (isForwardOrder)
    {
      currentTurnIdx++;
    }
    else
    {
      currentTurnIdx--;
    }
  }

  private void MoveToNextPlayer()
  {
    UpdateTurn();
    if (currentTurnIdx < 0 || currentTurnIdx >= playerIds.Count)
    {
      currentPhase = TurnPhase.MoveEnd;
      currentTurnIdx = isForwardOrder ? 0 : playerIds.Count - 1;
      EndMovePhase();
    }
    else
    {
      StartMovePhase();
    }
  }

  private void SkillToNextPlayer()
  {
    UpdateTurn();
    if (currentTurnIdx < 0 || currentTurnIdx >= playerIds.Count)
    {
      currentPhase = TurnPhase.SkillExecute;
      currentTurnIdx = isForwardOrder ? 0 : playerIds.Count - 1;
      SkillExecute();
    }
    else
    {
      StartSkillPhase();
    }
  }

  private void SkillExecuteToNextPlayer()
  {
    // 스킬 플레이는 돌아가면서가 아닌 스킬 큐에 담긴걸 다 사용했을때 종료
    UpdateTurn();
    if (IsSkillEnd())
    {
      currentPhase = TurnPhase.SkillExecute;
      currentTurnIdx = isForwardOrder ? 0 : playerIds.Count - 1;
      EndSkillPhase();
    }
    else
    {
      if (currentTurnIdx < 0 || currentTurnIdx >= playerIds.Count)
      {
        currentTurnIdx = isForwardOrder ? 0 : playerIds.Count - 1;
      }
      SkillExecute();
    }
  }

  // 현재 플레이어 ID 반환
  public int GetCurrentPlayerId()
  {
    return playerIds[currentTurnIdx];
  }

  // 현재 턴 단계 반환
  public TurnPhase GetCurrentPhase()
  {
    return currentPhase;
  }

  public int GetCurrentTurnCount()
  {
    return currentTurnCount;
  }
}