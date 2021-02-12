using System.Collections;
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

        // determine if baseFields will be adjacent to normal fields
        // if so then we need to add additional separation
        // we just need to check if the second to last field will be adjacent to first baseField
        if (fields[fields.Count - 3].transform.position.AreFieldsTouching(fields[fields.Count - 1].transform.position.CopyAndCreateNewVector(1, 0, 0)))
            verticalAdditionalBlocks += 2;
        // same here but we campare second field to first baseField
        if(fields[1].transform.position.AreFieldsTouching(fields[0].transform.position.CopyAndCreateNewVector(1, 0, 1)))
            horizontalAdditionalBlocks += 2;

        if (horizontalLength < 5)
        {
            int missingBlocks = 5 - horizontalLength;
            horizontalAdditionalBlocks = 1 + 2 * (missingBlocks-1);
        }

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

        for (int i = 0; i < 4; i++)
        {
            fields[baseFieldsIndexes[i]].GetComponent<Pole>().ChangeTerrainType(Pole.TerrainType.Grass);
        }

        return new Map(fields, baseFieldsIndexes, safeFields.ToArray(), safeEntranceFields.ToArray(), 4);
    }
    void AddNewField(Vector3 pos, Pole.TerrainType? terrainType = null)
    {
        if (terrainType == null)
            terrainType = (Pole.TerrainType)Random.Range(1, Pole.terrainTypeCount);
        bool isSafehouse = (terrainType == Pole.TerrainType.Base);
        GameObject field = Instantiate(fieldPrefabs[(int)terrainType].gameObject, pos, Quaternion.identity, origin);
        field.GetComponent<Pole>().SetStartingValues(fields.Count, isSafehouse, (Pole.TerrainType)terrainType);
        if (!isSafehouse)
            fields.Add(field);
        else
            safeFields.Add(field);
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
            for (int j = 0; j < fields.Count-1; j++)
            {
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
            int baseEntranceFieldIndex = numberOfFieldsInEveryQuarter * i + (int)(verticalAdditionalBlocks * (0.5f * i)) + (int)(horizontalAdditionalBlocks * (0.5f * (i - 1)));
            pos = fields[baseEntranceFieldIndex].transform.position;
            safeEntranceFields.Add(baseEntranceFieldIndex);
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

        baseFieldsIndexes[0] = 0;
        baseFieldsIndexes[2] = 2 * numberOfFieldsInEveryQuarter + verticalAdditionalBlocks + horizontalAdditionalBlocks;
        if (horizontalAdditionalBlocks <= 1)
        {
            baseFieldsIndexes[0]++;
            baseFieldsIndexes[2]++;
        }

        baseFieldsIndexes[1] = numberOfFieldsInEveryQuarter + verticalAdditionalBlocks;
        baseFieldsIndexes[3] = 3 * numberOfFieldsInEveryQuarter + horizontalAdditionalBlocks + 2 * verticalAdditionalBlocks;
        if (verticalAdditionalBlocks <= 1)
        {
            baseFieldsIndexes[1]++;
            baseFieldsIndexes[3]++;
        }
    }

}
