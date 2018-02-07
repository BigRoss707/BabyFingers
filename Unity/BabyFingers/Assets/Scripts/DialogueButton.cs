using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Option1()
    {
        DialogueController dc;
        if (DialogueController.TryGetManager(out dc))
        {
            dc.DialogueButton(1);
        }
    }

    public void Option2()
    {
        DialogueController dc;
        if (DialogueController.TryGetManager(out dc))
        {
            dc.DialogueButton(2);
        }
    }

    public void Option3()
    {
        DialogueController dc;
        if (DialogueController.TryGetManager(out dc))
        {
            dc.DialogueButton(3);
        }
    }

}
