using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillQueueView : MonoBehaviour
{
    // 스킬 스크롤 뷰
    [SerializeField] private SkillIcon skillScrollViewContentPrefab;
    
    // 마나
    [SerializeField] private SlideBarUi ManaProgressBar;
    
    void Start()
    {
        var gm = GameInstance.GetInstance().GameManager;
        var playerState = gm.GetPlayerState(gm.PlayerId);
        playerState.OnAddSkillAction += UpdateView;
        playerState.OnRemoveSkillAction += UpdateView;
        UpdateView();
        ManaProgressBar.Init(playerState.GetUseMp());
    }

    void UpdateView()
    {
        var gm = GameInstance.GetInstance().GameManager;
        var playerState = gm.GetPlayerState(gm.PlayerId);
        var content = gameObject.GetComponent<RectTransform>();

        foreach (Transform child in content.GetComponentInChildren<Transform>())
        {
            Destroy(child.gameObject);
        }
        
        int idx = 0;
        foreach (var skillCode in playerState.AttackCodes)
        {
            var skillObj = gm.CreateObject(skillScrollViewContentPrefab, content.gameObject);
            skillObj.Init(skillCode);
            skillObj.Index = idx++;
            skillObj.OnClick += _skillCode =>
            {
                playerState.OnRemoveSkill(skillObj.Index);
            };
            skillObj.SetEnableMinus();
        }
        
        ManaProgressBar.UpdatePersent(playerState.GetUseMp());
    }
}
