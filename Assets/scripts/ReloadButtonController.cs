using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadButtonController : MonoBehaviour
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
        gameController.GameReload();
    }
}
