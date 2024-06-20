using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CardGameController : MonoBehaviour
{
    public GameObject cardTable; // Reference to the card table game object
    public GameObject goodCardDisplay; // UI element for good card display
    public GameObject badCardDisplay;  // UI element for bad card display

    public GameObject step2;
    public GameObject win;
    public GameObject lose;
    public GameObject goodInstructions;
    public GameObject badInstructions;
    public GameObject salonDoor;

    private bool cardGamePlayed = false; // Flag to track card game state
    public static bool cardGameInProgress = false;
    bool isGoodCard;
    

    void Start()
    {
        goodCardDisplay.SetActive(false); // Initially hide good card display
        badCardDisplay.SetActive(false); // Initially hide bad card display
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !cardGamePlayed) // Check for mouse click on card table
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == cardTable)
            {
                StartCardGame();
                
            }
        }

        //When player presses escape so back to game mode and enemies start attacking if bad card
        if (cardGameInProgress & Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (isGoodCard)
            {
                goodCardDisplay.SetActive(false); // Show good card display
                EnemyImproved.SetCardGamePlayed(true);
                win.SetActive(true);
                goodInstructions.SetActive(false);
            }
            else
            {
                badCardDisplay.SetActive(false); // Show bad card display
                EnemyImproved.SetCardGamePlayed(false); // Call SetCardGamePlayed on enemy (lose)
                lose.SetActive(true);
                badInstructions.SetActive(false);
            }
            salonDoor.SetActive(false);
        }
    }

    void StartCardGame()
    {
        cardGamePlayed = true; // Enter card game state

        // Determine card outcome (good or bad) randomly
        isGoodCard = Random.Range(0, 2) == 0; // 50% chance of good card
        step2.SetActive(false);
        cardGameInProgress = true;
        isGoodCard = false;
        if (isGoodCard)
        {
            goodCardDisplay.SetActive(true); // Show good card display
            goodInstructions.SetActive(true);
        }
        else
        {
            badCardDisplay.SetActive(true); // Show bad card display
            badInstructions.SetActive(true);
        }
    }

}
