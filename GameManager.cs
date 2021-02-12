using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] cameras;
    public Map map;
    public Pawn pawnPrefab;
    public List<Stats> playerStats = new List<Stats>();
    public Material[] playerColors;
    [HideInInspector]
    public List<Pawn> pawns = new List<Pawn>();

    [SerializeField]
    GameObject musicPlayer;
    [SerializeField]
    [Tooltip("Flag's model which points to a destination place for a pawns mvoe")]
    GameObject destinationFlag;
    Pawn currentlyControlledPawn;
    int turn = 0;
    int currRollValue;
    int movesInTurn = 0;
    List<Player> players = new List<Player>();
    bool finishedMoving = true;
    bool debugModeOn = false;
    bool isTimeToChoose = false;
    // array of ids which are added to newly created pawns to distinguish them
    int[] ids = { 0, 0, 0, 0 };
    GM_Skills gmSkills;
    Sounds soundsScript;
    UIManager UIScript;
    MapGenerator mapGenerator;
    

    void Awake ()
    {
        UIScript = GetComponent<UIManager>();
        soundsScript = GetComponent<Sounds>();
        gmSkills = GetComponent<GM_Skills>();
        mapGenerator = GetComponent<MapGenerator>();
    }
    void ClearAllPawns()
    {
        for (int i = pawns.Count - 1; i >= 0; i--)
        {
            Destroy(pawns[i]);
        }
        pawns.Clear();
    }
	void CameraToggleInput()
    {
        if (Input.GetKeyUp(KeyCode.F1))
            SwitchCameraView(0);
        if (Input.GetKeyUp(KeyCode.F2))
            SwitchCameraView(1);
    }
	void Update ()
    {
        CameraToggleInput();

        #region DebugMode
        if (debugModeOn)
        {
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                Roll(1);
            }
            if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                Roll(2);
            }
            if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                Roll(3);
            }
            if (Input.GetKeyUp(KeyCode.Alpha4))
            {
                Roll(4);
            }
            if (Input.GetKeyUp(KeyCode.Alpha5))
            {
                Roll(5);
            }
            if (Input.GetKeyUp(KeyCode.Alpha6))
            {
                Roll(6);
            }
        }
        #endregion
            
        if(isTimeToChoose && !gmSkills.showRange)
        {
            CheckForMouseInput();
        }
        if(destinationFlag.activeSelf)
        {
            if (Input.GetMouseButtonUp(1)) destinationFlag.SetActive(false);
        }
        if (finishedMoving)
        {
            UIScript.SetRollButton(true); 
            finishedMoving = false;
        }
	}
    void CheckForMouseInput()
    {
        bool leftMB, rightMB;
        leftMB = Input.GetMouseButtonDown(0);
        rightMB = Input.GetMouseButtonDown(1);
        if (leftMB || rightMB)
        { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = 1 << 9 | 1 << 10;
            if (Physics.Raycast(ray, out hit, 1000f, layerMask))
            {
                GameObject go = hit.transform.gameObject;
                if (leftMB)
                {
                    UIScript.SetNotificationText("");
                    isTimeToChoose = false;
                    ObjectsHasBeenClicked(go);
                }
                if(rightMB) ShowDestinationFlag(go);
            }
        }
    }
    void Roll(int? x = null)
    {
        int rollResult;
        // if we are specifing roll number instead of randomizing it
        if (x != null && x >= 1 && x <= 6)
        {
            if (!UIScript.IsRollButtonInteractable()) 
                return;
            rollResult = (int)x;
        }
        else rollResult = Random.Range(1, 7);
        UIScript.SetRollButton(false);
        playerStats[turn].AddRolledNumber(rollResult);
        currRollValue = rollResult;
        UIScript.SetDiceText(rollResult.ToString());
        movesInTurn++;

        bool allCannotMove = true;
        // variable, in which we cache number of moves in case we need it later in invoking method Move
        int? savedMoves = 0; 
        // check if any of the currents player pawns can move
        if (players[turn].pawnsInGame > 0)
        {
            foreach (Pawn pawn in pawns)
            {
                if (pawn.Owner != turn)
                    continue;
                if ((savedMoves = GetMovesCountIfMoveIsPossible(pawn, out _)) != null)
                {
                    allCannotMove = false;
                    break;
                }
            }
        }
        if (allCannotMove)
        {
            if(rollResult == 6 && CanSpawn())
            {
                DestroyPawnIfOnField(map.baseFieldsIndexes[turn], "capturingSound");
                SpawnPawn();
                if (movesInTurn < 3) UIScript.SetRollButton(true);
                else EndOfTurn();
            }
            else
            {
                UIScript.SetNotificationText("No option to move.");
                EndOfTurn();
                return;
            }
        }
        else
        {
            if (rollResult != 6 && players[turn].pawnsInGame == 1)
            {
                Pawn p = GetFirstPawnFromPawns(turn);
                StartCoroutine(Move(p, (int)savedMoves));
            }
            else
            {
                if (rollResult != 6)
                    UIScript.SetNotificationText("Choose which pawn of yours you want to move.");
                else
                    UIScript.SetNotificationText("Click on your pawn or base to create new pawn.");
                isTimeToChoose = true;
                return;
            }
        }
    }
    bool CanSpawn()
    {
        return !IsFriendlyOrImmunePawnInField(map.baseFieldsIndexes[turn], false) && players[turn].pawnsInGame != 4;
    }
    IEnumerator Move(Pawn pawn, int movesToDo)
    {
        currentlyControlledPawn = pawn;
        Vector3 newPawnPosition;
        for (int i = 0; i < movesToDo; i++)
        {
            if (currentlyControlledPawn.IsSafe)
            {
                currentlyControlledPawn.CurrFieldIndex++;
                newPawnPosition = map.safeFields[turn * 4 + currentlyControlledPawn.CurrFieldIndex].transform.position;
            }
            else
            {
                // if pawn is at the entrance to their base
                if (currentlyControlledPawn.CurrFieldIndex == map.safeEntranceField[turn])
                {
                    newPawnPosition = map.safeFields[turn * 4].transform.position;
                    currentlyControlledPawn.IsSafe = true;
                    currentlyControlledPawn.CurrFieldIndex = 0;
                }
                else
                {
                    // we are checking if there is an enemy pawn there
                    if (i == movesToDo - 1)  // turn's last move
                    {
                        // calcualting pole index, ensuring that arrays index wont be outside of boudnries 
                        int indexToDelete = (currentlyControlledPawn.CurrFieldIndex == (map.fields.Count - 1)) ? 0 : (currentlyControlledPawn.CurrFieldIndex + 1);
                        DestroyPawnIfOnField(indexToDelete, "capturingSound");
                    }
                    currentlyControlledPawn.CurrFieldIndex++;
                    if (currentlyControlledPawn.CurrFieldIndex >= map.fields.Count)
                        currentlyControlledPawn.CurrFieldIndex = 0;
                    newPawnPosition = map.fields[currentlyControlledPawn.CurrFieldIndex].transform.position;
                }
            }
            currentlyControlledPawn.SetNewPosition(newPawnPosition);
            while (currentlyControlledPawn.IsMoving)
            {
                yield return new WaitForSeconds(0.03f);
            }
        }
        if (currRollValue == 6  && movesInTurn < 3) UIScript.SetRollButton(true);
        else
        {
            EndOfTurn();
        }
    }
    void EndOfTurn()
    {
        UIScript.SetNotificationText("");
        UIScript.SetNextTurnButton(true);
    }
    void UpdateEffectsCooldowns()
    {
        foreach (Pawn pawn in pawns)
        {
            if (pawn.Owner == turn)
            {
                List<int> indexesToRemove = new List<int>();
                for (int e = 0; e < pawn.effects.Count; e++) 
                {
                    if (pawn.effects[e].turnsToDisappear == 1) 
                        indexesToRemove.Add(e);
                    else 
                        pawn.effects[e].turnsToDisappear--;
                }
                for(int i = 0; i < indexesToRemove.Count; i++)
                {
                    pawn.effects.Remove(pawn.effects[indexesToRemove[i]]);
                }
            }
        }
    }
    void UpdateSkillsCooldowns()
    {
        for (int i = 0; i < players[turn].skills.Count; i++)
        {
            if (players[turn].skills[i].currCooldown > 0)
                players[turn].skills[i].currCooldown--;
        }
    }
    int? GetMovesCountIfMoveIsPossible(Pawn p, out bool IsSafe)
    {
        int maxForwardFields;
        int fieldsMoved;
        int targetFieldIndex;
        bool movedOverEntrance = true;
        if(p.IsSafe)
        {
            maxForwardFields = 3 - p.CurrFieldIndex;
            fieldsMoved = currRollValue;
            targetFieldIndex = p.CurrFieldIndex + fieldsMoved;
        }
        else
        {
            if (p.CurrFieldIndex > map.safeEntranceField[turn]) 
                maxForwardFields = map.fields.Count - (p.CurrFieldIndex - map.safeEntranceField[turn]);
            else 
                maxForwardFields = map.safeEntranceField[turn] - p.CurrFieldIndex;

            maxForwardFields += map.numberOfSafehouseFields;
            //
            //fieldsMoved = GetMovesAmount(p, out movedOverEntrance);
            movedOverEntrance = false;
            fieldsMoved = 0;
            float tempMovement = currRollValue + (p.CheckHasEffect(Effect.EffectType.AdditionalMovement) ? 1 : 0);
            int currField = p.CurrFieldIndex;
            float cost;
            while (tempMovement > 0)
            {
                cost = p.GetMovementCost(map.fields[currField].GetComponent<Pole>().terrainType);
                tempMovement -= cost;
                if (tempMovement >= 0)
                {
                    fieldsMoved++;
                    if (currField == map.safeEntranceField[turn])
                    {
                        movedOverEntrance = true;
                        fieldsMoved += (int)tempMovement;
                        break;
                    }
                    if (currField == map.fields.Count - 1)
                        currField = 0;
                    else
                        currField++;
                }
            }
            //
            if (movedOverEntrance)
                targetFieldIndex = fieldsMoved - (map.safeEntranceField[turn] - p.CurrFieldIndex) - 1;
            else
                targetFieldIndex = (p.CurrFieldIndex + fieldsMoved) % map.fields.Count;
        }
        IsSafe = movedOverEntrance;

        if (fieldsMoved > maxForwardFields)
            return null;

        if (IsFriendlyOrImmunePawnInField(targetFieldIndex, movedOverEntrance))
            return null;

        return fieldsMoved;
    }
    public void DestroyPawnIfOnField(int positionIndex, string soundName)
    {
        for(int i = 0; i < pawns.Count; i++)
        {
            if (pawns[i].CurrFieldIndex == positionIndex && !pawns[i].IsSafe)
            {
                Destroy(pawns[i].gameObject);
                playerStats[turn].SuccesfullKill();
                playerStats[pawns[i].Owner].PawnDestroyed();
                players[pawns[i].Owner].pawnsInGame -= 1;
                pawns.RemoveAt(i);
                soundsScript.PlaySound(soundName);
            }
        }
    }
    void ShowDestinationFlag(GameObject selectedObject)
    {
        if(selectedObject.CompareTag("Base"))
        {
            destinationFlag.SetActive(true);
            destinationFlag.transform.position = new Vector3(map.fields[map.baseFieldsIndexes[turn]].transform.position.x, map.fields[map.baseFieldsIndexes[turn]].transform.position.y + 1.5f, map.fields[map.baseFieldsIndexes[turn]].transform.position.z);
        }
        else
        {
            Pawn p = selectedObject.GetComponent<Pawn>();
            bool movedOverEntrence;
            int? fieldsMoved = GetMovesCountIfMoveIsPossible(p, out movedOverEntrence);
            if (fieldsMoved != null)
            {
                destinationFlag.SetActive(true);
                if(p.IsSafe)
                {
                    int safeFieldIndex = (4 * turn) + p.CurrFieldIndex + currRollValue;
                    destinationFlag.transform.position = GetSafeFieldPosition(safeFieldIndex, 1.5f);
                    return;
                }
                if (movedOverEntrence)
                {
                    int safeFieldIndex = (4 * turn) + (p.CurrFieldIndex + (int)fieldsMoved) - map.safeEntranceField[turn] - 1;
                    destinationFlag.transform.position = GetSafeFieldPosition(safeFieldIndex, 1.5f);
                }
                else
                {
                    int fieldIndex = (p.CurrFieldIndex + (int)fieldsMoved) % map.fields.Count;
                    destinationFlag.transform.position = new Vector3(map.fields[fieldIndex].transform.position.x, map.fields[fieldIndex].transform.position.y + 1.5f, map.fields[fieldIndex].transform.position.z);
                }
            }
        }
    }
    Vector3 GetSafeFieldPosition(int index, float heightOffset)
    {
        return new Vector3(map.safeFields[index].transform.position.x, map.safeFields[index].transform.position.y + heightOffset, map.safeFields[index].transform.position.z);
    }
    void ObjectsHasBeenClicked(GameObject selectedObject)
    {
        bool rolledSix = currRollValue == 6;
        bool isBaseObject = selectedObject.GetComponent<Base>()?.owner == turn;
        if (rolledSix && isBaseObject)
        {
            if (CanSpawn())
            {
                DestroyPawnIfOnField(map.baseFieldsIndexes[turn], "capturingSound");
                SpawnPawn();
                if (movesInTurn < 3) UIScript.SetRollButton(true);
                else EndOfTurn();
            }
            else
            {
                UIScript.SetNotificationText("You have reached your max number of pawns on the battlefield or something is blocking base entry.");
                isTimeToChoose = true;
            }
            return;
        }
        if(!rolledSix && isBaseObject)
        {
            UIScript.SetNotificationText("You need to roll 6 to send characters from your base.");
            isTimeToChoose = true;
            return;
        }
        if (selectedObject.GetComponent<Pawn>()?.Owner != turn)
        {
            UIScript.SetNotificationText("It is not your pawn.");
            isTimeToChoose = true;
            return;
        }
        int? moves = GetMovesCountIfMoveIsPossible(selectedObject.GetComponent<Pawn>(), out _);
        if (moves != null)
        {
            for (int i = 0; i < pawns.Count; i++)
            {
                if(pawns[i].name == selectedObject.name)
                {
                    StartCoroutine(Move(pawns[i], (int)moves));
                    break;
                }
            }
        }
        else
        {
            UIScript.SetNotificationText("This pawn cant move on that field.");
            isTimeToChoose = true;
        }
    }
    void SpawnPawn()
    {
        GameObject go = Instantiate(pawnPrefab.gameObject, new Vector3(map.fields[map.baseFieldsIndexes[turn]].transform.position.x, map.fields[map.baseFieldsIndexes[turn]].transform.position.y + 0.5f, map.fields[map.baseFieldsIndexes[turn]].transform.position.z), Quaternion.identity);
        go.GetComponentInChildren<Renderer>().material = playerColors[turn];
        Pawn pawn = go.GetComponent<Pawn>();
        pawn.Owner = turn;
        pawn.CurrFieldIndex = map.baseFieldsIndexes[turn];
        go.transform.name = "P" + turn + " pawn " + ids[turn];
        ids[turn] += 1;
        pawns.Add(pawn);
        players[turn].pawnsInGame += 1;
    }
    bool IsFriendlyOrImmunePawnInField(int fieldIndex, bool isInSafehouse)
    {
        foreach (Pawn pawn in pawns)
        {
            // jesli pole na ktore ma sie ruszyc pionek i pole na ktorym jeden z postawionych wczesniej pionkow to to samo pole
            if(fieldIndex == pawn.CurrFieldIndex && pawn.IsSafe == isInSafehouse)
            {
                // jesli pionek na danym polu jest pionkiem tego samego gracza ktorego jest tura albo pionek jest immune na zbicie
                if (pawn.Owner == turn || pawn.CheckHasEffect(Effect.EffectType.DamageImmune)) return true;
            }
        }
        return false;
    }
    void SwitchCameraView(int id)
    {
        bool isCameraSet = false;
        for(int i = 0; i < cameras.Length;i++)
        {
            cameras[i].SetActive(false);
            if(i == id)
            {
                isCameraSet = true;
                cameras[i].SetActive(true);
            }
        }
        // jesli id zostalo zle podane to domyslan kamera to kamera 0
        if (!isCameraSet)
            cameras[0].SetActive(true);
    }
    // public functions
    public void UseSkill(int slotIndex)
    {
        gmSkills.ActiveSkill(turn, players[turn].skills[slotIndex], slotIndex);
    }
    public void SkillCooldown(int slotIndex)
    {
        players[turn].skills[slotIndex].TurnOnCooldown();
        UIScript.UpdateSkillSlotAppearance(slotIndex, players[turn].skills[slotIndex].currCooldown);
    }
    public void SetMusicPlaying(GameObject checkBoxObject)
    {
        musicPlayer.SetActive(checkBoxObject.activeSelf);
    }
    public void SetDebugMode(GameObject checkBoxObject)
    {
        debugModeOn = checkBoxObject.activeSelf;
    }
    public Player GetCurrentPlayer()
    {
        return players[turn];
    }
    public Pawn GetFirstPawnFromPawns(int playerIndex)
    {
        for (int i = 0; i < pawns.Count; i++)
        {
            if (pawns[i].Owner == playerIndex) return pawns[i];
        }
        return null;
    }
    public void NextTurn()
    {
        UIScript.SetDiceText("");
        UIScript.SetNextTurnButton(false);
        UpdateSkillsCooldowns();
        UpdateEffectsCooldowns();
        turn++;
        if (turn == 4)
            turn = 0;
        UIScript.UpdateSkillsIcons(players[turn].skills);
        UIScript.SetPlayerNameLabel(playerStats[turn].name);
        movesInTurn = 0;
        finishedMoving = true;
    }
    public void RollButtonPressed()
    {
        Roll(null);
    }
    public void StartGame(string[] settings)  // array: [index] 0-3 - fraction name 4-7 - players names
    {
        // clearing existing playersStats then adding new ones (maybe i should just overwrite existing data?)
        playerStats.Clear();
        for (int i = 4; i < 8; i++)
        {
            playerStats.Add(new Stats(settings[i], 0, 0, 0, 0, 0, 0, 0, 0));
        }
        UIScript.UpdateStatsPanel();
        // reseting auxilary variables
        movesInTurn = 0;
        turn = 0;
        isTimeToChoose = false;
        for (int i = 0; i < ids.Length; i++)
        {
            ids[i] = 0;
        }
        finishedMoving = true;
        currentlyControlledPawn = null;
        ClearAllPawns();
        UIScript.ResetGameUI(playerStats[0].name);
        // clearing existing playersStats then adding new ones (maybe i should just overwrite existing data?)
        players.Clear();
        for (int i = 0; i < 4; i++)
        {
            players.Add(new Player(0, null));
            players[i].fractionType = FractionDB.FindByName(settings[i]);
            players[i].skills.Add(new Skill(SkillDB.skills[players[i].fractionType.skills[0]]));
            players[i].skills.Add(new Skill(SkillDB.skills[players[i].fractionType.skills[1]]));
        }
        UIScript.UpdateSkillsIcons(players[0].skills);
        map = mapGenerator.GenerateMap();
    }
    // 792 lines
}
