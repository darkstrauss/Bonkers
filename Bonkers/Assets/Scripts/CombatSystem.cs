using UnityEngine;
using System.Collections;

public class CombatSystem : MonoBehaviour {

    public float dTime;
    public int Health;
    public GameObject DodgeCircleL, DodgeCircleR;
    public GameObject ActiveEnemy;
    private PlayerMovement playerMovement;

    private IEnumerator coroutine1, coroutine2;

    private void Awake()
    {
        playerMovement = Camera.main.GetComponent<PlayerMovement>();
        ResetCo();
    }
    // Use this for initialization
    void Start () {
        Health = 100;
    }

    void OnEnable()
    {
        // Pause game
        // Update avaiblibe strikes
    }

    // Update is called once per frame
    void Update () {

        
        // check health
        // if health reach zero gameover
        // check stamina

	}

    public void StartComabt()
    {
        // pause game
    }
    private void ResetCo()
    {
        coroutine1 = DodgeVisualIE(dTime, DodgeCircleL);
        coroutine2 = DodgeVisualIE(dTime, DodgeCircleR);
    }


    public void IncomingAttack()
    {
        DodgeCircleL.transform.localScale = new Vector3(4f, 4f, 4f);
        DodgeCircleR.transform.localScale = new Vector3(4f, 4f, 4f);

        DodgeCircleL.SetActive(true);
        DodgeCircleR.SetActive(true);

        StartCoroutine(coroutine1);
        StartCoroutine(coroutine2);
    }

    // Used to cancel coroutines and dodge incoming attack
    public void DodgeVisual()
    {
        StopCoroutine(coroutine1);
        StopCoroutine(coroutine2);

        DodgeCircleL.SetActive(false);
        DodgeCircleR.SetActive(false);

        ResetCo();
    }

    // Function to interact with health
    public void HealthDown(int amountDOWN)
    {
        Health -= amountDOWN;
    }

    // Funchtion to increase health
    public void HealthUp (int amountUP)
    {
        Health += amountUP;
    }

    // Dislpay game over screen + score etc
    private void GameOver()
    {
        // Display things 
    } 

    IEnumerator DodgeVisualIE(float time, GameObject rORl)
    {
        Vector3 originalScale = rORl.transform.localScale;
        Vector3 destinationScale = new Vector3(1.5f, 1.5f, 1.5f);

        float currentTime = 0.0f;

        while(currentTime <= time)
        {
            rORl.transform.localScale = Vector3.Lerp(originalScale, destinationScale ,  Mathf.SmoothStep(0, 1 ,(currentTime / time)));
            currentTime += Time.deltaTime;
            yield return null;
        }
        rORl.SetActive(false);
        HealthDown(10);   
    }
}
