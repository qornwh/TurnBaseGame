using System;
using System.Collections.Generic;

public abstract class StateBase
{
    public virtual int Hp
    {
        get => _hp;
        set => SetValue(ref _hp, value);
    }

    public virtual int Mp
    {
        get => _mp;
        set => SetValue(ref _mp, value);
    }

    public virtual int Sh
    {
        get => _sh;
        set => SetValue(ref _sh, value);
    }

    public virtual int Move
    {
        get => _move;
        set => SetValue(ref _move, value);
    }

    public virtual int SkillLevel
    {
        get => _skillLevel;
        set => SetValue(ref _skillLevel, value);
    }

    protected int _hp;
    protected int _mp;
    protected int _sh;
    protected int _move;
    protected int _skillLevel;
    public event Action UpdateStatusAction;
    
    ~StateBase()
    {
        UpdateStatusAction = null; // 객체 소멸될때 Action에 등록된 모든 메서드 해제한다.
    }

    protected virtual void SetValue<T>(ref T field, T value)
    {
        // 공통 유틸 setValue <= 여기서 추가적인 작업을 넣을수 있다. 단, 프로퍼티로 생성한것만 된다.
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnUpdateStatusAction();
        }
    }

    protected void OnUpdateStatusAction()
    {
        UpdateStatusAction?.Invoke();
    }
}