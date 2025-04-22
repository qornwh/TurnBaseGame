using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillListView : MonoBehaviour
{
    // 스킬 스크롤 뷰
    [SerializeField] private SkillIcon skillScrollViewContentPrefab;

    void Start()
    {
        ScrollRect scrollRect = gameObject.GetComponent<ScrollRect>();
        var content = scrollRect.content;
        float height = 0f;

        var gm = GameInstance.GetInstance().GameManager;
        var playerState = gm.GetPlayerState(gm.PlayerId);

        var grid = content.GetComponent<GridLayoutGroup>();

        float n = content.GetComponent<RectTransform>().rect.width;
        n -= grid.spacing.x;
        n /= 2;
        grid.cellSize = new Vector2(n, n);
        
        if (playerState != null)
        {
            var gameDataManager = gm.GameDataManager;
            var skillList = gameDataManager.GetCharacterSkills(playerState.CharacterData.code);
            foreach (var skill in skillList)
            {
                var skillObj = gm.CreateObject(skillScrollViewContentPrefab, content.gameObject);
                skillObj.Init(skill.code);
                skillObj.OnClick += AddActionSkill;
            }
        }
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, height);
    }

    void AddActionSkill(int skillId)
    {
        var gm = GameInstance.GetInstance().GameManager;
        var playerState = gm.GetPlayerState(gm.PlayerId);
        playerState.OnAddSkill(skillId);
    }
}
