using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour {

    #region PublicVariables
    public int strikes = 3;
    public static int start = 0;
    #endregion

    #region PrivateVariables

    private bool tweetEventActive = false;
    private bool dialogueEventActive = false;

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
		
	}
	
	// Update is called once per frame
	void Update () {
		if(start == 0)
        {
            TweetController tc;
            if(TweetController.TryGetManager(out tc))
            {
                tc.StartEvent();
            }
            DialogueController dc;
            if (DialogueController.TryGetManager(out dc))
            {
                dc.StartEvent();
            }
            start++;
        }
	}

    public void Strike()
    {
        Debug.Log("STRIKE IN EVENT CONTROLLER");
    }

}
