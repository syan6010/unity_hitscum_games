using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameController : MonoBehaviour
{
    Text score;
    Text scoreChange;
    // 游戲結束的時候會顯示的文字
    Text gameOver;
    Button reloadBtn;
    Button gameOverButton;
    // 記錄總分的變數
    int scoreNum;
    // 檢測計時器是否到了，需要產生新的scum在畫面上
    bool countDownScumTimeUp => countDownScumIntervalSec <= 0;
    // 判斷畫面上的scum是否已經滿了
    bool scumOnScreenAreFull => ScumCountOnScreen >= maxScumOnScreen;
    // 檢測加減分提示顯示的時間是不是已經到了

    bool countDownScoreChangeTimeUp => countDownScoreChangeInterval <= 0;
    // 當變數設定為public之後會被序列化，所有數字的更動只能在panel當中修改
    // 過關的分數  
    public int maxScore;
    // 點擊真確標的scum的音效
    public AudioSource shootAudio;
    // 點擊錯誤標的good的音效
    public AudioSource wrongAudio;

    // 每x秒show出一隻scum
    public float showScumIntervalSec;
    // 用來倒數計時的變數
    private float countDownScumIntervalSec;
    // 同時允許幾隻scum出現在畫面上
    public float maxScumOnScreen;
    // 設定計分板出現在畫面上的時間長度
    public float showScoreChangeInterval;
    // 用來倒數計時的變數
    private float countDownScoreChangeInterval;

    public List<ScumController> scums;
    // 製作出沒有出現的scum，作爲隨機出現的清單
    public List<ScumController> HiddenScums
    {
        get 
        {
            var temScums = new List<ScumController>();

            foreach( var scum in scums )
            {
                if( !scum.IsActive )
                {
                    temScums.Add(scum);
                }
            }

            return temScums;
        }
    }

    // 計算目前有多少scum是顯示出來的
    public int ScumCountOnScreen
    {
        get
        {
            int scumNum = 0;
            foreach( var scum in scums )
            {
                if(scum.IsActive)
                {
                    scumNum += 1;
                }
            }

            return scumNum;
        }
    }


    void Start()
    {
        InitUI();  
        InitScumList();
        HideAllScums();
        // 調好每個scum的倒數計時器，每一個固定的時間產生新的scum
        ResetScumTime();
    }



    
    void FixedUpdate()
    {
        // 倒數計時，時間一到就顯示新的scum
        CountDownToShowScum();
        // 倒數計時，加減分提示持續的時間
        CountDownScoreChange();

    }

    void InitUI()
    {
        InitScore();
        InitGameOverBanner();
        InitReloadButton();
        InitGameOverButton();
    }

    // 1.初始化
    // 1.1 UI組件初始化-------------

    private void InitScore ()
    {
        score = GameObject.FindGameObjectWithTag( "Score" ).GetComponent<Text>();
        scoreChange = GameObject.FindGameObjectWithTag("ScoreChange").GetComponent<Text>();
        score.text = "總分： 0";
        scoreChange.text = "";
    }

    private void InitGameOverButton()
    {
        gameOverButton = GameObject.FindGameObjectWithTag("EndButton").GetComponent<Button>();
        HideScum(gameOverButton.gameObject);
    }

    private void InitReloadButton()
    {
        reloadBtn = GameObject.FindGameObjectWithTag("ReloadButton").GetComponent<Button>();
        HideScum(reloadBtn.gameObject);
    }
    private void InitGameOverBanner()
    {
        gameOver = GameObject.FindGameObjectWithTag("GameOver").GetComponent<Text>();
        gameOver.text = "";
    }

   // 1.2 scum初始化-------------

    private void InitScumList () {
        // 因爲這裏FindObjectsOfType的回傳值是陣列，所以要將資料形態轉換成我們的容器list
        scums = GameObject.FindObjectsOfType<ScumController>().ToList();
    }
    

    // 1.3 重新設定scum存在的時間 ps.利用panel當中的showScumIntervalSec設定
    public void ResetScumTime() 
    {
        countDownScumIntervalSec = showScumIntervalSec;
    }


    // -----------------------------------------------
    // 2.計時器

    // 2.1 特定秒數（showScumIntervalSec）后產生新的scum
    public void CountDownToShowScum()
    {
        countDownScumIntervalSec -= Time.fixedDeltaTime;
        // 等到x秒的倒數計時一到
        if(countDownScumTimeUp)
        {
            // 重設產出scum的時間
            ResetScumTime ();
            // 如果出現在熒幕的scum數量小於最大值就隨機在增加一個沒有active的scum顯示在畫面當中，若是
            // 同時間畫面上的scum已經大於3隻scum就不再出現scum
            if( !scumOnScreenAreFull )
            {
                ShowRandomScum();
            }
        }
    }


    // 2.2 特定時間后讓加分提示消失
    public void CountDownScoreChange()
    {
        countDownScoreChangeInterval -= Time.fixedDeltaTime;

        if(countDownScoreChangeTimeUp)
        {
            // 重新計時
            ResetScoreChangeTime();
            // 時間到了就讓加分版消失
            scoreChange.text = "";
        }
        
    }

    // -----------------------------------------------
    // 3.各項功能function

    void HideAllScums ()
    {
        foreach( var scum in scums )
        {
            HideScum( scum.gameObject );
        }
    }

    public void ShowScum ( GameObject scum )
    {
        scum.SetActive(true);
    }

    public void HideScum(GameObject scum)
    {
        scum.SetActive(false);
    }

    public void PlayAudio(int scumPoint)
    {
        if(scumPoint >= 0) shootAudio.Play();
        else wrongAudio.Play();
    }

    // 在尚未出現的scum列表當中隨機加入一個到畫面上
    public void ShowRandomScum() 
    {
        // 從scums當中隨機跳出一個scum
        int rnd = Random.Range(0, HiddenScums.Count);
        ScumController scum = HiddenScums[rnd];
        ShowScum(scum.gameObject);
        
    }

    // 將點到scynController物件的時候，增加該物件的分數（每個scum的點數不同）到計分板當中
    public void AddScore (int scumPoint) 
    {
        scoreNum += scumPoint;
        score.text = "總分：" + scoreNum.ToString();
        // 如果是正數要多加一個“+”
        string sign = scumPoint >= 0 ? "+" : "";
        scoreChange.text = sign + scumPoint.ToString();
        // 播放音效
        PlayAudio(scumPoint);

        // 如果分數 > panel設定的最大總分時，游戲暫停
        if(scoreNum > maxScore) 
        {
            GameStop();
        }
    }

    public void ResetScoreChangeTime()
    {
        countDownScoreChangeInterval = showScoreChangeInterval;
    }


    // -----------------------------------------------
    // 4. 游戲流程控制
    // 4.1 重新開始游戲
    public void GameReload()
    {
        Time.timeScale = 1;
        score.text = "";
        gameOver.text = "";
        HideScum(reloadBtn.gameObject);
        HideScum(gameOverButton.gameObject);
    }

    // 4.2 暫停游戲
    private void GameStop() 
    {
        HideAllScums(); 
        gameOver.text = "游戲結束";
        scoreNum = 0;
        TimeStop();
        ShowScum(reloadBtn.gameObject);
        ShowScum(gameOverButton.gameObject);
    }


    // 4.3 離開游戲
    public void GameOver()
    {
        // UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    // -----------------------------------------------
    // a.4.1 時間暫停
    public void TimeStop()
    {
        Time.timeScale = 0;
        scoreChange.text = "";
    }

    // -----------------------------------------------

}
