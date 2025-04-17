using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PriorityQueue<T>
{
  private Func<T, T, bool> _compare;
  private List<T> _items;

  public int Count => _items.Count - 1;

  public PriorityQueue(int capacity = 100, Func<T, T, bool> compare = null)
  {
    if (compare == null)
    {
      throw new ArgumentNullException(nameof(compare), "Comparison function cannot be null");
    }

    _items = new List<T>(capacity);
    _items.Add(default(T));
    _compare = compare;
  }

  public void Enqueue(T item)
  {
    _items.Add(item);
    int idx = _items.Count - 1;

    while (idx > 1 && _compare(_items[idx], _items[idx / 2]))
    {
      (_items[idx], _items[idx / 2]) = (_items[idx / 2], _items[idx]);
      idx /= 2;
    }
  }

  public T Dequeue()
  {
    if (_items.Count == 0)
    {
      return default(T);
    }

    int idx = 1;
    T temp = _items[_items.Count - 1];
    _items[_items.Count - 1] = _items[1];
    _items[1] = temp;

    int n = _items.Count - 1; // 맨 마지막 제외
    while (idx < n)
    {
      int left = idx * 2;
      int right = idx * 2 + 1;
      if (right < n)
      {
        // 자식 2개
        if (_compare(_items[left], _items[right]))
        {
          if (_compare(_items[left], _items[idx]))
          {
            (_items[idx], _items[left]) = (_items[left], _items[idx]);
            idx = right;
          }
        }
        else
        {
          if (_compare(_items[right], _items[idx]))
          {
            (_items[idx], _items[right]) = (_items[right], _items[idx]);
            idx = right;
          }
        }
      }
      else if (left < n)
      {
        // 자식 1개
        if (_compare(_items[left], _items[idx]))
        {
          (_items[idx], _items[left]) = (_items[left], _items[idx]);
          idx = left;
        }
      }

      if (!(idx == left || idx == right))
      {
        break;
      }
    }

    temp = _items[_items.Count - 1];
    _items.RemoveAt(_items.Count - 1);
    return temp;
  }

  public T Peek()
  {
    if (_items.Count == 0)
    {
      return default(T);
    }

    return _items[1];
  }
}

