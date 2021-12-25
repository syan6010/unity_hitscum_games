using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScumController : MonoBehaviour
{
    // 每個scum在畫面的最大停留時間
    public float maxTimeOnScreen;
    public float currentTimeOnScreen;
    // 設定每個scum被點擊的時候的分數

    public int scumPoint;

    GameController gameController;

    // 回傳這個gameobject目前是不是顯示狀態

    public bool IsActive => gameObject.activeInHierarchy;
    // 檢驗scum出現在畫面的時間是不是夠久了
    public bool TimesUp => currentTimeOnScreen < 0;
    void Start()
    {
        Init();
    }

    // 内建方法每0.02秒會更新一次，使用固定的時間計算上會比較準確
    private void FixedUpdate()
    {
        // 到了特定的時間就將自動顯示出來的scum隱藏起來
        TryCountdownToHide();
    }

    // 滑鼠點擊時加上點擊物件設定的分數（scumPoint）
    private void OnMouseDown()
    {
        gameController.AddScore(scumPoint);
        ResetCurrentTimeOnScreen();
        Hide();
    }

    private void Init() 
    {
        // 先拿到object在拿到控制項
        gameController = FindObjectOfType<GameController>().GetComponent<GameController>();
        ResetCurrentTimeOnScreen();
    }

    // 讓出現在畫面上的scum經過特定時間(maxtimeonScreen)過後就會自動消失
    private void TryCountdownToHide()
    {
        // 如果是出現的狀態就倒數計時
        if( IsActive )
        {
            CountDownCurrentTime();
        }

        // 在熒幕上已經停留了夠久了，就將scum隱藏，隱藏的scum會被加到GameController當中的HiddenScums這個
        // list，讓後在特定時間后優惠隨機出現在畫面上
        if( TimesUp ) 
        {
            ResetCurrentTimeOnScreen();
            Hide();
        }
    }


    // 3. 各項功能function
    // 3.1 隱藏物件
    private void Hide()
    {
        // gameObject指的是這個物件本身，要讓物件本身隱藏起來
        gameController.HideScum(gameObject);
    }


    // 3.2 將秒數重置
    private void ResetCurrentTimeOnScreen() {
        currentTimeOnScreen = maxTimeOnScreen;
    }

    // 3.3 倒數計時
    private void CountDownCurrentTime() 
    {
        currentTimeOnScreen -= Time.fixedDeltaTime;

    }
}
