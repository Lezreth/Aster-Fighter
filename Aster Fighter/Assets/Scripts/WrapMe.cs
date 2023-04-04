using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class WrapMe : MonoBehaviour
{
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        //  Check if this object moved offscreen in the horizontal axis and wrap it around to the opposite border
        if (transform.position.x > ScreenUtils.ScreenRight)
        {
            transform.position = new(ScreenUtils.ScreenLeft, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < ScreenUtils.ScreenLeft)
        {
            transform.position = new(ScreenUtils.ScreenRight, transform.position.y, transform.position.z);
        }

        //  Check if this object moved offscreen in the vertical axis and wrap it around to the opposite border
        if (transform.position.y < ScreenUtils.ScreenBottom)
        {
            transform.position = new(transform.position.x, ScreenUtils.ScreenTop, transform.position.z);
        }
        else if (transform.position.y > ScreenUtils.ScreenTop)
        {
            transform.position = new(transform.position.x, ScreenUtils.ScreenBottom, transform.position.z);
        }
    }
}
