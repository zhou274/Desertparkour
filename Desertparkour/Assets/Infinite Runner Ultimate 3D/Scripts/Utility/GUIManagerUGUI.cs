
/* 
Infinite Runner Ultimate Presented by Black Gear Studio ©
         Programmed by Subhojeet Pramanik

This script Manages the GUI
A sample script on carrying basic functions like saving and loading of scores, updating the main menu GUI , changing graphics and volume,

*/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using TTSDK;
using StarkSDKSpace;
using System.Collections.Generic;
using TTSDK.UNBridgeLib.LitJson;
public class GUIManagerUGUI : MonoBehaviour
{
	[HideInInspector]
	public bool GameStarted = false; //Whether Game has started
	public GameObject PlayerCamera; //The main Player Camera
	GameObject Player;
	PlayerControls p; //The Main Player PlayerControls
	bool movedtopos; //Moved to the end of transition position
	public GameObject MainMenuCamera; //The main camera in main menu
	public GameObject MainMenuGUI; //The GUI camera in main menu
	public GameObject PlayTimeGUI; //The Play time GUI camera
	private bool startPlayTransition;
	public float PlayTransitionSpeed = 5f;
	public AudioSource Music;  //The music AudioSource

	public Toggle ControlsTypeToggle; //Toggle that changes Player control in settings
	public GameObject PauseGUI;   //The Pause and score GUI parent
	public GameObject PauseMenuGUI;  //The Pause menu GUI parent
	public Slider MusicSlider;
	public Slider GraphicsSlider;
	public Toggle Fog;
	public GameObject DeathGUI;
	public TextMeshProUGUI HighScoreCoin;
	public TextMeshProUGUI HighScoreScore;
	public TextMeshProUGUI HighScoreDistance;
	public StoreManager Store;
	public GameObject RevivalButton; //This GameObject is deacativated if revival is not bought in store.
	public Text RevivalText;//The text that displays number of revivals available
	public GameObject InvincibleObj;
	public GameObject AdPanel;
	public static bool canInvincible = false;

	void Start()
	{
		startPlayTransition = false;
		GameStarted = false;
		Player = GameObject.FindGameObjectWithTag("Player");
		p = Player.GetComponent<PlayerControls>();
		movedtopos = false;
		if (PlayerPrefs.GetFloat("FirstTime") == 0)
		{ //If this is the first time the game is opened
			GamePreferencesCreate(); //Create the preferences
		}
		else
		{
			GamePreferencesReload(); //Else reload the saved preferences
		}

		HighScoreCoin.text = PlayerPrefs.GetFloat("Coin").ToString();
		HighScoreScore.text = PlayerPrefs.GetFloat("Score").ToString();
		//HighScoreDistance.text=PlayerPrefs.GetFloat("Distance").ToString();
	}

	public void PreStart()
	{
		AdPanel.SetActive(true);
	}

	public void OnAdClickPlay(bool a)
	{
		if (a)
		{
            ShowVideoAd("3f28gm4729nf1p30ft",
            (bol) => {
                if (bol)
                {

                    canInvincible = true;
                    AdPanel.SetActive(false);
                    hitMainMenuPlay();

                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
		}
		else
		{
            AdPanel.SetActive(false);
            hitMainMenuPlay();
        }


	}



	// Update is called once per frame
	public void hitMainMenuPlay()
	{
		startPlayTransition = true;
	}
	public void hitGamePlayPause()
	{
		if (GameStarted == true)
		{
			p.CurrentGameState = PlayerControls.GameState.Pause;
			PauseGUI.SetActive(false);
			PauseMenuGUI.SetActive(true);
			ShowInterstitialAd("54houj3lpu07e3o33v",
() =>
{
	Debug.LogError("--插屏广告完成--");

},
(it, str) =>
{
	Debug.LogError("Error->" + str);
});
		}
	}
	public void hitGamePlayResume()
	{

		ShowVideoAd("2s1kdjob7r35h9ib7h",
			(bol) =>
			{
				if (bol)
				{
					p.CurrentGameState = PlayerControls.GameState.Playing;
					PauseGUI.SetActive(true);
					PauseMenuGUI.SetActive(false);



					clickid = "";
					getClickid();
					apiSend("game_addiction", clickid);
					apiSend("lt_roi", clickid);


				}
				else
				{
					StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
				}
			},
			(it, str) =>
			{
				Debug.LogError("Error->" + str);
				//AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
			});


	}
	public void hitGamePlayMainMenu()
	{
		canInvincible = false;
		Application.LoadLevel(1);


	}

	public static string clickid;
	private static StarkAdManager starkAdManager;

	public static void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
	{
		starkAdManager = StarkSDK.API.GetStarkAdManager();
		if (starkAdManager != null)
		{
			var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
			mInterstitialAd.Load();
			mInterstitialAd.Show();
		}
	}

	/// <summary>
	/// </summary>
	/// <param name="adId"></param>
	/// <param name="closeCallBack"></param>
	/// <param name="errorCallBack"></param>
	public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
	{
		starkAdManager = StarkSDK.API.GetStarkAdManager();
		if (starkAdManager != null)
		{
			starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
		}
	}

	public void getClickid()
	{
		var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
		if (launchOpt.Query != null)
		{
			foreach (KeyValuePair<string, string> kv in launchOpt.Query)
				if (kv.Value != null)
				{
					Debug.Log(kv.Key + "<-参数-> " + kv.Value);
					if (kv.Key.ToString() == "clickid")
					{
						clickid = kv.Value.ToString();
					}
				}
				else
				{
					Debug.Log(kv.Key + "<-参数-> " + "null ");
				}
		}
	}

	public void apiSend(string eventname, string clickid)
	{
		TTRequest.InnerOptions options = new TTRequest.InnerOptions();
		options.Header["content-type"] = "application/json";
		options.Method = "POST";

		JsonData data1 = new JsonData();

		data1["event_type"] = eventname;
		data1["context"] = new JsonData();
		data1["context"]["ad"] = new JsonData();
		data1["context"]["ad"]["callback"] = clickid;

		Debug.Log("<-data1-> " + data1.ToJson());

		options.Data = data1.ToJson();

		TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
		   response => { Debug.Log(response); },
		   response => { Debug.Log(response); });
	}
	public void hitMainMenuExit()
	{
		Application.Quit();
	}
	void Update()
	{
		if (Store)   //Sample store codes to check whether item is bought and if so carrying out desired actions
		{
			if (Store.ExhaustibleUnitsBought(100) > 0)//Check if revival is bought in store. 100 is the item id for revival in store. It is an exhaustible item
			{
				RevivalButton.SetActive(true); //If bought activate revival button. Revival buttton is shown when player dies
				RevivalText.text = Store.ExhaustibleUnitsBought(100).ToString() + " revivals available";


			}
			else
			{
				RevivalButton.SetActive(false);
			}
		}
		if (startPlayTransition == true)
		{//Start Play transition animation
			if (GameStarted == false)
			{
				MainMenuCamera.transform.parent = null;
				MainMenuCamera.transform.position = Vector3.MoveTowards(MainMenuCamera.transform.position, PlayerCamera.transform.position, PlayTransitionSpeed * Time.deltaTime);//Camera move animation
				MainMenuCamera.transform.rotation = Quaternion.Slerp(MainMenuCamera.transform.rotation, PlayerCamera.transform.rotation, PlayTransitionSpeed * Time.deltaTime);
				MainMenuGUI.SetActive(false);

				if (Vector3.Distance(MainMenuCamera.transform.position, PlayerCamera.transform.position) < 0.1f)
				{

					Destroy(MainMenuCamera);
					PlayerCamera.SetActive(true);
					p.CurrentGameState = PlayerControls.GameState.Playing;
					GameStarted = true;
					PlayTimeGUI.SetActive(true);
					if (canInvincible == true)
					{
						InvincibleObj.SetActive(true);
					}
					else
					{
						InvincibleObj.SetActive(false);
					}
					startPlayTransition = false;
				}
			}
		}

		if (ControlsTypeToggle.isOn == true)
		{//Check change of controls in settings
			p.TrackType = PlayerControls.TrackTypeEnum.ThreeSlotTrack;
			PlayerPrefs.SetInt("ControlType", 1);
		}
		else
		{
			p.TrackType = PlayerControls.TrackTypeEnum.FreeHorizontalMovement;
			PlayerPrefs.SetInt("ControlType", 0);
		}
		//Graphics changing code
		if (Fog.isOn == true)
		{
			RenderSettings.fog = true;
			PlayerPrefs.SetInt("Fog", 1);
		}
		else
		{
			RenderSettings.fog = false;
			PlayerPrefs.SetInt("Fog", 0);
		}

		PlayerPrefs.SetFloat("Graphics", GraphicsSlider.value);
		if (GraphicsSlider.value == 0)
			QualitySettings.SetQualityLevel(1);

		else if (GraphicsSlider.value == 1)
			QualitySettings.SetQualityLevel(2);
		else if (GraphicsSlider.value == 2)
			QualitySettings.SetQualityLevel(3);
		else if (GraphicsSlider.value == 3)
			QualitySettings.SetQualityLevel(4);
		else if (GraphicsSlider.value == 4)
			QualitySettings.SetQualityLevel(5);
		else
			QualitySettings.SetQualityLevel(6);
		//end Graphics changing code
		Music.volume = MusicSlider.value;
		PlayerPrefs.SetFloat("Music", MusicSlider.value);


		if (GameStarted == true && p.CurrentGameState == PlayerControls.GameState.Dead)
		{
			PauseGUI.SetActive(false);
			DeathGUI.SetActive(true);
            canInvincible = false;
            InvincibleObj.SetActive(false);
            if (canInvincible == true)
            {
                InvincibleObj.SetActive(true);
            }
            else
            {
                InvincibleObj.SetActive(false);
            }

        }
		if (GameStarted == true && p.CurrentGameState == PlayerControls.GameState.Playing)
		{
			if (PauseGUI.activeInHierarchy == false)
				PauseGUI.SetActive(true);
            if (canInvincible == true)
            {
                InvincibleObj.SetActive(true);
            }
            else
            {
                InvincibleObj.SetActive(false);
            }
        }
	}



	void GamePreferencesReload()
	{
		if (PlayerPrefs.GetInt("ControlType") == 1)
		{
			p.TrackType = PlayerControls.TrackTypeEnum.ThreeSlotTrack;
			ControlsTypeToggle.isOn = true;
		}
		else
		{
			p.TrackType = PlayerControls.TrackTypeEnum.FreeHorizontalMovement;
			ControlsTypeToggle.isOn = false;
		}
		float value = PlayerPrefs.GetFloat("Graphics");
		GraphicsSlider.value = value;
		if (value == 0)
			QualitySettings.SetQualityLevel(1);

		else if (value == 1)
			QualitySettings.SetQualityLevel(2);
		else if (value == 2)
			QualitySettings.SetQualityLevel(3);
		else if (value == 3)
			QualitySettings.SetQualityLevel(4);
		else if (value == 4)
			QualitySettings.SetQualityLevel(5);
		else
			QualitySettings.SetQualityLevel(6);

		if (PlayerPrefs.GetInt("Fog") == 1)
		{
			RenderSettings.fog = true;
			Fog.isOn = true;

		}
		else
		{
			RenderSettings.fog = false;
			Fog.isOn = false;

		}

		Music.volume = PlayerPrefs.GetFloat("Music");
		MusicSlider.value = Music.volume;


	}
	void GamePreferencesCreate()
	{ //save default Game Preferences. Change default preferences from here
		PlayerPrefs.SetFloat("Graphics", 0.4f);
		PlayerPrefs.SetFloat("Music", 1f);
		PlayerPrefs.SetInt("Fog", 0);
		PlayerPrefs.SetInt("Effects", 0);
		PlayerPrefs.SetFloat("FirstTime", 1);
		PlayerPrefs.SetInt("ControlType", 0);//0 stands for free movement and 1 stands for swipe based similar to subway surfer
	}


}
