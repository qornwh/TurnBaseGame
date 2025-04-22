using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class BlockBase : MonoBehaviour, IBlock, IMouseHoverHandler
{
    // 게임오브젝트에서는 타입만 보도록 한다.
    // 추가로 클릭 이벤트만 있다.
    private WeakReference<BlockState<BlockBase>> _blockStateRef;
    public delegate void SelectBlockDelegate(BlockState<BlockBase> state);
    public event SelectBlockDelegate SelectBlock;
    
    public GameObject Child { get; set; }

    public void Initialize(BlockState<BlockBase> blockState)
    {
        _blockStateRef = new WeakReference<BlockState<BlockBase>>(blockState);
        var gm = GameInstance.GetInstance().GameManager;
        SelectBlock += gm.SelectBlock;
    }

    public void UpdateBlockView()
    {
        var gm = GameInstance.GetInstance().GameManager;
        var gdm = gm.GameDataManager;

        BlockType type = GetBlockType();
        if ((type & BlockType.Shodow) == BlockType.Shodow)
        {
            if (Child != null)
                Child.SetActive(false);
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            if (type == BlockType.Player) return;
            GameObject prefab = gm.SpawnPrefab(gdm.GetBlock((int)type).path, gameObject);
            Child = prefab;
            Child.SetActive(true);
        }
    }

    public void OnSelectBlock(BlockState<BlockBase> state)
    {
        SelectBlock?.Invoke(state);
    }

    public void OnMouseEnter()
    {
        // mouseIn
        BlockType type = GetBlockType();

        if (type == BlockType.MoveDst)
        { 
            Color color = Child.GetComponent<MeshRenderer>().material.color;
            color.a = 0.6f;
            Child.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    public void OnMouseExit()
    {
        // mouseOut
        BlockType type = GetBlockType();

        if (type == BlockType.MoveDst)
        {
            Color color = Child.GetComponent<MeshRenderer>().material.color;
            color.a = 0.2f;
            Child.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    public void OnMouseDown()
    {
        // mouseClick
        BlockType type = GetBlockType();

        if (type == BlockType.MoveDst)
        {
            _blockStateRef.TryGetTarget(out var blockState);
            OnSelectBlock(blockState);
        }
    }

    private BlockType GetBlockType()
    {
        if (_blockStateRef == null) 
            return BlockType.Empty;
        _blockStateRef.TryGetTarget(out var blockState);
        if (blockState == null) 
            return BlockType.Empty;

        return blockState.Type;
    }
}
