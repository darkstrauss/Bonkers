using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SphereCode : MonoBehaviour {

    public GameObject Parent;
    public int SphereID;
    private SphereParentCode spCode;
    private bool EnterBool = false;

	// Use this for initialization
	void Start ()
    {
        spCode = GetComponentInParent<SphereParentCode>();
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void OnEnter()
    {
        if (Input.GetMouseButton(0))
        {
            //Debug.Log("Enter " + SphereID);
            spCode.AddSphereID(SphereID);
            EnterBool = true;
            GetComponent<Button>().image.color = Color.yellow;       
        }
    }

    public void OnExit()
    {
        if (EnterBool == true)
        {
            GetComponent<Button>().image.color = Color.red;
            EnterBool = false;
        }        
    }
}
