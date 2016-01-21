using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectCollision : MonoBehaviour
{
    private static float HITDISTANCE = 0.6f;
    public Material seeThroughMat;
    public GameObject previousHit;
    public bool isTouchingDown, isTouchingRight;
    public List<GameObject> obscureListDown, obscureListRight;
    public List<Material> obscureMatListDown, obscureMatListRight;

    void Start()
    {
        previousHit = gameObject;
        isTouchingDown = false;
        isTouchingRight = false;
    }

    public void CollisionDetection()
    {
        RaycastHit hit;
        Ray rayBack = new Ray(transform.position, Vector3.back);
        Ray rayRight = new Ray(transform.position, Vector3.right);

        if (Physics.Raycast(rayBack, out hit))
        {
            if (hit.distance < HITDISTANCE && hit.collider.name != "ThirdPersonController" && hit.collider.tag != "floor")
            {
                isTouchingDown = true;
                RayCastFromHitObjectDown(hit.collider.gameObject);
                AddToList(hit.collider.gameObject, "Down");
            }
            else
            {
                isTouchingDown = false;
                ClearList();
            }

            Debug.DrawRay(rayBack.origin, rayBack.direction);
        }
        else
        {
            isTouchingDown = false;
            ClearList();
        }

        if (Physics.Raycast(rayRight, out hit))
        {
            if (hit.distance < HITDISTANCE && hit.collider.name != "ThirdPersonController" && hit.collider.tag != "floor")
            {
                isTouchingRight = true;
                RayCastFromHitObjectRight(hit.collider.gameObject);
                AddToList(hit.collider.gameObject, "Right");
            }
            else
            {
                isTouchingRight = false;
                ClearList();
            }

            Debug.DrawRay(rayRight.origin, rayRight.direction);
        }
        else
        {
            isTouchingRight = false;
            ClearList();
        }
    }

    private void AddToList(GameObject addThis, string direction)
    {
        if (direction == "Down")
        {
            if (!checkContents(addThis))
            {
                obscureListDown.Add(addThis);
                obscureMatListDown.Add(addThis.GetComponent<MeshRenderer>().material);
                addThis.GetComponent<MeshRenderer>().material = seeThroughMat;
            }
        }

        if (direction == "Right")
        {
            if (!checkContents(addThis))
            {
                obscureListRight.Add(addThis);
                obscureMatListRight.Add(addThis.GetComponent<MeshRenderer>().material);
                addThis.GetComponent<MeshRenderer>().material = seeThroughMat;
            }
        }
    }

    private bool checkContents(GameObject checkThis)
    {
        return (obscureListDown.Contains(checkThis) || obscureListRight.Contains(checkThis));
    }

    private void RayCastFromHitObjectDown(GameObject castFromThis)
    {
        Vector3 castPoint = new Vector3(castFromThis.transform.position.x, 0.0f, castFromThis.transform.position.z);
        RaycastHit hit;
        Ray rayLeft = new Ray(castPoint, Vector3.left);
        Ray rayRight = new Ray(castPoint, Vector3.right);

        if (Physics.Raycast(rayLeft, out hit))
        {
            if (hit.distance < HITDISTANCE)
            {
                AddToList(hit.collider.gameObject, "Down");
            } 
        }

        if (Physics.Raycast(rayRight, out hit))
        {
            if (hit.distance < HITDISTANCE)
            {
                AddToList(hit.collider.gameObject, "Down");
            }
        }
    }

    private void RayCastFromHitObjectRight(GameObject castFromThis)
    {
        Vector3 castPoint = new Vector3(castFromThis.transform.position.x, 0.0f, castFromThis.transform.position.z);
        RaycastHit hit;
        Ray rayUp = new Ray(castPoint, Vector3.forward);
        Ray rayDown = new Ray(castPoint, Vector3.back);

        if (Physics.Raycast(rayUp, out hit))
        {
            if (hit.distance < HITDISTANCE)
            {
                AddToList(hit.collider.gameObject, "Right");
            }
        }

        if (Physics.Raycast(rayDown, out hit))
        {
            if (hit.distance < HITDISTANCE)
            {
                AddToList(hit.collider.gameObject, "Right");
            }
        }
    }

    public void ClearList()
    {
        if (obscureListDown.Count > 0 && !isTouchingDown)
        {
            for (int i = 0; i < obscureListDown.Count; i++)
            {
                obscureListDown[i].GetComponent<Renderer>().material = obscureMatListDown[i];
            }
            obscureListDown.Clear();
            obscureMatListDown.Clear();
        }
        else if (obscureListRight.Count > 0 && !isTouchingRight)
        {
            for (int i = 0; i < obscureListRight.Count; i++)
            {
                obscureListRight[i].GetComponent<Renderer>().material = obscureMatListRight[i];
            }
            obscureListRight.Clear();
            obscureMatListRight.Clear();
        }
    }

    public void ForceClearList()
    {
        isTouchingRight = false;
        isTouchingDown = false;
        previousHit = gameObject;
        obscureListDown.Clear();
        obscureMatListDown.Clear();
        obscureListRight.Clear();
        obscureMatListRight.Clear();
    }
}
