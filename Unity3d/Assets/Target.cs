using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    // Start is called before the first frame update
    public float score = 0;

    public void TakeDamge (float po)
    {
        score += po;
        Debug.Log(score);
    }
}
