using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Instant")]
public class Card_Instant : ScriptableObject
{
    public string card_name;
    public string description;
    public string nation;
    public List<string> types;
    public Sprite artwork;
    public Sprite back_card;
    public int cost;
    public string artwork_name;
    public GameObject big_card_template;
    /// ////////////
    /// INSTANT EFFECTS
    public string effect_type; // PLAYER, OPPONENT, GLOBAL, PL_UNIT, OP_UNIT, UNIT, SLOT, SLOTAREA, PL_AREA, OP_AREA, PL_HAND, OP_HAND, PL_GRAVEYARD, OP_GRAVEYARD.. 
    public int duration; // 0- INSTANT EFFECT, >0 TURN DURATIONS
    // UNIT EFFECTS:
    public int m_att_mod;
    public int s_att_mod;
    public int r_att_mod;
    public int cost_mod;
    public int def_mod;
    public bool heal_to_max_def_mod;
    public int max_def_mod;
    public int movement_mod;
    public bool renew_movement_mod;
    public bool renew_if_attacked_mod;
    // SLOT EFFECTS:
    public GameObject UnitIco; // TO INSTANT UNIT
    // PLAYER EFFECTS:
    public int gold_mod;
    public int income_mod;
    public int morale_mod;
    
}
