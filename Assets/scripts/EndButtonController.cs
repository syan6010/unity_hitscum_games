using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndButtonController : MonoBehaviour
{
    GameController gameController;
    private void Start() 
    {
        Init();
    }

    private void Init() 
    {
        gameController = FindObjectOfType<GameController>().GetComponent<GameController>();
    }
    public void Click()
    {
        gameController.GameOver();
    }
}
