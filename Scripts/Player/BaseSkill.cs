using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseSkill
{

  // 모든 스킬은 해당 클래스 상속받는다, 그후 애니메이션 종료될때, 콜백 등록하기
  public int SkillCode { get; set; }
  public int SkillLevel { get; set; }
  public int SkillCost { get; set; }
  public int SkillDamage { get; set; }
}
