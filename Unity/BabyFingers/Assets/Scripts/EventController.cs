using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour {

    #region PublicVariables
    [Tooltip("Strikes that determine fail condition")]
    public int currentStrikes = 0;
    public int maxStrikes = 3;

    [Tooltip("Debug")]
    public static int start = 0;

    [Tooltip("Score")]
    public double score = 0;
    public double scoreMultiplier = 1;

    [Tooltip("Timers")]
    public double totalTime = 0;
    public double timeSinceLastEvent = 0;
    public double timeSinceLastSecondaryEvent = 0;
    public double timeSinceDifficultyIncrease = 0;

    [Tooltip("Frequencies")]
    public double difficultyIncreaseFrequency = 30;
    public double eventFrequency = 10;
    public double minEventFrequency = 2;
    public double altEventFrequency = 20;
    public double minAltEventFrequency = 5;

    [Tooltip("GameOver")]
    public GameObject gameOverScreen;

    #endregion

    #region PrivateVariables
    //End State Bool
    private bool gameInProgress = true;

    //Event activity trackers
    private bool tweetEventActive = false;
    private bool dialogueEventActive = false;
	private bool buttonEventActive = false;

    //Event timers
    private float tweetEventTimer = 0f;
    private float dialogueEventTimer = 0f;
	private float buttonEventTimer = 0f;

    private static EventController instance = null;
    #endregion

    public static bool TryGetManager(out EventController manager)
    {
        manager = instance;
        if (instance == null)
        {
            Debug.LogError("Trying to access EventController when no EventController is in the scene");
        }

        return (instance != null);
    }

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        timeSinceLastEvent = 1000;
	}
	
	// Update is called once per frame
	void Update () {
        if(gameInProgress)
        {
            //Update timers
            score += (double)Time.deltaTime * scoreMultiplier;
            totalTime += (double)Time.deltaTime;
            timeSinceLastEvent += (double)Time.deltaTime;
            timeSinceLastSecondaryEvent += (double)Time.deltaTime;
            timeSinceDifficultyIncrease += (double)Time.deltaTime;

            //Get harder periodically and change score modifier
            if (timeSinceDifficultyIncrease > difficultyIncreaseFrequency)
            {
                timeSinceDifficultyIncrease = 0;
                eventFrequency--;
                altEventFrequency--;
                scoreMultiplier++;
                if (eventFrequency < minEventFrequency)
                {
                    eventFrequency = minEventFrequency;
                }
                if(altEventFrequency < minAltEventFrequency)
                {
                    altEventFrequency = minAltEventFrequency;
                }
            }

            StartPrimaryEvent();
            StartSecondaryEvent();
           
            //End Game If Strikes Alloted Are Exceeded
            if(currentStrikes >= maxStrikes)
            {
                EndGame();
            }
        }
	}

    public void Strike()
    {
        Debug.Log("STRIKE IN EVENT CONTROLLER");
        currentStrikes++;
        StrikeController sc;
        if(StrikeController.TryGetManager(out sc))
        {
            sc.IncrementStrike();
        }
    }

    public void EndGame()
    {
        Debug.Log("GameOver");
        gameInProgress = false;
        if(gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
    }

    public void SetDialogueEventInactive()
    {
        dialogueEventActive = false;
    }

    public void SetTweetEventInactive()
    {
        tweetEventActive = false;
    }

	public void SetButtonEventInactive()
	{
		buttonEventActive = false;
	}

    /// <summary>
    /// Starts events associated with the primary event clock and frequency
    /// </summary>
    private void StartPrimaryEvent()
    {
        //Start Events
        if (timeSinceLastEvent > eventFrequency)
        {
            timeSinceLastEvent = 0;
            int randInt = Random.Range(0, 5);
			if ((randInt >= 0 && randInt <= 1) && !dialogueEventActive)
			{
				DialogueController dc;
				if (DialogueController.TryGetManager (out dc))
				{
					dc.StartEvent ();
					dialogueEventActive = true;
				}
			} 
			else if ((randInt >= 2 && randInt <= 3) && !tweetEventActive) 
			{
				TweetController tc;
				if (TweetController.TryGetManager (out tc))
				{
					tc.StartEvent ();
					tweetEventActive = true;
				}
			}
			else if (!buttonEventActive)
			{
				ButtonController bc;
				if (ButtonController.TryGetManager (out bc)) 
				{
					bc.StartEvent ();
					buttonEventActive = true;
				}
			}
				
        }
    }

    /// <summary>
    /// Starts events associated with the secondary event clock and frequency
    /// </summary>
    private void StartSecondaryEvent()
    {
        //Start Events
        if (timeSinceLastSecondaryEvent > altEventFrequency)
        {
            timeSinceLastSecondaryEvent = 0;
			//Currently, there's a 40% chance of dialogue, 40% chance of tweets, and 20% chance of the nuke button
            int randInt = Random.Range(0, 5);
            if ((randInt >= 0 && randInt <= 2) && !buttonEventActive)
            {
                ButtonController bc;
                if (ButtonController.TryGetManager(out bc))
                {
                    bc.StartEvent();
                    buttonEventActive = true;
                }
            }
            else if ((randInt == 3) && !dialogueEventActive)
            {
                DialogueController dc;
                if (DialogueController.TryGetManager(out dc))
                {
                    dc.StartEvent();
                    dialogueEventActive = true;
                }
            }
			else if (!tweetEventActive)
            {
                TweetController tc;
                if (TweetController.TryGetManager(out tc))
                {
                    tc.StartEvent();
                    tweetEventActive = true;
                }
            }
			

            //Debug.Log("SPAWNING SECONDARY EVENT");
        }
    }

}
