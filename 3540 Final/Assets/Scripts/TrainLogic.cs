using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainLogic : MonoBehaviour
{

    Rigidbody body;
    public GameObject smoke;
    public TrainLogic nextTrain;
    public GameObject runText;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();  
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Explosion() 
    {
        FindAnyObjectByType<StandardLevelManager>().ExplosionSignal();
        if (smoke != null && nextTrain != null)
        {
            smoke.SetActive(false);
            runText.SetActive(false);
            GetComponent<CinemachineDollyCart>().enabled = false;
            body.isKinematic = false;
            body.AddForce((transform.forward * 10000) + transform.up * 3000);
            body.AddTorque(Vector3.left * 3000 * Time.deltaTime);
            nextTrain.Explosion();
        }
        else 
        {
            GetComponent<CinemachineDollyCart>().enabled = false;
            body.isKinematic = false;
            body.AddForce((transform.forward * -10000) + transform.up * 3000);
            body.AddTorque(Vector3.left * -3000 * Time.deltaTime);
        }
    }
}
