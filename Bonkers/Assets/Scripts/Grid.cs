using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (MeshFilter), typeof (MeshRenderer), typeof (BoxCollider))]
public class Grid : MonoBehaviour
{

    private Material floorMaterial;
    //private Material waterMaterial;
    public int xSize, zSize;

    public Vector3[] floorVerts, waterVerts;

    private Mesh meshFloor, meshWallsBack, meshWallsFront;

    public List<GameObject> doors;
    public List<MapPosition> path;

    public GameObject floorObject;
    public AStarNode currentNode, goal;

    public GameObject player;

    public static int WALL = 0;
    public static int FLOOR = 1;
    public static int START = 2;
    public static int GOAL = 3;

    public int[,] map;

    public AStarNode[,] nodeMap;

    List<AStarNode> closed = new List<AStarNode>();
    List<AStarNode> open = new List<AStarNode>();

    private void Awake()
    {
        gameObject.isStatic = true;
        floorObject = this.gameObject;
        player = Camera.main.GetComponent<PlayerMovement>().player;
        Camera.main.GetComponent<PlayerMovement>().activeFloor = this.gameObject;
        Generate();
        GenerateAStarPath();
    }

    private void GenerateAStarPath()
    {
        map = new int[xSize, zSize];

        for (int x = 0, i = 0; x < xSize; x++)
        {
            for (int y = 0; y < zSize; y++, i++)
            {
                //Debug.Log("MAP X" + x + ", MAP Y" + y + ", MAP COUNT: " + i);
                map[x, y] = FLOOR;
            }
        }

        GenerateNodeMapFromIntMap();
    }

    private void GenerateNodeMapFromIntMap()
    {
        nodeMap = new AStarNode[xSize, zSize];

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < zSize; y++)
            {
                nodeMap[x, y] = new AStarNode(new MapPosition(x, y, map[x, y] > 0), 0f, 0f);
            }
        }
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

    public List<AStarNode> GetNeighbours(MapPosition current)
    {
        List<AStarNode> neighbours = new List<AStarNode>();

        if (current.yPos > 0 && map[current.xPos, current.yPos-1] == FLOOR && !closed.Contains(nodeMap[current.xPos, current.yPos - 1]))
        {
            neighbours.Add(nodeMap[current.xPos, current.yPos - 1]);
        }

        if (current.yPos < zSize - 1 && map[current.xPos, current.yPos + 1] == FLOOR && !closed.Contains(nodeMap[current.xPos, current.yPos + 1]))
        {
            neighbours.Add(nodeMap[current.xPos, current.yPos + 1]);
        }

        if (current.xPos > 0 && map[current.xPos - 1, current.yPos] == FLOOR && !closed.Contains(nodeMap[current.xPos - 1, current.yPos]))
        {
            neighbours.Add(nodeMap[current.xPos - 1, current.yPos]);
        }

        if (current.xPos < xSize - 1 && map[current.xPos + 1, current.yPos] == FLOOR && !closed.Contains(nodeMap[current.xPos + 1, current.yPos]))
        {
            neighbours.Add(nodeMap[current.xPos + 1, current.yPos]);
        }

        return neighbours;
    }

    private void Generate()
    {
        GameObject front = new GameObject("front");
        front.transform.parent = gameObject.transform;
        front.layer = 2;
        front.tag = "front";
        front.AddComponent<Wall>();
        GameObject doorFront = new GameObject("door");
        doorFront.layer = 2;
        doorFront.tag = "front";
        doorFront.transform.parent = front.transform;
        doorFront.AddComponent<Door>();
        doors.Add(doorFront);

        GameObject back = new GameObject("back");
        back.transform.parent = gameObject.transform;
        back.layer = 2;
        back.tag = "back";
        back.AddComponent<Wall>();
        GameObject doorBack = new GameObject("door");
        doorBack.layer = 2;
        doorBack.tag = "back";
        doorBack.transform.parent = back.transform;
        doorBack.AddComponent<Door>();
        doors.Add(doorBack);

        GameObject down = new GameObject("down");
        down.transform.parent = gameObject.transform;
        down.layer = 2;
        down.tag = "down";
        down.AddComponent<Wall>();
        GameObject doorDown = new GameObject("door");
        doorDown.layer = 2;
        doorDown.tag = "down";
        doorDown.transform.parent = down.transform;
        doorDown.AddComponent<Door>();
        doors.Add(doorDown);

        GameObject left = new GameObject("left");
        left.transform.parent = gameObject.transform;
        left.layer = 2;
        left.tag = "left";
        left.AddComponent<Wall>();
        GameObject doorLeft = new GameObject("door");
        doorLeft.layer = 2;
        doorLeft.tag = "left";
        doorLeft.transform.parent = left.transform;
        doorLeft.AddComponent<Door>();
        doors.Add(doorLeft);

        floorVerts = new Vector3[(xSize + 1) * (zSize + 1)];

        Vector2[] uvFloor = new Vector2[floorVerts.Length];

        floorMaterial = Resources.Load<Material>("Materials/floor");
        //waterMaterial = Resources.Load<Material>("Materials/water");

        for (int i = 0, y = 0; y < zSize + 1; y++)
        {
            for (int x = 0; x < xSize + 1; x++, i++)
            {
                floorVerts[i] = new Vector3(x, 0, y);
                uvFloor[i] = new Vector2((float)x / xSize, (float)y / zSize);
            }
        }

        GetComponent<MeshFilter>().mesh = meshFloor = new Mesh();
        meshFloor.name = "Generated Floor Mesh";

        meshFloor.vertices = floorVerts;
        meshFloor.uv = uvFloor;
        meshFloor.uv2 = uvFloor;

        int[] TrianglesFloor = new int[xSize * zSize * 6];

        for (int ti = 0, vi = 0, y = 0; y < zSize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                TrianglesFloor[ti] = vi;
                TrianglesFloor[ti + 3] = TrianglesFloor[ti + 2] = vi + 1;
                TrianglesFloor[ti + 4] = TrianglesFloor[ti + 1] = vi + xSize + 1;
                TrianglesFloor[ti + 5] = vi + xSize + 2;
                meshFloor.triangles = TrianglesFloor;
            }
        }

        meshFloor.triangles = TrianglesFloor;

        Vector3 colliderSize = new Vector3((float)xSize, 0.01f, (float)zSize);

        BoxCollider collider = GetComponent<BoxCollider>();
        collider.size = colliderSize;
        collider.center = new Vector3(xSize / 2, 0.0f, zSize / 2);

        meshFloor.RecalculateBounds();
        meshFloor.RecalculateNormals();

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = floorMaterial;
        meshRenderer.material.mainTextureScale = new Vector2(xSize, zSize);

        gameObject.tag = "floor";

        //GameObject water = new GameObject("water");
        //water.transform.parent = gameObject.transform;
        //water.tag = "water";
        //water.AddComponent<Water>();
    }

    public void ClearPath()
    {
        if (path != null)
        {
            path.Clear();
        }
        open.Clear();
        closed.Clear();
    }

    public List<MapPosition> FindPath(Vector3 destination)
    {
        if (path != null && path.Count > 0)
        {
            path.Clear();
        }
        closed.Clear();
        open.Clear();

        currentNode = nodeMap[Mathf.FloorToInt(player.transform.position.x), Mathf.FloorToInt(player.transform.position.z)];
        goal = nodeMap[Mathf.FloorToInt(destination.x), Mathf.FloorToInt(destination.z)];
        path = FindPath(currentNode.position, goal.position);

        return path;
    }

    public void OccupySpots(List<Vector3> vectorList)
    {
        for (int i = 0; i < vectorList.Count; i++)
        {
            map[(int)vectorList[i].x, (int)vectorList[i].z] = WALL;
        }
    }

    public void SetCurrentNode(Vector3 destination)
    {
        currentNode = nodeMap[Mathf.FloorToInt(destination.x), Mathf.FloorToInt(destination.z)];
    }

    private void OnDestroy()
    {
        nodeMap = null;
    }

    public List<GameObject> GetDoors()
    {
        return doors;
    }

    /*private void OnDrawGizmos()
    {
        if (floorVerts == null)
        {
            return;
        }

        Gizmos.color = Color.black;
        for (int i = 0; i < floorVerts.Length; i++)
        {
            Gizmos.DrawSphere(transform.TransformPoint(floorVerts[i]), 0.1f);
        }
    }*/
}
