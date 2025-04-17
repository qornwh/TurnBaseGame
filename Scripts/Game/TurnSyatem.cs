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
  SkillEnd,
  End
}

public class TurnSystem
{
  public event Action<int> OnTurnStart;
  public event Action<int> OnMovePhaseStart;
  public event Action OnMovePhaseEnd;
  public event Action<int> OnSkillPhaseStart;
  public event Action OnSkillPhaseEnd;
  public event Action<int> OnTurnEnd;

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
    currentPhase = TurnPhase.Start;
    OnTurnStart?.Invoke(playerIds[currentTurnIdx]);
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

  public void EndSkillPhase()
  {
    currentPhase = TurnPhase.SkillEnd;
    OnSkillPhaseEnd?.Invoke();
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
      currentPhase = TurnPhase.MoveEnd;
      currentTurnIdx = currentTurnIdx < 0 ? playerIds.Count - 1 : 0;
      EndSkillPhase();
    }
    else
    {
      StartSkillPhase();
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