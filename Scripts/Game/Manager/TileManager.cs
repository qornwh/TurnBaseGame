using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TileManager
{
    public delegate GameObject CreateGameObjectDelegate(Vector3 position, Quaternion rotation);
    public event CreateGameObjectDelegate CreateGameObject;
    public delegate GameObject CreateBlockDelegate(GameObject parent, Vector3 position, Quaternion rotation);
    public event CreateBlockDelegate CreateBlock;

    private int colSize = 8;
    private int rowSize = 6;
    public List<List<BlockState<BlockBase>>> FloorBlockStates { get; set; } // 바닥 블록
    public List<List<BlockState<BlockBase>>> RootBlockStates { get; set; } // 게임판판 블록
    public List<List<BlockState<BlockBase>>> ViewBlockStates { get; set; } // 이동경로, 스킬경로 블록

    public void Init()
    {
        var gm = GameInstance.GetInstance().GameManager;
        var gdm = gm.GameDataManager;
        int mapCode = gm.MapCode;
        
        var gameMap = gdm.GetMap(mapCode);
        var gameMapInfo = gdm.GetMapType(gameMap.type);
        colSize = gameMap.width;
        rowSize = gameMap.hight;
        var mapData = gameMap.data;
        
        // block  state를 초기화 한다
        FloorBlockStates = new List<List<BlockState<BlockBase>>>();
        RootBlockStates = new List<List<BlockState<BlockBase>>>();
        ViewBlockStates = new List<List<BlockState<BlockBase>>>();
        for (int y = 0; y < rowSize; y++)
        {
            FloorBlockStates.Add(new List<BlockState<BlockBase>>());
            RootBlockStates.Add(new List<BlockState<BlockBase>>());
            ViewBlockStates.Add(new List<BlockState<BlockBase>>());
            for (int x = 0; x < colSize; x++)
            {
                FloorBlockStates[y].Add(new BlockState<BlockBase>(x, y));
                RootBlockStates[y].Add(new BlockState<BlockBase>(x, y));
                ViewBlockStates[y].Add(new BlockState<BlockBase>(x, y));
            }
        }

        // 바닥 루트 생성
        GameObject rootFloorObject = OnCreateGameObject(new Vector3(0, -0.5f, 0), Quaternion.identity);
        foreach (var cols in FloorBlockStates)
        {
            foreach (var state in cols)
            {
                var child = OnCreateBlock(rootFloorObject, new Vector3(state.Col - colSize / 2 , 0, state.Row - rowSize / 2 ), Quaternion.identity).GetComponent<BlockBase>();
                child.Initialize(state);
                state.Object = child;
                state.GameObject = child;
                state.Type = BlockType.Floor;
            }
        }

        // 필드 루트 생성
        GameObject rootFiledObject = OnCreateGameObject(new Vector3(0, 0.5f, 0), Quaternion.identity);
        for (int i = 0; i < rowSize; i++)
        {
            for (int j = 0; j < colSize; j++)
            {
                var state = RootBlockStates[i][j];
                var child = OnCreateBlock(rootFiledObject, new Vector3(state.Col - colSize / 2 , 0, state.Row - rowSize / 2 ), Quaternion.identity).GetComponent<BlockBase>();
                child.Initialize(state);
                state.Object = child;
                state.GameObject = child;
                state.Type = (BlockType) mapData[i,j];
            }
        }

        // 쉐도우 루트 필드 생성 : 이동 범위, 공격 범위
        GameObject rootShadowFiledObject = OnCreateGameObject(new Vector3(0, 0.5f, 0), Quaternion.identity);
        foreach (var cols in ViewBlockStates)
        {
            foreach (var state in cols)
            {
                var child = OnCreateBlock(rootShadowFiledObject, new Vector3(state.Col - colSize / 2, 0, state.Row - rowSize / 2), Quaternion.identity).GetComponent<BlockBase>();
                child.Initialize(state);
                state.Object = child;
                state.GameObject = child;
                state.Type = BlockType.Shodow | BlockType.MoveDst;
            }
        }

        gm.SpawnPrefab(gameMapInfo.path);
    }

    public void ViewMovePinter(int col, int row, int distance)
    {
        // 가능한 경로 보이기
        for (int y = row - distance; y <= row + distance; y++)
        {
            if (!(0 <= y && y < rowSize)) continue;
            for (int x = col - distance; x <= col + distance; x++)
            {
                if (!(0 <= x && x < colSize)) continue;
                if (RootBlockStates[y][x].Type != BlockType.Empty && col != x && row != y) continue;
                var state = ViewBlockStates[y][x];
                state.Type = BlockType.MoveDst;
            }
        }

        var gm = GameInstance.GetInstance().GameManager;
        gm.SelectBlock(ViewBlockStates[row][col]);
    }

    public void HideMovePinter()
    {
        // 경로 닫기
        foreach (var cols in ViewBlockStates)
        {
            foreach (var state in cols)
            {
                state.Type = BlockType.Shodow;
            }
        }
    }

    public virtual GameObject OnCreateGameObject(Vector3 position, Quaternion rotation)
    {
        return CreateGameObject?.Invoke(position, rotation);
    }

    protected virtual GameObject OnCreateBlock(GameObject parent, Vector3 position, Quaternion rotation)
    {
        return CreateBlock?.Invoke(parent, position, rotation);
    }

    public Vector3 GetTilePosition(int col, int row)
    {
        return FloorBlockStates[row][col].GameObject.transform.position;
    }
    
    public int GetWidth()
    {
        return colSize;
    }

    public int GetHeight()
    {
        return rowSize;
    }
}