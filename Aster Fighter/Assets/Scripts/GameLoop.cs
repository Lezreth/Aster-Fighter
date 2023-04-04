using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Initializes the game
/// </summary>
public class GameLoop : MonoBehaviour 
{
    #region Serializers

    /// <summary>
    /// Ship prefab
    /// </summary>
    [SerializeField] private GameObject prefabShip;

    /// <summary>
    /// Asteroid prefab
    /// </summary>
    [SerializeField] private GameObject[] prefabAsteroids;

    /// <summary>
    /// Maximum number of asteroids allowed to be spawned at once
    /// </summary>
    [SerializeField] private int MaximumAsteroidCount = 5;

    /// <summary>
    /// Minimum time to wait before respawning the ship after it went boom
    /// </summary>
    [SerializeField] private int MinimumTimeBetweenShipRespawn = 5;

    /// <summary>
    /// Center message banner
    /// </summary>
    [SerializeField] private TextMeshProUGUI CenterMessage;

    /// <summary>
    /// Message informing the player how to reset their highscore
    /// </summary>
    [SerializeField]private TextMeshProUGUI ResetHighScoreMessage;

    #endregion
    #region Properties

    /// <summary>
    /// Flag to indicate the player has started the game and we should set everything up
    /// </summary>
    private bool initiateGamePlay = false;

    /// <summary>
    /// Flag to indicate if the player is currently playing the game
    /// </summary>
    private bool playing = false;

    /// <summary>
    /// The one ship in the game
    /// </summary>
    private GameObject ShipInstance { get; set; } = null;

    /// <summary>
    /// List of the asteroids currently in the game
    /// </summary>
    private List<GameObject> Asteroids = new();

    #endregion
    #region Fields

    /// <summary>
    /// Used to pace out the spawning of asteroids
    /// </summary>
    private Timer timer = null;

    /// <summary>
    /// Used to access the collision flag
    /// </summary>
    private ShipControl shipControl = null;

    /// <summary>
    /// The time when the ship blew up
    /// </summary>
    private float timeShipWentBoom = 0.0f;

    /// <summary>
    /// Number of lives the player has
    /// </summary>
    private int lives = 3;

    #endregion
    #region Constants

    /// <summary>
    /// Minimum time between asteroid spawns
    /// </summary>
    private const float MinSpawnTime = 1.0f;

    /// <summary>
    /// Maximum time between asteroid spawns
    /// </summary>
    private const float MaxSpawnTime = 5.0f;

    /// <summary>
    /// Total number of lives the player can have
    /// </summary>
    private const int TotalLives = 3;

    /// <summary>
    /// Initial time between asteroid spawns
    /// </summary>
    private const int AsteroidSpawnTime = 5;

    /// <summary>
    /// Time to wait between the player pressing space and spawning the ship
    /// </summary>
    private const int GameStartDelay = 1;

    #endregion

    /// <summary>
    /// Awake is called before Start
    /// </summary>
    private void Awake()
    {
        // initialize screen utils
        ScreenUtils.Initialize();
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        if (prefabShip == null) { Debug.LogError(prefabShip); return; }
        if (prefabAsteroids == null) { Debug.LogError(prefabAsteroids); return; }

        //  Start the asteroid spawn timer
        timer = gameObject.AddComponent<Timer>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        //  ****  Primary Control: The player is currently playing the game
        if (playing)
        {
            if (ShipInstance == null)
            {
                if (Time.realtimeSinceStartup - timeShipWentBoom >= MinimumTimeBetweenShipRespawn)
                {
                    //  Player ran into an asteroid, give them another ship
                    SpawnShip();
                }
            }
            else if (shipControl.DestroyMe)
            {
                //  Make ship go boom on hitting an asteroid
                Destroy(ShipInstance);
                timeShipWentBoom = Time.realtimeSinceStartup;

                //  Deduct a life, if no more lives left then game over
                lives--;
                if (lives == 0)
                {
                    playing = false;
                    timer.ResetTimer();
                    CenterMessage.enabled = true;
                    ResetHighScoreMessage.enabled = true;

                    //  Get rid of any remaining asteroids
                    foreach (GameObject asteroid in GameObject.FindGameObjectsWithTag("Asteroid"))
                    {
                        Destroy(asteroid);
                    }
                }
            }

            //  Spawn asteroids, controlled by a maximum amount and time delayed between spawns
            if (timer.Finished)
            {
                if (Asteroids.Count < MaximumAsteroidCount)
                {
                    //  Spawn an asteroid
                    SpawnAsteroids();
                }

                //  Randomize the time between asteroid spawns
                timer.Duration = Random.Range(MinSpawnTime, MaxSpawnTime);
                timer.Run();
            }
        }

        //  ****  Primary Control: The player has indicated they want to start playing, initialize everything
        else if (initiateGamePlay)
        {
            if (timer.Finished)
            {
                SpawnShip();
                ScoreManager.instance.ResetScore();
                lives = TotalLives;

                initiateGamePlay = false;
                playing = true;

                //  Timer is used to space out asteroid spawning
                timer.Duration = AsteroidSpawnTime;
                timer.Run();
            }
        }

        //  ****  Primary Control: Wait for the player to indicate they want to start playing by pressing the space bar
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                initiateGamePlay = true;

                //  Delay between hiding the start message and spawning the ship to allow time for the player to release the start button
                //  Without this delay the ship appear to will fire on entry
                timer.Duration = GameStartDelay;
                timer.Run();

                CenterMessage.enabled = false;
                ResetHighScoreMessage.enabled = false;
            }
            else if (Input.GetKeyDown(KeyCode.Backspace))
            {
                ScoreManager.instance.ResetHighScore();
            }
        }
    }

    private void Asteroid_Destroyed(AsteroidSizes AsteroidSize, Vector3 OldAsteroidLocation)
    {
        switch (AsteroidSize)
        {
            case AsteroidSizes.Small:
                ScoreManager.instance.AddPoint();
                break;
            case AsteroidSizes.Medium:
            case AsteroidSizes.Large:
                SpawnAsteroids(AsteroidSize, OldAsteroidLocation, Replacements: true);
                break;
            default:
                throw new System.Exception("Another impossible state reached!  Watch out for Nessy!");
        }
    }

    /// <summary>
    /// Spawns asteroids
    /// </summary>
    /// <param name="OldAsteroidSize">Size of the old asteroid if replacing a larger one</param>
    /// <param name="OldAsteroidLocation">Location of the old asteroid if replacing a larger one</param>
    /// <param name="Replacements">True = Replacing a larger asteroid, False = Spawning a new asteroid</param>
    private void SpawnAsteroids(AsteroidSizes OldAsteroidSize = default, Vector3 OldAsteroidLocation = default, bool Replacements = false)
    {
        if (Replacements)
        {
            //  Replacing a larger asteroid, replace it with two of the next smaller size
            AsteroidSizes newSize = OldAsteroidSize == AsteroidSizes.Large ? AsteroidSizes.Medium : AsteroidSizes.Small;

            for (int i = 0; i < 2; i++)
            {
                GameObject asteroid = SpawnAsteroid();
                asteroid.transform.position = OldAsteroidLocation;
                asteroid.GetComponent<AsteroidControl>().ChangeAsteroidSize(newSize);
            }
        }
        else
        {
            //  Not replacing a larger asteroid, just spawn one
            _ = SpawnAsteroid();
        }
    }


    private GameObject SpawnAsteroid()
    {
        GameObject asteroid = Instantiate(prefabAsteroids[Random.Range(0, prefabAsteroids.Length - 1)]);
        Asteroids.Add(asteroid);
        asteroid.GetComponent<AsteroidControl>().OnAsteroidDestroy += Asteroid_Destroyed;

        return asteroid;
    }

    /// <summary>
    /// Spawn a new ship and setup asteroid destruction event handler
    /// </summary>
    private void SpawnShip()
    {
        if (ShipInstance == null)
        {
            ShipInstance = Instantiate(prefabShip);
            ShipInstance.transform.position = Vector3.zero;
            shipControl = ShipInstance.GetComponent<ShipControl>();
        }
    }
}
