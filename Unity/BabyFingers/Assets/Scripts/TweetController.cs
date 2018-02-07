using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweetController : MonoBehaviour {

    #region PublicVariables
    public float maxTime = 10f;
    public float minTime = 5f;

    public Text tweetText;
    public Text tweetTimerText;

    public struct tweet
    {
        public string text;
        public List<string> tags;

        public tweet(string text_in, List<string> tags_in)
        {
            text = text_in;
            tags = tags_in;
        }
    }

    public List<tweet> tweets;
    public List<string> bannedWords;
    #endregion

    #region PrivateVariables
    private bool tweetEventActive = false;
    private float tweetEventTimer = 0f;
    private float currentTime;
    private int activeTweet = 0;

    private static TweetController instance = null;
    #endregion

    public static bool TryGetManager(out TweetController manager)
    {
        manager = instance;
        if (instance == null)
        {
            Debug.LogError("Trying to access TweetController when no TweetController is in the scene");
        }

        return (instance != null);
    }

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        _init_tweets();
        currentTime = maxTime;
	}
	
	// Update is called once per frame
	void Update () {
		if(tweetEventActive)
        {
            tweetEventTimer += Time.deltaTime;
            tweetTimerText.text = tweetEventTimer.ToString();
            if(tweetEventTimer > currentTime)
            {
                //Event timeout state
                Strike();
                EndEvent();
            }
        }
	}

    /// <summary>
    /// Called by Event Controller to start a new tweet event
    /// </summary>
    public void StartEvent()
    {
        tweetEventTimer = 0f;
        tweetEventActive = true;

        int rand = Random.Range(0, tweets.Count - 1);
        activeTweet = rand;
        tweetText.text = tweets[rand].text;

    }

    /// <summary>
    /// Called when the Event hits an end state, i.e timeout or response
    /// </summary>
    void EndEvent()
    {
        tweetEventTimer = 0f;
        tweetEventActive = false;

        tweetText.text = "Processing...";
        tweetTimerText.text = "";
    }

    /// <summary>
    /// Processes yes button feedback for event
    /// </summary>
    public void YesButton()
    {
        if(tweetEventActive)
        {
            if (ContainsBannedWords())
            {
                Debug.Log("Failure, tweet contained banned words");
                Strike();
            }
            EndEvent();
        }
    }

    /// <summary>
    /// Processes no button feedback for event
    /// </summary>
    public void NoButton()
    {
        if(tweetEventActive)
        {
            if (!ContainsBannedWords())
            {
                Debug.Log("Failure, tweet did not contain banned words");
                Strike();
            }
            EndEvent();
        }
    }

    /// <summary>
    /// Checks to see if the active tweet contains banned words
    /// </summary>
    /// <returns>false if no banned words contained</returns>
    public bool ContainsBannedWords()
    {
        foreach(string word in bannedWords)
        {
            if(tweets[activeTweet].tags.Contains(word))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Calls Strike in Event Controller denoting failure of event
    /// </summary>
    public void Strike()
    {
        EventController ec;
        if(EventController.TryGetManager(out ec))
        {
            ec.Strike();
        }
    }

    /// <summary>
    /// Set up the tweets structure
    /// </summary>
    private void _init_tweets()
    {
        tweets = new List<tweet>();
        tweets.Add(new tweet("This is a test tweet, banned words in this tweet are tweet", new List<string> { "tweet", "newtag" }));
    }

}
