using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerState PlayerState { get; set; }
    public int PlayerId { get; set; }
    public int Type { get; set; }

    public void Init(PlayerState characterData, Vector3 position)
    {
        PlayerState = characterData;
        PlayerState.OnMoveAction += PlayMoveAction;
        PlayerState.OnSkillAction += PlaySkillAction;
        gameObject.transform.position = position;
    }

    void PlayMoveAction(int col, int row)
    {
        var gm = GameInstance.GetInstance().GameManager;
        var tileManager = gm.TileManager;
        var newPosition = tileManager.GetTilePosition(col, row);
        gameObject.transform.position = newPosition;
    }

    void PlaySkillAction(int skillCode)
    {

    }
}
