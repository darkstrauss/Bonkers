﻿using UnityEngine;
using System.Collections;

public class MockEnemy : MonoBehaviour
{
    public static float STARTCOMBATDISTANCE = 1.5f;
    public GameObject player,CBS;
    public bool activeEnemy;
    public int Health;
    public int Dmg = 15;

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
            StartCoroutine(Attackcylce());
            attacked = true;
        }
        if (Health <= 0)
        {
            DEAD();
        }
    }

    IEnumerator Attackcylce()
    {
        CBS.SetActive(true);
        CBS.GetComponent<CombatSystem>().ActiveEnemy = gameObject;
        CBS.GetComponent<CombatSystem>().IncomingAttack();
        yield return new WaitForSeconds(Random.Range(10.0f, 20.0f));
        StartCoroutine(Attackcylce());
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
        CBS.SetActive(false);
        Destroy(gameObject);
    }
}
