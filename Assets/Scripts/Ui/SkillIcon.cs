using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button buttonSKill;
    [SerializeField] private Button buttonMinus;

    public event Action<int> ClickAction;
    public event Action<int> MouseEnterAction;
    public event Action<int> MouseOutAction;
    
    private SkillData SkillData { get; set; }
    private PlayerState PlayerState { get; set; }
    
    public int Index { get; set; }

    private void Awake()
    {
        if (buttonMinus != null)
        {
            buttonMinus.gameObject.SetActive(false);
        }
    }

    public void Init(int code)
    {
        var gm = GameInstance.GetInstance().GameManager;
        var gameDataManager = gm.GameDataManager;
        PlayerState = gm.GetPlayerState(gm.PlayerId);
        var skill = gameDataManager.GetSkill(code);
        if (skill != null)
        {
            SkillData = skill;
            SetButtonImage(gm.LoadSpriteFromResources(SkillData.iconPath));
        }
    }

    public void SetEnableMinus(bool flag = true)
    {
        buttonMinus.gameObject.SetActive(flag);
    }

    private void SetButtonImage(Sprite sprite)
    {
        if (buttonSKill == null) return;

        Image buttonImage = buttonSKill.GetComponent<Image>();

        if (buttonImage == null)
        {
            buttonImage = buttonSKill.gameObject.AddComponent<Image>();
        }

        buttonImage.sprite = sprite;
        buttonImage.raycastPadding = new Vector4(3,3,3,3);
    }

    public void OnClick()
    {
        ClickAction?.Invoke(SkillData.code);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseEnterAction?.Invoke(SkillData.code);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseOutAction?.Invoke(SkillData.code);
    }
}
