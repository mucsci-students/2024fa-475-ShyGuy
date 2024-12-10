using UnityEngine;
using System.Collections;

public class Airplane : MonoBehaviour
{

    public float speed;

    void Start()
    {
        transform.position = new Vector3(-150,15,0);
    }

    // Update is called once per frame
    void Update()
    {

        float moveAmount = speed * Time.deltaTime;
        transform.Translate(Vector3.forward * moveAmount);
    }

    


}
