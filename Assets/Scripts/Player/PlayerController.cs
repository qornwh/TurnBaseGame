using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    class PathNode
    {
        public PathNode Parent { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int G { get; set; }
        public int H { get; set; }
        public int Cost() => H + G;

        public PathNode(int g, int h, int x, int y, PathNode parent)
        {
            Parent = parent;
            X = x;
            Y = y;
            H = h;
            G = g;
        }
    }

    private TileManager _tileManager;
    private MovePath _movePath;
    private SkillExecute _skillExecute;

    void Awake()
    {
        _movePath = GetComponent<MovePath>();
        _skillExecute = GetComponent<SkillExecute>();
    }
    
    void Start()
    {
        var gm = GameInstance.GetInstance().GameManager;
        var tileManager = gm.TileManager;
        _tileManager = tileManager;
        _movePath.PathComplatedAction += gm.EndTurn;
        _skillExecute.SkillComplatedAction += gm.EndTurn;
    }

    private void GetEnemyPos(PlayerState playerState, ref Vector2Int target)
    {
        var gm = GameInstance.GetInstance().GameManager;

        int enemyTeam = playerState.Team == 1 ? 2 : 1;
        PlayerState enemy = gm.TeamDic[enemyTeam][0];
        
        foreach (var state in gm.TeamDic[enemyTeam])
        {
            if (!state.IsDead())
            {
                enemy = state;
                break;
            }
        }
        
        target = enemy.Position;
    }

    private void Neighbor(int x, int y, Vector2Int target, PathNode node, int[,] dist,
        PriorityQueue<PathNode> pq)
    {
        // 범위 내에 들어가 있어야 한다.
        if (0 <= x && x < _tileManager.GetWidth() && 0 <= y && y < _tileManager.GetHeight())
        {
            var blockArray2D = _tileManager.RootBlockStates;
            if (dist[y, x] == 0 && blockArray2D[y][x].Type == BlockType.Empty)
            {
                int nextG = node.G + 1;
                int nextH = Mathf.Abs(x - target.x) + Mathf.Abs(y - target.y);
                dist[y, x] = nextH;
                pq.Enqueue(new PathNode(nextG, nextH, x, y, node));
            }
        }
    }

    public void AutoMoveTo(PlayerState playerState)
    {
        Vector2Int target = new Vector2Int(0, 0);
        GetEnemyPos(playerState, ref target);
        
        MoveTo(playerState, target, true);
    }

    public void MoveTo(PlayerState playerState, Vector2Int target, bool auto = false)
    {
        int posX = playerState.Position.x;
        int posY = playerState.Position.y;
        int distance = playerState.Move;
        {
            // 캐릭터 이동, astar 알고리즘으로 최단 경로 찾기
            int[,] dist = new int[_tileManager.GetHeight(), _tileManager.GetWidth()];
            PriorityQueue<PathNode> pq = new PriorityQueue<PathNode>(100, (a, b) => a.Cost() < b.Cost());
            pq.Enqueue(new PathNode(0, Mathf.Abs(posX - target.x) + Mathf.Abs(posY - target.y), posX, posY, null));

            List<Vector2Int> paths = new List<Vector2Int>();
            List<PathNode> nodes = new List<PathNode>();
            while (pq.Count > 0)
            {
                var node = pq.Dequeue();
                nodes.Add(node);
                int x = node.X;
                int y = node.Y;
                int g = node.G;
                int h = node.H;

                // 도착
                if ((auto && h == 1) || (!auto && h == 0))
                {
                    while (node != null)
                    {
                        paths.Add(new Vector2Int(node.X, node.Y));
                        node = node.Parent;
                    }
                    break;
                }
                // 상하좌우만 체크
                Neighbor(x - 1, y, target, node, dist, pq);
                Neighbor(x + 1, y, target, node, dist, pq);
                Neighbor(x, y - 1, target, node, dist, pq);
                Neighbor(x, y + 1, target, node, dist, pq);
            }
            
            paths.Reverse();
            if (paths.Count > distance + 1)
                paths.RemoveRange(distance + 1, paths.Count - distance - 1);

            if (paths.Count > 0)
            {
                int newPosX = paths[paths.Count - 1].x;
                int newPosY = paths[paths.Count - 1].y;
                _movePath.StartMove(paths);
                _tileManager.RootBlockStates[posY][posX].Type = BlockType.Empty;
                _tileManager.RootBlockStates[newPosY][newPosX].Type = BlockType.Player;
                playerState.Position = new Vector2Int(newPosX, newPosY);
            }
        }
    }

    public void AutoSkillTo(PlayerState playerState)
    {
        Debug.Log("ai 구현 필요!!!!");
    }

    public void Skill(PlayerState playerState, int skillCode)
    {
        var gm = GameInstance.GetInstance().GameManager;
        var tm = gm.TileManager;
        var gdm = gm.GameDataManager;
        
        var skillLevel = gdm.GetSkill(skillCode).levelRange[playerState.SkillLevel];
        int relativeX = playerState.Position.x - skillLevel.center[0];
        int relativeY = playerState.Position.y - skillLevel.center[1];
        
        List<Vector2Int> tilePositions = new List<Vector2Int>();
        tm.SkillPointerList(relativeX, relativeY, skillLevel.range, tilePositions);
        
        List<Vector3> positions = new List<Vector3>();
        foreach (var position in tilePositions) 
        {
            positions.Add(tm.GetTileWordPosition(position));
        }
        
        _skillExecute.StartSkill(skillCode, positions);
    }
}