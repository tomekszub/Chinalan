using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testObject : MonoBehaviour
{
    public GameObject test;
    public GameObject test1;
    public GameObject test2;
    Map map;
    MapGenerator mapGenerator;
    // Start is called before the first frame update
    void Start()
    {
        mapGenerator = GetComponent<MapGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GenerateMap();
    }
    void GenerateMap()
    {
        mapGenerator.ClearOriginOfExistingMap();
        map = mapGenerator.GenerateMap();
    }
}
