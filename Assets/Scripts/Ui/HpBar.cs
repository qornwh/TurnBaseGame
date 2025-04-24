using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private Camera _camera;
    private PlayerState _state;
    
    public void Init(PlayerState state)
    {
        _state = state;
        _state.UpdateStatusAction += UpdateHp;

        var gm = GameInstance.GetInstance().GameManager;
        _camera = gm.GameCamera;
    }

    public void UpdateHp()
    {
        if (_state != null)
            slider.value = (float)(_state.Hp) / _state.MaxHp;
    }

    void Update()
    {
        if (_camera != null)
            transform.forward = _camera.transform.forward;
    }
}
