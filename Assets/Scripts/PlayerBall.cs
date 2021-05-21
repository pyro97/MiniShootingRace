using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBall : MonoBehaviour
{
    public Transform target;
    public float firingAngle = 45.0f;
    public float gravity = 1.2f;
    private Transform myTransform;
    private Rigidbody rigidBody;
    private List<Vector3> positionsList;
    private Text scoreText;
    private bool isTouchBoard,isInNet;
    private Image progressBar;
    private int bonus=1;
    AudioSource sourceShoot,sourcePoint,sourceWood,sourceIron,sourceBasketEffect;
    public AudioClip soundShoot,soundPoint,soundWood,soundIron,soundBasketEffect;
    private GameObject panelPoints;
    private PlayerPrefsHandler playerPrefs;

    //inside class


    void Awake()
    {
        playerPrefs = gameObject.AddComponent<PlayerPrefsHandler>();

        //Check if the user preferences have muted effects and if true the effects are muted
        if (playerPrefs.GetIsMutedEffects())
        {
            sourceShoot = AddAudio(soundShoot, false, false, 0f);
            sourcePoint = AddAudio(soundPoint, false, false, 0f);
            sourceWood = AddAudio(soundWood, false, false, 0f);
            sourceIron = AddAudio(soundIron, false, false, 0f);
            sourceBasketEffect = AddAudio(soundBasketEffect, false, false, 0f);
        }
        else
        {
            sourceShoot = AddAudio(soundShoot, false, false, 1f);
            sourcePoint = AddAudio(soundPoint, false, false, 1f);
            sourceWood = AddAudio(soundWood, false, false, 1f);
            sourceIron = AddAudio(soundIron, false, false, 0.5f);
            sourceBasketEffect = AddAudio(soundBasketEffect, false, false, 1f);
            sourceBasketEffect.pitch = 1.5f;
        }
            

        progressBar = GameObject.Find("ProgressBar").transform.GetChild(0).GetComponent<Image>();
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        panelPoints = GameObject.Find("PanelPoints");

    }

    // Start is called before the first frame update
    void Start()
    {
        positionsList = new List<Vector3>();
        myTransform = this.gameObject.transform;
        GameDataManager.StarPosition = myTransform.position;
        GameDataManager.Coins = 0;
        rigidBody = this.gameObject.GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
        scoreText.text="0";
        panelPoints.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //If user swipe up starts the coroutine of throw ball
        if (GameDataManager.IsSwipe)
        {
            isInNet = false;
            isTouchBoard = false;
            StartCoroutine(ThrowBall());
        }

        //If ball is thrown, all variabels returned to the initial value
        if (GameDataManager.IsThrowBall)
        {
            progressBar.fillAmount = 1;
            progressBar.color = Color.black;
            GameDataManager.IsThrowBall = false;
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            sourceBasketEffect.Stop();
            sourceBasketEffect.enabled = false;
            panelPoints.SetActive(false);

            //Select new position for the next ball throw
            ManagePositions();
            rigidBody.transform.SetPositionAndRotation(GameDataManager.StarPosition, new Quaternion());
            GameDataManager.IsCameraFollow = false;
        }

    }

    IEnumerator ThrowBall()
    {
        sourceShoot.enabled = true;
        sourceShoot.Play();
        GameDataManager.IsSwipe = false;
        rigidBody.isKinematic = false;

        // Short delay before the ball is thrown
        yield return new WaitForSeconds(0.2f);

        // Move projectile to the position of throwing object + add some offset if needed.
        myTransform.position += new Vector3(0, 0.0f, 0);

        // Calculate distance to target
        float target_Distance = Vector3.Distance(myTransform.position, target.position);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float ball_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        ball_Velocity += GameDataManager.ErrorVelocity;

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(ball_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(ball_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        // Rotate ball to face the target.
        myTransform.rotation = Quaternion.LookRotation(target.position - myTransform.position);

        //Translate the ball while the flight time ends. 
        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
            myTransform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            yield return null;
        }

        //Set ball gravity to true so the ball can fall to the ground
        rigidBody.useGravity = true;

        //Camera follow ball
        yield return new WaitForSeconds(0.2f);
        GameDataManager.IsCameraFollow = true;
        yield return new WaitForSeconds(0.8f);
        GameDataManager.IsThrowBall = true;
    }


    //Select a random position from the list
    private void ManagePositions()
    {
        int n = positionsList.Count;

        //If List is empty beacause all the positions are removed, the list will be initialize.
        if (n == 0)
        {
            positionsList = new List<Vector3>
            {
                new Vector3(0.1f, 0.1f, 0.3f),
                new Vector3(-0.1f, 0.1f, 0.2f),
                new Vector3(0.1f, 0.1f, 0.1f),
                new Vector3(-0.1f, 0.1f, 0.1f),
                new Vector3(0.1f, 0.1f, 0.2f),
                new Vector3(-0.1f, 0.1f, 0.3f)
            };
        }

        //Select a random number from the lenght of list
        float range = n - 0.1f;
        int randNumber = (int)Random.Range(0,range);

        //Set the selected position using index of the list.
        GameDataManager.StarPosition = positionsList[randNumber];

        //Remove the position selected from list, so the next position will different compared to the previous one 
        positionsList.RemoveAt(randNumber);
    }


    //Check if ball trigger with Net, Hoop, Billboard.
    private void OnTriggerEnter(Collider other)
    {
        sourceShoot.Stop();
        sourceShoot.enabled = false;

        //The ball has triggered an invisible object to score points
        if (other.CompareTag("Finish"))
        {
            /*
             Check if the ball is entered in the basket hoop and it has triggered the score point invisible object,
             because sometimes if the shoot is short, the ball can trigger only the score object without the check net
             and obviously it is not a valid point.
             */
            if (isInNet)
            {
                int points;
                sourcePoint.enabled = true;
                sourcePoint.Play();

                GameDataManager.IsBasketPoint = true;
                isInNet = false;

                //If the fireball bonus is activated, the bonus increment is 2
                if (GameDataManager.IsFireball)
                {
                    bonus = 2;
                }
                else
                {
                    bonus = 1;
                }

                /*
                Check if the ball has touched the BillBoard before and if it's true it will be assigned a
                random point between 4 and 5
                */
                if (isTouchBoard)
                {
                    GameDataManager.Coins += 10;
                    int randomBoard = Random.Range(4, 6);
                    points = randomBoard * bonus;
                    GameDataManager.Score += points;
                }
                else
                {
                    //If the ball didn't touch the billboard, check if the shoot is perfect and assign 3 points, else 2
                    if (GameDataManager.IsPerfectShoot)
                    {
                        GameDataManager.Coins += 5;
                        GameDataManager.IsPerfectShoot = false;
                        points = 3 * bonus;
                        GameDataManager.Score += points;
                    }
                    else
                    {
                        GameDataManager.Coins += 3;
                        points = 2 * bonus;
                        GameDataManager.Score += points;
                    }
                }
                panelPoints.SetActive(true);

                //Show the points scored and change color if fireball is activated or not.
                if (GameDataManager.IsFireball)
                {
                    panelPoints.transform.GetChild(0).GetComponent<Text>().color = Color.red;
                }
                else
                {
                    panelPoints.transform.GetChild(0).GetComponent<Text>().color = new Color32(0,255,250,255);
                }
                panelPoints.transform.GetChild(0).GetComponent<Text>().text = "+"+points;
                scoreText.text = "" + GameDataManager.Score;
            }
        }

        //The ball has triggered an invisible net at the start of the basket hoop to check if the ball is entered
        if (other.CompareTag("CheckNet"))
        {
            isInNet = true;
        }
    }

    //Check if ball has collided with some objects of the scene.
    private void OnCollisionEnter(Collision collision)
    {
        sourceShoot.Stop();
        sourceShoot.enabled = false;

        //If ball has collided the billboard and it didn't enter in the hoop yet, set the touchBoard bool to true
        if (collision.gameObject.CompareTag("BillBoard"))
        {
            if (!isInNet)
            {
                sourceWood.enabled = true;
                sourceWood.Play();
                isTouchBoard = true;
            }
        }

        //Check if the ball has collided the iron of the basket hoop to start the relative sound.
        if(collision.gameObject.CompareTag("IronHoop"))
        {
            sourceIron.enabled = true;
            sourceIron.Play();
        }

        /*
        Check if the ball has collided the column of the basket to start the relative sound
        and to add force to the ball towards the camera.
        */
        if (collision.gameObject.CompareTag("Column"))
        {
            sourceWood.enabled = true;
            sourceWood.Play();
            rigidBody.AddForce(2*new Vector3(0, 5f, 5f));
        }

        /*
        Check if the ball has fallen on the pitch to start the relative sound
        and to add force to the ball to bounce.
        */
        if (collision.gameObject.CompareTag("Pitch"))
        {
            sourceBasketEffect.enabled = true;
            sourceBasketEffect.Play();
            rigidBody.AddForce(2 * new Vector3(0, 5f, 5f));
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
