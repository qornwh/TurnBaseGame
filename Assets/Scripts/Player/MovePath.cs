using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MovePath : MonoBehaviour
{
    private readonly float _moveSpeed = 2f; // 고정
    private List<Vector2Int> _pathPoints;
    private int _currentIndex;
    private TileManager _tileManager;

    public event Action PathComplatedAction;

    void Start()
    {
        var gm = GameInstance.GetInstance().GameManager;
        var tileManager = gm.TileManager;
        _tileManager = tileManager;
    }

    public void StartMove(List<Vector2Int> path)
    {
        _pathPoints = path;
        _currentIndex = 1;
        StopAllCoroutines();
        StartCoroutine(MovePathUpdate());
    }

    // Update is called once per frame
    private IEnumerator MovePathUpdate()
    {
        while (_currentIndex < _pathPoints.Count)
        {
            Vector3 target = _tileManager.GetTileWordPosition(_pathPoints[_currentIndex]);
            
            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                Vector3 direction = (transform.position - target).normalized;
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.SetPositionAndRotation(Vector3.MoveTowards(transform.position, target, _moveSpeed * Time.deltaTime), rotation);
                yield return null;
            }

            transform.position = target;
            _currentIndex++;
        }
        OnPathComplete();
    }

    private void OnPathComplete()
    {
        PathComplatedAction?.Invoke();
    }

    private void OnDestroy()
    {
        PathComplatedAction = null;
    }
}
