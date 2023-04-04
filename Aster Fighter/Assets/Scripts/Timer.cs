using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Timer : MonoBehaviour
{
    #region Fields

    // timer duration
    public float totalSeconds = 0;

    // timer execution
    private float elapsedSeconds = 0;

    // support for Finished property
    private bool started = false;

    #endregion

    #region Properties

    /// <summary>
    /// Sets the duration of the timer
    /// The duration can only be set if the timer isn't currently running
    /// </summary>
    /// <value>duration</value>
    public float Duration
    {
        set
        {
            if (!Running)
            {
                totalSeconds = value;
            }
        }
    }

    /// <summary>
    /// Gets whether or not the timer has finished running
    /// This property returns false if the timer has never been started
    /// </summary>
    /// <value>true if finished; otherwise, false.</value>
    public bool Finished
    {
        get { return started && !Running; }
    }

    /// <summary>
    /// Gets whether or not the timer is currently running
    /// </summary>
    /// <value>true if running; otherwise, false.</value>
    public bool Running { get; private set; } = false;

    #endregion

    #region Methods

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        // update timer and check for finished
        if (Running)
        {
            elapsedSeconds += Time.deltaTime;
            if (elapsedSeconds >= totalSeconds)
            {
                Running = false;
            }
        }
    }

    /// <summary>
    /// Runs the timer
    /// Because a timer of 0 duration doesn't really make sense,
    /// the timer only runs if the total seconds is larger than 0
    /// This also makes sure the consumer of the class has actually 
    /// set the duration to something higher than 0
    /// </summary>
    public void Run()
    {
        // only run with valid duration
        if (totalSeconds > 0)
        {
            started = true;
            Running = true;
            elapsedSeconds = 0;
        }
    }

    /// <summary>
    /// Stop and reset the timer
    /// </summary>
    public void ResetTimer()
    {
        started = false;
        Running = false;
        elapsedSeconds = 0;
    }

    #endregion
}
