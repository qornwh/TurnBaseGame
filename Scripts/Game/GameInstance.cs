using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstance
{
    private static GameInstance _instance;
    public GameInstance()
    {

    }
    public static GameInstance GetInstance()
    {
        if (_instance == null)
        {
            _instance = new GameInstance();
        }
        return _instance;
    }
    public GameManager GameManager { get; set; }
}
