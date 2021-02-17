using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject[] basePrefabs;
    [SerializeField]
    int numberOfFieldsInEveryQuarter = 12;
    [SerializeField]
    Transform origin;
    [SerializeField]
    Pole[] fieldPrefabs;
    [SerializeField]
    PointOfInterest[] pointsOfInterestfieldPrefabs;

    List<GameObject> safeFields;
    List<int> safeEntranceFields;
    List<GameObject> fields;
    int[] baseFieldsIndexes;
    int verticalAdditionalBlocks = 1, horizontalAdditionalBlocks = 1;
    int zBorder = 0;

    public Map GenerateMap()
    {
        verticalAdditionalBlocks = 1;
        horizontalAdditionalBlocks = 1;
        fields = new List<GameObject>();
        safeFields = new List<GameObject>();
        safeEntranceFields = new List<int>();
        baseFieldsIndexes = new int[4];
        Vector3? pos = new Vector3();
        Vector3 lastValidPos = (Vector3)pos; 
        AddNewField((Vector3)pos);
        numberOfFieldsInEveryQuarter = 12;
        for (int i = 1; i < numberOfFieldsInEveryQuarter; i++)
        {
            pos = GenerateNextPosition((Vector3)pos);
            if (pos == null)
            {
                Debug.LogWarning("Can't generate new position.");
                break;
            }
            lastValidPos = (Vector3)pos;
            AddNewField((Vector3)pos);
        }
        lastValidPos = lastValidPos.CopyAndCreateNewVector(0, 0, 1);
        AddNewField(lastValidPos);

        int verticalLength = (int)(lastValidPos.z - fields[0].transform.position.z);
        int horizontalLength = (int)(fields[0].transform.position.x - lastValidPos.x);

        if (verticalLength < 5)
        {
            int missingBlocks = 5 - verticalLength;
            verticalAdditionalBlocks = 1 + 2 * missingBlocks;
        }
        if (horizontalLength < 5)
        {
            int missingBlocks = 5 - horizontalLength;
            horizontalAdditionalBlocks = 1 + 2 * (missingBlocks - 1);
        }
        // determine if baseFields will be adjacent to normal fields
        // if so then we need to add additional separation
        // we just need to check if the second to last field will be adjacent to first baseField
        if (fields[fields.Count - 3].transform.position.AreFieldsTouching(fields[fields.Count - 1].transform.position.CopyAndCreateNewVector(1, 0, 0)))
            verticalAdditionalBlocks += 2;
        // same here but we campare second field to first baseField
        if (fields[1].transform.position.AreFieldsTouching(fields[0].transform.position.CopyAndCreateNewVector(1, 0, 1)))
            horizontalAdditionalBlocks += 2;

        for (int i = 1; i < verticalAdditionalBlocks; i++)
        {
            lastValidPos = lastValidPos.CopyAndCreateNewVector(0, 0, 1);
            AddNewField(lastValidPos);
        }

        lastValidPos = AddNewMapQuarterAndReturnLastPos(lastValidPos, true, 2, true);

        lastValidPos = AddNewMapQuarterAndReturnLastPos(lastValidPos, false, 3, false);

        AddNewMapQuarterAndReturnLastPos(lastValidPos, true, 4, false);

        FillOtherFieldArrays();

        int baseDistance = 4;
        
        for (int i = 0; i < 4; i++)
        {
            Vector3 temp2 = safeFields[4 * i].transform.position - safeFields[4 * i + 1].transform.position;
            Quaternion rot = Quaternion.LookRotation(new Vector3(-temp2.x,0, -temp2.z));
            GameObject go = Instantiate(basePrefabs[i], fields[baseFieldsIndexes[i]].transform.position.CopyAndCreateNewVector(temp2.x*baseDistance,0.5f,temp2.z*baseDistance), rot, origin);
            go.transform.Rotate(0,90,0);
        }

        GeneratePointsOfInterest();

        return new Map(fields, baseFieldsIndexes, safeFields.ToArray(), safeEntranceFields.ToArray(), 4, numberOfFieldsInEveryQuarter);
    }
    void AddNewField(Vector3 pos, Pole.TerrainType? terrainType = null, Quaternion rot = default)
    {
        GameObject prefab;

        if (terrainType == null)
            prefab = fieldPrefabs[Random.Range(1, Pole.terrainTypeCount)].gameObject;
        else
            prefab = fieldPrefabs[(int)terrainType].gameObject;

        GameObject field = Instantiate(prefab, pos, rot, origin);
        field.GetComponent<Pole>().ID = fields.Count;

        if (terrainType != Pole.TerrainType.Base)
            fields.Add(field);
        else
            safeFields.Add(field);
    }
    void AddNewPointOfInterest(Vector3 pos, int poiAdjacentFieldIndex, PointOfInterest.PointOfInterestType? poiType = null, Quaternion rot = default)
    {
        GameObject prefab;
        if (poiType == null)
            prefab = pointsOfInterestfieldPrefabs[Random.Range(0, PointOfInterest.pointOfInterestTypeCount)].gameObject;
        else
            prefab = pointsOfInterestfieldPrefabs[(int)poiType].gameObject;

        GameObject poi = Instantiate(prefab, pos, rot, origin);

        fields[poiAdjacentFieldIndex].GetComponent<Pole>().adjacentPointOfInterest = poi.GetComponent<PointOfInterest>();
    }
    Vector3? GenerateNextPosition(Vector3 previous)
    {
        Vector3[] positions = GetAvaiablePosition(previous);
        if (positions.Length == 0) 
            return null;
        int index = Random.Range(0, positions.Length);
        if (positions[index].z > zBorder) zBorder = (int)positions[index].z;
        return positions[index];
    }
    Vector3[] GetAvaiablePosition(Vector3 startingPosition)
    {
        List<Vector3> avaiablePositions = new List<Vector3> 
        {
            new Vector3(startingPosition.x-1, startingPosition.y, startingPosition.z),
            new Vector3(startingPosition.x, startingPosition.y, startingPosition.z+1)
        };
        if (startingPosition.x + 1 <= 0)
            avaiablePositions.Add(new Vector3(startingPosition.x - 1, startingPosition.y, startingPosition.z));
        if (startingPosition.z - 1 >= zBorder)
            avaiablePositions.Add(new Vector3(startingPosition.x, startingPosition.y, startingPosition.z - 1));

        for (int i = avaiablePositions.Count-1; i >= 0; i--)
        {
            // fields.Count - 1 because we want to exclude last field (its adjacent to all avaiable field positions) 
            for (int j = 0; j < fields.Count; j++)
            {
                if (fields[j].transform.position == startingPosition)
                    continue;
                if (avaiablePositions[i].AreFieldsTouching(fields[j].transform.position))
                {
                    avaiablePositions.RemoveAt(i);
                    break;
                }
            }
        }
        return avaiablePositions.ToArray();
    }

    Vector3 AddNewMapQuarterAndReturnLastPos(Vector3 lastValidPos, bool flipXAxis, byte quarter, bool positiveAxisOffset)
    {
        int lastFieldsIndex = fields.Count - 1;
        Vector3 pos;
        int i, iMax;
        if (flipXAxis)
        {
            i = lastFieldsIndex - verticalAdditionalBlocks;
            iMax = lastFieldsIndex - numberOfFieldsInEveryQuarter - verticalAdditionalBlocks + 1;
            lastValidPos = fields[lastFieldsIndex - verticalAdditionalBlocks/2].transform.position;
        }
        else
        {
            i = lastFieldsIndex - horizontalAdditionalBlocks;
            iMax = lastFieldsIndex - numberOfFieldsInEveryQuarter - horizontalAdditionalBlocks + 1;
            lastValidPos = fields[lastFieldsIndex - horizontalAdditionalBlocks / 2].transform.position;
        }

        for (; i >= iMax; i--)
        {
            pos = fields[i].transform.position.MirrorPositionAlong(lastValidPos, flipXAxis, !flipXAxis);
            AddNewField(pos);
        }

        lastValidPos = fields[fields.Count - 1].transform.position;
        // creating separation field between quarters at specified offset (posiitive(1) or negative(-1))
        int offset = positiveAxisOffset ? 1 : -1;
        int xMax = flipXAxis ? horizontalAdditionalBlocks : verticalAdditionalBlocks;
        for (int x = 1; x < xMax+1; x++)
        {
            lastValidPos = lastValidPos.CopyAndCreateNewVector(offset * (flipXAxis ? 1 : 0), 0, offset * (flipXAxis ? 0 : 1));
            AddNewField(lastValidPos);
        }
        return lastValidPos;
    }
    public void ClearOriginOfExistingMap()
    {
        foreach (Transform child in origin)
        {
            Destroy(child.gameObject);
        }
    }
    void FillOtherFieldArrays()
    {
        Vector3 pos;
        int[] offsets = { 1,0,-1,0};
        for (int i = 4; i > 0; i--)
        {
            int safeEntranceFieldIndex = numberOfFieldsInEveryQuarter * i + (int)(verticalAdditionalBlocks * (0.5f * i)) + (int)(horizontalAdditionalBlocks * (0.5f * (i - 1)));
            pos = fields[safeEntranceFieldIndex].transform.position;
            safeEntranceFields.Add(safeEntranceFieldIndex);
            for (int j = 0; j < 4; j++)
            {
                pos = pos.CopyAndCreateNewVector(offsets[i - 1], 0, offsets[4 - i]);
                AddNewField(pos, Pole.TerrainType.Base);
            }
        }

        // we have to swap safeEntrance and safeFields belongign to 1 and 3 player
        //because we were going the other way around when assigning them (see -----^) 
        int temp = safeEntranceFields[1];
        safeEntranceFields[1] = safeEntranceFields[3];
        safeEntranceFields[3] = temp;

        for (int i = 4; i < 8; i++)
        {
            GameObject tempF = safeFields[i];
            safeFields[i] = safeFields[i + 8];
            safeFields[i + 8] = tempF;
        }

        SetBaseFieldsIndexes();
    }
    void SetBaseFieldsIndexes()
    {
        baseFieldsIndexes[0] = 0;
        baseFieldsIndexes[1] = numberOfFieldsInEveryQuarter + verticalAdditionalBlocks;
        baseFieldsIndexes[2] = 2 * numberOfFieldsInEveryQuarter + verticalAdditionalBlocks + horizontalAdditionalBlocks;
        baseFieldsIndexes[3] = 3 * numberOfFieldsInEveryQuarter + horizontalAdditionalBlocks + 2 * verticalAdditionalBlocks;

        int difference = verticalAdditionalBlocks - horizontalAdditionalBlocks;
        
        if (difference < 0)
        {
            difference = difference / 2;
            // difference is negative
            baseFieldsIndexes[0] = fields.Count + difference;
            baseFieldsIndexes[2] += difference;
        }
        else if (difference > 0)
        {
            difference = difference / 2;
            baseFieldsIndexes[1] -= difference;
            baseFieldsIndexes[3] -= difference;
        }
    }
    void GeneratePointsOfInterest()
    {
        HashSet<int> bannedFieldsIndexes = new HashSet<int>();
        foreach (int i in baseFieldsIndexes)
        {
            bannedFieldsIndexes.Add(i);
        }
        foreach (int i in safeEntranceFields)
        {
            bannedFieldsIndexes.Add(i);
            if (i >= fields.Count - 1)
                bannedFieldsIndexes.Add(0);
            else
                bannedFieldsIndexes.Add(i + 1);
            if (i <= 0)
                bannedFieldsIndexes.Add(fields.Count-1);
            else
                bannedFieldsIndexes.Add(i - 1);
        }
        Vector3[] avaiablePos;
        for (int i = 0; i < fields.Count; i++)
        {
            if(!bannedFieldsIndexes.Contains(i))
            {
                avaiablePos = GetAvaiablePosition(fields[i].transform.position);
                if (avaiablePos.Length != 0)
                    avaiablePos = ExcludeFieldsTouchingSafeFields(avaiablePos);
                if (avaiablePos.Length != 0)
                {
                    Vector3 v = avaiablePos[Random.Range(0, avaiablePos.Length)];
                    Vector3 temp2 = v - fields[i].transform.position;
                    Quaternion rot = Quaternion.LookRotation(new Vector3(temp2.x, 0, temp2.z));
                    AddNewPointOfInterest(v, i, null, rot);
                }
            }
        }
    }
    Vector3[] ExcludeFieldsTouchingSafeFields(Vector3[] avaiablePositions)
    {
        List<Vector3> newAvaiablePos = new List<Vector3>();
        bool areTouching;
        for (int x = avaiablePositions.Length - 1; x >= 0; x--)
        {
            areTouching = false;
            for (int j = 0; j < safeFields.Count; j++)
            {
                if (avaiablePositions[x].AreFieldsTouching(safeFields[j].transform.position))
                {
                    areTouching = true;
                    break; ;
                }
            }
            if(!areTouching)
                newAvaiablePos.Add(avaiablePositions[x]);
        }
        return newAvaiablePos.ToArray();
    }
}
