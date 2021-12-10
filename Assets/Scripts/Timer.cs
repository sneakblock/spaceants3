using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public bool counting = true;
    private float time;
    public Text t;
    
    // Start is called before the first frame update
    void Start()
    {
        time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (counting)
        {
            time += Time.deltaTime;
            t.text = (time.ToString("n2") + "s");
        }
    }
}
