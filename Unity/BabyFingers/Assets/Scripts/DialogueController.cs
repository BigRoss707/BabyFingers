using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    #region PublicVariables
    public float maxTime = 10f;
    public float minTime = 5f;

    public long dialogueEventCount = 0;
    public long dialogueEventDifficultyFrequency = 2;

    public Text dialogueText;
    public Text option1;
    public Text option2;
    public Text option3;
    public Text dialogueTimerText;

    public List<Dialogue> dialogues;

    public struct Dialogue
    {
        public string dialogue;
        public string opt1;
        public string opt2;
        public string opt3;
        public int correct_op;

        public Dialogue(string dialogue_in, string opt1_in, string opt2_in, string opt3_in, int correct_op_in)
        {
            dialogue = dialogue_in;
            opt1 = opt1_in;
            opt2 = opt2_in;
            opt3 = opt3_in;
            correct_op = correct_op_in;
        }
    }
    #endregion

    #region PrivateVariables
    private bool dialogueEventActive = false;
    private float dialogueEventTimer = 0f;
    private float currentTime;
    private int activeDialogue = 0;

    private static DialogueController instance = null;
    #endregion

    public static bool TryGetManager(out DialogueController manager)
    {
        manager = instance;
        if (instance == null)
        {
            Debug.LogError("Trying to access DialogueController when no DialogueController is in the scene");
        }

        return (instance != null);
    }

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        __init_dialogue();
        currentTime = maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogueEventActive)
        {
            dialogueEventTimer += Time.deltaTime;
            dialogueTimerText.text = (currentTime-dialogueEventTimer).ToString("0.00");
            if (dialogueEventTimer > currentTime)
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
        //update difficulty
        dialogueEventCount++;
        currentTime = maxTime - dialogueEventCount / dialogueEventDifficultyFrequency;
        if (currentTime < minTime)
        {
            currentTime = minTime;
        }

        dialogueEventTimer = 0f;
        dialogueEventActive = true;

        //Set up associated GUI
        int rand = Random.Range(0, dialogues.Count - 1);
        activeDialogue = rand;
        dialogueText.text = dialogues[rand].dialogue;
        option1.text = dialogues[rand].opt1;
        option2.text = dialogues[rand].opt2;
        option3.text = dialogues[rand].opt3;

    }

    /// <summary>
    /// Called when the Event hits an end state, i.e timeout or response
    /// </summary>
    void EndEvent()
    {
        dialogueEventTimer = 0f;
        dialogueEventActive = false;

        dialogueText.text = "...";
        option1.text = "";
        option2.text = "";
        option3.text = "";
        dialogueTimerText.text = "";

        EventController ec;
        if(EventController.TryGetManager(out ec))
        {
            ec.SetDialogueEventInactive();
        }
    }

    /// <summary>
    /// Map button response to correct response
    /// </summary>
    bool CorrectButton(int inputButton)
    {
        if(inputButton == dialogues[activeDialogue].correct_op)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Processes button 1 feedback for event
    /// </summary>
    public void DialogueButton(int buttonNum)
    {
        if (dialogueEventActive)
        {
            if (!CorrectButton(buttonNum))
            {
                Debug.Log("Failure, wrong dialogue choice");
                Strike();
            }
            EndEvent();
        }
    }

    /// <summary>
    /// Calls Strike in Event Controller denoting failure of event
    /// </summary>
    public void Strike()
    {
        EventController ec;
        if (EventController.TryGetManager(out ec))
        {
            ec.Strike();
        }
    }

    void __init_dialogue()
    {
        dialogues = new List<Dialogue>();
        dialogues.Add(new Dialogue("How's my hair?", "It looks like a dead animal.", "Amazing sir!", "Passable.", 2));
        dialogues.Add(new Dialogue("I'm going to cut funds from many smaller unnecessary programs like after school programs and NASA, which should help fund the wall.", "That sounds like a good idea but it's going to anger a lot of people.", "Great idea sir I'm sure people will understand.", "Thats a terrible way to fund the wall.", 2));
        dialogues.Add(new Dialogue("I scored a 73 on my last game of golf. Pretty good for a self taught golfer!", "I’d expect nothing less from the President of the United States.", "Wow, that’s pretty alright sir.", "You’re more likely to get impeached than score a 73 in golf.", 1));
        dialogues.Add(new Dialogue("My approval rating seems to be slowly dropping, but that can't be because i'm the best president ever! Why is my approval rating dropping?!", "Clearly they've lost their minds, you're an amazing president.", "No president is perfect, but you can do whatever you put your mind to.", "Best president? HA more like worst president on the planet.", 1));
        dialogues.Add(new Dialogue("I’m really craving some taco bell right now. Send someone to get me a Doritos locos taco. Maybe a churro bite too?", "It probably doesn't matter, just get what you want.", "Maybe skip the taco bell and go to the gym instead…", "Of course sir I'll have it here in 5 minutes.", 3));
        dialogues.Add(new Dialogue("I noticed you aren't sending all my tweets, why are you not sending my tweets?", "You sound like a raging twelve year old screaming at his mom because he hasn't had his snack.", "You probably shouldn't be tweeting some of these things, it's just to protect you.", "The world simply isn't ready for your brilliance.", 3));
        dialogues.Add(new Dialogue("I didn't think being a president would be this hard. I thought it was just telling people what to do and making them do everything!", "This is the presidency of the United States of America, not a free ride at your dad's company.", "It kind of is, but ultimately responsibility falls on you to make the right decisions.", "You handle it well sir. You’ve accomplished many great things in your short time as president.", 3));
        dialogues.Add(new Dialogue("I cant believe any country would threaten us with nukes! Id just press my button and wipe them off the map!", "Antagonizing world leaders will just get us all killed, please play nice.", "Our military and nuclear power is unrivaled, nobody can touch you sir!", "You should never take the threat of nuclear war lightly. There are no winners when the bombs begin to drop.", 2));
    }

}
