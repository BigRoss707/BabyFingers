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
	private bool pauseCheck = false;
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
        if (dialogueEventActive & !pauseCheck)
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

		if (Input.GetKeyDown ("escape")) 
		{
			PauseEvent();
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

	public void PauseEvent()
	{
		if (pauseCheck == false)
		{
			pauseCheck = true;
		}

		else
		{
			pauseCheck = false;
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
        dialogues.Add(new Dialogue("Maybe I should build a golf range on the White House grounds so I can play more often without leaving the White House.", "That doesn't sound like a priority right now. Maybe focus that money into a program or something else.", "That's an excellent idea sir. Then you could avoid public scrutiny and play golf in peace.", "That’s a terrible idea, you’re the president not the CEO of Six Flags.", 2));
        dialogues.Add(new Dialogue("The Russian election is over. Maybe I should congratulate the winner…", "Of course sir, it could improve over sea relations. Great idea.", "You probably shouldn't, don't want people jumping to conclusions.", "That's an awful idea. Might as well send him some wine and hallmark card while your at it.", 1));
        dialogues.Add(new Dialogue("It's getting close to summer, I think i'm going get a tan.", "I don't think you really need to worry about that sir, i’d focus on your policies.", "You’re plenty “tan” already… maybe focus on actual problems you should be fixing.", "You will look amazing with a tan sir.", 3));
        dialogues.Add(new Dialogue("Can I get a snack bar next to my desk so I don't have to leave my office when i'm hungry?", "I don't really think that's a priority, you should be more focused on working and not snacking.", "Excellent idea, you would be able to focus more on work without leaving the office.", "Instead we should replace your desk with a treadmill. You could probably use that more.", 2));
        dialogues.Add(new Dialogue("Send someone to pick up a new Armani suit. I need to look good for this next press conference.", "Don't worry sir, you look fine in anything you wear.", "I can send someone but you need to focus on what your going to say during the conference.", "You really don't need another suit…", 1));
        dialogues.Add(new Dialogue("Who’s the greatest president in the history of America?", "Is this a test or…", "Clearly you are the best president sir.", "It’s hard to say sir we’ve had many great presidents, including you.", 2));
        dialogues.Add(new Dialogue("The White House is so old… I think i'm going to demolish it and make a new one in my own vision.", "You would anger a lot of people doing that, I would highly suggest reconsidering.", "You can't tear down a national monument…", "Of course sir, out with the old and in with the new.", 3));
        dialogues.Add(new Dialogue("Maybe I could get a pro golfer to teach me some moves. Improve my golf game.", "You don't need a pro to teach you how to play, you’re already amazing at golf.", "Or you could actually do your job and run the country.", "That's probably not the best use of your time or money sir.", 1));
        dialogues.Add(new Dialogue("I think I'm going to cut and dye my hair to make myself look younger.", "No amount of dye or any hair cut is going to fix what's going on there.", "It couldn't hurt.", "You’re perfect just the way you are sir.", 3));
    }

}
