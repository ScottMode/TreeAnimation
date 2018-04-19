using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.Analytics;

//	only add new ones to the end
public enum ModalState
{
	None = -1
}

public enum ScreenState
{
	None
}

public class MenuManager : Singleton<MenuManager>
{
	#region Fields
	//Dictionary to access each of the screens
	private Dictionary<ScreenState, UIScreen> screens = new Dictionary<ScreenState, UIScreen>();

	//The screen stack
	[SerializeField]
	List<ScreenState> screensStack = new List<ScreenState>();

	//Events
	public Dictionary<string, UnityEvent> eventDictionary = new Dictionary<string, UnityEvent>();
	#endregion

	#region Properties
	//Main
	public ScreenState currentState { get; private set; }

	//A reference to the canvas' rect transform
	public RectTransform CanvasRectTransform {get; private set;}
	#endregion

	//Never touch this
    protected MenuManager(){}

	void Awake()
	{
		// Adds screens not loaded from an external scene
		for(int i = 0; i < transform.childCount; ++i)
		{
			// Hide each screen
			transform.GetChild(i).gameObject.SetActive(false);

			//Get reference to the actual UI screen and add it to the dictionary
			UIScreen childScreen = transform.GetChild(i).GetComponent<UIScreen>();
			if(childScreen)
			{
				if(!screens.ContainsKey(childScreen.state))
				{
					screens.Add(childScreen.state, childScreen);
				}
				else
				{
					Debug.LogError("Duplicate screens: " + childScreen.state);
				}
			}
		}
	}
		
	void Start()
	{
		//Assign the canvas rect transform
		CanvasRectTransform = gameObject.GetComponent<RectTransform>();
	}

	#region Main Functions
	/// <summary>
	/// Adds a UI screen to the stack and deletes its scene
	/// </summary>
	/// <param name="screen">Screen.</param>
	public void AddScreen(UIScreen screen)
	{
		//Store a ref to the screen's go
		UnityEngine.SceneManagement.Scene rootScene = screen.gameObject.scene;

		//Set the screens transform to the main scene's canvas
		screen.transform.SetParent(transform);
		screen.transform.SetAsFirstSibling();
		//Make sure the screen is scaled and positioned correctly
		screen.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
		screen.gameObject.GetComponent<RectTransform>().sizeDelta = Vector2.one;
		screen.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f, 0.5f);
		screen.gameObject.GetComponent<RectTransform>().offsetMax = Vector2.zero;
		screen.gameObject.GetComponent<RectTransform>().offsetMin = Vector2.zero;

		//Hide the screen
		screen.gameObject.SetActive(false);

		//Add this screen to the screens dictionary
		if(!screens.ContainsKey(screen.state))
		{
			screens.Add(screen.state, screen);
		}
		else
		{
			Debug.LogError("Duplicate screens: " + screen.state);
		}

		//Remove the scene the screen came from
		LoadSceneManager.Instance.UnloadScene(rootScene);
	}

	/// <summary>
	/// Goes to a specific UI screen
	/// </summary>
	/// <param name="newScreen">New screen.</param>
	public void GoToScreen(string newScreen)
	{
		try
		{
			ScreenState newState = (ScreenState) Enum.Parse(typeof(ScreenState), newScreen, true);

			GoToScreen(newState);
		}
		catch (ArgumentException)
		{
			Debug.LogErrorFormat("{0} is not a memeber of the ScreenState enum", newScreen);
		}
	}
	public void GoToScreen(ScreenState newState)
	{
		//	Check if already on that screen and return
		if(newState == currentState)
		{
			Debug.LogWarning("Already on screen: " + newState);
			return;
		}

		//	The screen we are going to
		UIScreen outScreen;

		//	Check if have a connection to new screen
		if(!screens.TryGetValue(newState, out outScreen))
		{
			Debug.LogError("Screen not found: " + newState);
			return;
		}

		if(currentState != ScreenState.None)
		{
			//	Turn the current screen off
			screens[currentState].gameObject.SetActive(false);
		}

		//	Switch screens
		screens[newState].gameObject.SetActive(true);

		//	Add new screen to the stack
		/*if(!screensStack.Contains(newState))
		{
			screensStack.Add(newState);
		}*/

		currentState = newState;

		Analytics.CustomEvent("goToScreen", new Dictionary<string, object>
			{
				{"screen", currentState.ToString()}
			});
	}

	/// <summary>
	/// Returns a UI screen given the screen name. It's up to the user to type the screen
	/// </summary>
	/// <returns>The screen.</returns>
	/// <param name="screenName">Screen name.</param>
	public UIScreen GetScreen(ScreenState stateName)
	{
		if(screens.ContainsKey(stateName))
		{
			return screens[stateName];
		}
		else
		{
			Debug.LogError(stateName + " Screen was not found");
			return null;
		}
	}

	/// <summary>
	/// Enables a specific screen
	/// </summary>
	/// <param name="screenStr">Screen string.</param>
	public void EnableScreen(ScreenState state)
	{
		if (screens.ContainsKey(state) && screens[state].gameObject != null)
		{
			screens[state].gameObject.SetActive(true);
		}
		else
		{
			Debug.LogError(state + " is NULL");
		}
	}
	#endregion

}