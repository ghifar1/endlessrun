using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectControl : MonoBehaviour
{
    public Transform pool;

    private void OnEnable()
    {
        for (int i = 0; i < pool.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
