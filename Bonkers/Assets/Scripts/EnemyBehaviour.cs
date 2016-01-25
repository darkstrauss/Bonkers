using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehaviour : MonoBehaviour
{
    public static float GOTOPLAYERDISTANCE = 3.5f;
    public static float STARTCOMBARDISTANCE = 1.5f;
    private static int WALL = 0;
    private static int FLOOR = 1;
    private static int START = 2;
    private static int GOAL = 3;

    private GameObject CBS;
    private float moveSpeed = 3.0f;
    private GameObject player;
    private PlayerMovement playerMovement;
    private Grid grid;
    private MapPosition playerPosition, enemyPosition;
    public AStarNode currentNode, goal;
    List<AStarNode> closed = new List<AStarNode>();
    List<AStarNode> open = new List<AStarNode>();
    List<MapPosition> path;
    public AStarNode[,] nodeMap;
    private int xSize, zSize;
    public int[,] map;
    public bool selectedPlayer = false;
    private bool isIdle, pathComplete, isWaiting;
    bool process;

    public int pathCount;

    public void Start()
    {
        playerMovement = Camera.main.GetComponent<PlayerMovement>();
        CBS = Camera.main.GetComponent<PlayerMovement>().CBS;
        player = playerMovement.player;
        grid = playerMovement.GetFloor();
        xSize = grid.xSize;
        zSize = grid.zSize;
        nodeMap = grid.nodeMap;
        map = grid.map;

        StateMachine("idle");
    }

    private IEnumerator Move()
    {
        Debug.Log("start");
        process = true;

        while (path != null && path.Count > 0 && process)
        {
            Vector3 movePosition = new Vector3((float)path[path.Count - 1].xPos + 0.5f, 0.0f, (float)path[path.Count - 1].yPos + 0.5f);
            Quaternion targetRotation = Quaternion.LookRotation(movePosition - gameObject.transform.position);
            gameObject.transform.rotation = targetRotation;

            while (!gameObject.transform.position.Equals(movePosition))
            {
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, movePosition, moveSpeed * Time.deltaTime);
                Debug.Log("OI");
                yield return new WaitForSeconds(0.01f);
            }

            if (path.Count > 0)
            {
                path.RemoveAt(path.Count - 1);
                Debug.Log(path.Count);
            }

            if (path.Count <= 1)
            {
                ResetPath();
                Debug.Log("done");
                
            }
        }

        process = false;
    }

    private List<MapPosition> FindPath(MapPosition start, MapPosition goal)
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < zSize; y++)
            {
                nodeMap[x, y].parent = null;
            }
        }

        AStarNode startNode = nodeMap[start.xPos, start.yPos];
        startNode.g = 0;
        startNode.f = startNode.g + MapPosition.EucludianDistance(start, goal);
        open.Add(startNode);

        while (open.Count > 0)
        {
            AStarNode currentNode = open[0];

            if (currentNode.position == goal)
            {
                path = new List<MapPosition>();

                int count = 0;

                while (currentNode.parent != null)
                {
                    count++;
                    path.Add(currentNode.position);
                    currentNode = nodeMap[currentNode.parent.xPos, currentNode.parent.yPos];
                    if (count > xSize * zSize * 6)
                    {
                        Debug.LogError("out of range");
                        break;
                    }
                }
                path.Add(start);
                return path;
            }

            open.RemoveAt(0);
            closed.Add(currentNode);

            List<AStarNode> neighbours = GetNeighbours(currentNode.position);

            foreach (AStarNode neighbour in neighbours)
            {
                if (closed.Contains(neighbour))
                {
                    continue;
                }

                float g = currentNode.g + neighbour.f;

                bool inOpenList = open.Contains(neighbour);
                if (!inOpenList || g < neighbour.g)
                {
                    neighbour.parent = currentNode.position;
                    neighbour.g = g;
                    neighbour.f = g + MapPosition.EucludianDistance(neighbour.position, goal);

                    if (!inOpenList)
                    {
                        int index = 0;
                        while (index < open.Count && open[index].f < neighbour.f)
                        {
                            index++;
                        }
                        open.Insert(index, neighbour);
                    }
                }
            }
        }

        return null;
    }

    public void StateMachine(string state)
    {
        isIdle = false;
        selectedPlayer = false;
        isWaiting = false;
        switch (state)
        {
            case "idle":
                path = GetPath();
                StartCoroutine(Move());
                isIdle = true;
                Debug.Log("idle");
                break;
            case "chase":
                path = GetPathToPlayer();
                StartCoroutine(Move());
                selectedPlayer = true;
                Debug.Log("chasing");
                break;
            case "wait":
                ResetPath();
                ResetPosition(transform.position);
                isWaiting = true;
                Debug.Log("waiting");
                break;
        }
    }

    private List<AStarNode> GetNeighbours(MapPosition current)
    {
        List<AStarNode> neighbours = new List<AStarNode>();

        if (current.yPos > 0 && map[current.xPos, current.yPos - 1] == FLOOR && !closed.Contains(nodeMap[current.xPos, current.yPos - 1]))
            neighbours.Add(nodeMap[current.xPos, current.yPos - 1]);

        if (current.yPos < zSize - 1 && map[current.xPos, current.yPos + 1] == FLOOR && !closed.Contains(nodeMap[current.xPos, current.yPos + 1]))
            neighbours.Add(nodeMap[current.xPos, current.yPos + 1]);

        if (current.xPos > 0 && map[current.xPos - 1, current.yPos] == FLOOR && !closed.Contains(nodeMap[current.xPos - 1, current.yPos]))
            neighbours.Add(nodeMap[current.xPos - 1, current.yPos]);

        if (current.xPos < xSize - 1 && map[current.xPos + 1, current.yPos] == FLOOR && !closed.Contains(nodeMap[current.xPos + 1, current.yPos]))
            neighbours.Add(nodeMap[current.xPos + 1, current.yPos]);

        return neighbours;
    }

    private List<MapPosition> GetPath()
    {
        ResetPath();

        

        currentNode = nodeMap[Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z)];
        List<AStarNode> neighbours = GetNeighbours(currentNode.position);
        goal = nodeMap[neighbours[Random.Range(0, neighbours.Count)].position.xPos, neighbours[Random.Range(0, neighbours.Count)].position.yPos];

        path = FindPath(currentNode.position, goal.position);

        if (path != null && path.Count > 1)
        {
            path.RemoveAt(path.Count - 1);
        }

        return path;
    }

    private List<MapPosition> GetPathToPlayer()
    {
        ResetPath();

        currentNode = nodeMap[Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z)];
        goal = nodeMap[Mathf.FloorToInt(player.transform.position.x), Mathf.FloorToInt(player.transform.position.z)];

        return FindPath(currentNode.position, goal.position);
    }

    private void Update()
    {
        if (!process && (gameObject.transform.position - player.transform.position).magnitude <= GOTOPLAYERDISTANCE && !selectedPlayer && !CBS.activeInHierarchy)
        {
            StateMachine("chase");
        }

        if (!isWaiting && (gameObject.transform.position - player.transform.position).magnitude <= STARTCOMBARDISTANCE || CBS.activeInHierarchy)
        {
            StateMachine("wait");
        }

        if (!process && (gameObject.transform.position - player.transform.position).magnitude >= GOTOPLAYERDISTANCE && !CBS.activeInHierarchy)
        {
            StateMachine("idle");
        }
    }

    private void ResetPosition(Vector3 position)
    {
        transform.position = new Vector3(Mathf.Floor(position.x) + 0.5f, 0.0f, Mathf.Floor(position.z) + 0.5f);
    }

    private void ResetPath()
    {
        if (path != null && path.Count > 0)
        {
            path.Clear();
        }

        open.Clear();
        closed.Clear();
    }
}
