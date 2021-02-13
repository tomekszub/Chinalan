using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingMaster : MonoBehaviour
{
    [SerializeField]
    GameObject highlightVisualizatioinPrefab;

    List<GameObject> highlightObjectsPool = new List<GameObject>();

    public GameObject GetHighlightObject()
    {
        GameObject go = GetFreeHighlightObject();
        if (go == null)
        {
            go = Instantiate(highlightVisualizatioinPrefab);
            highlightObjectsPool.Add(go);
        }
        return go;
    }
    public void ReclaimAllHighlightObjects()
    {
        foreach (var go in highlightObjectsPool)
        {
            go.SetActive(false);
        }
    }
    GameObject GetFreeHighlightObject()
    {
        if (highlightObjectsPool.Count == 0) return null;

        foreach (var go in highlightObjectsPool)
        {
            if (!go.activeSelf)
                return go;
        }

        return null;
    }
}
