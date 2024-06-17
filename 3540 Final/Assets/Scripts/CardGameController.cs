using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameController : MonoBehaviour
{
    public GameObject cardTable; // Reference to the card table game object
    public GameObject goodCardDisplay; // UI element for good card display
    public GameObject badCardDisplay;  // UI element for bad card display

    public GameObject step2;
    public GameObject win;
    public GameObject lose;

    private bool cardGamePlayed = false; // Flag to track card game state
    

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
    }

    void StartCardGame()
    {
        cardGamePlayed = true; // Enter card game state

        // Determine card outcome (good or bad) randomly
        bool isGoodCard = Random.Range(0, 2) == 0; // 50% chance of good card
        step2.SetActive(false);

        if (isGoodCard)
        {
            goodCardDisplay.SetActive(true); // Show good card display
            Level1Enemy.SetCardGamePlayed(true);
            win.SetActive(true);
        }
        else
        {
            badCardDisplay.SetActive(true); // Show bad card display
            Level1Enemy.SetCardGamePlayed(false); // Call SetCardGamePlayed on enemy (lose)
            lose.SetActive(true);
        }
    }

}
