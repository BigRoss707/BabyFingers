using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
	#region PublicVariables
	public float maxTime = 10f;
	public float minTime = 5f;

	public long buttonEventCount = 0;
	public long buttonEventDifficultyFrequency = 2;

	public Text buttonText;
	public Text buttonTimerText;

    public Animator anim;
    public Animator bgAnim;
    public float animTime = 11.667f;
	#endregion

	#region PrivateVariables
	private bool buttonEventActive = false;
	private bool pauseCheck = false;
	private float buttonEventTimer = 0f;
	private float currentTime;

	private static ButtonController instance = null;
	#endregion

	public static bool TryGetManager(out ButtonController manager)
	{
		manager = instance;
		if (instance == null)
		{
			Debug.LogError("Trying to access ButtonController when no ButtonController is in the scene");
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
		currentTime = maxTime;
	}

	// Update is called once per frame
	void Update()
	{
		if (buttonEventActive & !pauseCheck)
		{
			buttonEventTimer += Time.deltaTime;
			buttonTimerText.text = (currentTime - buttonEventTimer).ToString("0.00");
			if (buttonEventTimer > currentTime)
			{
				//Event timeout state
				Debug.Log("The president launched a nuke, you've lost.");
				Lose();
				EndEvent();
			}
		}

		if (Input.GetKeyDown ("escape"))
		{
			PauseEvent();
		}
	}

	/// <summary>
	/// Called by Event Controller to start a new button event
	/// </summary>
	public void StartEvent()
	{
		//update difficulty
		buttonEventCount++;
		currentTime = maxTime - buttonEventCount / buttonEventDifficultyFrequency;
		if (currentTime < minTime)
		{
			currentTime = minTime;
		}
        // Animation Setup
        anim.speed = animTime / currentTime;
        ResetAnimations();
        anim.SetBool("Press", true);
        EventController ec;
        if(EventController.TryGetManager(out ec))
        {
            ec.Reach();
        }

		buttonEventTimer = 0f;
		buttonEventActive = true;

		//Set up associated GUI
		buttonText.text = "Swat the president's hand away from the nuke button!";
	}

	/// <summary>
	/// Called when the Event hits an end state, i.e timeout or response
	/// </summary>
	void EndEvent()
	{
		buttonEventTimer = 0f;
		buttonEventActive = false;

		buttonText.text = "...";
		buttonTimerText.text = "";

		EventController ec;
		if(EventController.TryGetManager(out ec))
		{
			ec.SetButtonEventInactive();
            ec.Idle(); //Reset bg animations
        }

        //Animation Reset
        ResetAnimations();
    }

	public void PauseEvent()
	{
		if (pauseCheck == false)
		{
            anim.enabled = false;
			pauseCheck = true;
		}

		else
		{
            anim.enabled = true;
			pauseCheck = false;
		}
	}

	public void ButtonClicked()
	{
		if (buttonEventActive)
		{
			EndEvent();
		}
	}

	/// <summary>
	/// Calls Strike in Event Controller denoting failure of event
	/// </summary>
	public void Lose()
	{
        EventController ec;
		if (EventController.TryGetManager(out ec))
		{
            ec.Idle();
            ec.Strike();
			ec.Strike();
			ec.Strike();
		}
	}

    public void ResetAnimations()
    {
        anim.SetBool("Press", false);
    }

}

