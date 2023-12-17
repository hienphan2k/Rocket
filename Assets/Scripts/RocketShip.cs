using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketShip : MonoBehaviour
{
    [SerializeField] float mainThrust = 2000f;
    [SerializeField] float rotationThrust = 500f;
    [SerializeField] ParticleSystem mainEngineParticles, explosionParticles;

    Rigidbody myRigidBody;
    AudioSource myAudioSource;
    GameController gameController;

    bool isAlive = true;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        myAudioSource = GetComponent<AudioSource>();
        gameController = FindObjectOfType<GameController>();
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
                myRigidBody.isKinematic = true;
                gameController.NextLevel();
                break;

            default:
                isAlive = false;
                gameController.ResetGame();
                explosionParticles.Play();
                break;

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
                myAudioSource.Play();
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
