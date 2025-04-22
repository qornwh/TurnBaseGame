using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : StateBase
{
    public readonly int AttackCount = 8;
    public int PlayerID { get; set; }
    public int MaxHp { get; set; }
    public int MaxMp { get; set; }
    public Vector2Int Position { get; set; }
    public int Team { get; set; }
    public bool IsAi { get; set; }
    public override int Move
    {
        get
        {
            int move = base.Move;
            foreach (var buff in PlayerBuffStates)
            {
                move += buff.Move;
            }
            return move;
        }
    }
    public LinkedList<int> AttackCodes { get; set; }
    public CharacterData CharacterData { get; set; }
    public List<PlayerBuffState> PlayerBuffStates { get; set; }
    public event Action<Vector2Int> OnMoveAction;
    public event Action OnAutoMoveAction;
    public event Action<int> OnSkillAction;
    public event Action OnAddSkillAction;
    public event Action OnRemoveSkillAction;

    public PlayerState(int playerId, Vector2Int position, int team, bool isAi, CharacterData characterData)
    {
        PlayerID = playerId;
        CharacterData = characterData;
        AttackCodes = new LinkedList<int>();
        PlayerBuffStates = new List<PlayerBuffState>();

        MaxHp = characterData.hp;
        Hp = characterData.hp;
        MaxMp = characterData.mp;
        Mp = characterData.mp;
        Sh = characterData.sh;
        Move = characterData.move;
        SkillLevel = 1;
        Position = position;
        Team = team;
        IsAi = isAi;
    }

    public void AddBuff(PlayerBuffState buff)
    {
        buff.UpdateStatusAction += OnUpdateStatusAction; // <= 버프 스텟에서도 값이 바뀌게 되면 지금 스텟변경 액션을 등록해서 불필요한 코드를 줄인다.
        OnUpdateStatusAction(); // 버프가 등록되면 한번 호출
    }

    public void TurnUpdate()
    {
        for (int i = PlayerBuffStates.Count - 1; i >= 0; i--)
        {
            var buff = PlayerBuffStates[i];
            if (buff.IsTurn)
            {
                --buff.TurnCount;
                if (buff.TurnCount == 0)
                {
                    PlayerBuffStates.RemoveAt(i);
                }
            }
        }
        OnUpdateStatusAction(); // 턴 종료시 한번 호출해 준다.
    }

    public Tuple<int, int> GetSh()
    {
        int buf = 0;
        foreach (var buffState in PlayerBuffStates)
        {
            buf += buffState.Sh;
        }
        return new Tuple<int, int>(Sh, buf);
    }

    public Tuple<int, int> GetMaxHp()
    {
        int buf = 0;
        foreach (var buffState in PlayerBuffStates)
        {
            buf += buffState.Hp;
        }
        return new Tuple<int, int>(MaxHp, buf);
    }

    public Tuple<int, int> GetMaxMp()
    {
        int buf = 0;
        foreach (var buffState in PlayerBuffStates)
        {
            buf += buffState.Mp;
        }
        return new Tuple<int, int>(MaxMp, buf);
    }

    public Tuple<int, int> GetMove()
    {
        int buf = 0;
        foreach (var buffState in PlayerBuffStates)
        {
            buf += buffState.Move;
        }
        return new Tuple<int, int>(Move, buf);
    }

    public Tuple<int, int> GetSkillLevel()
    {
        int buf = 0;
        foreach (var buffState in PlayerBuffStates)
        {
            buf += buffState.SkillLevel;
        }
        return new Tuple<int, int>(SkillLevel, buf);
    }

    public bool IsDead()
    {
        return Hp <= 0;
    }

    public void OnMove(Vector2Int pos)
    {
        OnMoveAction?.Invoke(pos);
    }

    public void OnAutoMove()
    {
        OnAutoMoveAction?.Invoke();
    }

    public void OnSkill(int skillCode)
    {
        OnSkillAction?.Invoke(skillCode);
    }

    public int GetUseMp()
    {
        int mp = Mp;
        foreach (var attack in AttackCodes)
        {
            var skillData = CharacterData.Skills.Find(data => attack == data.code);
            
            if (skillData != null)
            {
                mp -= skillData.manaCost;
            }
        }
        return mp;
    }

    public void OnAddSkill(int skillCode)
    {
        // 여기서 마나 체크
        int mana = GetUseMp();
        if (AttackCodes.Count < AttackCount)
        {
            var skillData = CharacterData.Skills.Find(data => skillCode == data.code);
            if (skillData != null)
            {
                if (skillData.manaCost <= mana)
                {
                    AttackCodes.AddLast(skillCode);
                    OnAddSkillAction?.Invoke();
                }
            }
        }
    }

    public void OnRemoveSkill(int index)
    {
        // 여기서 마나 체크
        int idx = 0;
        foreach (var skill in AttackCodes)
        {
            if (index == idx)
            {
                AttackCodes.Remove(skill);
                break;
            }
            idx++;
        }
        OnRemoveSkillAction?.Invoke();
    }
}