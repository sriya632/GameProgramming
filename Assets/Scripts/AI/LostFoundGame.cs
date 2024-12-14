using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LostFoundGame : MonoBehaviour
{
    public bool hasPlayedIntroDialogue;

    public GameObject spawnedItem;
    public float spawnRadius = 18f;
    public GameObject forkSpawner;
    public GameObject monkey;
    public GameObject stealDialogueUI;
    public GameObject stoleForkUI;
    private bool stolenFrom;
    public bool gameIsOver;

    Inventory inventory;
    FollowPlayer fpScript; 

    // Start is called before the first frame update
    void Start()
    {
        hasPlayedIntroDialogue = false;
        stolenFrom = false;
        gameIsOver = false;
        inventory = Inventory.instance;
        fpScript = monkey.GetComponent<FollowPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
         if (Input.GetMouseButtonDown(0) && stolenFrom) {
            stolenFrom = false;
            //Debug.Log("disable the UI again");
            stealDialogueUI.SetActive(false);
            stoleForkUI.SetActive(false);
            if (fpScript != null) {
                fpScript.ChasePlayer();
            }
        }     
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            // if player FIRST goes up to muskrat, start playing the dialogue and start game
            if (!hasPlayedIntroDialogue) {
                StartLostFoundGame();
                hasPlayedIntroDialogue = true;
            }
            // if the fork has been put in the inventory, and player goes up to muskrat again, end game
            // also, remove the fork from the inventory (to "return to the muskrat")
            Item fork = null;
            foreach (Item item in inventory.items) {
                if (item.name == "Fork") {
                    fork = item; // Store the item if it matches
                    break; // Exit the loop once the item is found
                }
            }
            if (fork != null && hasPlayedIntroDialogue) {
                StopLostFoundGame();
                inventory.Remove(fork);
            }
        }

    }

    public void StartLostFoundGame() {
        // play the opening dialogue, and make sure the dialogue doesn't replay after it ends
        Dialogue.Activate("LostFoundIntro");
        hasPlayedIntroDialogue = true;

        // spawn the fork at a random place on the farm
        SpawnFork();

        // have the monkey start chasing the player (if player goes within certain radius of monkey)
        if (fpScript != null) { // && fork hasn't been found
            fpScript.ChasePlayer();
        }
        else {
            Debug.Log("the script is not found");
        }
    }

    public void StopLostFoundGame() {
        //stop monkey from chasing you
        if (fpScript != null) { 
            fpScript.StopChasePlayer();
        }
        else {
            Debug.Log("the script is not found");
        }

        gameIsOver = true;
        Dialogue.Activate("LostFoundOutro");
        // TODO: Collect the coins reward


    }

    private void SpawnFork() {
        Vector3 randSpawnPos = forkSpawner.transform.position + Random.insideUnitSphere * spawnRadius;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randSpawnPos, out hit, spawnRadius, NavMesh.AllAreas)) {
            Instantiate(spawnedItem, hit.position, Quaternion.identity);
        }
    }

    public void StealFromInventory() {
        //FollowPlayer fpScript = monkey.GetComponent<FollowPlayer>();
        //if monkey collides with player, take something out of inventory
        int randInt = Random.Range(0, inventory.items.Count);

        if (inventory.items.Count == 0) {
            //Debug.Log("You don't have any items in your inventory for the monkey to steal...He'll still chase you though!");
        }
        //there is at least 1 item in inventory
        else {
            // get random item in inventory - if it's the fork, spawn another fork
            Item itemToRemove = inventory.items[randInt];
            inventory.Remove(itemToRemove);
            
            if (itemToRemove.name == "Fork" ) {
                //Debug.Log("removing the fork.");
                stoleForkUI.SetActive(true);
                
                if (stoleForkUI.activeSelf && !stolenFrom) {
                    stolenFrom = true;
                    if (fpScript != null) { 
                        fpScript.StopChasePlayer();
                    }
                }
                SpawnFork(); // spawn the fork in another place if lost it
            }
            else {
                stealDialogueUI.SetActive(true);
                
                if (stealDialogueUI.activeSelf && !stolenFrom) {
                    stolenFrom = true;
                    if (fpScript != null) { 
                        fpScript.StopChasePlayer();
                    }
                }
            }
        }    

    }

}