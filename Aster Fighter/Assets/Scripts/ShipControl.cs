using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ShipControl : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// Laser beam prefab
    /// </summary>
    [SerializeField] private GameObject prefabProjectile;

    /// <summary>
    /// The thruster sound
    /// </summary>
    [SerializeField] private GameObject ThrusterSound;

    #endregion
    #region Properties

    /// <summary>
    /// Flag telling main controller if the ship should be destroyed
    /// </summary>
    public bool DestroyMe { get; private set; } = false;

    #endregion
    #region Fields

    /// <summary>
    /// Rigidbody component of the ship
    /// </summary>
    private Rigidbody2D shipRigidBody = null;

    /// <summary>
    /// Thrusters effect
    /// </summary>
    private ParticleSystem.EmissionModule newEmitter;

    #endregion
    #region Constants

    /// <summary>
    /// Multiplier for ship thrust
    /// </summary>
    private const float ShipThrustFactor = 0.5f;

    /// <summary>
    /// Multiplier for ship turn speed
    /// </summary>
    private const float ShipTurnFactor = -2.0f;

    /// <summary>
    /// Multiplier for projectile thrust
    /// </summary>
    private const float ProjectileThrustFactor = 50.0f;

    #endregion

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        shipRigidBody = GetComponent<Rigidbody2D>();

        newEmitter = gameObject.GetComponentInChildren<ParticleSystem>().emission;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        //  Get ship movement information
        Vector2 keyboardPress = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        //  Rotate the ship
        if (keyboardPress.x != 0)
        { RotateShip(keyboardPress.x); }

        //  Apply thrust to the ship
        if (keyboardPress.y > 0)
        {
            ShipThrust(true);
        }
        else
        {
            ShipThrust(false);
        }

        //  Get weapon firing information and act on it
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireProjectile();
        }
    }

    /// <summary>
    /// Rotate the ship in the specified direction.
    /// </summary>
    /// <param name="DirectionFactor">Direction and speed for rotating the ship.</param>
    private void RotateShip(float DirectionFactor)
    {
        transform.Rotate(new Vector3(0, 0, DirectionFactor * ShipTurnFactor));
    }

    /// <summary>
    /// Add forward momentum to the ship
    /// </summary>
    private void ShipThrust(bool thrustOn)
    {
        //  Fire/stop firing the thruster effect
        newEmitter.enabled = thrustOn;
        ThrusterSound.SetActive(thrustOn);

        if (thrustOn)
        {
            //  Move the ship forward
            shipRigidBody.AddForce(transform.up * ShipThrustFactor, ForceMode2D.Impulse);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            DestroyMe = true;
        }
    }

    /// <summary>
    /// Fire a projectile
    /// </summary>
    private void FireProjectile()
    {
        GameObject projectile = Instantiate(prefabProjectile);
        projectile.transform.SetPositionAndRotation(transform.position, transform.rotation);
        projectile.GetComponent<Rigidbody2D>().AddForce(projectile.transform.up * ProjectileThrustFactor, ForceMode2D.Impulse);
    }
}
