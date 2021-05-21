using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private Text timerText;
    private float finishTime;
    private GameObject panelCountdown,panelGame,panelBottom;
    private Image progressBar,energyBar,iconEnergy;
    private float lev1, lev2, lev3, levPerf, energyAmount;
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;
    AudioSource sourceCount,sourceEnergy,sourceEnd,sourceTimeout;
    public AudioClip soundCount,soundEnergy,soundEnd,soundTimeout;
    private bool endGame,timeOut;
    private PlayerPrefsHandler playerPrefs;
    List<string> charachters = new List<string> { "Basket Ball", "Atom Ball","Bomb Ball"};
    

    void Awake()
    {
        if (AudioListener.pause)
        {
            AudioListener.pause = false;
            AudioListener.volume = 1f;
        }

        playerPrefs = gameObject.AddComponent<PlayerPrefsHandler>();

        //Only the player ball selected by the user is activated in the game
        foreach(string s in charachters)
        {
            if (!s.Equals(playerPrefs.GetCharacter()))
            {
                GameObject.Find(s).SetActive(false);
            }
        }

        //Check if the user preferences have muted effects and if true the effects are muted
        if (playerPrefs.GetIsMutedEffects())
        {
            sourceCount = AddAudio(soundCount, false, false, 0f);
            sourceEnergy = AddAudio(soundEnergy, false, false, 0f);
            sourceEnd = AddAudio(soundEnd, false, false, 0f);
            sourceTimeout = AddAudio(soundTimeout, false, false, 0f);
        }
        else
        {
            sourceCount = AddAudio(soundCount, false, false, 0.5f);
            sourceEnergy = AddAudio(soundEnergy, false, false, 1f);
            sourceEnd = AddAudio(soundEnd, false, false, 0.5f);
            sourceTimeout = AddAudio(soundTimeout, false, false, 1f);
        }

        panelGame = GameObject.Find("PanelGame");
        panelBottom = GameObject.Find("PanelBottom");
        timerText = GameObject.Find("TimerText").GetComponent<Text>();
        panelCountdown = GameObject.Find("PanelCountdown");

    }

    // Start is called before the first frame update
    void Start()
    {
        progressBar = panelBottom.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        energyBar = panelGame.transform.GetChild(2).GetChild(0).GetComponent<Image>();
        iconEnergy = panelGame.transform.GetChild(2).GetChild(2).GetComponent<Image>();
        iconEnergy.color = Color.white;
        progressBar.fillAmount = 0;
        progressBar.color = Color.black;
        energyBar.fillAmount = 0;
        finishTime = Time.time+4;
        GameDataManager.IsFinishTime = false;
        GameDataManager.IsPausa = false;
        GameDataManager.IsSwipe = false;
        GameDataManager.IsBasketPoint = false;
        GameDataManager.IsPerfectShoot = false;
        GameDataManager.IsFireball = false;
        GameDataManager.Score = 0;
        GameDataManager.NumFireball = 0;
        timerText.text = Constants.START_TIME;
        GameDataManager.IsCountDown = true;
        panelGame.SetActive(false);
        panelBottom.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {
        if (!GameDataManager.IsPausa)
        {
            if (GameDataManager.IsCountDown)
            {
                StartCountDown();
            }
            else
            {
                panelGame.SetActive(true);
                panelBottom.SetActive(true);
                StartTimer();
        
                if (!GameDataManager.IsThrowBall)
                {
                    if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                    {
                        SwipeTouch();
                    }
                    else
                    {
                        Swipe();
                    }
                }


                if (GameDataManager.IsBasketPoint && !GameDataManager.IsFireball)
                {
                    CheckEnergyBar();
                }

                if (GameDataManager.IsFireball)
                {
                    StartTimerEnergy();
                }

            }
        }
    }

    //Check the value of the energy bar
    public void CheckEnergyBar()
    {
        GameDataManager.IsBasketPoint = false;

        //while the energy value is less then 1, the energy bar is incremented of 0.25.
        if (energyBar.fillAmount <1)
        {
            energyBar.fillAmount += 0.25f;

            //if the energy bar is full, fireball bonus is activated, the energy icon is now red and the energy sound is enabled
            if (energyBar.fillAmount == 1)
            {
                GameDataManager.NumFireball += 1;
                iconEnergy.color = Color.red;
                iconEnergy.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                energyAmount = 0;
                GameDataManager.IsFireball = true;
                sourceEnergy.enabled = true;
                sourceEnergy.Play();
            }
        }
    }

    //When the fireball is activated, the timer energy starts and every 2 seconds the energy bar decrements of 0.25 
    public void StartTimerEnergy()
    {

        energyAmount += Time.deltaTime;
        int n = (int)energyAmount % 60;

        switch (n)
        {
            case 2:
                sourceEnergy.Stop();
                sourceEnergy.enabled = false;
                energyBar.fillAmount = 0.75f;
                break;
            case 4:
                energyBar.fillAmount = 0.50f;
                break;
            case 6:
                energyBar.fillAmount = 0.25f;
                break;
            case 8:
                energyBar.fillAmount = 0;
                break;
            case 9:
                GameDataManager.IsFireball = false;
                iconEnergy.color = Color.white;
                iconEnergy.transform.localScale = new Vector3(1f, 1f, 1f);
                break;
            default:
                break;
        }
    }

    //It starts the countdown of 3 seconds with the text numbers shown on the screen
    private void StartCountDown()
    {

        float t = finishTime - Time.time;
        int tSec = (int)t;
        if (tSec >0)
        {
            if (tSec == 4)
            {
                sourceCount.enabled = true;
                sourceCount.Play();
            }

                panelCountdown.transform.GetChild(0).GetComponent<Text>().text = tSec + "";
        }
        else if(tSec==0)
        {
            panelCountdown.transform.GetChild(0).GetComponent<Text>().text = "Go!";
        }
        else
        {
            //Countdown is finished and so the match timer of 60 seconds starts
            GameDataManager.IsCountDown = false;
            panelCountdown.SetActive(false);
            finishTime += 61;
        }
    }

    //It calculates the match timer of 60 seconds
    public void StartTimer()
    {
        float t = finishTime- Time.time;
        int tSec = (int)t;
        string min = ((int)t / 60).ToString();
        string sec = (t % 60).ToString("f2");

        //if the minutes are 0, this is set to the text.
        if (sec.Length < 5)
        {
            sec = "0" + sec;
        }

        timerText.text = min+":"+sec.Replace(".",":");

        //the last 4 seconds is played the countdown sound
        if (tSec < 4 && !timeOut)
        {
            panelBottom.transform.GetChild(1).gameObject.SetActive(false);
            timeOut = true;
            sourceTimeout.enabled = true;
            sourceTimeout.Play();
        }

        //the last second is played the sound about the end of the match
        if (tSec < 1 && !endGame)
        {
            endGame = true;
            sourceTimeout.Stop();
            sourceTimeout.enabled = false;
            sourceEnd.enabled = true;
            sourceEnd.pitch = 1.5f;
            sourceEnd.Play();
        }

        //if timer is 0, The game is set to pause and the final menu is opened
        if (sec.Equals("00.00") || sec.Contains("-"))
        {
            timerText.text = Constants.FINISH_TIME;
            GameDataManager.IsPausa=true;
            GameDataManager.IsFinishTime = true;
            panelGame.SetActive(false);
            panelBottom.SetActive(false);
        }


    }

    //it is used to check swipe if the device is not Android or Apple
    public void Swipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //save began touch 2d point
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        if (Input.GetMouseButtonUp(0))
        {
            //save ended touch 2d point
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            //create vector from the two points
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

            //normalize the 2d vector
            currentSwipe.Normalize();

            //swipe upwards
            if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
            {
                //Swipe detected and calculate the error for the velocity of the shoot
                GameDataManager.IsSwipe = true;
                GameDataManager.ErrorVelocity = CalculateError();
            }
        }
    }

    //it is used to check swipe if the device is Android or Apple
    public void SwipeTouch()
    {
        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                //save began touch 2d point
                firstPressPos = new Vector2(t.position.x, t.position.y);
            }
            if (t.phase == TouchPhase.Ended)
            {
                //save ended touch 2d point
                secondPressPos = new Vector2(t.position.x, t.position.y);

                //create vector from the two points
                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                //normalize the 2d vector
                currentSwipe.Normalize();

                //swipe upwards
                if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    //Swipe detected and calculate the error for the velocity of the throwing
                    GameDataManager.IsSwipe = true;
                    GameDataManager.ErrorVelocity = CalculateError();
                }

            }
        }
    }

    //Calculate the error to assign to the velocity of the throwing based on the y of the start position (high swipe up)
    public float CalculateError()
    {
        CalculateLevels();

        float vel=0;

        //LEVEL 3
        if (secondPressPos.y > lev3)
        {
            vel += 0.1f;
            progressBar.fillAmount = 1f;
            progressBar.color = Color.red;
        }
        //LEVEL perf-3
        else if (secondPressPos.y > levPerf && secondPressPos.y < lev3)
        {
            vel += 0.05f;
            progressBar.fillAmount = 0.75f;
            progressBar.color = new Color32(255, 137, 0, 255);
        }

        //LEVEL 2-perfect
        else if (secondPressPos.y > lev2 && secondPressPos.y < levPerf)
        {
 
            if (secondPressPos.y > lev2 + 100 && secondPressPos.y < levPerf - 100)
            {
                GameDataManager.IsPerfectShoot = true;
            }
            progressBar.fillAmount = 0.5f;
            progressBar.color = Color.green;

        }
        //LEVEL1-2
        else if (secondPressPos.y > lev1 && secondPressPos.y < lev2)
        {
            if(secondPressPos.y>lev1+90 && secondPressPos.y < lev2 + 90)
            {
                vel -= 0.02f;
            }
            else
            {
                vel -= 0.05f;
            }
            progressBar.fillAmount = 0.36f;
            progressBar.color = new Color32(195, 236, 34, 255);
        }
        //LEVEL1
        else if (secondPressPos.y < lev1)
        {
            vel -= 0.1f;
            progressBar.fillAmount = 0.18f;
            progressBar.color = Color.yellow;
        }
        return vel;
    }

    //Create 3 types of levels for errors and the type of level is selected based on the z of the start position
    public void CalculateLevels()
    {
        if (GameDataManager.StarPosition.z == 0.1f)
        {
            lev1 = 400f;
            lev2 = 600f;
            levPerf = 900f;
            lev3 = 1200f;
        }
        else if (GameDataManager.StarPosition.z == 0.2f)
        {
            lev1 = 800f;
            lev2 = 1000f;
            levPerf = 1300f;
            lev3 = 1600f;
        }
        else
        {
            lev1 = 1200f;
            lev2 = 1400f;
            levPerf = 1700f;
            lev3 = 2000f;
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

}
