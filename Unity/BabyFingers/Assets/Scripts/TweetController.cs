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
        tweets.Add(new tweet("People think the wall is a bad idea. But why would it be bad when we don't have to pay for it? Mexico will make restitution for the illegal entry of thousands of Mexicans by paying for the wall. #buildthewall", new List<string> { "wall", "Mexico" }));
        tweets.Add(new tweet("I love minorities, but we need to keep lowlifes out of our country. #BorderSecurity ", new List<string> { "minority", "border" }));
        tweets.Add(new tweet("Jobs and wages on the rise and yet nobody seems to care. People whining on and on about Russia and the FBI! Get your priorities straight people! #MakingAmericaGreatAgain #Nobodyseemstocare", new List<string> { "Russia", "FBI" }));
        tweets.Add(new tweet("Played some golf yesterday. I had the highest score! I am the best at golf! #greatpresident #greatgolfer", new List<string> {  }));
        tweets.Add(new tweet("Even talk show hosts are siding with fake news outlets. Ignore their lies, down with the fake outlets!", new List<string> { "fake news" }));
        tweets.Add(new tweet("Guns aren’t dangerous, people are dangerous. I just thought of that myself! #deepthoughts #wisepresident", new List<string> { "guns" }));
        tweets.Add(new tweet("My nuke button is bigger than your nuke button. That means it has more nukes. Just give me a reason. #TwitchyFingers #NoMyFingersAren'tTiny", new List<string> { "nuke/nuclear" }));
        tweets.Add(new tweet("My approval rating is low? No, your approval rating is low! #FAKENEWS!", new List<string> { "fake news"}));
        tweets.Add(new tweet("I am the greatest great President that ever Presidented greatly #greatpresident", new List<string> {  }));
        tweets.Add(new tweet("The wall is the best idea any president has ever had because I am the best president there ever was.", new List<string> { "wall"}));
        tweets.Add(new tweet("Contrary to popular belief, I do not hate immigrants. I love all mexican food! What would America be like without Taco Bell!", new List<string> { "immigrant" }));
        tweets.Add(new tweet("Psst, fellow rich people, my tax reform bill is going to make you a lot of money. #Don'tTellPoorPeople #SHHH", new List<string> { "tax" }));
        tweets.Add(new tweet("The best thing to enter America from mexico is its food. Doritos locos tacos are amazing. Happy Cinco de Mayo! #tacos #cincodemayo", new List<string> { "Mexico" }));
        tweets.Add(new tweet("I think it's fair to say that anyone that doesn't agree with me is guilty of treason.", new List<string> { "treason" }));
        tweets.Add(new tweet("People are calling for the House of Representatives to impeach me, but guess what, I am the Representatives!", new List<string> { "impeach" }));
     
    }

    private void _init_possible_banned_words()
    {
        possibleBannedWords = new List<string>();
        possibleBannedWords.Add("wall");
        possibleBannedWords.Add("minority");
        possibleBannedWords.Add("border");
        possibleBannedWords.Add("Russia");
        possibleBannedWords.Add("FBI");
        possibleBannedWords.Add("fake news");
        possibleBannedWords.Add("guns");
        possibleBannedWords.Add("nuke/nuclear");
        possibleBannedWords.Add("immigrant");
        possibleBannedWords.Add("tax");
        possibleBannedWords.Add("Mexico");
        possibleBannedWords.Add("treason");
        possibleBannedWords.Add("impeach");
    }

}
