using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweetController : MonoBehaviour {

    #region PublicVariables
    public double timeSinceLastBannedChange = 0;
    public double bannedWordChangeFrequency = 15;

    public float maxTime = 10f;
    public float minTime = 5f;

    public long tweetEventCount = 0;
    public long tweetEventDifficultyFrequency = 2;

    public Text tweetText;
    public Text tweetTimerText;

    public Text bannedWord1;
    public Text bannedWord2;
    public Text bannedWord3;
    public Text bannedWord4;
    public Text bannedWord5;

    public int maxBannedWords = 5;

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
    public List<string> possibleBannedWords;
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
        _init_possible_banned_words();
        currentTime = maxTime;

        //Randomize Banned List
        PermuteBannedList();
        //UpdateBannedList(); Should be unecessary
    }
	
	// Update is called once per frame
	void Update () {
        //Update timers
        timeSinceLastBannedChange += Time.deltaTime;

		if(tweetEventActive)
        {
            tweetEventTimer += Time.deltaTime;
            tweetTimerText.text = tweetEventTimer.ToString("0.00");
            if(tweetEventTimer > currentTime)
            {
                //Event timeout state
                Strike();
                EndEvent();
            }
        }
        else
        {
            //Change banned words
            if (timeSinceLastBannedChange > bannedWordChangeFrequency)
            {
                timeSinceLastBannedChange = 0;
                int rand = Random.Range(0, 3);
                bool notChanged = false;
                switch (rand)
                {
                    case 0:
                        PermuteBannedList();
                        break;
                    case 1:
                        notChanged = AddBannedList();
                        break;
                    case 2:
                        ScrambleBannedList();
                        break;
                }
                if(notChanged)
                {
                    PermuteBannedList();
                }
                //UpdateBannedList(); Should be unecessary
            }
        }
    }

    /// <summary>
    /// Called by Event Controller to start a new tweet event
    /// </summary>
    public void StartEvent()
    {
        //update difficulty
        tweetEventCount++;
        currentTime = maxTime - tweetEventCount / tweetEventDifficultyFrequency;
        if(currentTime < minTime)
        {
            currentTime = minTime;
        }

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

        EventController ec;
        if (EventController.TryGetManager(out ec))
        {
            ec.SetTweetEventInactive();
        }
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
    /// Changes/Adds a single value to the banned words list
    /// </summary>
    public void PermuteBannedList()
    {
        int indexToRemove = Random.Range(0, bannedWords.Count);
        bannedWords.RemoveAt(indexToRemove);
        int indexToAdd = Random.Range(0, possibleBannedWords.Count);
        while(indexToAdd < possibleBannedWords.Count)
        {
            if(!bannedWords.Contains(possibleBannedWords[indexToAdd]))
            {
                bannedWords.Add(possibleBannedWords[indexToAdd]);
                break;
            }
            indexToAdd++;
        }
        UpdateBannedList();
    }

    /// <summary>
    /// Adds to the banned list or scrambles the list if it is unable to add
    /// </summary>
    public bool AddBannedList()
    {
        bool wordAdded = false;
        if(bannedWords.Count < maxBannedWords)
        {
            int indexToAdd = Random.Range(0, possibleBannedWords.Count);
            while (indexToAdd < possibleBannedWords.Count)
            {
                if (!bannedWords.Contains(possibleBannedWords[indexToAdd]))
                {
                    wordAdded = true;
                    bannedWords.Add(possibleBannedWords[indexToAdd]);
                    break;
                }
                indexToAdd++;
            }
        }
        UpdateBannedList();
        return wordAdded;
    }

    /// <summary>
    /// Scrambles the list of banned words
    /// </summary>
    public void ScrambleBannedList()
    {
        //Randomly re-add banned words equal to the number of words previously in the banned list
        int bannedWordCount = bannedWords.Count;
        bannedWords = new List<string>();
        for(int i = 0; i < bannedWordCount; i++)
        {
            AddBannedList();
        }
        //If any adds fail iterate over the banned list adding words
        //Should only occur due to bad repetitive rng or overly small banned word list
        for (int i = 0; i < possibleBannedWords.Count; i++)
        {
            //if there aren't as many banned words as there should be
            if (bannedWords.Count < bannedWordCount)
            {
                if (!bannedWords.Contains(possibleBannedWords[i]))
                {
                    bannedWords.Add(possibleBannedWords[i]);
                }
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// Updates the banned word list UI to reflect the current lits of banned words
    /// </summary>
    public void UpdateBannedList()
    {
        bannedWord1.text = "";
        bannedWord2.text = "";
        bannedWord3.text = "";
        bannedWord4.text = "";
        bannedWord5.text = "";
        for (int i = 0; i < bannedWords.Count; i++)
        {
            if(i == 0)
            {
                bannedWord1.text = bannedWords[i];
            }
            else if(i == 1)
            {
                bannedWord2.text = bannedWords[i];
            }
            else if(i == 2)
            {
                bannedWord3.text = bannedWords[i];
            }
            else if(i == 3)
            {
                bannedWord4.text = bannedWords[i];
            }
            else if(i == 4)
            {
                bannedWord5.text = bannedWords[i];
            }
            else
            {
                Debug.Log("Banned Words Exceed Max Allowed");
                break;
            }
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

    private void _init_possible_banned_words()
    {
        possibleBannedWords = new List<string>();
        possibleBannedWords.Add("tweet");
        possibleBannedWords.Add("not tweet");
    }

}
