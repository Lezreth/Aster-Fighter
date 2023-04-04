using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Constrols aspects, behavior, and properites of asteroids
/// </summary>
public class AsteroidControl : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// Minimum rotational force to apply
    /// </summary>
    [SerializeField] private float MinTorque = -1f;

    /// <summary>
    /// Maximum rotational force to apply
    /// </summary>
    [SerializeField] private float MaxTorque = 1f;

    /// <summary>
    /// Minimum directional force to apply
    /// </summary>
    [SerializeField] private float MinImpulseForce = 1f;

    /// <summary>
    /// Maximum directional force to apply
    /// </summary>
    [SerializeField] private float MaxImpulseForce = 5f;

    #endregion
    #region Fields

    /// <summary>
    /// Size of the asteroid
    /// </summary>
    public AsteroidSizes AsteroidSize { get; private set; }

    /// <summary>
    /// Flag that indicates if this asteroid was destroyed by a projectile or by game over field clearing.
    /// </summary>
    private bool DestroyedByProjectile = false;

    #endregion
    #region Constants

    /// <summary>
    /// Minimum size an asteroid can be
    /// </summary>
    private const int MinimumAsteroidSize = 1;

    /// <summary>
    /// Maximum size an asteroid can be
    /// </summary>
    private const int MaximumAsteroidSize = 3;

    #endregion
    #region Events

    /// <summary>
    /// Delegate for spawning smaller replacement asteroids
    /// </summary>
    /// <param name="AsteroidSize"></param>
    public delegate void AsteroidDestroyedDelegate(AsteroidSizes AsteroidSize, Vector3 AsteroidLocation);

    /// <summary>
    /// Event for notifying main loop that an asteroid was destroyed
    /// </summary>
    public AsteroidDestroyedDelegate OnAsteroidDestroy;

    #endregion

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        //  Randomize the size
        float rng = Random.value;
        switch (rng)
        {
            case < 0.3f:
                ChangeAsteroidSize(AsteroidSizes.Small);
                break;
            case >= 0.3f and < 0.6f:
                ChangeAsteroidSize(AsteroidSizes.Medium);
                break;
            default:
                ChangeAsteroidSize(AsteroidSizes.Large);
                break;
        }


        //  Randomize the starting position, but somewhere offscreen
        Vector2 spawnPoint = transform.position;

        if (Random.Range(0, 2) > 0)
        {
            spawnPoint.x = Random.Range(ScreenUtils.ScreenLeft, ScreenUtils.ScreenRight);

            if (Random.Range(0, 2) > 0)
            {
                spawnPoint.y = ScreenUtils.ScreenTop;
            }
            else
            {
                spawnPoint.y = ScreenUtils.ScreenBottom;
            }
        }
        else
        {
            spawnPoint.y = Random.Range(ScreenUtils.ScreenBottom, ScreenUtils.ScreenTop);

            if (Random.Range(0, 2) > 0)
            {
                spawnPoint.x = ScreenUtils.ScreenRight;
            }
            else
            {
                spawnPoint.x = ScreenUtils.ScreenLeft;
            }
        }

        transform.position = spawnPoint;

        //  Randomize the starting direction and velocity
        float angle = Random.Range(0, 2 * Mathf.PI);
        Vector2 direction = new(Mathf.Cos(angle), Mathf.Sin(angle));
        float magnitude = Random.Range(MinImpulseForce, MaxImpulseForce);

        GetComponent<Rigidbody2D>().AddForce(direction * magnitude, ForceMode2D.Impulse);
        GetComponent<Rigidbody2D>().AddTorque(Random.Range(MinTorque, MaxTorque), ForceMode2D.Impulse);
    }

    /// <summary>
    /// Change the size of this asteroid
    /// </summary>
    /// <param name="asteroidSize">New Size of the asteroid</param>
    public void ChangeAsteroidSize(AsteroidSizes asteroidSize)
    {
        AsteroidSize = asteroidSize;

        //  Scale the asteroid according to the choosen size
        gameObject.transform.transform.localScale = AsteroidSize switch
        {
            AsteroidSizes.Small => new(1, 1, 1),
            AsteroidSizes.Medium => new(2, 2, 2),
            AsteroidSizes.Large => new(3, 3, 3),
            _ => throw new System.Exception("Asteroid size reached an impossible value!  The world is going to implode!"),
        };
    }

    /// <summary>
    /// Sets the destroyed by projectile flag.
    /// </summary>
    public void SetDestroyedByProjectile()
    {
        DestroyedByProjectile = true;
    }

    /// <summary>
    /// Called when [destroy].  Add a pont to the player's score if this asteroid is the smallest size and destroyed with a projectile.
    /// </summary>
    private void OnDestroy()
    {
        if (DestroyedByProjectile)
        {
            OnAsteroidDestroy(AsteroidSize, gameObject.transform.position);
        }
    }
}
