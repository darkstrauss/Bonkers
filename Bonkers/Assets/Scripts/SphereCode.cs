using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SphereCode : MonoBehaviour {

    public GameObject Parent;
    public int SphereID;
    private SphereParentCode spCode;
    private GUILayer UILayer;
    private bool EnterBool = false;
    private bool Touchthis = false;

    // Use this for initialization
    void Start ()
    {
        UILayer = GetComponentInParent<GUILayer>();
        spCode = GetComponentInParent<SphereParentCode>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!EnterBool)
        {
            foreach (Touch touch in Input.touches)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject == gameObject.transform)
                    {
                        Touchthis = true;
                    }
                }
            }
        }
	}

    public void OnEnter()
    {
        Text text = GameObject.FindGameObjectWithTag("Text").GetComponent<Text>();
        // text.text = "Calling OnEnter";
        if ((Camera.main.GetComponent<PlayerMovement>().CBS.GetComponent<CombatSystem>().Stamina >= 50f) && (Input.GetMouseButton(0) || (Touchthis)))
        {
            // text.text = "Is Done";
            spCode.AddSphereID(SphereID);
            EnterBool = true;
            GetComponent<Button>().image.color = Color.yellow;
        } 
        else if ((Camera.main.GetComponent<PlayerMovement>().CBS.GetComponent<CombatSystem>().Stamina <= 50f) && (Input.GetMouseButton(0) || (Touchthis)))
        {
            GetComponent<Button>().image.color = new Color(1f, 0.5f, 0.5f);
            EnterBool = true;
        }      
    }

    public void OnExit()
    {
        if (EnterBool == true)
        {
            GetComponent<Button>().image.color = Color.red;
            EnterBool = false;
            Touchthis = false;
        }        
    }
}
