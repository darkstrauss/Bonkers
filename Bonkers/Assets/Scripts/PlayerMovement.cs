using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public List<GameObject> floors;
    public List<GameObject> doors;
    public List<GameObject> floorTiles;
    private Camera mainCamera;
    public GameObject player, activeFloor, pointer, previousHit, floorTile, CBS;
    private Grid activeGrid;
    public Vector3 target, destinationPosition;
    private Transform playerTransform;
    private float previousCast, pastDistance, previousMove;
    public float remainingDistance, moveSpeed, destinationDistance;
    public GameObject instantiatedPointer;
    public GameObject debugParticle;
    public Material seeThroughMat;
    private Material tempMat;
    private bool raycastObscure, checkForObject;
    public bool traveling, inCombat;

    void Start()
    {
        mainCamera = Camera.main;
        playerTransform = player.transform;
        destinationPosition = playerTransform.position;
        previousCast = 0;
        raycastObscure = false;
        traveling = false;
        inCombat = false;
        previousHit = gameObject;
        previousMove = 0;
        checkForObject = false;
    }

	void Update ()
    {
        Travel();

        FollowPlayer();
    }

    void LateUpdate()
    {
        Vector3 cameraPosition = new Vector3(player.transform.position.x + 4.3f, 9.0f, player.transform.position.z - 7.3f);
        mainCamera.transform.position = cameraPosition;
    }

    private void Travel()
    {
        destinationDistance = Vector3.Distance(destinationPosition, playerTransform.position);

        if (destinationDistance < .01f)
        {
            remainingDistance = 0;
            ResetPosition(player.transform.position);
            if (activeFloor != null && activeFloor.GetComponent<Grid>().path != null && activeFloor.GetComponent<Grid>().path.Count > 0)
            {
                activeFloor.GetComponent<Grid>().ClearPath();
            }
            
            Destroy(instantiatedPointer);
            if (checkForObject)
            {
                player.GetComponent<ObjectCollision>().CollisionDetection();
                checkForObject = false;
            }
        }
        else if (destinationDistance > .01f)
        {
            // To Reset Speed to default
            if (!checkForObject)
            {
                checkForObject = true;
            }

            if (remainingDistance == 0)
            {
                remainingDistance = destinationDistance;
            }
        }

        previousMove += Time.deltaTime;

        // Moves the Player if the Left Mouse Button was clicked
        if (Input.GetMouseButtonUp(0) && previousMove > 0.3f && !inCombat)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "floor")
                {
                    player.GetComponent<ObjectCollision>().CollisionDetection();
                    Vector3 targetPoint = hit.point;
                    destinationPosition = new Vector3(Mathf.Floor(targetPoint.x) + 0.5f, 0, Mathf.Floor(targetPoint.z) + 0.5f);
                    if (instantiatedPointer != null)
                    {
                        DestroyObject(instantiatedPointer);
                    }
                    instantiatedPointer = Instantiate(pointer, destinationPosition, Quaternion.identity) as GameObject;

                    if (traveling)
                    {
                        StopAllCoroutines();
                        traveling = false;
                    }

                    StartCoroutine(Move());
                }
            }

            previousMove = 0;
        }
    }

    private IEnumerator Move()
    {
        bool process = false;
        traveling = true;
        List<MapPosition> path = GetFloor().FindPath(destinationPosition);

        if (floorTiles != null && floorTiles.Count > 0)
        {
            for (int i = 0; i < floorTiles.Count; i++)
            {
                Destroy(floorTiles[i]);
            }
            floorTiles.Clear();
        }

        if (path != null && path.Count > 1)
        {
            for (int i = 0; i < path.Count; i++)
            {
                Vector3 position = new Vector3((float)path[i].xPos + 0.5f, 0.01f, (float)path[i].yPos + 0.5f);

                GameObject tile = Instantiate(floorTile, position, Quaternion.identity) as GameObject;
                floorTiles.Add(tile);
            }

            process = true;
        }

        yield return new WaitForSeconds(0.2f);

        while (process)
        {
            Vector3 movePosition = new Vector3(path[path.Count - 1].xPos + 0.5f, 0.0f, path[path.Count - 1].yPos + 0.5f);
            Quaternion targetRotation = Quaternion.LookRotation(movePosition - playerTransform.position);
            playerTransform.rotation = targetRotation;

            while (!playerTransform.position.Equals(movePosition))
            {
                playerTransform.position = Vector3.MoveTowards(playerTransform.position, movePosition, moveSpeed * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }

            Destroy(floorTiles[floorTiles.Count - 1]);
            floorTiles.RemoveAt(floorTiles.Count - 1);

            if (path.Count == 0)
            {
                process = false;
            }
            else
            {
                path.RemoveAt(path.Count - 1);
            }
        }

        traveling = false;
    }

    private void FollowPlayer()
    {
        previousCast += Time.deltaTime;

        if (destinationDistance < remainingDistance - 0.95f)
        {
            remainingDistance = destinationDistance;
            raycastObscure = true;
        }
        else
        {
            raycastObscure = false;
        }

        if (raycastObscure)
        {
            player.GetComponent<ObjectCollision>().CollisionDetection();
        }
    }

    public void ClearRoomList()
    {
        GetFloor().ClearPath();
        doors.RemoveRange(0, doors.Count);

        if (floorTiles != null && floorTiles.Count > 0)
        {
            for (int i = 0; i < floorTiles.Count; i++)
            {
                Destroy(floorTiles[i]);
            }
            floorTiles.Clear();
        }
    }

    public void ResetPosition(Vector3 position)
    {
        destinationPosition = new Vector3(Mathf.Floor(position.x) + 0.5f, 0.0f, Mathf.Floor(position.z) + 0.5f);
        player.transform.position = destinationPosition;
    }

    public Grid GetFloor()
    {
        return activeFloor.GetComponent<Grid>();
    }
}
