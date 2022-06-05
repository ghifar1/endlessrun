using UnityEngine;

public class EnvironmentControl : MonoBehaviour
{
    

    [SerializeField] private CharacterControl character;

    [Header("Pools")]
    [SerializeField] private Transform middleObstaclePool;
    [SerializeField] private Transform sideObstaclePool;
    [SerializeField] private Transform pointsPool;
    [SerializeField] private Transform platformPool;

    [Header("Objects")]
    [SerializeField] private GameObject[] middleObstacles;
    [SerializeField] private GameObject[] sideObstacles;
    [SerializeField] private GameObject[] points;
    
    Vector3 platformEndPosition,platformStartPosition;

    public float moveSpeed;

	// Start is called before the first frame update
	void Start()
    {
        platformStartPosition = transform.GetChild(0).position + new Vector3(0, 0, 10);
        platformEndPosition = transform.GetChild(transform.childCount - 1).position;

        PopulatePoolCar(middleObstaclePool, middleObstacles, 5);
        PopulatePoolCar(sideObstaclePool, sideObstacles, 10);
        PopulatePoolCoin(pointsPool, points, 5);

        PopulatePlatformPool(3);

    }

    // Update is called once per frame
    void Update()
    {
        // fungsi jalan maju
        if(character.isRunning)
        {
            for(int i=0; i < transform.childCount; i++)
            {
                Transform platform = transform.GetChild(i);

                platform.position += Vector3.back * moveSpeed * Time.deltaTime;

                if(platform.position.z <= platformEndPosition.z)
                {
                    //CleanObject(platform);
                    platform = SpawnPlatform(platform); // newPlatform
                    SpawnObject(platform); // <<<
                }
            }
        }

        //fungsi jalan mundur
        if(character.isRollback)
        {
            for(int i = transform.childCount -1; i >= 0; i--)
            {
                Transform platform = transform.GetChild(i);
                platform.position += Vector3.forward * moveSpeed * Time.deltaTime;
                if(platform.position.z >= platformStartPosition.z)
                {
                    platform.position = transform.GetChild(transform.childCount - 1).position - new Vector3(0, 0, 10);
                    platform.SetAsLastSibling();
                    //CleanObject(platform);

                }
            }
        }
    }

    void PopulatePoolCar(Transform pool, GameObject[] objects, int amount)
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

    void PopulatePoolCoin(Transform pool, GameObject[] objects, int amount)
    {
        for(int i=0; i < objects.Length; i++)
        {
            for(int j=0; j < amount; j++) 
            {
                GameObject obj = Instantiate(objects[i], pool);
                obj.GetComponent<ObjectControl>().OnEnableCoin();   
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
                            spawning.transform.localPosition = new Vector3(-1.2f, 0, 0);
                            spawning.transform.localEulerAngles = new Vector3(0, 160, 0);

                        } else
                        {
                            spawning.transform.localPosition = new Vector3(-0.9f, 0, 0);
                    
                        }
                    break;

                    case 1:
                        spawning.transform.localPosition = new Vector3(0, 0, 0);
                    break;

                    case 2:
                        if(spawning.tag == "obstacle")
                        {
                            spawning.transform.localPosition = new Vector3(1.2f, 0, 0);
                            spawning.transform.localEulerAngles = new Vector3(0, 220, 0);

                        } else
                        {
                            spawning.transform.localPosition = new Vector3(0.9f, 0, 0);
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
        for(int i =0; i < transform.childCount; i++)
        {
            GameObject obj = Instantiate(transform.GetChild(i).gameObject, platformPool);
            obj.SetActive(false);
        }
    }

    void CleanObject(Transform platform)
    {
        for(int j=platform.GetChild(0).childCount -1; j >= 0; j++)
        {
            Transform obj = platform.GetChild(0).GetChild(j);
            obj.parent = obj.GetComponent<ObjectControl>().pool;
            obj.gameObject.SetActive(false);
        }
    }

    Transform SpawnPlatform(Transform platform)
    {
        Transform newPlatform = platformPool.GetChild(Random.Range(0, platformPool.childCount));
        newPlatform.parent = transform;
        platform.parent = platformPool;
        platform.gameObject.SetActive(false);

        newPlatform.position = transform.GetChild(1).position + new Vector3(0, 0, 10);
        newPlatform.SetAsFirstSibling();
        newPlatform.gameObject.SetActive(true);

        return newPlatform;
    }
}
