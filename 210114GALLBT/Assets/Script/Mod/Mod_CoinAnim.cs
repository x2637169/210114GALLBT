using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mod_CoinAnim : MonoBehaviour
{
    private Rigidbody rb;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(Random.Range(-580, 580), 400f, 0);
        timer = 3;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer<0) { Destroy(gameObject); }
        transform.Rotate(new Vector3(90 * Time.deltaTime*4, 40 * Time.deltaTime * 4, 40 * Time.deltaTime * 4));
    }

 
}
