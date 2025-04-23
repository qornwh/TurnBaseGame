using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerController _playerController;
    public PlayerState PlayerState { get; set; }
    public int PlayerId { get; set; }
    public int Type { get; set; }
    private GameObject currentPrefab;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    public void Init(PlayerState state, Vector3 position)
    {
        PlayerState = state;
        PlayerState.OnMoveAction += PlayMove;
        PlayerState.OnAutoMoveAction += AutoMove;
        PlayerState.OnAutoSkillAction += AutoSkill;
        PlayerState.OnSkillAction += PlaySkillAction;
        gameObject.transform.position = position;
        
        // 프리팹 로드
        var gm = GameInstance.GetInstance().GameManager;
        SetCharacterPrefab(gm.LoadPrefab(PlayerState.CharacterData.Path));
    }

    public void SetCharacterPrefab(GameObject prefab)
    {
        if (currentPrefab != null)
        {
            Destroy(currentPrefab);
        }
        
        currentPrefab = Instantiate(prefab, gameObject.transform);
    }

    void PlayMove(Vector2Int pos)
    {
        _playerController.MoveTo(PlayerState, pos);
    }

    void AutoMove()
    {
        _playerController.AutoMoveTo(PlayerState);
    }

    void AutoSkill()
    {
        _playerController.AutoSkillTo(PlayerState);
    }

    void PlaySkillAction(int skillCode)
    {
        _playerController.Skill(PlayerState, skillCode);
    }
}
