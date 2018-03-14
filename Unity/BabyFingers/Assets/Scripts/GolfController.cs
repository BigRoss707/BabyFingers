﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfController : MonoBehaviour {

    #region PUBLIC VARIABLES
    public GameObject dragPrefab;
    public GameObject goal;
    public GameObject goalSlot;
    public GameObject start;
    public GameObject startSlot;
    public GameObject draggable;

    public float maxTime = 10f;
    public float minTime = 5f;

    public long golfEventCount = 0;
    public long golfEventDifficultyFrequency = 2;
    #endregion

    #region PRIVATE VARIABLES
    private bool golfEventActive = false;
    private float golfEventTimer = 0f;
    private float currentTime;
    private static GolfController instance = null;
    #endregion

    public static bool TryGetManager(out GolfController manager)
    {
        manager = instance;
        if (instance == null)
        {
            Debug.LogError("Trying to access GolfController when no GolfController is in the scene");
        }

        return (instance != null);
    }

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        currentTime = maxTime;
	}
	
	// Update is called once per frame
	void Update () {
	    if(golfEventActive)
        {
            golfEventTimer += Time.deltaTime;
            //dialogueTimerText.text = (currentTime - dialogueEventTimer).ToString("0.00");
            if (CheckGoalHasChild())
            {
                EndEvent();
                return;
            }
            if(golfEventTimer > currentTime)
            {
                Strike();
                EndEvent();
                return;
            }
        }
	}

    public void StartEvent()
    {
        //update difficulty
        golfEventCount++;
        currentTime = maxTime - golfEventCount / golfEventDifficultyFrequency;
        if (currentTime < minTime)
        {
            currentTime = minTime;
        }

        RandomizeStartLocation();
        golfEventTimer = 0f;
        golfEventActive = true;
    }

    public void EndEvent()
    {
        golfEventTimer = 0f;
        golfEventActive = false;

        ReparentDraggableToStart();

        EventController ec;
        if (EventController.TryGetManager(out ec))
        {
            ec.SetGolfEventInactive();
        }
    }

    public bool CheckGoalHasChild()
    {
        if(goalSlot.transform.childCount > 0)
        {
            return true;
        }
        return false;
    }

    public void ReparentDraggableToStart()
    {
        start.SetActive(false);
        draggable.transform.SetParent(startSlot.transform);
    }

    public void RandomizeStartLocation()
    {
        float minX, maxX, minY, maxY;
        RectTransform rt = dragPrefab.GetComponent<RectTransform>();
        Debug.Log("width: " + rt.rect.width.ToString());
        Debug.Log("height: " + rt.rect.height.ToString());
        minX = -(rt.rect.width/2);
        minY = -(rt.rect.height/2);
        maxX = goal.GetComponent<RectTransform>().rect.x;
        maxY = rt.rect.height/2;
        Debug.Log("minX: " + minX.ToString() + " maxX: " + maxX.ToString() + " minY: " + minY.ToString() + " maxY: " + maxY.ToString());
        //Vector3 newPos = new Vector3(Random.Range(minX, maxX), Random.Range(minY,maxY), 0);
        //Debug.Log("newPos: " + newPos.ToString());
        RectTransform srt = start.GetComponent<RectTransform>();
        srt.anchoredPosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        //srt.rect.Set(Random.Range(minX, maxX), Random.Range(minY, maxY), srt.rect.width, srt.rect.height);
        start.SetActive(true);
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
}