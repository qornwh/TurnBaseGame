using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviour
{
    [SerializeField] private GameObject protrait;
    [SerializeField] private SlideBarUi hp;
    [SerializeField] private SlideBarUi mp;
    [SerializeField] private SubStateUi sh;
    [SerializeField] private SubStateUi sl;
    [SerializeField] private SubStateUi mv;
    public PlayerState PlayerState { get; set; }
    
    public void Init(PlayerState playerState)
    {
        // state : 이동, 스킬레벨, 실드 초기화
        var gm = GameInstance.GetInstance().GameManager;
        PlayerState = playerState;
        
        hp.Init(PlayerState.MaxHp);
        mp.Init(PlayerState.MaxMp);
        sh.Init("SH", PlayerState.Sh);
        sl.Init("SL", PlayerState.SkillLevel);
        mv.Init("MV", PlayerState.Move);
        PlayerState.UpdateStatusAction += UpdateState;

        
        Image image = protrait.GetComponent<Image>();

        if (image == null)
        {
            image = protrait.AddComponent<Image>();
        }
        image.sprite = gm.LoadSpriteFromResources(PlayerState.CharacterData.iconPath);
        image.type = Image.Type.Filled;
    }

    void UpdateState()
    {
        // 여기서 hp, mp, 스테이트 갱신
        hp.UpdatePersent(PlayerState.Hp);
        mp.UpdatePersent(PlayerState.Mp);
        sh.UpdateState(PlayerState.Sh);
        sl.UpdateState(PlayerState.SkillLevel);
        mv.UpdateState(PlayerState.Move);
    }
}
