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

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    public void Init(PlayerState state, Vector3 position)
    {
        PlayerState = state;
        PlayerState.OnMoveAction += PlayMove;
        PlayerState.OnAutoMoveAction += AutoMove;
        PlayerState.OnSkillAction += PlaySkillAction;
        gameObject.transform.position = position;
    }

    void PlayMove(Vector2Int pos)
    {
        _playerController.MoveTo(PlayerState, pos);
    }

    void AutoMove()
    {
        _playerController.AutoMoveTo(PlayerState);
    }

    void PlaySkillAction(int skillCode)
    {

    }
}
