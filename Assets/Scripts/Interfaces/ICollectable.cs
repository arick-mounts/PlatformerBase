
using UnityEngine;

interface ICollectable 
{
    public CollectibleManager cManager {
        get;
        set;
    }

    void OnTriggerEnter(Collider col);
    void onPickup();
    void destroy();
}
