using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// 스킬 타입 열거형
public enum SkillType
{
    Attack,
    Buff,
    Debuff
}

// 스킬 서브타입 열거형
public enum SkillSubType
{
    // 공격 서브타입
    SelfArea,
    TargetArea,

    // 버프 서브타입
    HealHp,
    HealMp,
    IncreaseAttack,
    IncreaseDefense,

    // 디버프 서브타입
    DecreaseAttack,
    DecreaseDefense
}

// 스킬 효과 인터페이스
public interface ISkillEffect
{
    void ApplyEffect();
}

// 공격 효과 컴포넌트
[System.Serializable]
public class AttackEffect : ISkillEffect
{
    public int damage;
    public int areaSize;

    public void ApplyEffect()
    {
        // 공격 효과 적용 로직
    }
}

// 회복 효과 컴포넌트
[System.Serializable]
public class HealEffect : ISkillEffect
{
    public int healAmount;

    public void ApplyEffect()
    {
    }
}

// 버프 효과 컴포넌트
[System.Serializable]
public class BuffEffect : ISkillEffect
{
    public int increasePercent;
    public int durationTurn;

    public void ApplyEffect()
    {
        // 버프 효과 적용 로직
    }
}

// 디버프 효과 컴포넌트
[System.Serializable]
public class DebuffEffect : ISkillEffect
{
    public int decreasePercent;
    public int durationTurn;

    public void ApplyEffect()
    {
        // 디버프 효과 적용 로직
    }
}

[System.Serializable]
public class Level
{
    public int level;
    public int[] center;
    public int[,] range;
}

// 스킬 클래스
[System.Serializable]
public class SkillData
{
    public int code;
    public string name;
    public SkillType type;
    public SkillSubType subtype;
    public int manaCost;
    public string description;
    public string iconPath;
    public string path;
    public List<Level> levelRange;

    // 스킬 효과 컴포넌트
    private ISkillEffect effect;

    // 효과 컴포넌트 설정
    public void SetEffect(ISkillEffect effectComponent)
    {
        effect = effectComponent;
    }

    // 스킬 사용
    public void UseSkill()
    {
        if (effect != null)
        {
            effect.ApplyEffect();
        }
        else
        {
            Debug.LogError($"Skill {name} has no effect component!");
        }
    }

    public override string ToString()
    {
        return $"Name: {name}, Type: {type}, Subtype: {subtype}, ManaCost: {manaCost}, Description: {description}";
    }
}