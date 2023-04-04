using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        if (gameObject.transform.position.x > ScreenUtils.ScreenRight
            || gameObject.transform.position.x < ScreenUtils.ScreenLeft
            || gameObject.transform.position.y > ScreenUtils.ScreenTop
            || gameObject.transform.position.y < ScreenUtils.ScreenBottom)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Collision with an asteroid happened, flag it
    /// </summary>
    /// <param name="collision">Asteroid the projectile collided with</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            collision.gameObject.GetComponent<AsteroidControl>().SetDestroyedByProjectile();
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
