using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private GameObject panelGame, panelBottom, panelMenu, panelWarning;
    private bool isTimerPause;
    private float f;
    private int record;
    private PlayerPrefsHandler playerPrefs;
    AudioSource[] menuMusics;
    AudioSource sourceClick,gameMusic,endMusic;
    public AudioClip soundClick;
    private bool finalMenuOpened;
    private string warningAlert;

    void Awake()
    {
        playerPrefs = gameObject.AddComponent<PlayerPrefsHandler>();
        menuMusics = this.GetComponents<AudioSource>();
        gameMusic = menuMusics[0];
        endMusic = menuMusics[1];

        //Check if the user preferences have muted music and if true the musics are muted 
        if (playerPrefs.GetIsMutedMusic())
        {
            gameMusic.volume = 0f;
            gameMusic.enabled = false;
            endMusic.volume = 0f;
            endMusic.enabled = false;
        }
        else
        {
            gameMusic.volume = 1f;
            gameMusic.enabled = false;
            endMusic.volume = 1f;
            endMusic.enabled = false;
        }

        //Check if the user preferences have muted effects and if true the effects are muted
        if (playerPrefs.GetIsMutedEffects())
        {
            sourceClick = AddAudio(soundClick, false, false, 0f);

        }
        else
        {
            sourceClick = AddAudio(soundClick, false, false, 1f);
        }


        panelGame = GameObject.Find("PanelGame");
        panelBottom = GameObject.Find("PanelBottom");
        panelMenu = GameObject.Find("PanelMenu");
        panelWarning = GameObject.Find("PanelWarning");

        SetResponsiveCanvas();
    }


    // Start is called before the first frame update
    void Start()
    {
        warningAlert = "";
        panelMenu.SetActive(false);
        panelWarning.SetActive(false);
        record = playerPrefs.GetRecord();

    }

    // Update is called once per frame
    void Update()
    {
        //check if the timer pause has to be enabled
        if (isTimerPause)
        {
            //Start short delay when the user resume the match after he close the pause menu.
            StartDelay();
        }

        //check if the timer is finished and if is true open the final menu
        if (GameDataManager.IsFinishTime)
        {
            HandleFinalMenu();
            finalMenuOpened = true;
        }
    }

    //handle the pause menu to open or close it
    public void HandlePauseMenu(string action)
    {
        sourceClick.enabled = true;
        sourceClick.Play();
        if (action.Equals("OpenPausa"))
        {
            gameMusic.enabled = true;
            gameMusic.Play();
            GameDataManager.IsPausa = true;
            panelGame.SetActive(false);
            panelBottom.SetActive(false);
            panelMenu.SetActive(true);
            panelMenu.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            panelMenu.transform.GetChild(0).GetChild(1)
                .GetChild(4).gameObject.GetComponent<Text>().text = "Score: " + GameDataManager.Score;
        }
        //
        else if (action.Equals("ClosePausa"))
        {
            gameMusic.Stop();
            gameMusic.enabled = false;
            panelGame.SetActive(true);
            panelBottom.SetActive(true);
            panelMenu.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            panelMenu.SetActive(false);
            isTimerPause = true;
        }
    }

    //Open final menu and all the elements are calculated and shown (score, best, money)
    public void HandleFinalMenu()
    {
        int finalCoins = GameDataManager.Coins + (5 * GameDataManager.NumFireball);

        if (!finalMenuOpened)
        {
            endMusic.enabled = true;
            endMusic.Play();
            playerPrefs.SetNumMatch(playerPrefs.GetNumMatch() + 1);
            playerPrefs.SetMoney(playerPrefs.GetMoney() + finalCoins);

        }
        panelGame.SetActive(false);
        panelBottom.SetActive(false);
        panelMenu.SetActive(true);
        panelMenu.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        panelMenu.transform.GetChild(0).GetChild(0)
            .GetChild(3).gameObject.GetComponent<Text>().text = "Best: " + record;
        panelMenu.transform.GetChild(0).GetChild(0)
            .GetChild(4).gameObject.GetComponent<Text>().text = "Score: " + GameDataManager.Score;
        panelMenu.transform.GetChild(0).GetChild(0)
            .GetChild(5).GetChild(1).gameObject.GetComponent<Text>().text = "" + finalCoins;
        if (GameDataManager.Score > playerPrefs.GetRecord())
        {
            playerPrefs.SetRecord(GameDataManager.Score);
            panelMenu.transform.GetChild(0).GetChild(0)
                .GetChild(2).GetChild(1).gameObject.GetComponent<Text>().text = "NEW RECORD!";
        }

    }

    
    public void Restart()
    {
        //if user press restart button in the pause menu
        if (!GameDataManager.IsFinishTime)
        {
            warningAlert = "Restart";
            //the warning panel is opened to warn the user that if he continue this action he will lose the last data game
            panelWarning.SetActive(true);

        }

        //if user press restart button in the final menu, stop music and reload the same scene from the beginning
        else
        {
            gameMusic.Stop();
            gameMusic.enabled = false;
            SceneManager.LoadScene("GameScene");
        }
       
    }

    public void BackToHome()
    {
        //if user press home button in the pause menu
        if (!GameDataManager.IsFinishTime)
        {
            warningAlert = "Home";
            //the warning panel is opened to warn the user that if he continue this action he will lose the last data game
            panelWarning.SetActive(true);
        }

        //if user press home button in the final menu, stop music and load the Home scene
        else
        {
            gameMusic.Stop();
            gameMusic.enabled = false;
            SceneManager.LoadScene("HomeScene");
        }
     
    }

    //Warning panel is enabled from pause menu if user want to restart or go to home
    public void HandleWarningLoseData(string action)
    {
        //Check if user select yes or no

        //if yes, stop music and if the warning panel is opend from restart action, restart the scene else load home scene
        if (action.Equals("yes"))
        {
            gameMusic.Stop();
            gameMusic.enabled = false;
            if (warningAlert.Equals("Home"))
            {
                SceneManager.LoadScene("HomeScene");
            }
            else if (warningAlert.Equals("Restart"))
            {
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                panelWarning.SetActive(false);
            }
        }

        //if no, close the warning panel and return to the pause menu
        else if (action.Equals("no"))
        {
            panelWarning.SetActive(false);
        }
    }

    //timer that starts when user resume the match and it remains the game in pause for an other 1 second before to resume to play
    public void StartDelay()
    {
        f += Time.time;
        int n = (int)f % 60;
        if (n == 1)
        {
            GameDataManager.IsPausa = false;
            isTimerPause = false;
        }
    }

    //Create new audio source and set the most important parameters
    public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        newAudio.clip = clip;
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.enabled = false;
        newAudio.volume = vol;
        return newAudio;
    }

    //Set responsive based on various layouts of Android and Apple Devices
    public void SetResponsiveCanvas()
    {
        GameObject menuPause = panelMenu.transform.GetChild(0).gameObject;
        GameObject menuWarning = panelWarning.transform.GetChild(0).gameObject;

        RectTransform rtPause = menuPause.gameObject.GetComponent<RectTransform>();
        RectTransform rtWarning = menuWarning.gameObject.GetComponent<RectTransform>();

        if ((Screen.height > 2600 && Screen.height < 3000) && (Screen.width > 1100 && Screen.width < 1500))
        {

        }
        else if ((Screen.height > 2500 && Screen.height < 2900) && (Screen.width > 1100 && Screen.width < 1500))
        {
            

            rtPause.offsetMax = new Vector2(rtPause.offsetMax.x, -400);
            rtPause.offsetMin = new Vector2(rtPause.offsetMin.x, 400);

            rtWarning.offsetMax = new Vector2(rtWarning.offsetMax.x, -350);
            rtWarning.offsetMin = new Vector2(rtWarning.offsetMin.x, 400);
            

        }
        else if ((Screen.height > 2100 && Screen.height < 2400) && (Screen.width > 1000 && Screen.width < 1500))
        {

            rtPause.offsetMin = new Vector2(rtPause.offsetMin.x, 400);

            rtWarning.offsetMax = new Vector2(rtWarning.offsetMax.x, -400);

        }
        else if ((Screen.height > 1900 && Screen.height < 2100) && (Screen.width > 1000 && Screen.width < 1500))
        {

            rtPause.offsetMax = new Vector2(rtPause.offsetMax.x, -400);
            rtPause.offsetMin = new Vector2(rtPause.offsetMin.x, 400);

            rtWarning.offsetMax = new Vector2(rtWarning.offsetMax.x, -350);
            rtWarning.offsetMin = new Vector2(rtWarning.offsetMin.x, 400);
        }

        else if ((Screen.height > 1200 && Screen.height < 1500) && (Screen.width > 700 && Screen.width < 1000))
        {
            rtPause.offsetMax = new Vector2(rtPause.offsetMax.x, -400);
            rtPause.offsetMin = new Vector2(rtPause.offsetMin.x, 400);

            rtWarning.offsetMax = new Vector2(rtWarning.offsetMax.x, -350);
            rtWarning.offsetMin = new Vector2(rtWarning.offsetMin.x, 400);


        }

        else if ((Screen.height > 780 && Screen.height < 1200) && (Screen.width > 400 && Screen.width < 700))
        {
            rtPause.offsetMax = new Vector2(rtPause.offsetMax.x, -350);
            rtPause.offsetMin = new Vector2(rtPause.offsetMin.x, 350);

            rtWarning.offsetMax = new Vector2(rtWarning.offsetMax.x, -350);
            rtWarning.offsetMin = new Vector2(rtWarning.offsetMin.x, 300);

        }
        else if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
        {
            GameObject.Find("Canvas").GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            rtPause.offsetMax = new Vector2(-200,rtPause.offsetMax.y);
            rtPause.offsetMin = new Vector2(200, rtPause.offsetMin.y);

            rtWarning.offsetMax = new Vector2(-200, rtWarning.offsetMax.y);
            rtWarning.offsetMin = new Vector2(200, rtWarning.offsetMin.y);
        }
    }


}
