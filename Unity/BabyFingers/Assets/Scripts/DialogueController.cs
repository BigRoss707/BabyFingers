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
            dialogueTimerText.text = dialogueEventTimer.ToString("0.00");
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
    }

}
