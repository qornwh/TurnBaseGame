using System;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터 타입 열거형
public enum CharacterType
{
    Guardian,   // 수호
    Carrier,    // 캐리
    Nuker       // 누커
}
// 캐릭터 클래스
[System.Serializable]
public class CharacterData
{
    public string name;
    public int code;
    public CharacterType type;
    public int hp;
    public int mp;
    public int sh;
    public int move;
    public int mpRegen;
    public List<int> skillIds;
    public string description;
    public string iconPath;
    public string Path;

    // 스킬 목록
    public List<SkillData> Skills { get; set; } = new List<SkillData>();

    // 스킬 추가
    public void AddSkill(SkillData skill)
    {
        Skills.Add(skill);
    }

    public override string ToString()
    {
        return $"Name: {name}, Code: {code}, Type: {type}, HP: {hp}, MP: {mp}, SH: {sh}, Move: {move}, MPRegen: {mpRegen}, SkillIds: {string.Join(", ", skillIds)}, Description: {description}";
    }
}