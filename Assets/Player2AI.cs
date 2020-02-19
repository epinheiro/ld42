using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2AI : MonoBehaviour {

    public Manager manager;
    private Dictionary<TileTypeEnum, float> tileToCost;

    public class PqItem : IComparable
    {
        public IntVector2 val;
        public float priority;
        public IntVector2 prev;

        public PqItem(IntVector2 val, float priority, IntVector2 prev)
        {
            this.val = val;
            this.priority = priority;
            this.prev = prev;
        }

        public int CompareTo(object obj)
        {            
            if (obj == null) return 1;

            PqItem otherItem = obj as PqItem;
            if (otherItem != null)
                return this.priority.CompareTo(otherItem.priority);
            else
                throw new ArgumentException("Object is not a PqItem");
        }
    }

    private IntVector2 dijkstra(IntVector2 start, IntVector2 end)
    {
        PriorityQueue<PqItem> pq = new PriorityQueue<PqItem>();
        Dictionary<IntVector2, IntVector2> prev = new Dictionary<IntVector2, IntVector2>();
        Dictionary<IntVector2, IntVector2> visited = new Dictionary<IntVector2, IntVector2>();

        pq.Enqueue(new PqItem(start, 0, null));

        while(pq.Count > 0)
        {
            PqItem v = pq.Dequeue();
            if (visited.ContainsKey(v.val)) continue;
            visited.Add(v.val, v.prev);
            Debug.Log("Visited " + v.val.x + " " + v.val.y);
            List<IntVector2> moves = manager.possibleMoves(v.val.x, v.val.y, true);
            foreach(IntVector2 move in moves)
            {
                HexagonTile tile = manager.getHexGrid()[move.y][move.x];
                float dist = v.priority + tileToCost[tile.terrainType];
                pq.Enqueue(new PqItem(move, dist, v.val));
            }
        }

        if (!visited.ContainsKey(end))
        {
            Debug.Log("no end :(");
            return null;
        }

        IntVector2 prevT = end;
        Debug.Log("prev x " + prevT.x + " y " + prevT.y);
        while(visited[prevT] != start)
        {
            prevT = visited[prevT];
            Debug.Log("prev x " + prevT.x + " y " + prevT.y);
        }

        if (prevT == end)
        {
            return null;
        }

        return prevT;
    }

    private void botAct()
    {
        if(manager.getActivePowerup() != null)
        {
            PowerupScript powerup = manager.getActivePowerup();
            if(powerup.powerupType == PowerupEnum.DESTROY || powerup.powerupType == PowerupEnum.TELEPORT)
            {
                List<IntVector2> moves = manager.possibleMoves(manager.getPlayers()[0].GridX, manager.getPlayers()[0].GridY);
                if (moves.Count > 0)
                {
                    manager.tilePressed(moves[0].x, moves[0].y, true);
                } else
                {
                    List<List<HexagonTile>> hexGrid = manager.getHexGrid();
                    foreach(List<HexagonTile> row in hexGrid)
                    {
                        foreach(HexagonTile t in row)
                        {
                            if(!t.isDestroyed())
                            {
                                manager.tilePressed(t.gridX, t.gridY, true);
                                return;
                            }
                        }
                    }
                }
            } else
            {
                List<List<HexagonTile>> hexGrid = manager.getHexGrid();
                foreach (List<HexagonTile> row in hexGrid)
                {
                    foreach (HexagonTile t in row)
                    {
                        if (t.isDestroyed())
                        {
                            manager.tilePressed(t.gridX, t.gridY, true);
                            return;
                        }
                    }
                }
            }

            return;
        }

        IntVector2 start = new IntVector2(manager.getPlayers()[1].GridX, manager.getPlayers()[1].GridY);
        IntVector2 end = new IntVector2(manager.getPlayers()[0].GridX, manager.getPlayers()[0].GridY);
        IntVector2 next = dijkstra(start, end);
        if(next == null || UnityEngine.Random.Range(0, 10) < 2)
        {
            Debug.Log("Random! " + (next == null));
            List<IntVector2> moves = manager.possibleMoves(manager.getPlayers()[1].GridX, manager.getPlayers()[1].GridY);
            if (moves.Count == 0) return;
            IntVector2 move = moves[UnityEngine.Random.Range(0, moves.Count)];
            manager.tilePressed(move.x, move.y, true);
        } else
        {
            manager.tilePressed(next.x, next.y, true);
        }
    }

	// Use this for initialization
	void Start () {
        tileToCost = new Dictionary<TileTypeEnum, float>()
        {
            {TileTypeEnum.DESERT, 1 }, {TileTypeEnum.FOREST, 1 }, {TileTypeEnum.PLAINS, 0 }, {TileTypeEnum.SWAMP, 1.7f }
        };

    }
	
	// Update is called once per frame
	void Update () {
        if (manager.getCurrentTurn() == 1 && Manager.aiActive) botAct();
    }
}
