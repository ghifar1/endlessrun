using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentControl : MonoBehaviour
{
    public float moveSpeed;

    [Header("Pools")]
    public Transform middleObstaclePool;
    public Transform sideObstaclePool;
    public Transform pointsPool;
    public Transform platformPool;

    [Header("Objects")]
    public GameObject[] middleObstacles;
    public GameObject[] sideObstacles;
    public GameObject[] points;

    Vector3 platformEndPosition, platformStartPosition;


    // Start is called before the first frame update
    void Start()
    {
        platformStartPosition = transform.GetChild(0).position + new Vector3(0, 0, 10);
        platformEndPosition = transform.GetChild(transform.childCount - 1).position;

        PopulatePool(middleObstaclePool, middleObstacles, 5);
        PopulatePool(sideObstaclePool, sideObstacles, 10);
        PopulatePool(pointsPool, points, 5);

        PopulatePlatformPool(3);

    }

    // Update is called once per frame
    void Update()
    {
        // fungsi jalan maju
        if(GameControl.current.myChar.isRunning == true)
        {
            for(int i=0; i < transform.childCount; i++)
            {
                Transform platform = transform.GetChild(1);
                platform.position += Vector3.back * moveSpeed * Time.deltaTime;

                if(platform.position.z <= platformEndPosition.z)
                {
                    CleanObject(platform);
                    //   platform = 
                }
            }
        }
    }

    void PopulatePool(Transform pool, GameObject[] objects, int amount)
    {
        for(int i=0; i < objects.Length; i++)
        {
            for(int j=0; j < amount; j++)
            {
                GameObject obj = Instantiate(objects[i], pool);
                obj.GetComponent<ObjectControl>().pool = pool;
                obj.SetActive(false);
            }
        }
    }

    void PopulatePlatformPool(int amount)
    {
        for(int i =0; i < transform.childCount; i++)
        {
            for(int j=0; j < amount; j++)
            {
                GameObject obj = Instantiate(transform.GetChild(i).gameObject, platformPool);
                obj.SetActive(false);
            }
        }
    }

    void CleanObject(Transform platform)
    {
        for(int j=platform.GetChild(0).childCount -1; j >= 0; j--)
        {
            Transform obj = platform.GetChild(0).GetChild(j);
            obj.parent = obj.GetComponent<ObjectControl>().pool;
            obj.gameObject.SetActive(false);
        }
    }
}
