using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Newtonsoft.Json;

public class GameDataManager
{
    private GameData gameData;
    private Dictionary<int, SkillData> skillDictionary = new Dictionary<int, SkillData>();
    private Dictionary<int, CharacterData> characterDictionary = new Dictionary<int, CharacterData>();
    private Dictionary<int, BlockData> blockDictionary = new Dictionary<int, BlockData>();
    private Dictionary<int, MapTypeInfo> mapInfoDictionary = new Dictionary<int, MapTypeInfo>();
    private Dictionary<int, MapData> mapDictionary = new Dictionary<int, MapData>();

    public GameDataManager()
    {
        LoadGameData();
    }

    private void LoadGameData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("GameJson/Characters");
        if (jsonFile != null)
        {
            gameData = JsonConvert.DeserializeObject<GameData>(jsonFile.text);
            Debug.Log(gameData.ToString());
            InitializeGameData();
        }
        else
        {
            Debug.LogError("Characters.json file not found in Resources folder!");
        }
    }

    private void InitializeGameData()
    {
        // 스킬 초기화
        foreach (var skillData in gameData.skills)
        {
            skillDictionary.Add(skillData.code, skillData);
        }

        // 캐릭터 초기화
        foreach (var characterData in gameData.characterInfo)
        {
            CharacterData character = characterData;

            // 캐릭터의 스킬 설정
            foreach (var skillId in character.skillIds)
            {
                if (skillDictionary.TryGetValue(skillId, out SkillData skill))
                {
                    character.AddSkill(skill);
                }
                else
                {
                    Debug.LogError($"Skill with ID {skillId} not found for character {character.name}!");
                }
            }

            characterDictionary.Add(character.code, character);
        }

        foreach (var blockData in gameData.blockTypes)
        {
            blockDictionary.Add(blockData.code, blockData);
        }

        foreach (var info in gameData.mapTypes)
        {
            mapInfoDictionary.Add(info.code, info);
        }

        foreach (var map in gameData.maps)
        {
            mapDictionary.Add(map.code, map);
        }
    }

    // 캐릭터 가져오기
    public CharacterData GetCharacter(int code)
    {
        if (characterDictionary.TryGetValue(code, out CharacterData character))
        {
            return character;
        }
        else
        {
            Debug.LogError($"Character with code {code} not found!");
            return null;
        }
    }

    // 스킬 가져오기
    public SkillData GetSkill(int code)
    {
        if (skillDictionary.TryGetValue(code, out SkillData skill))
        {
            return skill;
        }
        else
        {
            Debug.LogError($"Skill with code {code} not found!");
            return null;
        }
    }

    public List<SkillData> GetSkills()
    {
        return gameData.skills;
    }

    // 캐릭터가 가진 스킬 데이터 가져오기
    public List<SkillData> GetCharacterSkills(int code)
    {
        List<SkillData> skills = new List<SkillData>();
        if (characterDictionary.TryGetValue(code, out CharacterData character))
        {
            return character.Skills;
        }
        return skills;
    }
    
    // 블록 타일 가져오기
    public BlockData GetBlock(int code)
    {
        if (blockDictionary.TryGetValue(code, out BlockData block))
        {
            return block;
        }
        else
        {
            Debug.LogError($"block with code {code} not found!");
            return null;
        }
    }
    
    // 맵 데이터 가져오기
    public MapTypeInfo GetMapType(int code)
    {
        if (mapInfoDictionary.TryGetValue(code, out MapTypeInfo info))
        {
            return info;
        }
        else
        {
            Debug.LogError($"mapType with code {code} not found!");
            return null;
        }
    }

    public MapData GetMap(int code)
    {
        if (mapDictionary.TryGetValue(code, out MapData map))
        {
            return map;
        }
        else
        {
            Debug.LogError($"map with code {code} not found!");
            return null;
        } 
    }
}
