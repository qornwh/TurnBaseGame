using System;
using UnityEngine;

[Flags]
public enum BlockType
{
    Empty         = 0,
    Floor         = 1,   // 바닥
    Wall          = 2,   // 벽
    Spawn         = 3,   // 스폰위치
    Acceleration  = 4,   // 속도증가
    Enforce       = 5,   // 기술강화
    Player        = 6,   // 플레이어
    MoveDst       = 10,  // 이동 범위
    AttackDst     = 11,  // 공격 범위
    MoveSelectDst = 12,  // 이동 범위 선택
    Shodow        = 100, // 쉐도우
}

public class BlockState<T>
{
    public Vector2Int Position { get; set; }

    private BlockType _type;
    private IBlock _object;
    private T _gameObject;
    
    public BlockState(Vector2Int position)
    {
        Position = position;
        Type = BlockType.Empty;
    }
    
    public BlockType Type
    {
        get
        {
            return _type;
        }
        set
        {
            _type = value;
            OnUpdateState();
        }
    }

    public IBlock Object
    {
        get
        {
            return _object;
        }
        set
        {
            _object = value;
            if (_object != null)
                UpdateState += _object.UpdateBlockView;
        }
    }
    
    public T GameObject {get; set;}

    public delegate void EventStateHander();
    public event EventStateHander UpdateState;

    public void OnUpdateState()
    {
        UpdateState?.Invoke();
    }
}