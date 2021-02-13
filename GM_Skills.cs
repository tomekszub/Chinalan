using System.Collections.Generic;
using UnityEngine;

public class GM_Skills : MonoBehaviour
{
    GameManager gm;
    public bool showRange = false;
    int skillsIndex = 0;
    int range = 0;
    int turn = 0;
    bool temp = false;
    int lastFieldID = 0;
    Skill currSkill;
    void Awake()
    {
        gm = GetComponent<GameManager>();
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
                if (Input.GetMouseButtonUp(0) && temp)
                {
                    UseSkill(id);
                    DeselectFields();
                    showRange = false;
                }
                if (id != lastFieldID)
                {
                    //nowe pole zaznaczone
                    DeselectFields();
                    temp = false;
                }
                if (id >= gm.map.baseFieldsIndexes[turn] && id <= gm.map.baseFieldsIndexes[turn] + gm.map.numberOfFieldsInEveryQuarter- 1 && !temp)
                {
                    gm.map.fields[id].GetComponent<Renderer>().material.color = Color.black;
                    lastFieldID = id;
                    temp = true;
                }
                if (Input.GetMouseButtonUp(1))
                {
                    for (int i = 1; i < range + 1; i++)
                    {
                        gm.map.fields[(id + i) % gm.map.fields.Count].GetComponent<Pole>().BackToOriginalColor();
                        if (id - i < 0) gm.map.fields[gm.map.fields.Count + (id - i)].GetComponent<Pole>().BackToOriginalColor();
                        else gm.map.fields[id - i].GetComponent<Pole>().BackToOriginalColor();
                    }
                }
                if (Input.GetMouseButtonDown(1) && temp)
                {
                    for (int i = 1; i < range + 1; i++)
                    {
                        gm.map.fields[(id + i) % gm.map.fields.Count].GetComponent<Renderer>().material.color = Color.black;
                        if (id - i < 0) gm.map.fields[gm.map.fields.Count + (id - i)].GetComponent<Renderer>().material.color = Color.black;
                        else gm.map.fields[id - i].GetComponent<Renderer>().material.color = Color.black;
                    }
                    temp = true;
                }
            }
            else
            {
                if(Input.GetMouseButtonDown(0))
                {
                    temp = true;
                    showRange = false;
                }
                if(temp)
                {
                    //Debug.Log("spowrotem na normalny");
                    gm.map.fields[lastFieldID].GetComponent<Pole>().BackToOriginalColor();
                    for (int i = 1; i < range + 1; i++)
                    {
                        gm.map.fields[(lastFieldID + i) % gm.map.fields.Count].GetComponent<Pole>().BackToOriginalColor();
                        if (lastFieldID - i < 0) gm.map.fields[gm.map.fields.Count + (lastFieldID - i)].GetComponent<Pole>().BackToOriginalColor();
                        else gm.map.fields[lastFieldID - i].GetComponent<Pole>().BackToOriginalColor();
                    }
                    temp = false;
                }
            }
        }
    }
    public void ActiveSkill(int turn, Skill skill, int skillIndex)
    {
        this.turn = turn;
        currSkill = skill;
        range = currSkill.boundaryRange;
        skillsIndex = skillIndex;
        showRange = true;
    }
    void DeselectFields()
    {
        gm.map.fields[lastFieldID].GetComponent<Pole>().BackToOriginalColor();
        for (int i = 1; i < range + 1; i++)
        {
            gm.map.fields[(lastFieldID + i) % gm.map.fields.Count].GetComponent<Pole>().BackToOriginalColor();
            if (lastFieldID - i < 0) gm.map.fields[gm.map.fields.Count + (lastFieldID - i)].GetComponent<Pole>().BackToOriginalColor();
            else gm.map.fields[lastFieldID - i].GetComponent<Pole>().BackToOriginalColor();
        }
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
