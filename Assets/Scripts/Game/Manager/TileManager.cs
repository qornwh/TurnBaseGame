using System;
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
                FloorBlockStates[y].Add(new BlockState<BlockBase>(new Vector2Int(x, y)));
                RootBlockStates[y].Add(new BlockState<BlockBase>(new Vector2Int(x, y)));
                ViewBlockStates[y].Add(new BlockState<BlockBase>(new Vector2Int(x, y)));
            }
        }
        
        // 초기 포지션 계산 <= 여기서만 사용
        var accPosition = new Func<Vector2Int, Vector3>((Vector2Int position) =>
        {
            return new Vector3((float) position.x - colSize / 2, 0, (float) position.y - rowSize / 2);
        });

        // 바닥 루트 생성
        GameObject rootFloorObject = OnCreateGameObject(new Vector3(0, -0.5f, 0), Quaternion.identity);
        foreach (var cols in FloorBlockStates)
        {
            foreach (var state in cols)
            {
                var child = OnCreateBlock(rootFloorObject, accPosition(state.Position), Quaternion.identity).GetComponent<BlockBase>();
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
                var child = OnCreateBlock(rootFiledObject, accPosition(state.Position), Quaternion.identity).GetComponent<BlockBase>();
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
                var child = OnCreateBlock(rootShadowFiledObject, accPosition(state.Position), Quaternion.identity).GetComponent<BlockBase>();
                child.Initialize(state);
                state.Object = child;
                state.GameObject = child;
                state.Type = BlockType.Shodow | BlockType.MoveDst;
            }
        }

        gm.SpawnPrefab(gameMapInfo.path);
    }

    public void ViewMovePinter(int curX, int curY, int distance)
    {
        var gm = GameInstance.GetInstance().GameManager;
        gm.SelectBlock(ViewBlockStates[curY][curX]);

        curY -= distance;
        curX -= distance;
        // 가능한 경로 보이기
        for (int y = 0; y <= distance * 2; y++)
        {
            int relativeY = curY + y;
            if (relativeY < 0 || relativeY >= rowSize) continue;
            for (int x = 0; x <= distance * 2; x++)
            {
                int relativeX = curX + x;
                if (curX == x && curY == y) continue;
                if (relativeX < 0 || relativeX >= colSize) continue;
                if (Mathf.Abs(x - distance) + Mathf.Abs(y - distance) > distance) continue; // 맨해튼 거리
                if (RootBlockStates[relativeY][relativeX].Type != BlockType.Empty) continue;
                var state = ViewBlockStates[relativeY][relativeX];
                state.Type = BlockType.MoveDst;
            }
        }
    }

    public void ViewSkillPointer(int curX, int curY, in int[,] range)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        SkillPointerList(curX, curY, range, positions);

        foreach (var position in positions)
        {
            var state = ViewBlockStates[position.y][position.x];
            state.Type = BlockType.MoveDst;
        }
    }

    public void SkillPointerList(int curX, int curY, in int[,] range, List<Vector2Int> list, List<Vector2Int> players = null)
    {
        for (int y = 0; y < range.GetLength(0); y++)
        {
            int relativeY = curY + y;
            if (relativeY < 0 || relativeY >= rowSize) continue;
            for (int x = 0; x < range.GetLength(1); x++)
            {
                int relativeX = curX + x;
                if (relativeX < 0 || relativeX >= colSize) continue;
                if (RootBlockStates[relativeY][relativeX].Type == BlockType.Wall) continue;
                if (range[y,x] == 0) continue;
                var state = ViewBlockStates[relativeY][relativeX];
                list.Add(state.Position);
                if (players != null)
                {
                    if (RootBlockStates[relativeY][relativeX].Type == BlockType.Player) 
                        players.Add(state.Position);
                }
            }
        }
    }

    public bool TriggerTargetPosition(int curX, int curY, in int[,] range, Vector2Int targetPosition)
    {
        for (int y = 0; y < range.GetLength(0); y++)
        {
            int relativeY = curY + y;
            if (relativeY < 0 || relativeY >= rowSize) continue;
            for (int x = 0; x < range.GetLength(1); x++)
            {
                int relativeX = curX + x;
                if (relativeX < 0 || relativeX >= colSize) continue;
                if (RootBlockStates[relativeY][relativeX].Type == BlockType.Wall) continue;
                if (range[y,x] == 0) continue;
                
                if (targetPosition.x == relativeX && targetPosition.y == relativeY) return true;
            }
        }
        return false;
    }

    public void HideMovePointer()
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

    public Vector3 GetTilePosition(Vector2Int pos)
    {
        return FloorBlockStates[pos.y][pos.x].GameObject.transform.position;
    }

    public Vector3 GetTileWordPosition(Vector2Int pos)
    {
        return FloorBlockStates[pos.y][pos.x].GameObject.transform.localPosition;
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