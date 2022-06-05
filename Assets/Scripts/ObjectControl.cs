using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectControl : MonoBehaviour
{
    public Transform pool;
    [SerializeField] private GameObject[] points;
    
    
    private void OnEnable()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
    public void OnEnableCoin()
    {
        for (int i = 0; points.Length > i; i++)
        {
            points[i].SetActive(true);
        }
    }
}
