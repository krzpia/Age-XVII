using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JetBrains.Annotations;

public class BattleField : MonoBehaviour
{
    public GameObject BattleField_Object;
    public GameObject PlayerGraveyard;
    public GameObject OpponentGraveyard;
    public GameObject Slot_Object;
    public GameObject Temp_Unit_Container_Object;
    public Button NextTurnButton;
    public GameObject AttackPanel;
    public Button ShockAttackButton;
    public Button PositionAttackButton;
    public Button BackAttackButton;
    public bool position_attack_chosen;
    public bool shock_attack_chosen;
    public bool back_attack_chosen;
    public bool siege_attack_chosen;
    public Text shock_att_value;
    public Text position_att_value;
    public Text opp_shock_or_position_att_value;
    public Text opp_position_att_value;
    private int opp_m_or_r_att_val;

    // Start is called before the first frame update
    void Start()
    {
        AttackPanel.SetActive(false);
        int x_counter = 0;
        int y_counter = 0;
        for (int i = 0; i < 32; i++)
        {

            GameObject slot = Instantiate(Slot_Object, new Vector2(0, 0), Quaternion.identity);
            slot.transform.SetParent(BattleField_Object.transform, false);
            slot.transform.GetComponent<Image>().color = new Color32(255, 255, 255, 30);
            slot.GetComponent<CardSlot>().slot_id = i;
            slot.GetComponent<CardSlot>().x = x_counter;
            slot.GetComponent<CardSlot>().y = y_counter;
            if (y_counter > 1)
            {
                slot.GetComponent<CardSlot>().act_player_zone = true;
            }

            x_counter++;
            if (x_counter > 7)
            {
                x_counter = 0;
                y_counter++;
            }
        }
    }
    public void NextTurn()
    {
        /// REPOSITIONING UNITS ON BATTLE FIELD
        List<GameObject> unit_ico_list = GetListofAllUnitsIconsInBattlefield();
        PickUpAllUnits(unit_ico_list);
        ReverseAllUnitsBattleSlotPositions(unit_ico_list);
        PositionAllUnits(unit_ico_list);
        unit_ico_list.Clear();
        /// RESETING MOVEMENT AND ATTACK BOOL OF PLAYER UNITS
        List<GameObject> active_player_unit_ico_list = GetListofActivePlayerUnitsIconsInBattlefield();
        RefreshMovementofUnitIcoList(active_player_unit_ico_list);
        RefreshAttackofUnitIcoList(active_player_unit_ico_list);
        active_player_unit_ico_list.Clear();

    }

    public void KillUnit(GameObject unit_ico)
    {
        unit_ico.GetComponent<Animator>().SetBool("DeathAnim", true);
        StartCoroutine(Deactivatein2sec(unit_ico));
        unit_ico.GetComponent<CardIco>().interactable = false;
    }

    IEnumerator Deactivatein2sec(GameObject unit_ico)
    {
        yield return new WaitForSeconds(2f);
        //+++Debug.Log("KILLING UNIT WITH BATTLEFIELD ROOT METHOD");
        if (unit_ico.GetComponent<CardIco>().owner == TurnSystem.Active_Player)
        {
            //Debug.Log("UNIT OWNER = PLAYER");
            unit_ico.transform.SetParent(PlayerGraveyard.transform, false);
        }
        else
        {
            //Debug.Log("UNIT OWNER = OPPONENT");
            unit_ico.transform.SetParent(OpponentGraveyard.transform, false);
        }
        unit_ico.SetActive(false);
    }

    public GameObject GetUnitIcoFromSlotbyPosition(int slot_x, int slot_y)
    {
        List<GameObject> all_units = GetListofAllUnitsIconsInBattlefield();
        foreach (GameObject unit in all_units)
        {
            if (unit.GetComponent<CardIco>().slot_x == slot_x && unit.GetComponent<CardIco>().slot_y == slot_y)
            {
                return unit;
            }
        }
        Debug.LogWarning("UNIT NOT FOUND!");
        return null;
    }

    public GameObject GetSlotbyPosition(int slot_x, int slot_y)
    {
        List<GameObject> all_slots = GetListofAllSlots();
        foreach (GameObject slot in all_slots)
        {
            if(slot.GetComponent<CardSlot>().x == slot_x && slot.GetComponent<CardSlot>().y == slot_y)
            {
                Debug.Log("ZWRACAM SLOT " + slot + "Z POZYCJI" + slot_x + "," + slot_y);
                return slot;
            }
        }
        Debug.LogWarning("NIE ZNALAZLEM SLOTU!");
        return null;
    }

    public List<GameObject> GetListofUnitInRangeofAttackbyPosition(int slot_x, int slot_y)
    {
        List<GameObject> units_in_range = new List<GameObject>();
        List<GameObject> all_units = GetListofAllUnitsIconsInBattlefield();
        //Debug.Log("ALL UNITS COUNT: " + all_units.Count);
        foreach (GameObject unit in all_units)
        {
            if (unit.GetComponent<CardIco>().slot_x == (slot_x + 1) && unit.GetComponent<CardIco>().slot_y == slot_y)
            {
                if (unit.GetComponent<CardIco>().owner == TurnSystem.Op_Player)
                {
                    units_in_range.Add(unit);
                }
            }
            if (unit.GetComponent<CardIco>().slot_x == (slot_x - 1) && unit.GetComponent<CardIco>().slot_y == slot_y)
            {
                if (unit.GetComponent<CardIco>().owner == TurnSystem.Op_Player)
                {
                    units_in_range.Add(unit);
                }
            }
            if (unit.GetComponent<CardIco>().slot_x == slot_x && unit.GetComponent<CardIco>().slot_y == (slot_y + 1))
            {
                if (unit.GetComponent<CardIco>().owner == TurnSystem.Op_Player)
                {
                    units_in_range.Add(unit);
                }
            }
            if (unit.GetComponent<CardIco>().slot_x == slot_x && unit.GetComponent<CardIco>().slot_y == (slot_y - 1))
            {
                if (unit.GetComponent<CardIco>().owner == TurnSystem.Op_Player)
                {
                    units_in_range.Add(unit);
                }
            }
        }
        //Debug.Log("IN ATTACK RANGE: " + units_in_range.Count + "UNITS");
        return units_in_range;
    }

    public int ComputeDistanceBetweenTwoUnits(GameObject first_Unit, GameObject second_Unit)
    {
        int x_distance = Math.Abs(first_Unit.GetComponent<CardIco>().slot_x - second_Unit.GetComponent<CardIco>().slot_x);
        int y_distance = Math.Abs(first_Unit.GetComponent<CardIco>().slot_y - second_Unit.GetComponent<CardIco>().slot_y);
        int dist = x_distance + y_distance;
        //Debug.Log("DISTANCE BETWEEN ATTACKING UNITS: " + dist);
        return dist;
    }

    IEnumerator PutWoundfor2sec(GameObject unit)
    {
        Debug.Log("PUT WOUND FOR 2 sec");
        unit.GetComponent<CardIco>().woundObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        unit.GetComponent<CardIco>().woundObject.SetActive(false);
    }

    public void AttackEngine(GameObject attack_unit, GameObject defence_unit)
    {
        if (ComputeDistanceBetweenTwoUnits(attack_unit, defence_unit) > 1 && ComputeDistanceBetweenTwoUnits(attack_unit, defence_unit) < 4)
        {
            Debug.Log("SIEGE ATTACK");
            siege_attack_chosen = true;
            shock_attack_chosen = false;
            position_attack_chosen = false;
            back_attack_chosen = false;
            GameObject attacker_slot = GetSlotbyPosition(attack_unit.GetComponent<CardIco>().slot_x, attack_unit.GetComponent<CardIco>().slot_y);
            GameObject defender_slot = GetSlotbyPosition(defence_unit.GetComponent<CardIco>().slot_x, defence_unit.GetComponent<CardIco>().slot_y);
            // 1. TURN ON SIEGE CORUTINE
            StartCoroutine(SiegeAnimation(attack_unit, defence_unit, attacker_slot, defender_slot));
            // 4. UPDATE BATTLEFIELD
            ResetSlotsColor();
            foreach (GameObject unit in GetListofAllUnitsIconsInBattlefield())
            {
                unit.GetComponent<CardIco>().UnmarkPosibleEnemyUnitsAsTargets();
            }
            // 5. KONIEC FAZY ATAKU!
            UnblockAllInteraction();
            // KONIEC FAZY ATAKU!
        }
        else if (ComputeDistanceBetweenTwoUnits(attack_unit, defence_unit) == 1)
        {
            Debug.Log("ATTACK ENGINE STARTING");
            GameObject attacker_slot = GetSlotbyPosition(attack_unit.GetComponent<CardIco>().slot_x, attack_unit.GetComponent<CardIco>().slot_y);
            GameObject defender_slot = GetSlotbyPosition(defence_unit.GetComponent<CardIco>().slot_x, defence_unit.GetComponent<CardIco>().slot_y);
            // 1. TURN ON CORUTINE WITH PANEL 
            StartCoroutine(AttackWait(attack_unit, defence_unit, attacker_slot, defender_slot));
            // 4. UPDATE BATTLEFIELD
            ResetSlotsColor();
            foreach (GameObject unit in GetListofAllUnitsIconsInBattlefield())
            {
                unit.GetComponent<CardIco>().UnmarkPosibleEnemyUnitsAsTargets();
            }
            // 5. KONIEC FAZY ATAKU!
            UnblockAllInteraction();
            // KONIEC FAZY ATAKU!
        }
        else
        {
            Debug.Log("POZA ZASIEGIEM LUB SELF");
        }
    }

    IEnumerator SiegeAnimation(GameObject attack_unit, GameObject defence_unit, GameObject attacker_slot, GameObject defender_slot)
    {
        // PLAY SOUND
        SoundManagerScript.PlaySound("siege");
        // PLAY ANIMATION
        attack_unit.GetComponent<Animator>().SetBool("SiegeAnim", true);
        yield return new WaitForSeconds(2.5f);
        attack_unit.GetComponent<Animator>().SetBool("SiegeAnim", false);
        SiegeAttackCalculation(attack_unit, defence_unit, attacker_slot, defender_slot);
    }

    public void SiegeAttackCalculation(GameObject attack_unit, GameObject defence_unit, GameObject attacker_slot, GameObject defender_slot)
    {
        // 2. REDUCING MOVEMENT
        attack_unit.transform.GetComponent<CardIco>().ReduceMovement(1);
        // 2.1 HAS ATTACKED BOOL PUT INTO TRUE
        attack_unit.transform.GetComponent<CardIco>().has_attacked = true;
        // 2.2 ATTACK RESULTS
        defence_unit.GetComponent<CardIco>().act_def -= (attack_unit.GetComponent<CardIco>().s_att + attack_unit.GetComponent<CardIco>().s_att_mod);
        // ADD WOUND corutine
        StartCoroutine(PutWoundfor2sec(defence_unit));
        CalculateBattleEffects(attack_unit, defence_unit, attacker_slot, defender_slot);
    }

    IEnumerator AttackWait(GameObject attack_unit, GameObject defence_unit, GameObject attacker_slot, GameObject defender_slot)
    {
        if (defence_unit.GetComponent<CardIco>().r_att >= defence_unit.GetComponent<CardIco>().m_att)
        {
            opp_m_or_r_att_val = defence_unit.GetComponent<CardIco>().r_att;
        }
        else
        {
            opp_m_or_r_att_val = defence_unit.GetComponent<CardIco>().m_att;
        }
        ChooseAttackType(attack_unit.GetComponent<CardIco>().m_att, attack_unit.GetComponent<CardIco>().r_att, defence_unit.GetComponent<CardIco>().r_att, opp_m_or_r_att_val);
        BlockAllInteraction();
        yield return new WaitUntil(AnsweredAttack);
        AttackPanel.SetActive(false);
        if (back_attack_chosen) // NIE KONTYNUUJEMY ATAKU - POWROT
        {
            //Debug.Log("POWROT NA POZYCJE - ATAKUJACY KLIKNAL BACK");
            back_attack_chosen = true;
            // nie trace ruchu
            attack_unit.transform.GetComponent<CardIco>().ReduceMovement(-1);
            CalculateBattleEffects(attack_unit, defence_unit, attacker_slot, defender_slot);
            /// czyszcze animacje
            foreach (GameObject unit in GetListofAllUnitsIconsInBattlefield())
            {
                unit.GetComponent<CardIco>().UnmarkPosibleEnemyUnitsAsTargets();
                unit.GetComponent<CardIco>().UnmarkPosibleSiegeTargets();
            }
            /// odblokowuje interakcje
            UnblockAllInteraction();
        }
        else // WYBRANO FORME ATAKU - PRZECHODZE DO ANIMACJI
        {
            StartCoroutine(AttackAnimationCorutine(attack_unit, defence_unit, attacker_slot, defender_slot));
        }
        
    }

    IEnumerator AttackAnimationCorutine(GameObject attack_unit, GameObject defence_unit, GameObject attacker_slot, GameObject defender_slot)
    {
        if (shock_attack_chosen)
        {
            SoundManagerScript.PlaySound("melee");
            if (AttackShockDirectionCalculator(attacker_slot, defender_slot) == "X-1")
            {
                attack_unit.GetComponent<Animator>().SetBool("ShockX-1Anim", true);
            }
            if (AttackShockDirectionCalculator(attacker_slot, defender_slot) == "X+1")
            {
                attack_unit.GetComponent<Animator>().SetBool("ShockX+1Anim", true);
            }
            if (AttackShockDirectionCalculator(attacker_slot, defender_slot) == "Y-1")
            {
                attack_unit.GetComponent<Animator>().SetBool("ShockY-1Anim", true);
            }
            if (AttackShockDirectionCalculator(attacker_slot, defender_slot) == "Y+1")
            {
                attack_unit.GetComponent<Animator>().SetBool("ShockY+1Anim", true);
            }
        }
        if (position_attack_chosen)
        {
            SoundManagerScript.PlaySound("position");
            attack_unit.GetComponent<Animator>().SetBool("RifleFireAnim", true);
        }
        yield return new WaitForSeconds(2.5f);
        attack_unit.GetComponent<Animator>().SetBool("RifleFireAnim", false);
        attack_unit.GetComponent<Animator>().SetBool("ShockX-1Anim", false);
        attack_unit.GetComponent<Animator>().SetBool("ShockX+1Anim", false);
        attack_unit.GetComponent<Animator>().SetBool("ShockY-1Anim", false);
        attack_unit.GetComponent<Animator>().SetBool("ShockY+1Anim", false);
        AttackCalculations(attack_unit, defence_unit, attacker_slot, defender_slot);
    }

    public string AttackShockDirectionCalculator(GameObject attacker_slot, GameObject defender_slot)
    {
        string direction = "";
        if (attacker_slot.GetComponent<CardSlot>().y < defender_slot.GetComponent<CardSlot>().y)
        {
            direction = "Y-1";
        }
        if (attacker_slot.GetComponent<CardSlot>().y > defender_slot.GetComponent<CardSlot>().y)
        {
            direction = "Y+1";
        }
        if (attacker_slot.GetComponent<CardSlot>().x > defender_slot.GetComponent<CardSlot>().x)
        {
            direction = "X-1";
        }
        if (attacker_slot.GetComponent<CardSlot>().x < defender_slot.GetComponent<CardSlot>().x)
        {
            direction = "X+1";
        }
        return direction;
    }

    public void AttackCalculations(GameObject attack_unit, GameObject defence_unit, GameObject attacker_slot, GameObject defender_slot)
    {
        // 2. ATTACK CALCULATIONS:
        // 2.1 REDUCING MOVEMENT
        attack_unit.transform.GetComponent<CardIco>().ReduceMovement(1);
        // 2.2 HAS ATTACKED BOOL PUT INTO TRUE
        attack_unit.transform.GetComponent<CardIco>().has_attacked = true;
        // ATTACK WRECZ
        if (shock_attack_chosen)
        {
            /////// MODYFIKATORY ATAKU WRECZ SHOCK (pike...)
            int defender_att_mod = 0;
            int attacker_att_mod = 0;
            if (defence_unit.GetComponent<CardIco>().card.pike && attack_unit.GetComponent<CardIco>().IfCavalry())
            {
                Debug.Log("PIKEMAN BONUS APPLIED!");
                defender_att_mod += 2;
            }
            //////// END MOD
            /// DEFENDER HITS ATTACKER
            attack_unit.GetComponent<CardIco>().act_def -= (defence_unit.GetComponent<CardIco>().m_att + defence_unit.GetComponent<CardIco>().m_att_mod + defender_att_mod);
            // WOUND TO ATTACKER
            if (defence_unit.GetComponent<CardIco>().m_att > 0)
            {
                 StartCoroutine(PutWoundfor2sec(attack_unit));
            }
            /// ATTACKER HITS DEFENDER
            defence_unit.GetComponent<CardIco>().act_def -= (attack_unit.GetComponent<CardIco>().m_att + attack_unit.GetComponent<CardIco>().m_att_mod + attacker_att_mod);
            // WOUND TO DEFENDER
            if (attack_unit.GetComponent<CardIco>().m_att > 0)
            {
                StartCoroutine(PutWoundfor2sec(defence_unit));
            }
            CalculateBattleEffects(attack_unit, defence_unit, attacker_slot, defender_slot);
        }
        // ATAK POZYCYJNY
        if (position_attack_chosen)
        {
            /// DEFENDER HITS ATTACKER
            attack_unit.GetComponent<CardIco>().act_def -= (defence_unit.GetComponent<CardIco>().r_att + defence_unit.GetComponent<CardIco>().r_att_mod);
            // ADD WOUND corutine
            if (defence_unit.GetComponent<CardIco>().r_att > 0)
            {

                StartCoroutine(PutWoundfor2sec(attack_unit));
            }
            /// ATTACKER HITS DEFENDER
            defence_unit.GetComponent<CardIco>().act_def -= (attack_unit.GetComponent<CardIco>().r_att + attack_unit.GetComponent<CardIco>().r_att_mod);
            // ADD WOUND corutine
            if (attack_unit.GetComponent<CardIco>().r_att > 0)
            {
                StartCoroutine(PutWoundfor2sec(defence_unit));
            }
            CalculateBattleEffects(attack_unit, defence_unit, attacker_slot, defender_slot);
        }
    }

    public void CalculateBattleEffects(GameObject attack_unit, GameObject defence_unit, GameObject attacker_slot, GameObject defender_slot)
    {
        // 2.5 STATS VISUALIZATION 
        attack_unit.GetComponent<CardIco>().StatsActualization();
        defence_unit.GetComponent<CardIco>().StatsActualization();
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        // 3. CHECK LIVE OF UNITS AND POSITIONING OF UNITS
        bool kill_attacker = false;
        bool kill_defender = false;
        if (attack_unit.GetComponent<CardIco>().CheckLive() == false)
        {
            // ATAKUJACY GINIE
            kill_attacker = true;
        }

        if (defence_unit.GetComponent<CardIco>().CheckLive() == false)
        {
            // BRONIACY GINIE 
            kill_defender = true;
            if (shock_attack_chosen)
            {
                // JEZELI ATAK WRECZ
                if (attack_unit.GetComponent<CardIco>().CheckLive() == true)
                {
                    // JEZELI ATAKUJACY PRZEZYL IDZIE NA SLOT BRONIACEGO
                    attack_unit.transform.GetComponent<CanvasGroup>().alpha = 1f;
                    attack_unit.transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    defender_slot.GetComponent<CardSlot>().PutUnit(attack_unit);
                    if (attack_unit.GetComponent<CardIco>().card.trample == true)
                    {
                        Debug.Log("TRAMPLE!");
                        attack_unit.GetComponent<CardIco>().has_attacked = false;
                    }
                }
            }
            if (position_attack_chosen)
            {
                // JEZELI ATAK POZYCYJNY
                if (attack_unit.GetComponent<CardIco>().CheckLive() == true)
                {
                    // JEZELI ATAKUJACY PRZEZYL WRACA NA MIEJSCE
                    defender_slot.GetComponent<CardSlot>().SendUnittoOldSlot(attack_unit);
                }
            }
            if (siege_attack_chosen)
            {
                /// JEZELI SIEGE ATTACK ZAWSZE WRACA NA SWOJE MIEJSCE
                defender_slot.GetComponent<CardSlot>().SendUnittoOldSlot(attack_unit);
            }
        }
        else
        {
            // BRONIACY PRZEZYL I ATAKUJACY PRZEZYL - ATAKUJACY WRACA NA SWOJE MIEJSCE
            defender_slot.GetComponent<CardSlot>().SendUnittoOldSlot(attack_unit);
        }
        //3.5 KILLING UNITS
        if (kill_attacker)
        {
            KillUnit(attack_unit);
        }
        if (kill_defender)
        {
            KillUnit(defence_unit);
        }
        //////////////////////////////////////////////////////////////////////////////
        // 4. CZYSZCZE BATTLEFIELD, ZERUJE ANIMACJE
        foreach (GameObject unit in GetListofAllUnitsIconsInBattlefield())
        {
            unit.GetComponent<CardIco>().UnmarkPosibleEnemyUnitsAsTargets();
            unit.GetComponent<CardIco>().UnmarkPosibleSiegeTargets();
        }
        UnblockAllInteraction();
        shock_attack_chosen = false;
        position_attack_chosen = false;
        back_attack_chosen = false;
        siege_attack_chosen = false;
        // KONIEC ATAKU
    }

    public void ChooseAttackType(int m_att_val, int r_att_val, int op_r_att_val, int opp_m_or_r_att_val)
    {
        siege_attack_chosen = false;
        shock_attack_chosen = false;
        position_attack_chosen = false;
        back_attack_chosen = false;
        BackAttackButton.interactable = true;
        ShockAttackButton.interactable = false;
        PositionAttackButton.interactable = false;
        if (m_att_val > 0)
        {
            //Debug.Log("MATT_VALm_att_val: " + m_att_val);
            ShockAttackButton.interactable = true;
        }
        if (r_att_val > 0)
        {
            //Debug.Log("R_ATT_VALm_att_val:" + r_att_val);
            PositionAttackButton.interactable = true;
        }
        //Debug.Log(m_att_val + ","+ r_att_val);
        shock_att_value.text = m_att_val.ToString();
        position_att_value.text = r_att_val.ToString();
        opp_position_att_value.text = op_r_att_val.ToString();
        opp_shock_or_position_att_value.text = opp_m_or_r_att_val.ToString();
        AttackPanel.SetActive(true);
    }

    public void PositionAttackChosen()
    {
        SoundManagerScript.PlaySound("click");
        position_attack_chosen = true;
        PositionAttackButton.interactable = false;
        ShockAttackButton.interactable = false;
    }

    public void ShockAttackChosen()
    {
        SoundManagerScript.PlaySound("click");
        shock_attack_chosen = true;
        ShockAttackButton.interactable = false;
        PositionAttackButton.interactable = false;
    }

    public void BackAttackChosen()
    {
        SoundManagerScript.PlaySound("click");
        back_attack_chosen = true;
        ShockAttackButton.interactable = false;
        PositionAttackButton.interactable = false;
    }

    public bool AnsweredAttack()
    {
        if (shock_attack_chosen || position_attack_chosen || back_attack_chosen)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PositionAllUnits(List<GameObject> unit_ico_list)
    {
        foreach (Transform slot in BattleField_Object.transform)
        {
            foreach (GameObject unit_ico in unit_ico_list)
            {
                if (slot.GetComponent<CardSlot>().x == unit_ico.transform.GetComponent<CardIco>().slot_x &&
                    slot.GetComponent<CardSlot>().y == unit_ico.transform.GetComponent<CardIco>().slot_y)
                {
                    slot.GetComponent<CardSlot>().PutUnit(unit_ico);
                }
            }
        }
    }

    public void RefreshMovementofUnitIcoList(List<GameObject> list_units)
    {
        foreach (GameObject unit_ico in list_units)
        {
            unit_ico.GetComponent<CardIco>().movement = unit_ico.GetComponent<CardIco>().max_movement;
        }
    }

    public void RefreshAttackofUnitIcoList(List<GameObject> list_units)
    {
        foreach (GameObject unit_ico in list_units)
        {
            unit_ico.GetComponent<CardIco>().has_attacked = false;
        }
    }

    public void PickUpAllUnits(List<GameObject> list_all_units_on_battlefield)
    {
        foreach (GameObject unit_ico in list_all_units_on_battlefield)
        {
            unit_ico.transform.SetParent(Temp_Unit_Container_Object.transform, false);
        }
        foreach (Transform slot in BattleField_Object.transform)
        {
            slot.GetComponent<CardSlot>().ClearCardUnitVariable();
        }
    }

    public void ComputeMorale()
    {
        int penalty = 0;
        List<GameObject> list_unit = GetListofActivePlayerUnitsIconsInBattlefield();
        foreach (GameObject unit in list_unit)
        {
            if (unit.GetComponent<CardIco>().slot_y == 0)
            {
                penalty += 2;
            }
            else if (unit.GetComponent<CardIco>().slot_y == 1)
            {
                penalty += 1;    
            }
        }
        TurnSystem.Op_Player.Morale -= penalty;
    }

    public List<GameObject> GetListofActivePlayerUnitsIconsInBattlefield()
    {
        List<GameObject> list_unit_icons = new List<GameObject>();
        foreach (Transform slot in BattleField_Object.transform)
        {
            //Debug.Log(slot.transform.childCount);
            if (slot.transform.childCount > 0)
            {
                if (slot.transform.GetChild(0).gameObject.gameObject.GetComponent<CardIco>().owner == TurnSystem.Active_Player)
                {
                    list_unit_icons.Add(slot.transform.GetChild(0).gameObject);
                }
            }
        }
        //Debug.Log("FUNCTION GET LIST OF ACTIVE PLAYER UNITS IN BATTLEFIELD return`s a list: " + list_unit_icons);
        return list_unit_icons;
    }

    public List<GameObject> GetListofOppPlayerUnitsIconsInBattlefield()
    {
        List<GameObject> list_unit_icons = new List<GameObject>();
        foreach (Transform slot in BattleField_Object.transform)
        {
            //Debug.Log(slot.transform.childCount);
            if (slot.transform.childCount > 0)
            {
                if (slot.transform.GetChild(0).gameObject.gameObject.GetComponent<CardIco>().owner == TurnSystem.Op_Player)
                {
                    list_unit_icons.Add(slot.transform.GetChild(0).gameObject);
                }
            }
        }
        //Debug.Log("FUNCTION GET LIST OF ACTIVE PLAYER UNITS IN BATTLEFIELD return`s a list: " + list_unit_icons);
        return list_unit_icons;
    }

    public List<GameObject> GetListofAllUnitsIconsInBattlefield()
    {
        List<GameObject> list_unit_icons = new List<GameObject>();
        foreach (Transform slot in BattleField_Object.transform)
        {
            //Debug.Log(slot.transform.childCount);
            if (slot.transform.childCount > 0)
            {
                //Debug.Log(slot.transform.GetChild(0).name + " ON THE SLOT: x:"+ slot.GetComponent<CardSlot>().x + ", y:" + slot.GetComponent<CardSlot>().y);
                list_unit_icons.Add(slot.transform.GetChild(0).gameObject);
                //Debug.Log(slot.transform.GetChild(0).gameObject.name);
            }
        }
        //Debug.Log("FUNCTION GETLISTOFALLUNITSINBATTLEFIELD return`s a list: " + list_unit_icons);
        return list_unit_icons;
    }

    public void BlockInteractionAllUnits()
    {
        List<GameObject> all_units = GetListofAllUnitsIconsInBattlefield();
        foreach (GameObject unit in all_units)
        {
            unit.GetComponent<CanvasGroup>().blocksRaycasts = false;
            unit.GetComponent<CardIco>().interactable = false;
        }
    }

    public void UnblockInteractionAllUnits()
    {
        List<GameObject> all_units = GetListofAllUnitsIconsInBattlefield();
        foreach (GameObject unit in all_units)
        {
            //Debug.Log(unit.name + "SET TO INTERACTABLE TRUE");
            unit.GetComponent<CanvasGroup>().blocksRaycasts = true;
            unit.GetComponent<CardIco>().interactable = true;
        }
    }

    public void BlockInteractionAllSlots()
    {
        List<GameObject> all_slots = GetListofAllSlots();
        foreach(GameObject slot in all_slots)
        {
            slot.GetComponent<Image>().raycastTarget = false;
        }
    }

    public void UnblockInteractionAllSlots()
    {
        List<GameObject> all_slots = GetListofAllSlots();
        foreach (GameObject slot in all_slots)
        {
            slot.GetComponent<Image>().raycastTarget = true;
        }
    }

    public void BlockAllInteraction()
    {
        //Debug.Log("BLOCK ALL BATTLEFIELD INTERACTION");
        BlockInteractionAllSlots();
        BlockInteractionAllUnits();
        NextTurnButton.interactable = false;
    }

    public void UnblockAllInteraction()
    {
        //Debug.Log("UNBLOCK ALL BATTLEFIELD INTERACTION");
        UnblockInteractionAllSlots();
        UnblockInteractionAllUnits();
        NextTurnButton.interactable = true;
    }

    public void ReverseAllUnitsBattleSlotPositions(List<GameObject> list_all_units_on_battlefield)
    {
        //List<GameObject> reversed_list = new List<GameObject>();
        foreach (GameObject unit_ico in list_all_units_on_battlefield)
        {
            unit_ico.GetComponent<CardIco>().slot_x = 7 - unit_ico.GetComponent<CardIco>().slot_x;
            unit_ico.GetComponent<CardIco>().slot_y = 3 - unit_ico.GetComponent<CardIco>().slot_y;
            //Debug.Log(unit_ico.name + " CHANGING x and Y");
        }
    }

    public List<GameObject> GetListofAllSlots()
    {
        List<GameObject> all_slots = new List<GameObject>(); 
        foreach (Transform slot in BattleField_Object.transform)
        {
            all_slots.Add(slot.gameObject);
        }
        return all_slots;
    }

    public List<GameObject> GetListofActPlayerSlots()
    {
        List<GameObject> act_slots = new List<GameObject>();
        foreach (Transform slot in BattleField_Object.transform)
        {
            if (slot.GetComponent<CardSlot>().y > 1)
            {
                act_slots.Add(slot.gameObject);
            }
        }
        return act_slots;
    }

    public List<GameObject> GetListofOppPlayerSlots()
    {
        List<GameObject> opp_slots = new List<GameObject>();
        foreach (Transform slot in BattleField_Object.transform)
        {
            if (slot.GetComponent<CardSlot>().y <= 1)
            {
                opp_slots.Add(slot.gameObject);
            }
        }
        return opp_slots;
    }

    public void MarkGreenActPlayerSlots()
    {
        List<GameObject> act_slots = GetListofActPlayerSlots();
        foreach (GameObject slot in act_slots)
        {
            slot.GetComponent<CardSlot>().ChangeColorToGreen();
        }
        
    }

    public void MarkGreenOppPlayerSlots()
    {
        List<GameObject> act_slots = GetListofOppPlayerSlots();
        foreach (GameObject slot in act_slots)
        {
            slot.GetComponent<CardSlot>().ChangeColorToGreen();
        }

    }

    public void MarkRedActPlayerSlots()
    {
        List<GameObject> act_slots = GetListofActPlayerSlots();
        foreach (GameObject slot in act_slots)
        {
            slot.GetComponent<CardSlot>().ChangeColorToRed();
        }

    }

    public void MarkRedOppPlayerSlots()
    {
        List<GameObject> opp_slots = GetListofOppPlayerSlots();
        foreach (GameObject slot in opp_slots)
        {
            slot.GetComponent<CardSlot>().ChangeColorToRed();
        }

    }

    public void MarkGreenActPlayerUnits()
    {
        List<GameObject> act_player_units = GetListofActivePlayerUnitsIconsInBattlefield();
        foreach (GameObject act_player_unit in act_player_units)
        {
            act_player_unit.GetComponent<CardIco>().PutMaskColorToGreen();
        }
    }

    public void MarkGreenOppPlayerUnits()
    {
        List<GameObject> opp_units = GetListofOppPlayerUnitsIconsInBattlefield();
        foreach (GameObject opp_unit in opp_units)
        {
            opp_unit.GetComponent<CardIco>().PutMaskColorToGreen();
        }
    }

    public void ResetUnitsColor()
    {
        List<GameObject> all_units = GetListofAllUnitsIconsInBattlefield();
        foreach (GameObject unit in all_units)
        {
            unit.GetComponent<CardIco>().ResetMaskColor();
        }
    }

    public void ResetSlotsColor()
    {
        List<GameObject> all_slots = GetListofAllSlots();
        foreach (GameObject slot in all_slots)
        {
            slot.GetComponent<Image>().color = new Color32(255, 255, 255, 30);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject unit_ico in GetListofActivePlayerUnitsIconsInBattlefield())
        {
            unit_ico.GetComponent<Canvas>().sortingLayerName = "Attacker";
            unit_ico.GetComponent<SpriteRenderer>().sortingOrder = 10;
        }
        foreach (GameObject unit_ico in GetListofOppPlayerUnitsIconsInBattlefield())
        {
            unit_ico.GetComponent<Canvas>().sortingLayerName = "Defender";
            unit_ico.GetComponent<SpriteRenderer>().sortingOrder = 4;
        }
    }
}
