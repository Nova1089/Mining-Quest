// GameDev.tv Challenge Club. Got questions or want to share your nifty solution?
// Head over to - http://community.gamedev.tv

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class GameHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text FoodAmountText;

    [SerializeField] private int foodCollected = 0;

    private void Awake()
    {
        int gameHandlerCount = FindObjectsOfType<GameHandler>().Length;
        if (gameHandlerCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);

        }
    }

    private void Start()
    {
        FoodAmountText = GameObject.Find("FoodAmountText").GetComponent<TMP_Text>();

    }

    private void Update()
    {
        FoodAmountText = GameObject.Find("FoodAmountText").GetComponent<TMP_Text>();

        FoodAmountText.text = foodCollected.ToString();
    }

    public void updateFood()
    {
        foodCollected++;
    }

    public void resetFoodCount()
    {
        foodCollected = 0;
    }

}
