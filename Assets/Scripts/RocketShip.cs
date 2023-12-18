using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketShip : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;

    [SerializeField] float mainThrust = 2000f;
    [SerializeField] float rotationThrust = 500f;
    [SerializeField] AudioClip mainEngine, deathExplosionSFX, successLevelSFX;
    [SerializeField] ParticleSystem mainEngineParticles, explosionParticles;

    Rigidbody myRigidBody;
    AudioSource myAudioSource;
    GameController gameController;
    HealthBar myHealthBar;

    bool isAlive = true;
    int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        myAudioSource = GetComponent<AudioSource>();

        gameController = FindObjectOfType<GameController>();
        myHealthBar = FindObjectOfType<HealthBar>();

        currentHealth = maxHealth;
        myHealthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if(isAlive)
        {
            RocketMovement();
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isAlive || !gameController.collisionEnabled) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                print("I'm OKAY");
                break;

            case "Finish":
                SuccessRoutine();
                break;

            default:
                TakeDamage(20);
                break;
        }
    }

    private void DeathRoutine()
    {
        isAlive = false;
        AudioSource.PlayClipAtPoint(deathExplosionSFX, Camera.main.transform.position);
        gameController.ResetGame();
        explosionParticles.Play();
    }

    private void SuccessRoutine()
    {
        myRigidBody.isKinematic = true;
        AudioSource.PlayClipAtPoint(successLevelSFX, Camera.main.transform.position);
        gameController.NextLevel();
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        myHealthBar.SetHealth(currentHealth);

        if(currentHealth == 0)
        {
            DeathRoutine();
        }
    }

    private void RocketMovement()
    {
        float rotationSpeed = Time.deltaTime * rotationThrust;

        Thrusting();
        Rotating(rotationSpeed);
    }

    private void Thrusting()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (!myAudioSource.isPlaying)
            {
                myAudioSource.PlayOneShot(mainEngine);
            }

            mainEngineParticles.Play();
            myRigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        }
        else
        {
            mainEngineParticles.Stop();
            myAudioSource.Stop();
        }
    }

    private void Rotating(float rotationSpeed)
    {
        myRigidBody.freezeRotation = true;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationSpeed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationSpeed);
        }

        myRigidBody.freezeRotation = false;
    }

    private void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z);
    }
}
