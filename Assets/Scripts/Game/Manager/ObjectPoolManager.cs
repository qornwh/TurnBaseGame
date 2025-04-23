using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    [Serializable]
    public class ObjectInfo
    {
        public int objectCode;
        public GameObject perfab;
    }

    public class ObjectPool
    {
        private IObjectPool<GameObject> _objectPool;
        ObjectInfo _objectInfo;

        public ObjectPool(ObjectInfo info)
        {
            _objectInfo = info;
            _objectPool = new ObjectPool<GameObject>(CreatePrefab,
                OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject);
        }

        public IObjectPool<GameObject> GetPool()
        {
            return _objectPool;
        }

        private GameObject CreatePrefab()
        {
            GameObject objectInstance = Instantiate(_objectInfo.perfab);
            return objectInstance;
        }
        
        private void OnReleaseToPool(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }
        
        private void OnGetFromPool(GameObject gameObject)
        {
            gameObject.SetActive(true);
        }
        
        private void OnDestroyPooledObject(GameObject gameObject)
        {
            Destroy(gameObject);
        }
    }
    
    [SerializeField]
    private List<ObjectInfo> _objectInfos;
    private Dictionary<int, ObjectPool> _objectPoolDic = new Dictionary<int, ObjectPool>();

    void Start()
    {
        _objectInfos = new List<ObjectInfo>();
        _objectPoolDic = new Dictionary<int, ObjectPool>();
        
        var gm = GameInstance.GetInstance().GameManager;
        var gdm = gm.GameDataManager;
        var skills = gdm.GetSkills();

        foreach (var skill in skills)
        {
            _objectInfos.Add(new ObjectInfo()
            {
                objectCode = skill.code,
                perfab = gm.LoadPrefab(skill.path)
            });
        }

        init();
    }
    
    private void init()
    {
        foreach (var obj in _objectInfos)
        {
            if (!_objectPoolDic.ContainsKey(obj.objectCode))
            {
                _objectPoolDic.Add(obj.objectCode, new ObjectPool(obj));
            }
        }
    }

    public ObjectPool GetObjectPool(int objectCode)
    {
        return _objectPoolDic[objectCode];
    }
}