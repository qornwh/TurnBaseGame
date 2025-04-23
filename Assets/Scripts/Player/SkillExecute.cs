using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SkillExecute : MonoBehaviour
{    
    private int _skillCode;
    private List<GameObject> _skillList;
    public event Action SkillComplatedAction;
    
    void Start()
    {
        _skillList = new List<GameObject>();
    }

    public void StartSkill(int skillCode, List<Vector3> positions)
    {
        // duration 동안 진행하고 turnend 발생시킨다
        StopAllCoroutines();
        
        var gm = GameInstance.GetInstance().GameManager;
        var opm = gm.ObjectPoolManager;
        var objectPool = opm.GetObjectPool(skillCode);
        if (objectPool != null)
        {
            foreach (var position in positions)
            {
                var obj = objectPool.GetPool().Get();
                obj.transform.position = position;
                _skillList.Add(obj);
            }
        }
        StartCoroutine(SkillUpdate(skillCode));
    }

    private IEnumerator SkillUpdate(int skillCode)
    {
        yield return new WaitForSeconds(1f);
        
        var gm = GameInstance.GetInstance().GameManager;
        var opm = gm.ObjectPoolManager;
        var objectPool = opm.GetObjectPool(skillCode);
        if (objectPool != null)
        {
            foreach (var skill in _skillList)
            {
                objectPool.GetPool().Release(skill);
            }
        }
        _skillList.Clear();
        OnSkillComplated();
    }

    private void OnSkillComplated()
    {
        SkillComplatedAction?.Invoke();
    }

    private void OnDestroy()
    {
        SkillComplatedAction = null;
    }
}
