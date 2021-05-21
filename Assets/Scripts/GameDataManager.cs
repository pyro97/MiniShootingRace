using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance; //Declare an instance of GamaManager
    public static bool IsThrowBall { get; set; } //check if the ball is thrown
    public static bool IsCountDown { get; set; } //check if the countdown is started
    public static bool IsCameraFollow { get; set; } // check if the camera can follow the ball
    public static int Score { get; set; } = 0; // contains the score of the match
    public static int Coins { get; set; } // contains the collected coins of the match
    public static int NumFireball { get; set; } // contains the number of times that the fireball bonus has been enabled 
    public static bool IsFinishTime { get; set; } // check if the match time is finished
    public static float Timer { get; set; } = 60; // contains the seconds of the timer to play the match
    public static bool PauseGame; // It is set to true if the game has to go in pause 
    public static bool IsSwipe { get; set; } // check if the user did a swipe up
    public static bool IsPerfectShoot { get; set; } //check if the shoot is perfect
    public static Vector3 StarPosition { get; set; } //contains the initial position where the ball will be throwed
    public static float ErrorVelocity { get; set; } // contains the value about the error to edit the shoot velocity
    public static bool IsBasketPoint { get; set; } // check if it is scored a point
    public static bool IsFireball { get; set; } // check if the fireball bonus is enable

    //This bool is set to true if PausaGame is true and set TimeScale to 0 (freeze the scene, so this is in pause)
    public static bool IsPausa
    {
            get
            {
                return PauseGame;
            }

            set
            {
            PauseGame = value;
                if (value)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
            }
        
    }



    void Awake()
    {
        //This code defines Singleton

        /*
         If the GameManager istance doesn't exist, so the istance is this same script, else delete this gameobject
         to use the present GameObject
         */
     
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
            //This statement makes it permanent
        }
        else
        {
            Destroy(transform.root.gameObject);
            return;
        }
    }

}
