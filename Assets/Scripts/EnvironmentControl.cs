using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentControl : MonoBehaviour
{
    public float moveSpeed;
    public static EnvironmentControl current;

    [Header("Pools")]
    public Transform middleObstaclePool;
    public Transform sideObstaclePool;
    public Transform pointsPool;
    public Transform platformPool;

    [Header("Season Pools")]
    public string[] seasonNames;
    public int activeSeasonIndex;
    public int lastActiveSeasonindex;

    [Header("Objects")]
    public GameObject[] middleObstacles;
    public GameObject[] sideObstacles;
    public GameObject[] points;

    private Vector3 platformEndPosition, platformStartPosition;


    // Start is called before the first frame update
    void Start()
    {
        current = this;
        activeSeasonIndex = 0;
        lastActiveSeasonindex = activeSeasonIndex;
        platformStartPosition = transform.GetChild(0).position + new Vector3(0, 0, 50);
        platformEndPosition = transform.GetChild(transform.childCount - 1).position - new Vector3(0,0, 70);

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
                Transform platform = transform.GetChild(i);
                platform.position += Vector3.back * moveSpeed * Time.deltaTime;

                if(platform.position.z <= platformEndPosition.z)
                {
                    CleanObject(platform);
                    platform = SpawnPlatform(platform);

                    int spawnObjectAmount = Mathf.CeilToInt(moveSpeed / 3);

                    for(int j =0; j < spawnObjectAmount; j++)
                    {
                        SpawnObject(platform);
                    }
                    
                }
            }
        }

        //fungsi jalan mundur
        if(GameControl.current.myChar.isRollback == true)
        {
            for(int i = transform.childCount -1; i >= 0; i--)
            {
                Transform platform = transform.GetChild(i);
                platform.position += Vector3.forward * moveSpeed * Time.deltaTime;
                if(platform.position.z >= platformStartPosition.z)
                {
                    platform.position = transform.GetChild(transform.childCount - 1).position - new Vector3(0, 0, 50);
                    platform.SetAsLastSibling();
                    CleanObject(platform);

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
                Debug.Log(obj);
                obj.GetComponent<ObjectControl>().pool = pool;
                obj.SetActive(false);
            }
        }
    }

    GameObject GetRandomObjectFromPool(Transform pool)
    {
        if(pool.childCount <= 0)
        {
            return null;
        }

        return pool.GetChild(Random.Range(0, pool.childCount)).gameObject;
    }

    int lastObstaclelaneSpawn = -1;
    int obstacleSpawnStep;

    void SpawnObject(Transform platform)
    {
        int obstacleLane = Random.Range(0, 3);
        for (int j = 0; j < 3; j++)
        {
            GameObject spawning = null;
            Transform obstaclePool;
            if(j==1)
            {
                obstaclePool = middleObstaclePool;
            } else
            {
                obstaclePool = sideObstaclePool;
            }

            if(j==obstacleLane && Random.Range(0,2) == 0 && lastObstaclelaneSpawn != obstacleLane)
            {
                spawning = GetRandomObjectFromPool(obstaclePool);
                lastObstaclelaneSpawn = obstacleLane;
                obstacleSpawnStep = 0;
            }

            if(spawning == null && obstacleSpawnStep > 1)
            {
                obstacleSpawnStep = 0;
                lastObstaclelaneSpawn = -1;
            }

            if(spawning == null && Random.Range(0,2) == 0)
            {
                spawning = GetRandomObjectFromPool(pointsPool);
            }

            if(spawning != null)
            {
                spawning.transform.parent = platform.GetChild(0);
                switch(j)
                {
                    case 0:
                        if(spawning.tag == "obstacle")
                        {
                            spawning.transform.localPosition = new Vector3(-1.5f, 0, Random.Range(-50.0f, 50.0f));
                            spawning.transform.localEulerAngles = new Vector3(0, 160, 0);

                        } else
                        {
                            spawning.transform.localPosition = new Vector3(-1.0f, 0, Random.Range(-50.0f, 50.0f));
                    
                        }
                    break;

                    case 1:
                        spawning.transform.localPosition = new Vector3(0, 0, Random.Range(-50.0f, 50.0f));
                    break;

                    case 2:
                        if(spawning.tag == "obstacle")
                        {
                            spawning.transform.localPosition = new Vector3(1.5f, 0, Random.Range(-50.0f, 50.0f));
                            spawning.transform.localEulerAngles = new Vector3(0, 220, 0);

                        } else
                        {
                            spawning.transform.localPosition = new Vector3(1.0f, 0, Random.Range(-50.0f, 50.0f));
                        }
                    break;


                }
                spawning.SetActive(true);

            }
        }

        obstacleSpawnStep++;
    }

    void PopulatePlatformPool(int amount)
    {
        int totalPlatform = platformPool.childCount;
        for (int i=0; i < totalPlatform; i++)
        {
            Transform seasonObject = platformPool.GetChild(i);
            Debug.Log("INI PLATFORM");
            Debug.Log(seasonObject.name);

            int totalChildSeason = seasonObject.childCount;
            for(int j=0; j < totalChildSeason; j++)
            {
                //Debug.Log(seasonObject.name + " ANAK KE " + j);
                //GameObject obj = seasonObject.GetChild(j).gameObject;
                //Debug.Log(obj.name);

                GameObject obj = Instantiate(seasonObject.GetChild(j).gameObject, seasonObject);
                obj.SetActive(false);
                //for (int k=0; k < amount; k++)
                //{
                //    //GameObject obj = Instantiate(seasonObject.GetChild(k).gameObject, seasonObject);
                //    //obj.SetActive(false);
                //}
            }
            
        }

        //for(int i =0; i < transform.childCount; i++)
        //{
        //    for(int j=0; j < amount; j++)
        //    {
        //        GameObject obj = Instantiate(transform.GetChild(i).gameObject, platformPool);
        //        obj.SetActive(false);
        //    }
        //}
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

    Transform SpawnPlatform(Transform platform)
    {
        Debug.Log("Active season:" + activeSeasonIndex);
        Transform seasonPool = platformPool.GetChild(activeSeasonIndex);
        Transform newPlatform = seasonPool.GetChild(Random.Range(0, seasonPool.childCount));
        //Transform newPlatform = platformPool.GetChild(Random.Range(0, platformPool.childCount));
        newPlatform.parent = transform;
        if(platform.tag == "summer")
        {
            platform.parent = platformPool.GetChild(0);
        } else
        {
            platform.parent = platformPool.GetChild(1);
        }
        //platform.parent = seasonPool;
        platform.gameObject.SetActive(false);

        newPlatform.position = transform.GetChild(0).position + new Vector3(0, 0, 50);
        newPlatform.SetAsFirstSibling();
        newPlatform.gameObject.SetActive(true);

        return newPlatform;
    }
}
