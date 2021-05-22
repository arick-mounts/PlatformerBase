using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, ICollectable
{
    public CollectibleManager cManager { get; set; }

    public void Start()
    {
        cManager = FindObjectOfType<CollectibleManager>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            this.onPickup();
            this.destroy();
        }
    }

    public void onPickup()
    {
        cManager.addPoint();
    }

    public void destroy()
    {
        Destroy(this.gameObject);
    }
}
