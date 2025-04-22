using System;
using System.Collections.Generic;
using UnityEngine;

// 게임 데이터 클래스
[System.Serializable]
public class GameData
{
  public List<string> characterTypes;
  public List<string> skillTypes;
  public List<CharacterData> characterInfo;
  public List<SkillData> skills;

  // 맵도 추가하기
  public List<BlockData> blockTypes;
  public List<MapTypeInfo> mapTypes;
  public List<MapData> maps;
}