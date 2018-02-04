using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweetButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Yes()
    {
        TweetController tc;
        if(TweetController.TryGetManager(out tc))
        {
            tc.YesButton();
        }
    }

    public void No()
    {
        TweetController tc;
        if (TweetController.TryGetManager(out tc))
        {
            tc.NoButton();
        }
    }
}
