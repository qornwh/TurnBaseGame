using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AiController
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

    private TileManager _tileManagerRef;

    public void Init()
    {
        var gm = GameInstance.GetInstance().GameManager;
        var tileManager = gm.TileManager;
        _tileManagerRef = tileManager;
    }

    private void GetEnemyPos(PlayerState playerState, out int posX, out int posY)
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
        
        posX = enemy.PosX;
        posY = enemy.PosY;
    }

    private void Neighbor(int x, int y, int enemyPosX, int enemyPosY, PathNode node, int[,] dist,
        PriorityQueue<PathNode> pq)
    {
        // 범위 내에 들어가 있어야 한다.
        if (0 <= x && x < _tileManagerRef.GetWidth() && 0 <= y && y < _tileManagerRef.GetHeight())
        {
            var blockArray2D = _tileManagerRef.RootBlockStates;
            if (dist[y, x] == 0 && blockArray2D[y][x].Type == BlockType.Empty)
            {
                int nextG = node.G + 1;
                int nextH = Mathf.Abs(x - enemyPosX) + Mathf.Abs(y - enemyPosY);
                dist[y, x] = nextH;
                pq.Enqueue(new PathNode(nextG, nextH, x, y, node));
            }
        }
    }

    public BlockState<BlockBase> AutoMove(PlayerState playerState, int distance)
    {
        // 맵 들고오기
        var blockArray2D = _tileManagerRef.RootBlockStates;
        int posX = playerState.PosX;
        int posY = playerState.PosY;
        {
            // 적위치
            GetEnemyPos(playerState, out int enemyPosX, out int enemyPosY);

            // 캐릭터 이동, astar 알고리즘으로 최단 경로 찾기
            int[,] dist = new int[_tileManagerRef.GetHeight(), _tileManagerRef.GetWidth()];
            PriorityQueue<PathNode> pq = new PriorityQueue<PathNode>(100, (a, b) => a.Cost() < b.Cost());
            pq.Enqueue(new PathNode(0, Mathf.Abs(posX - enemyPosX) + Mathf.Abs(posY - enemyPosY), posX, posY, null));

            List<Tuple<int, int>> paths = new List<Tuple<int, int>>();
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
                if (h == 1)
                {
                    while (node != null)
                    {
                        paths.Add(new Tuple<int, int>(node.X, node.Y));
                        node = node.Parent;
                    }
                    break;
                }
                // 상하좌우만 체크
                Neighbor(x - 1, y, enemyPosX, enemyPosY, node, dist, pq);
                Neighbor(x + 1, y, enemyPosX, enemyPosY, node, dist, pq);
                Neighbor(x, y - 1, enemyPosX, enemyPosY, node, dist, pq);
                Neighbor(x, y + 1, enemyPosX, enemyPosY, node, dist, pq);
            }

            paths.Reverse();
            if (paths.Count <= distance)
                return blockArray2D[paths.Last().Item2][paths.Last().Item1];
            return blockArray2D[paths[distance].Item2][paths[distance].Item1];
        }
    }

    public void AutoSkill(PlayerState playerState)
    {
    }
}