using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerState
{
    public readonly int AttackCount = 8;
    
    public int PlayerID { get; set; }
    public int Hp { get; set; }
    public int Mp { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }
    public int Team { get; set; }
    public bool IsAi { get; set; }
    public LinkedList<int> AttackCodes { get; set; } = new LinkedList<int>();
    public CharacterData CharacterData { get; set; }
    public event Action<int, int> OnMoveAction;
    public event Action<int> OnSkillAction;
    public event Action OnAddSkillAction;
    public event Action OnRemoveSkillAction;

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