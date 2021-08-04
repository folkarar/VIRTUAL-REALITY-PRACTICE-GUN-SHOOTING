using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMove : MonoBehaviour
{
    Vector3 startPos;
    private Transform _Target;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            transform.position= new Vector3(-0.3f,0f,3f);
        }
        else if(Input.GetKeyDown(KeyCode.P))
        {
            transform.position= startPos;
        }
    }
}
