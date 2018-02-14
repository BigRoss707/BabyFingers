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
    public double timeSinceDifficultyIncrease = 0;

    [Tooltip("Frequencies")]
    public double difficultyIncreaseFrequency = 30;
    public double eventFrequency = 10;
    public double minEventFrequency = 2;

    #endregion

    #region PrivateVariables
    //End State Bool
    private bool gameInProgress = true;

    //Event activity trackers
    private bool tweetEventActive = false;
    private bool dialogueEventActive = false;

    //Event timers
    private float tweetEventTimer = 0f;
    private float dialogueEventTimer = 0f;

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
            timeSinceDifficultyIncrease += (double)Time.deltaTime;

            //Get harder periodically and change score modifier
            if (timeSinceDifficultyIncrease > difficultyIncreaseFrequency)
            {
                timeSinceDifficultyIncrease = 0;
                eventFrequency--;
                scoreMultiplier++;
                if (eventFrequency < minEventFrequency)
                {
                    eventFrequency = minEventFrequency;
                }
            }

            //Start Events
            if (timeSinceLastEvent > eventFrequency)
            {
                timeSinceLastEvent = 0;
                int randInt = Random.Range(0, 2);
                if ((randInt == 0) && !dialogueEventActive)
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
            }
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
    }

    public void EndGame()
    {
        gameInProgress = false;
    }

    public void SetDialogueEventInactive()
    {
        dialogueEventActive = false;
    }

    public void SetTweetEventInactive()
    {
        tweetEventActive = false;
    }

}
