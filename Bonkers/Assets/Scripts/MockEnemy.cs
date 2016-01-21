using UnityEngine;
using System.Collections;

public class MockEnemy : MonoBehaviour
{
    public static float STARTCOMBATDISTANCE = 1.5f;
    public GameObject player,CBS;
    public bool activeEnemy;
    public int Health;

    private bool attacked = false;



    // Use this for initialization
    void Start()
    {
        player = Camera.main.GetComponent<PlayerMovement>().player;
        CBS = Camera.main.GetComponent<PlayerMovement>().CBS;
    }

    // Update is called once per frame
    void Update()
    {

        // CHeck for distance between player and enemy

        // Check for attacking state

        if (((gameObject.transform.position - player.transform.position).magnitude) <= STARTCOMBATDISTANCE && (attacked == false))
        {
            Debug.Log("TO CLOSE");
            Attackcylce();
            attacked = true;
        }

    }

    private void Attackcylce()
    {
        // Call for an attack
        Debug.Log("Starting Attack");
        CBS.SetActive(true);
        CBS.GetComponent<CombatSystem>().IncomingAttack();
    }

    public void HealthDown(int amountDOWN)
    {
        Health -= amountDOWN;
    }

    // Funchtion to increase health
    public void HealthUp(int amountUP)
    {
        Health += amountUP;
    }

    private void DEAD()
    {
        Destroy(gameObject);
    }
}
