using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalWatcher : MonoBehaviour
{
    // it'd be nice, one day, to have a shared manager object which holds these values, so that we don't have to put them into the SoundManager and the GoalWatcher both. But for now... deal with it. *does sassy look*
    [Tooltip("IMPORTANT! This is where we link up all the ConeZones. Increase the size to the number of 'talkers' you have, make sure they have SoundConeManager components on them, and then drag them into here to link them. ")]
    public List<SoundConeManager> goalConeZones = new List<SoundConeManager>();
    public int totalGoals;
    [Tooltip("Read Only. Updated constantly to reflect the state of the goals.")]
    public int goalsRightNow;
    public bool watch = false;
    public int totalSamples = 0;
    public List<int> sampleValues = new List<int>();
    

    void Start()
    {
        if (goalConeZones.Count < 1)
        {
            Debug.LogError("Error in GoalWatcher. No cone zones defined. You need to drag some ConeZones in there, or we won't know whether they're scoring goals.");
        }
        totalGoals = goalConeZones.Count;

    }

    // feels like a good place to do this. I donno if we're using FixedUpdate anywhere so far, but if not, then we could scale this to 1 second to break down our percentage that way. Or, just change it back to update and use math(s).
    void FixedUpdate()
    {
        if (watch)
        {
            // check all the goals to see what the state is.
            int goalCounter = 0;
            for (int i = 0; i < goalConeZones.Count; i++)
            {
                if (goalConeZones[i].charID == 111)
                {
                    Debug.LogError("GoalWatcher | Unable to read charID of goalConeZone. Fix the script execution order and this should go away.");
                }
                var charID = goalConeZones[i].charID;   // charID and i should be the same. But, those get set by the SoundManager script, so if this script runs first then it might not work
                var goalState = goalConeZones[i].goal;
                //Debug.Log("Iterating over defined goals and checking values. Goal #" + charID + ": " + goalState);
                if (goalState)
                {
                    // they're in the sweet spot here.
                    goalCounter++;
                }
                else
                {
                    // they'r really stinking it up. How are they so bad at our amazing game??
                }
            }
            totalSamples++;
            sampleValues.Add(goalCounter);
            goalsRightNow = goalCounter;
            //Debug.Log("Goal percent: " + GetGoalsPercent().ToString());
        }
    }

    public float GetGoalsPercent()
    {
        int individualSample;
        float goalPercent = 0f;
        float allGoalPercent = 0f;
        for (int i = 0; i < totalSamples; i++)
        {
            individualSample = sampleValues[i];
            goalPercent = Mathf.Round(((float)individualSample / (float)totalGoals) * 100f) / 100f;
            allGoalPercent += goalPercent;
        }
        return allGoalPercent / totalSamples;
    }

    public void StartWatching()
    {
        totalSamples = 0;
        sampleValues = new List<int>();
        this.watch = true;
    }

    public void StopWatching()
    {
        this.watch = false;
    }
}
