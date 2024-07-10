using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObject : MonoBehaviour
{
    [SerializeField] public float speed;

    void Start()
    {
        Destroy(gameObject, 60);    
    }

    private void Update()
    {
        var rotation = Time.deltaTime * speed;
        transform.Rotate(0,0,rotation);    
    }
}
