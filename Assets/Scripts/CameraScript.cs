using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private GameObject player;        
    private Vector3 offset;
    private PlayerPrefsHandler playerPrefs;

    // Start is called before the first frame update
    void Start()
    {
        playerPrefs = gameObject.AddComponent<PlayerPrefsHandler>();

        //Main Camera follows selected player by user
        player = GameObject.Find(playerPrefs.GetCharacter());

        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        if (!GameDataManager.IsCameraFollow)
        {
            Vector3 nuovo= player.transform.position + offset;
            transform.position = new Vector3(player.transform.position.x, nuovo.y, nuovo.z);

        }
    }
}
