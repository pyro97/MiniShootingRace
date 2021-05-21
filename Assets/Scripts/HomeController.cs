using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;


public class HomeController : MonoBehaviour
{
    AudioSource gameMusic, sourceClick;
    public AudioClip soundClick;
    private GameObject panelSettings,panelButtons,panelGame,panelWarning;
    PlayerPrefsHandler playerPrefsHandler;
    Toggle volumeToggle, effectsToggle;

    //dictionary contains couple of characther name and price name
    Dictionary<string,int> charachters = new Dictionary<string, int>{
        { "Basket Ball", 0 },{ "Bomb Ball",500 },{"Atom Ball",1000 }
    };

    public Sprite[] charactersImage=new Sprite[3];
    int indexCharacter;

    // Start is called before the first frame update
    void Awake()
    {
        playerPrefsHandler = gameObject.AddComponent<PlayerPrefsHandler>();

        //check if the first time and if true initialize all preferences
        if (playerPrefsHandler.IsFirstTime())
        {
            playerPrefsHandler.CreateFirstTimePref();
        }

        if (AudioListener.pause)
        {
            AudioListener.pause = false;
            AudioListener.volume = 1f;
        }

        gameMusic = this.GetComponent<AudioSource>();
        panelSettings = GameObject.Find("PanelSettings");
        panelButtons = GameObject.Find("PanelButtons");
        panelGame = GameObject.Find("PanelGame");
        panelWarning = GameObject.Find("PanelWarning");

        volumeToggle = panelSettings.transform.GetChild(0).GetChild(4).GetChild(1).GetComponent<Toggle>();
        effectsToggle = panelSettings.transform.GetChild(0).GetChild(5).GetChild(1).GetComponent<Toggle>();

        //Check if the user preferences have muted musics and if true the musics are muted
        if (!playerPrefsHandler.GetIsMutedMusic())
        {
            gameMusic.enabled = true;
            gameMusic.Play();
        }
        else
        {
            gameMusic.Stop();
            gameMusic.enabled = false;
        }

        //Check if the user preferences have muted effects and if true the effects are muted
        if (playerPrefsHandler.GetIsMutedEffects())
        {
            sourceClick = AddAudio(soundClick,false, false, 0f);
        }
        else
        {
            sourceClick = AddAudio(soundClick, false, false, 1f);
        }

        SetResponsiveCanvas();

    }

    void Start()
    {

        panelSettings.SetActive(false);
        panelGame.SetActive(false);
        panelWarning.SetActive(false);

        //add listeners to volume and effect toggle to listen the change about on/of of these buttons
        volumeToggle.onValueChanged.AddListener(CheckToggleVolume);
        effectsToggle.onValueChanged.AddListener(CheckToggleEffects);

        sourceClick.enabled = true;


    }

    //called when the play button is clicked and it loads the Game Scene
    public void PlayGame()
    {
        sourceClick.enabled = true;
        sourceClick.Play();
        SceneManager.LoadScene("GameScene");
    }

    //Handle Setting Menu when the user click on the setting button
    public void HandleSettings(string action)
    {
        sourceClick.Play();

        //Set all data in the setting menu based on the user preferences
        if (action.Equals("OpenSettings"))
        {
            EventSystem.current.SetSelectedGameObject(null);

            panelButtons.SetActive(false);
            panelSettings.SetActive(true);
            GameObject menuSettings = panelSettings.transform.GetChild(0).gameObject;
            menuSettings.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = playerPrefsHandler.GetNumMatch()+"";
            menuSettings.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = playerPrefsHandler.GetRecord() + "";
            menuSettings.transform.GetChild(3).GetChild(1).GetComponent<Text>().text = playerPrefsHandler.GetMoney() + "";

            ColorBlock cb = volumeToggle.colors;
            ColorBlock cb1 = effectsToggle.colors;

            if (playerPrefsHandler.GetIsMutedMusic())
            {
                cb.normalColor = Color.white;
                cb.highlightedColor = Color.white;
                cb.pressedColor = Color.white;
                volumeToggle.colors = cb;
            }
            else
            {
                cb.normalColor = Color.green;
                cb.highlightedColor = Color.green;
                cb.pressedColor = Color.green;
                volumeToggle.colors = cb;
            }

            if (playerPrefsHandler.GetIsMutedEffects())
            {
                cb1.normalColor = Color.white;
                cb1.highlightedColor = Color.white;
                cb1.pressedColor = Color.white;
                effectsToggle.colors = cb1;
            }
            else
            {
                cb1.normalColor = Color.green;
                cb1.highlightedColor = Color.green;
                cb1.pressedColor = Color.green;
                effectsToggle.colors = cb1;
            }


        }
        else if (action.Equals("CloseSettings"))
        {

            panelButtons.SetActive(true);
            panelSettings.SetActive(false);


        }
    }

    //called when there is a change of toggle volume
    public void CheckToggleVolume(bool isOn)
    {
        sourceClick.Play();
        
        EventSystem.current.SetSelectedGameObject(null);

        ColorBlock col = volumeToggle.colors;

        //if the toogle is set to on, the music preference is set to unmuted and the toggle color is set to green
        if (isOn)
        {
            playerPrefsHandler.SetMutedMusic(false);
            col.normalColor = Color.green;
            col.highlightedColor = Color.green;
            col.pressedColor = Color.green;
            volumeToggle.colors = col;
            gameMusic.enabled = true;
            gameMusic.Play();
        }

        //if the toogle is set to off, the music preference is set to muted and the toggle color is set to white
        else
        {
            playerPrefsHandler.SetMutedMusic(true);
            col.normalColor = Color.white;
            col.highlightedColor = Color.white;
            col.pressedColor = Color.white;
            volumeToggle.colors = col;
            gameMusic.Stop();
            gameMusic.enabled = false;
        }
    }

    //called when there is a change of toggle effect
    public void CheckToggleEffects(bool isOn)
    {
  
        sourceClick.Play();
        
        EventSystem.current.SetSelectedGameObject(null);

        ColorBlock col2 = effectsToggle.colors;

        //if the toogle is set to on, the effect preference is set to unmuted and the toggle color is set to green
        if (isOn)
        {
            playerPrefsHandler.SetMutedEffects(false);
            col2.normalColor = Color.green;
            col2.highlightedColor = Color.green;
            col2.pressedColor = Color.green;
            effectsToggle.colors = col2;
            sourceClick.volume = 1f;
        }

        //if the toogle is set to off, the effect preference is set to muted and the toggle color is set to white
        else
        {
            playerPrefsHandler.SetMutedEffects(true);
            col2.normalColor = Color.white;
            col2.highlightedColor = Color.white;
            col2.pressedColor = Color.white;
            effectsToggle.colors = col2;
            sourceClick.volume = 0f;

        }
    }

    //Handle the player menu selection
    public void HandleGameSelection(string action)
    {
        //if the menu is opened, all data are shown: user money,first ball character and the button to select or buy ball

        sourceClick.Play();
        Text money = panelGame.transform.GetChild(0).GetChild(3).GetChild(1).GetComponent<Text>();
        GameObject panelSelection = panelGame.transform.GetChild(0).GetChild(1).gameObject;
        if (action.Equals("OpenGame"))
        {
            panelGame.SetActive(true);
            money.text = playerPrefsHandler.GetMoney()+"";
            indexCharacter = 0;
            panelSelection.transform.GetChild(0).GetComponent<Text>().text = charachters.ElementAt(0).Key;//character name
            panelSelection.transform.GetChild(1).GetComponent<Image>().sprite = charactersImage[0];//image
            panelSelection.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);//arrow left
            panelSelection.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);//arrow right


            if (playerPrefsHandler.GetCharacter().Equals(charachters.ElementAt(0).Key)){
                panelSelection.transform.GetChild(3).GetComponent<Image>().color = new Color32(15, 120, 21, 255); ;
                panelSelection.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "Selected";
            }
            else
            {
                panelSelection.transform.GetChild(3).GetComponent<Image>().color = new Color32(249,218,69,255);
                panelSelection.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "Select";
            }

        }
        else if (action.Equals("CloseGame"))
        {
     
            panelGame.SetActive(false);

        }
    }

    //Handle panel game selection when user press the left arrow and calculate what character will be shown
    public void HandleLeftArrow()
    {
        sourceClick.Play();

        GameObject panelSelection = panelGame.transform.GetChild(0).GetChild(1).gameObject;
        indexCharacter -= 1;

        //if exist the previous character
        if (indexCharacter>0)
        {
            panelSelection.transform.GetChild(0).GetComponent<Text>().text = charachters.ElementAt(indexCharacter).Key;//character name
            panelSelection.transform.GetChild(1).GetComponent<Image>().sprite = charactersImage[indexCharacter];//image
            panelSelection.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);//arrow left
            panelSelection.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);//arrow right

            CheckSelection(charachters.ElementAt(indexCharacter).Key);
        }

        //you are at the index of the first character and so the left arrow is disabled
        else
        {
            panelSelection.transform.GetChild(0).GetComponent<Text>().text = charachters.ElementAt(indexCharacter).Key;//character name
            panelSelection.transform.GetChild(1).GetComponent<Image>().sprite = charactersImage[indexCharacter];//image
            panelSelection.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);//arrow left
            panelSelection.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);//arrow right

            CheckSelection(charachters.ElementAt(indexCharacter).Key);
        }
    }

    //Handle panel game selection when user press the right arrow and calculate what character will be shown
    public void HandleRightArrow()
    {
        sourceClick.Play();

        GameObject panelSelection = panelGame.transform.GetChild(0).GetChild(1).gameObject;
        indexCharacter += 1;

        //if exist the next character
        if (charachters.Count-1 > indexCharacter)
        {
            panelSelection.transform.GetChild(0).GetComponent<Text>().text = charachters.ElementAt(indexCharacter).Key;//character name
            panelSelection.transform.GetChild(1).GetComponent<Image>().sprite = charactersImage[indexCharacter];//image
            panelSelection.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);//arrow left
            panelSelection.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);//arrow right
            
            CheckSelection(charachters.ElementAt(indexCharacter).Key);
        }

        //you are at the index of the last character and so the right arrow is disabled
        else
        {
            panelSelection.transform.GetChild(0).GetComponent<Text>().text = charachters.ElementAt(indexCharacter).Key;//character name
            panelSelection.transform.GetChild(1).GetComponent<Image>().sprite = charactersImage[indexCharacter];//image
            panelSelection.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);//arrow left
            panelSelection.transform.GetChild(2).GetChild(1).gameObject.SetActive(false);//arrow right

            CheckSelection(charachters.ElementAt(indexCharacter).Key);
        }
    }

    //check if the corrent character is selected, can be selected or has to be purchased
    public void CheckSelection(string name)
    {

        GameObject panelSelection = panelGame.transform.GetChild(0).GetChild(1).gameObject;

        //if the character exists in the user key preferences so it is selected or it can be selected
        if (playerPrefsHandler.GetPlayerByName(name))
        {
            //if the character is the current, so it is selected
            if (playerPrefsHandler.GetCharacter().Equals(name))
            {
                panelSelection.transform.GetChild(3).GetComponent<Image>().color = new Color32(15, 120, 21, 255); ;
                panelSelection.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "Selected";
            }

            //if the character is not the current, so it can be selected
            else
            {
                panelSelection.transform.GetChild(3).GetComponent<Image>().color = new Color32(249, 218, 69, 255);
                panelSelection.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "Select";
            }

        }

        //if the character doesn't exist in the user key preferences so it has to be purchased
        else
        {
            panelSelection.transform.GetChild(3).GetComponent<Image>().color = new Color32(249, 218, 69, 255);
            panelSelection.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "Buy "+charachters[name];
        }
    }

    //check what is the next status of selection button after it has been clicked
    public void HandleButtonSelection()
    {
        sourceClick.Play();

        GameObject panelSelection = panelGame.transform.GetChild(0).GetChild(1).gameObject;
        Text money = panelGame.transform.GetChild(0).GetChild(3).GetChild(1).GetComponent<Text>();

        //character exists in the user key preferences and so the selection state button becomes "Selected"
        if (playerPrefsHandler.GetPlayerByName(charachters.ElementAt(indexCharacter).Key))
        {
            playerPrefsHandler.SetCharacter(charachters.ElementAt(indexCharacter).Key);
            panelSelection.transform.GetChild(3).GetComponent<Image>().color = new Color32(15, 120, 21, 255); ;
            panelSelection.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "Selected";

        }

        //character doesn't exist in the user key preferences and so check if the user has money
        else
        {
            //if user has the necessary money he can buy the character and the state of button becames "Select"
            if(playerPrefsHandler.GetMoney() >= charachters.ElementAt(indexCharacter).Value)
            {
                playerPrefsHandler.SetMoney(playerPrefsHandler.GetMoney() - charachters.ElementAt(indexCharacter).Value);
                money.text = playerPrefsHandler.GetMoney() + "";
                playerPrefsHandler.SetPlayerByName(charachters.ElementAt(indexCharacter).Key);
                panelSelection.transform.GetChild(3).GetComponent<Image>().color = new Color32(249, 218, 69, 255);
                panelSelection.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "Select";
            }

            //if user doesn't have necessary money and so the warning panel is shown
            else
            {
                panelWarning.SetActive(true);
            }
        }
    }

    //Close the warning panel
    public void CloseWarning()
    {
        sourceClick.Play();
        panelWarning.SetActive(false);

    }

    //Create new audio source and set the most important parameters
    public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake,float vol)
    {
        AudioSource newAudio = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        newAudio.clip = clip;
        newAudio.loop = loop;
        newAudio.enabled = enabled;
        newAudio.playOnAwake = playAwake;
        newAudio.enabled = false;
        newAudio.volume = vol;
        return newAudio;
    }

    //Set responsive based on various layouts of Android and Apple Devices
    public void SetResponsiveCanvas()
    {
        GameObject menuSettings = panelSettings.transform.GetChild(0).gameObject;
        GameObject menuSelection = panelGame.transform.GetChild(0).gameObject;
        GameObject menuWarning = panelWarning.transform.GetChild(0).gameObject;

        RectTransform rtSettings = menuSettings.gameObject.GetComponent<RectTransform>();
        RectTransform rtSelection = menuSelection.gameObject.GetComponent<RectTransform>();
        RectTransform rtWarning = menuWarning.gameObject.GetComponent<RectTransform>();

        if ((Screen.height > 2600 && Screen.height < 3000) && (Screen.width > 1100 && Screen.width < 1500))
        {

            rtSettings.offsetMax = new Vector2(rtSettings.offsetMax.x, -300);
            rtSelection.offsetMax = new Vector2(rtSelection.offsetMax.x, -300);
            rtWarning.offsetMax = new Vector2(rtWarning.offsetMax.x, -450);


        }
        else if ((Screen.height > 2500 && Screen.height < 2900) && (Screen.width > 1100 && Screen.width < 1500))
        {
            rtSettings.offsetMax = new Vector2(rtSettings.offsetMax.x, -250);
            rtSettings.offsetMin = new Vector2(rtSettings.offsetMin.x, 250);

            rtSelection.offsetMax = new Vector2(rtSelection.offsetMax.x, -250);
            rtSelection.offsetMin = new Vector2(rtSelection.offsetMin.x, 250);

            rtWarning.offsetMax = new Vector2(rtWarning.offsetMax.x, -400);
            rtWarning.offsetMin = new Vector2(rtWarning.offsetMin.x, 400);


        }
        else if ((Screen.height > 2100 && Screen.height < 2400) && (Screen.width > 1000 && Screen.width < 1500))
        {
            //2160x1080
            rtSettings.offsetMax = new Vector2(rtSettings.offsetMax.x, -300);
            rtSelection.offsetMax = new Vector2(rtSelection.offsetMax.x, -300);
            rtWarning.offsetMax = new Vector2(rtWarning.offsetMax.x, -450);

        }
        else if ((Screen.height > 1900 && Screen.height < 2100) && (Screen.width > 1000 && Screen.width < 1500))
        {

            rtSettings.offsetMax = new Vector2(rtSettings.offsetMax.x, -250);
            rtSettings.offsetMin = new Vector2(rtSettings.offsetMin.x, 250);

            rtSelection.offsetMax = new Vector2(rtSelection.offsetMax.x, -250);
            rtSelection.offsetMin = new Vector2(rtSelection.offsetMin.x, 250);

            rtWarning.offsetMax = new Vector2(rtWarning.offsetMax.x, -400);
            rtWarning.offsetMin = new Vector2(rtWarning.offsetMin.x, 400);


        }

        else if ((Screen.height > 1200 && Screen.height < 1500) && (Screen.width > 700 && Screen.width < 1000))
        {
            rtSettings.offsetMax = new Vector2(rtSettings.offsetMax.x, -250);
            rtSettings.offsetMin = new Vector2(rtSettings.offsetMin.x, 250);

            rtSelection.offsetMax = new Vector2(rtSelection.offsetMax.x, -250);
            rtSelection.offsetMin = new Vector2(rtSelection.offsetMin.x, 250);

            rtWarning.offsetMax = new Vector2(rtWarning.offsetMax.x, -400);
            rtWarning.offsetMin = new Vector2(rtWarning.offsetMin.x, 400);

        }

        else if ((Screen.height > 780 && Screen.height < 1200) && (Screen.width > 400 && Screen.width < 700))
        {
            rtSettings.offsetMax = new Vector2(rtSettings.offsetMax.x, -200);
            rtSettings.offsetMin = new Vector2(rtSettings.offsetMin.x, 200);

            if((Screen.height > 1000 && Screen.height < 1200) && (Screen.width > 600 && Screen.width < 700)){
                rtSelection.offsetMax = new Vector2(rtSelection.offsetMax.x, -300);

            }else
            {
                rtSelection.offsetMax = new Vector2(rtSelection.offsetMax.x, -200);
            }
            rtSelection.offsetMin = new Vector2(rtSelection.offsetMin.x, 200);

            rtWarning.offsetMax = new Vector2(rtWarning.offsetMax.x, -350);
            rtWarning.offsetMin = new Vector2(rtWarning.offsetMin.x, 350);

        }
        else if(Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
        {
            GameObject.Find("Canvas").GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            rtSelection.offsetMax = new Vector2(-200, rtSelection.offsetMax.y);
            rtSelection.offsetMin = new Vector2(200, rtSelection.offsetMin.y);

            rtWarning.offsetMax = new Vector2(-200, rtWarning.offsetMax.y);
            rtWarning.offsetMin = new Vector2(200, rtWarning.offsetMin.y);

            rtSettings.offsetMax = new Vector2(-200, rtSettings.offsetMax.y);
            rtSettings.offsetMin = new Vector2(200, rtSettings.offsetMin.y);

            rtSettings.offsetMax = new Vector2(rtSettings.offsetMax.x, -20);
            rtSettings.offsetMin = new Vector2(rtSettings.offsetMin.x, 20);

            rtSelection.offsetMax = new Vector2(rtSelection.offsetMax.x, -20);
            rtSelection.offsetMin = new Vector2(rtSelection.offsetMin.x, 20);
        }
    }
}
