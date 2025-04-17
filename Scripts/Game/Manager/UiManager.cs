using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager
{
    [SerializeField]
    private GameObject _doneUi;
    [SerializeField]
    private GameObject _playUi;
    public Dictionary<string, GameObject> UiDic { get; set; } = new Dictionary<string, GameObject>();

    public void Init()
    {
        var gm = GameInstance.GetInstance().GameManager;
        string attackUiPrefabPath = "Prefabs/AttackUi"; // 프리팹 경로
        GameObject attackUiInstance = gm.SpawnPrefab(attackUiPrefabPath);
        UiDic.Add("AttackUi", attackUiInstance);
        UiDic["AttackUi"].SetActive(false);
    }

    public void CreateDoneUi()
    {
        string doneUiPrefabPath = "Prefabs/DoneUi"; // 프리팹 경로
        var gm = GameInstance.GetInstance().GameManager;
        GameObject doneUiInstance = gm.SpawnPrefab(doneUiPrefabPath);
        doneUiInstance.SetActive(true);
    }
}