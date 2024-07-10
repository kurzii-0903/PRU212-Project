using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
    [SerializeField] private float spawnPerSecond;
    [SerializeField] private float spawnDelay;
    [SerializeField] private float spawnTime;
    [SerializeField] private Transform[] transforms;
    [SerializeField] private GameObject[] rooks;

    public void Start()
    {
        StartCoroutine(waitAndSpawn(spawnDelay, spawnPerSecond));   
    }

    private Vector2 getRandomPosition()
    {
        Debug.Log("get rnd pos");
        Vector2 vector = new Vector2(Random.Range(transforms[0].position.x, transforms[1].position.x),transforms[0].position.y);
        return vector;
    }

    private GameObject getRandomGameObject()
    {
        int randomScale = Random.Range(3, 9);
        float randomGravityScale = Random.Range(0.125f, 0.125f);
        GameObject rook = rooks[Random.Range(0,rooks.Length)];
        rook.transform.localScale = new Vector3 (randomScale,randomScale,randomScale);
        rook.GetComponent<Rigidbody2D>().gravityScale = randomGravityScale;
        return rook;
    }

    private IEnumerator waitAndSpawn(float delay, float spawnpersec)
    {
        yield return new WaitForSeconds(delay);
        while (true)
        {
            {
                Instantiate(getRandomGameObject(), getRandomPosition(), Quaternion.identity);
                Debug.Log("Spawn success");

                yield return new WaitForSeconds(spawnpersec);
            }

        }
    }

    public void OnDisable()
    {
        StopAllCoroutines();
    }
}
