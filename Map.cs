using UnityEngine;
using System.Collections.Generic;
public class Map
{
    public readonly List<GameObject> fields;
    public readonly int[] safeEntranceField;
    public readonly GameObject[] safeFields;
    public readonly int[] baseFieldsIndexes;
    public readonly int numberOfSafehouseFields = 0;
    public readonly int numberOfFieldsInEveryQuarter;

    public Map(List<GameObject> fields, int[] baseFieldsIndexes, GameObject[] safeFields, int[] safeEntranceField, int numberOfPlayers, int numberOfFieldsInEveryQuarter)
    {
        this.fields = fields;
        this.baseFieldsIndexes = baseFieldsIndexes;
        this.safeFields = safeFields;
        this.safeEntranceField = safeEntranceField;
        if(safeFields != null && safeFields.Length > 0) 
            numberOfSafehouseFields = safeFields.Length / numberOfPlayers;
        this.numberOfFieldsInEveryQuarter = numberOfFieldsInEveryQuarter;
    }
}