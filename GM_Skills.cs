using System.Collections.Generic;
using UnityEngine;

public class GM_Skills : MonoBehaviour
{
    private const int rangeOfFractionSkills = 5;
    GameManager gm;
    UIManager UIScript;
    [HideInInspector]
    public bool showRange = false;
    int skillsIndex = 0;
    int range = 0;
    int turn = 0;
    bool isInRange = false;
    bool isCleaningNecessary = false;
    int lastFieldID = -1;
    Skill currSkill;
    PoolingMaster pool;
    void Awake()
    {
        gm = GetComponent<GameManager>();
        UIScript = GetComponent<UIManager>();
        pool = GetComponent<PoolingMaster>();
    }
    void Update()
    {
        if(showRange)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, (1 << 13)))
            {
                int id = hit.transform.GetComponent<Pole>().ID;
                if (id != lastFieldID)
                {
                    lastFieldID = id;
                    isInRange = IsInRangeOfBase(id);
                    if (!isInRange)
                    {
                        UIScript.SetNotificationText("This field is outside of your base range.");
                        return;
                    }
                    pool.ReclaimAllHighlightObjects();
                    UIScript.SetNotificationText("");
                    HighlightSkillRange(id);
                    isCleaningNecessary = true;
                }
                else
                {
                    if (!isInRange)
                        return;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    UseSkill(id);
                    CancelShowingRange();
                }
            }
            else
            {
                UIScript.SetNotificationText("");
                if (isCleaningNecessary && Input.GetMouseButtonDown(0))
                {
                    CancelShowingRange();
                }
            }
        }
    }
    void SpawnHighlightPrefab(int fieldId)
    {
        GameObject go = pool.GetHighlightObject();
        go.SetActive(true);
        go.transform.position = gm.map.fields[fieldId].transform.position.CopyAndCreateNewVector(0,0.5f,0);
    }
    void HighlightSkillRange(int id)
    {
        SpawnHighlightPrefab(id);
        for (int i = 1; i < range + 1; i++)
        {
            SpawnHighlightPrefab((id + i) % gm.map.fields.Count);
            if (id - i < 0)
                SpawnHighlightPrefab(gm.map.fields.Count + (id - i));
            else
                SpawnHighlightPrefab(id - 1);
        }
    }
    bool IsInRangeOfBase(int fieldIndex)
    {
        int minIndex = gm.map.baseFieldsIndexes[turn] - rangeOfFractionSkills;
        if (minIndex < 0)
            minIndex = gm.map.fields.Count - minIndex;
        int maxIndex = gm.map.baseFieldsIndexes[turn] + rangeOfFractionSkills % gm.map.fields.Count;

        if (minIndex > maxIndex)
            return fieldIndex >= minIndex || fieldIndex <= maxIndex;
        else
            return fieldIndex >= minIndex && fieldIndex <= maxIndex;
    }
    public void ActiveSkill(int turn, Skill skill, int skillIndex)
    {
        this.turn = turn;
        currSkill = skill;
        range = currSkill.boundaryRange;
        skillsIndex = skillIndex;
        lastFieldID = -1;
        UIScript.ToggleSkillHighlight(skillIndex, true);
        showRange = true;
    }
    void CancelShowingRange()
    {
        pool.ReclaimAllHighlightObjects();
        showRange = false;
        UIScript.ToggleSkillHighlight(skillsIndex, false);
        isCleaningNecessary = false;
    }
    void UseSkill(int index)  // PAMIETAC !!! aby efekty immuna wstawiac insertem(0) a retsze addem przez co immuny beda wyzej w tablicy(liscie)
    {
        gm.SkillCooldown(skillsIndex);
        string killSound = "random string";   // nazwa pliku dzwiekowego , w przypadku podania blednej nazwy klasa sound uruchomi domyslny dzwiek
        // jesli skill naklada efekty w domyslny sposob(wszystkie na ten sam obszar)
        bool defaultAction = true;
        bool killUnits = false;
        //lista efektow dodanych na zaznaczone pionki
        List<Effect> e = new List<Effect>();
        //tablica indexow pol na ktorych roztacza sie skil
        //int[] indexes = new int[2 * range + 1];
        List<int> indexes = new List<int>();
        indexes.Add(index);
        for (int i = 0; i < (2 * range); i++)
        {
            indexes.Add(0);
        }
        for (int i = 0; i < range; i++)
        {
            //Debug.Log("Rozmiar tablicy: " + (range+ 1) + " indexes[" + (2 * i + 1) + "] = " + ((index + i) % gm.map.fields.Count));
            indexes[2 * i + 1] = (index + i + 1) % gm.map.fields.Count;
            if (index - i - 1 < 0) indexes[2 * i + 2] = gm.map.fields.Count + (index - i - 1);
            else indexes[2 * i + 2] = index - i - 1;
        }
        if(currSkill.effectType != Effect.EffectType.Other)
        {
            e.Add(new Effect(currSkill.effectType, currSkill.duration));
        }
        else if (currSkill.name == "Purification Blessing")
        {
            e.Add(new Effect(Effect.EffectType.SlowImmune, currSkill.duration));
            e.Add(new Effect(Effect.EffectType.StunImmune, currSkill.duration));
        }
        else if (currSkill.name == "Thunder Strike")
        {
            defaultAction = false;
            e.Add(new Effect(Effect.EffectType.Acceleration, currSkill.duration));
            for (int i = 0; i < indexes.Count; i++)
            {
                for (int p = 0; p < gm.pawns.Count; p++)
                {
                    Pawn pawn = gm.pawns[p].GetComponent<Pawn>();
                    if (indexes[i] == pawn.CurrFieldIndex)
                    {
                        if (i == 0)  // centrum
                        {
                            if(!pawn.CheckHasEffect(Effect.EffectType.DamageImmune))
                                gm.DestroyPawnIfOnField(indexes[0], "thunderSound");
                        }
                        else
                        {
                            if (!e[0].immuneEffect) pawn.effects.Add(e[0]);
                            else pawn.effects.Insert(0, e[0]);
                            //gm.pawns[p].GetComponent<Pawn>().ApplyEffect(e[0]);
                        }
                    }
                }
            }
        }
        else if (currSkill.name == "Rain Of Arrows")
        {
            killUnits = true;
            killSound = "arrowSound";
        }
        else if(currSkill.name == "Cavalry Charge" || currSkill.name == "Bloody Ambush")
        {
            killUnits = true;
            List<int> indexesToRemove = new List<int>();
            for (int i = 0; i < indexes.Count; i++)
            {
                if(Random.value <= currSkill.successChance)
                {
                    //Debug.Log("Random.value <= 0.5f");
                    indexesToRemove.Add(indexes[i]);
                }
            }
            foreach (var ind in indexesToRemove)
            {
                indexes.Remove(ind);
            }
        }

        if (defaultAction)
        {
            foreach (int i in indexes)
            {
                for (int p = 0; p < gm.pawns.Count; p++)
                {
                    Pawn pawn = gm.pawns[p].GetComponent<Pawn>();
                    if (i == pawn.CurrFieldIndex)
                    {
                        if (currSkill.targetLimit == Skill.TargetType.Allies && pawn.Owner != turn) 
                            continue;
                        if (currSkill.targetLimit == Skill.TargetType.Enemies && pawn.Owner == turn) 
                            continue;
                        if (killUnits)
                        {
                            if(!pawn.CheckHasEffect(Effect.EffectType.DamageImmune))
                                gm.DestroyPawnIfOnField(i, killSound);
                        }
                        else
                        {
                            foreach (Effect ef in e)
                            {
                                if (!ef.immuneEffect) 
                                    pawn.effects.Add(ef);
                                else 
                                    pawn.effects.Insert(0, ef);
                            }
                        }
                       
                    }
                }
            }
        }
    }
}
