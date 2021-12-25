using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodController : MonoBehaviour
{
   public float maxTimeOnScreen = 2.5f;
    public float currentTimeOnScreen = 0;

    public int scumPoint;

    GameController gameController;

    public bool IsActive => gameObject.activeInHierarchy;
    public bool TimesUp => currentTimeOnScreen < 0;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // 内建方法每0.02秒會更新一次，使用固定的時間計算上會比較準確
    private void FixedUpdate()
    {
        TryCountdownToHide();
    }

    private void Init() 
    {
        // 先拿到object在拿到控制項
        gameController = FindObjectOfType<GameController>().GetComponent<GameController>();
        ResetCurrentTimeOnScreen();
    }

    //隱藏物件
    private void Hide()
    {
        // gameObject指的是這個物件本身，要讓物件本身隱藏起來
        gameController.HideScum(gameObject);
    }

    private void OnMouseDown()
    {
        gameController.AddScore(scumPoint);
        ResetCurrentTimeOnScreen();
        Hide();
    }

    // 將秒數重置
    private void ResetCurrentTimeOnScreen() {
        currentTimeOnScreen = maxTimeOnScreen;
    }

    private void CountDownCurrentTime() 
    {
        currentTimeOnScreen -= Time.fixedDeltaTime;

    }

    private void TryCountdownToHide()
    {
        if( IsActive )
        {
            Debug.Log("isactivae");
            CountDownCurrentTime();
        }

        // 在熒幕上已經停留了夠久了
        if( TimesUp ) 
        {
            ResetCurrentTimeOnScreen();
            Hide();
        }
    }

}
