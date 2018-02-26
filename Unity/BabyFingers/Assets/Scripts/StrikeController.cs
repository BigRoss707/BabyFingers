using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrikeController : MonoBehaviour {

    public Toggle strike1;
    public Toggle strike2;
    public Toggle strike3;

    public int strikesActive = 0;

    #region PrivateVariables
    private static StrikeController instance = null;
    #endregion

    public static bool TryGetManager(out StrikeController manager)
    {
        manager = instance;
        if (instance == null)
        {
            Debug.LogError("Trying to access StrikeController when no StrikeController is in the scene");
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
		
	}

    public void IncrementStrike()
    {
        strikesActive++;
        UpdateStrikes();
    }

    void UpdateStrikes()
    {
        if(strikesActive == 0)
        {
            strike1.isOn = false;
            strike2.isOn = false;
            strike3.isOn = false;
        }
        if (strikesActive == 1)
        {
            strike1.isOn = true;
            strike2.isOn = false;
            strike3.isOn = false;
        }
        if (strikesActive == 2)
        {
            strike1.isOn = true;
            strike2.isOn = true;
            strike3.isOn = false;
        }
        if (strikesActive >= 3)
        {
            strike1.isOn = true;
            strike2.isOn = true;
            strike3.isOn = true;
        }
    }
}
