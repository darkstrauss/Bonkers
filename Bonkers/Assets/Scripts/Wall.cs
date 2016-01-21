using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider))]
public class Wall : MonoBehaviour {
    public Grid floorGrid;

    public int xSize, ySize, zSize, gridX;

    public Vector3[] wallVerts;

    private List<GameObject> trees;

    private Mesh mesh;

    public GameObject floorObject;

    private Material wallMat;

    private Vector2 materialScale;

    private string direction;

    void Awake()
    {
        gameObject.isStatic = true;

        gameObject.transform.localPosition = Vector3.zero;

        xSize = GetComponentInParent<Grid>().xSize;
        zSize = GetComponentInParent<Grid>().zSize;
        gridX = xSize;
        floorObject = transform.parent.gameObject;
        floorGrid = floorObject.GetComponent<Grid>();
        direction = gameObject.name;

        wallMat = Resources.Load<Material>("Materials/wall");

        trees = new List<GameObject>();

        if (direction == "front" || direction == "down")
        {
            ySize = 1;
        }
        else
        {
            ySize = 5;
        }
        NewWall();
    }

    public void NewWall()
    {
        wallVerts = new Vector3[4];
        Vector2[] uv = new Vector2[wallVerts.Length];

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Generated Wall Mesh";

        for (int i = 0, y = 0; y < 2; y++)
        {
            for (int x = 0, z = 0; x < wallVerts.Length / 2; x++, z++, i++)
            {
                if (direction == "left")
                {
                    wallVerts[i] = new Vector3(0, y * ySize, z * zSize);
                    Vector3 colliderSize = new Vector3(0.01f, ySize, zSize);
                    SetBoxCollider(colliderSize);
                }
                else if (direction == "down")
                {
                    wallVerts[i] = new Vector3(x * xSize, y * ySize);
                    Vector3 colliderSize = new Vector3(xSize, ySize, 0.01f);
                    SetBoxCollider(colliderSize);
                }
                else if (direction == "front")
                {
                    wallVerts[i] = new Vector3(gridX, y * ySize, z * zSize);
                    Vector3 colliderSize = new Vector3(0.01f, ySize, zSize);
                    SetBoxCollider(colliderSize);
                }
                else if (direction == "back")
                {
                    wallVerts[i] = new Vector3(x * xSize, y * ySize, zSize);
                    Vector3 colliderSize = new Vector3(xSize, ySize, 0.01f);
                    SetBoxCollider(colliderSize);
                }
                else
                {
                    Debug.LogError("NO DIRECTION SET/INPROPPER");
                }

                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
            }
        }

        mesh.vertices = wallVerts;
        mesh.uv = uv;
        mesh.uv2 = uv;


        int[] TrianglesWall = new int[6];

        TrianglesWall[0] = 0;
        TrianglesWall[1] = 2;
        TrianglesWall[2] = 1;
        TrianglesWall[3] = TrianglesWall[1];
        TrianglesWall[4] = 3;
        TrianglesWall[5] = TrianglesWall[2];

        mesh.triangles = TrianglesWall;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = wallMat;

        if (direction == "down" || direction == "back")
        {
            materialScale = new Vector2(xSize, ySize);
        }
        else
        {
            materialScale = new Vector2(zSize, ySize);
        }

        meshRenderer.material.mainTextureScale = materialScale;

        mesh.Optimize();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        ///*
        /// this is used to make the generated walls invisible
        ///*
        meshRenderer.enabled = false;

        PlaceWallStructures(direction);
        PlaceTrees(direction);
    }

    private void SetBoxCollider(Vector3 colliderSize)
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.size = colliderSize;
        if (direction == "front")
        {
            collider.center = new Vector3((float)xSize, (float)ySize / 2, (float)zSize / 2);
        }
        else if (direction == "back")
        {
            collider.center = new Vector3((float)xSize / 2, (float)ySize / 2, (float)zSize);
        }
        else if (direction == "down")
        {
            collider.center = new Vector3((float)xSize / 2, (float)ySize / 2, 0.0f);
        }
        else if (direction == "left")
        {
            collider.center = new Vector3(0.0f, (float)ySize / 2, (float)zSize / 2);
        }
    }

    private void PlaceWallStructures(string direction)
    {
        GameObject wall = Instantiate(Resources.Load("Prefabs/Chocolate Wall"), Vector3.zero, Quaternion.identity) as GameObject;
        Vector3 placementLocation = Vector3.zero;
        wall.transform.parent = transform;

        if (direction == "front")
        {
            //dont rotate, offsetZ + 1 unit, offsetX = gridXsize
            int choice = Mathf.RoundToInt(Random.Range(0, 3));
            if (choice == 0)
            {
                placementLocation = new Vector3(xSize, 0, Random.Range(1, zSize / 2));
            }
            else if (choice == 1)
            {
                placementLocation = new Vector3(xSize, 0, Random.Range(zSize / 2 + 2, zSize));
            }
            else if (choice == 2)
            {
                Destroy(wall);
            }

            if (wall != null)
            {
                wall.transform.position = placementLocation;
            }
        }
        else if (direction == "back")
        {
            //rotate 90 degrees, offsetX + 1 unit, offsetZ = gridZsize
            int choice = Mathf.RoundToInt(Random.Range(0, 3));
            if (choice == 0)
            {
                placementLocation = new Vector3(Random.Range(1, xSize / 2), 0, zSize);
            }
            else if (choice == 1)
            {
                placementLocation = new Vector3(Random.Range(xSize / 2 + 2, xSize), 0, zSize);
            }
            else if (choice == 2)
            {
                Destroy(wall);
            }

            if (wall != null)
            {
                wall.transform.position = placementLocation;
                wall.transform.Rotate(Vector3.up, 90.0f);
            }
        }
        else if (direction == "down")
        {
            //rotate 90 degrees, offsetX + 1 unit
            int choice = Mathf.RoundToInt(Random.Range(0, 3));
            if (choice == 0)
            {
                placementLocation = new Vector3(Random.Range(1, xSize / 2), 0, 0);
            }
            else if (choice == 1)
            {
                placementLocation = new Vector3(Random.Range(xSize / 2 + 2, xSize), 0, 0);
            }
            else if (choice == 2)
            {
                Destroy(wall);
            }

            if (wall != null)
            {
                wall.transform.position = placementLocation;
                wall.transform.Rotate(Vector3.up, 90.0f);
            }
        }
        else if (direction == "left")
        {
            //dont rotate, offsetZ + 1 unit
            int choice = Mathf.RoundToInt(Random.Range(0, 3));
            if (choice == 0)
            {
                placementLocation = new Vector3(0, 0, Random.Range(1, zSize / 2));
            }
            else if (choice == 1)
            {
                placementLocation = new Vector3(0, 0, Random.Range(zSize / 2 + 2, zSize));
            }
            else if (choice == 2)
            {
                Destroy(wall);
            }

            if (wall != null)
            {
                wall.transform.position = placementLocation;
            }
        }
        else
        {
            Debug.LogError("NO DIRECTION SET/INPROPPER");
        }
    }

    private void PlaceTrees(string direction)
    {
        if (direction == "front")
        {
            for (int i = 0; i < zSize; i++)
            {
                GameObject tree = Instantiate(Resources.Load("Prefabs/Tree"), Vector3.zero, Quaternion.identity) as GameObject;
                trees.Add(tree);
                tree.transform.parent = transform;
                Vector3 randomRotation = new Vector3(0, Random.Range(-360, 360), 0);
                tree.transform.Translate(new Vector3(xSize + 0.5f + (i % 2), 0, i * 1 + 0.5f));
                tree.transform.Rotate(randomRotation);
            }
        }
        else if (direction == "back")
        {
            for (int i = 0; i < xSize; i++)
            {
                GameObject tree = Instantiate(Resources.Load("Prefabs/Tree"), Vector3.zero, Quaternion.identity) as GameObject;
                trees.Add(tree);
                tree.transform.parent = transform;
                Vector3 randomRotation = new Vector3(0, Random.Range(-360, 360), 0);
                tree.transform.Translate(new Vector3(0.5f + i * 1, 0, zSize + 0.5f + (i % 2)));
                tree.transform.Rotate(randomRotation);
            }
        }
        else if (direction == "down")
        {
            for (int i = 0; i < xSize; i++)
            {
                GameObject tree = Instantiate(Resources.Load("Prefabs/Tree"), Vector3.zero, Quaternion.identity) as GameObject;
                trees.Add(tree);
                tree.transform.parent = transform;
                Vector3 randomRotation = new Vector3(0, Random.Range(-360, 360), 0);
                tree.transform.Translate(new Vector3(0.5f + i * 1, 0, -0.5f - (i % 2)));
                tree.transform.Rotate(randomRotation);
            }
        }
        else if (direction == "left")
        {
            for (int i = 0; i < zSize; i++)
            {
                GameObject tree = Instantiate(Resources.Load("Prefabs/Tree"), Vector3.zero, Quaternion.identity) as GameObject;
                trees.Add(tree);
                tree.transform.parent = transform;
                Vector3 randomRotation = new Vector3(0, Random.Range(-360, 360), 0);
                tree.transform.transform.Translate(new Vector3(-0.5f - (i % 2), 0, i * 1 + 0.5f));
                tree.transform.Rotate(randomRotation);
            }
        }
        else
        {
            Debug.LogError("NO DIRECTION SET/INPROPPER");
        }

        trees[trees.Count / 2].GetComponentInChildren<MeshRenderer>().material = Resources.Load<Material>("Materials/SeeThrough");
        trees[trees.Count / 2 + 1].GetComponentInChildren<MeshRenderer>().material = Resources.Load<Material>("Materials/SeeThrough");
        trees[trees.Count / 2 - 1].GetComponentInChildren<MeshRenderer>().material = Resources.Load<Material>("Materials/SeeThrough");
    }

    private void OnDrawGizmos()
    {
        if (wallVerts == null)
        {
            return;
        }

        Gizmos.color = Color.black;
        //for (int i = 0; i < wallVerts.Length; i++)
        {
            //Gizmos.DrawSphere(wallVerts[i], 0.1f);
        }
    }
}
