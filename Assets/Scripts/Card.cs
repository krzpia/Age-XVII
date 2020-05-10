using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Unit")]
public class Card: ScriptableObject {
    public string card_name;
    public string description;
    public string add_info;
    public string nation;
    public List<string> types;
    public Sprite artwork;
    public Sprite back_card;
    public int cost;
    public int m_att;
    public int r_att;
    public int s_att;
    public int max_mov;
    public int max_def;
    public int act_def;
    public string artwork_name;
    public GameObject card_icon_template;
    public GameObject big_card_template;
    public bool charge;   // IN CARD ICO start funcion mmax movement on first turn 
    public bool trample;  // IN BATTLE RESULTS put has_attacked into true after succesfull shock Attack
    public bool pike; // TO DO

}
