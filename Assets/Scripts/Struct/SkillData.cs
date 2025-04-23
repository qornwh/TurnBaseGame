using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
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
    AreaAttack,
    TargetAttack,
    
    // 버프 서브타입
    HealHp,
    HealMp,
    IncreaseAttack,
    IncreaseDefense,

    // 디버프 서브타입
    DecreaseAttack,
    DecreaseDefense
}

public class AttackEffect : SkillData
{
    public int damage;
}

public class HealHpEffect : SkillData
{
    public int healAmount;
}
public class BuffEffect : SkillData
{
    public int increasePercent;
    public int durationTurn;
}

public class Level
{
    public int level;
    public int[] center;
    public int[,] range;
}

// 스킬 클래스
[JsonConverter(typeof(SkillJsonConverter))]
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

    public override string ToString()
    {
        return $"Name: {name}, Type: {type}, Subtype: {subtype}, ManaCost: {manaCost}, Description: {description}";
    }
}