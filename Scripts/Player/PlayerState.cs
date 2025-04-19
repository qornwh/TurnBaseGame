using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerState : StateBase
{
    public readonly int AttackCount = 8;
    public int PlayerID { get; set; }
    public int MaxHp { get; set; }
    public int MaxMp { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }
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
    public event Action<int, int> OnMoveAction;
    public event Action<int> OnSkillAction;
    public event Action OnAddSkillAction;
    public event Action OnRemoveSkillAction;

    public PlayerState(int playerId, int posX, int posY, int team, bool isAi, CharacterData characterData)
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
        PosX = posX;
        PosY = posY;
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

    public bool IsDead()
    {
        return Hp <= 0;
    }

    public void OnMove(int arg1, int arg2)
    {
        OnMoveAction?.Invoke(arg1, arg2);
    }

    public void OnSkill(int skillCode)
    {
        OnSkillAction?.Invoke(skillCode);
    }

    public void OnAddSkill(int skillCode)
    {
        if (AttackCodes.Count < AttackCount)
        {
            AttackCodes.AddLast(skillCode);
            OnAddSkillAction?.Invoke();
        }
    }

    public void OnRemoveSkill(int index)
    {
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