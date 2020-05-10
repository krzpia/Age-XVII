using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardIco : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public Card card;
    public GameObject BigCardTemplate;
    public GameObject Ico_Art_Bcg;
    public Image artworkImage;
    public GameObject woundObject;
    public GameObject AlfaMask;
    public Text MattText;
    public Text RattText;
    public Text SattText;
    public Text act_defText;
    public Text max_defText;
    private Color32 sweColor;
    private Color32 polColor;
    private Color32 ukrColor;
    private Color32 turColor;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Player owner;
    public static bool during_dragging;
    public bool active_unit;
    public bool has_moved;
    public bool has_attacked;
    public int act_def;
    public int act_def_mod;
    public int max_def;
    public int max_def_mod;
    public int m_att;
    public int m_att_mod;
    public int r_att;
    public int r_att_mod;
    public int s_att;
    public int s_att_mod;
    public int movement;
    public int max_movement;
    public Vector2 start_position;
    public int slot_x;
    public int slot_y;
    public Transform old_parent;
    public GameObject IcoBorderObject;
    public bool interactable;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        start_position = rectTransform.anchoredPosition;
        interactable = true;
        //Debug.Log("AWAKENING UNIT ICO");
        //Debug.Log(IcoBorderObject.GetComponent<Image>().color);
    }

    void Start()
    {
        IcoBorderObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        sweColor = new Color32(154, 185, 200, 255);
        polColor = new Color32(230, 203, 153, 255);
        artworkImage.sprite = card.artwork;
        MattText.text = card.m_att.ToString();
        m_att = card.m_att;
        m_att_mod = 0;
        SattText.text = card.s_att.ToString();
        s_att = card.s_att;
        s_att_mod = 0;
        RattText.text = card.r_att.ToString();
        r_att = card.r_att;
        r_att_mod = 0;
        act_defText.text = card.act_def.ToString();
        act_def = card.act_def;
        act_def_mod = 0;
        max_defText.text = card.max_def.ToString();
        max_def = card.max_def;
        max_def_mod = 0;
        /// COLOT OF UNIT ICO 
        if (card.nation == "SWE")
        {
            Ico_Art_Bcg.GetComponent<Image>().color = sweColor;
        }
        else if (card.nation == "POL")
        {
            Ico_Art_Bcg.GetComponent<Image>().color = polColor;
        }
        else
        {
            Ico_Art_Bcg.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
        }
        /// MOVEMENT - CHARGE ABILITY
        max_movement = card.max_mov;
        if (card.charge)
        {
            movement = max_movement;
        }
        else
        {
            movement = 0;
        }

    }

    public void StatsActualization()
    {
        MattText.text = (m_att + m_att_mod).ToString();
        SattText.text = (s_att + s_att_mod).ToString();
        RattText.text = (r_att + r_att_mod).ToString();
        act_defText.text = (act_def + act_def_mod).ToString();
        max_defText.text = (max_def + max_def_mod).ToString();
        if (act_def + act_def_mod < max_def + max_def_mod)
        {
            act_defText.color = new Color32(125, 0, 0, 255);
        }
        else if (act_def + act_def_mod > max_def + max_def_mod)
        {
            act_defText.color = new Color32(0, 180, 0, 255);
        }
        else
        {
            act_defText.color = new Color32(0, 0, 0, 255);
        }
        
    }

    public void HealUnit()
    {
        act_def = (max_def + max_def_mod);
        StatsActualization();
    }

    public bool CheckLive()
    {
        //Debug.Log("CHECK LIVE UNIT");
        if ((GetComponent<CardIco>().act_def + GetComponent<CardIco>().act_def_mod) <= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void Kill()
    {
        if (GetComponent<CardIco>().owner == TurnSystem.Active_Player)
        {
            //Debug.Log("KILLING ACTIVE PLAYERS UNIT");
            transform.SetParent(GameObject.Find("PlayerGraveyard").transform,false);
            //Debug.Log("OBIEKT PLAYER GRAVEYARD: " + GameObject.Find("PlayerGraveyard").transform);
            interactable = false;
            gameObject.SetActive(false);
        }
        else if (GetComponent<CardIco>().owner == TurnSystem.Op_Player)
        {
            //Debug.Log("KILLING OPP PLAYERS UNIT");
            transform.SetParent(GameObject.Find("OpponentGraveyard").transform, false);
            //Debug.Log("OBIEKT OPPONENT GRAVEYARD: " + GameObject.Find("OpponentGraveyard").transform);
            interactable = false;
            gameObject.SetActive(false);
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //Debug.Log("Right Button Pressed on the " + card.card_name);
            BigCardTemplate.GetComponent<CardDisplay>().Show_Big_Card(card, act_def, m_att + m_att_mod, r_att+ r_att_mod, s_att + s_att_mod);
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("Pointer up on Ico");
        BigCardTemplate.GetComponent<CardDisplay>().End_Show_Big_Card();
    }


    public int MovementDistanceCounter(int dest_x, int dest_y)
    {
        int distance_x = Math.Abs(slot_x - dest_x);
        int distance_y = Math.Abs(slot_y - dest_y);
        int overall_dist = distance_x + distance_y;
        //Debug.Log("DISTANCE OF UNIT TO THIS SLOT: " + overall_dist);
        return overall_dist;

    }

    public bool CheckMovementPosibility(int distance)
    {
        if (movement >= distance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ReduceMovement(int distance_moved)
    {
        movement -= distance_moved;
    }

    public List<GameObject> GetSlotsinMovementRange()
    {
        List<GameObject> all_slots = GameObject.Find("BattleField").GetComponent<BattleField>().GetListofAllSlots();
        List<GameObject> in_movement_range_slots = new List<GameObject>();
        foreach (GameObject slot in all_slots)
        {
            int distance = MovementDistanceCounter(slot.GetComponent<CardSlot>().x, slot.GetComponent<CardSlot>().y);
            if (movement >= distance)
            {
                in_movement_range_slots.Add(slot);
            }
            //Debug.Log(slot.GetComponent<CardSlot>().slot_id + " has distance = " + distance + " from unit " + this.name);
        }
        return in_movement_range_slots;
    }

    public List<GameObject> GetListofUnitsinRange(int min_range, int max_range)
    {
        List<GameObject> all_units = GameObject.Find("BattleField").GetComponent<BattleField>().GetListofAllUnitsIconsInBattlefield();
        List<GameObject> in_siege_range_units = new List<GameObject>();
        foreach (GameObject unit in all_units)
        {
            int distance = MovementDistanceCounter(unit.GetComponent<CardIco>().slot_x, unit.GetComponent<CardIco>().slot_y);
            if (min_range < distance && distance < max_range)
            {
                in_siege_range_units.Add(unit);
            }
            //Debug.Log(slot.GetComponent<CardSlot>().slot_id + " has distance = " + distance + " from unit " + this.name);
        }
        return in_siege_range_units;
    }

    public void ChangeSlotColorinMovementRange()
    {
        foreach (GameObject slot in GameObject.Find("BattleField").GetComponent<BattleField>().GetListofAllSlots())
        {
            slot.GetComponent<CardSlot>().ChangeColorToRed();
        }

        List<GameObject> in_range_slots = GetSlotsinMovementRange();
        foreach (GameObject slot in in_range_slots)
        {
            slot.GetComponent<CardSlot>().ChangeColorToGreen();
        }
    }

    public void MarkPosibleSiegeTargets()
    {
        if (has_attacked == false)
        {
            /// ZASIEG ARTYLERII -3 pola. Atak pola sasiedniego tylko position!
            List<GameObject> in_range_units = GetListofUnitsinRange(1, 4);
            foreach (GameObject unit in in_range_units)
            {
                if (unit.GetComponent<CardIco>().owner == TurnSystem.Op_Player)
                {
                    unit.GetComponent<Animator>().SetBool("InSiegeRangeAnim", true);
                }
            }
        }
    }

    public void UnmarkPosibleSiegeTargets()
    {
        List<GameObject> in_range_units = GameObject.Find("BattleField").GetComponent<BattleField>().GetListofAllUnitsIconsInBattlefield();
        foreach (GameObject unit in in_range_units)
        {
            unit.GetComponent<Animator>().SetBool("InSiegeRangeAnim", false);
        }

    }

    public void MarkPosibleEnemyUnitsAsTargets()
    {
        if (has_attacked == false)
        {
            List<GameObject> targets = GameObject.Find("BattleField").GetComponent<BattleField>().GetListofUnitInRangeofAttackbyPosition(slot_x, slot_y);
            foreach (GameObject target in targets)
            {
                target.GetComponent<Animator>().SetBool("InRangeAnim", true);
            }
        }
    }

    public void UnmarkPosibleEnemyUnitsAsTargets()
    {
        List<GameObject> all_units = GameObject.Find("BattleField").GetComponent<BattleField>().GetListofAllUnitsIconsInBattlefield();
        foreach (GameObject unit in all_units)
        {
            unit.GetComponent<Animator>().SetBool("InRangeAnim", false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (interactable)
        {
            GetComponent<SpriteRenderer>().sortingOrder = 16;
            GetComponent<Canvas>().sortingLayerName = "Attacker";
            if (owner == TurnSystem.Active_Player && movement > 0)
            {
                canvasGroup.alpha = .6f;
                canvasGroup.blocksRaycasts = false;
                start_position = rectTransform.anchoredPosition;
                old_parent = transform.parent;
                during_dragging = true;
                //Debug.Log("Begin Drag");
                MarkPosibleEnemyUnitsAsTargets();
                if (s_att > 0)
                {
                    MarkPosibleSiegeTargets();
                }
            }
            else
            {
                Debug.Log("Play Animation Disabled");
                // ANIMACJA
                GetComponent<Animator>().SetBool("DisabledIcoAnim", true);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (interactable)
        {
            //Debug.Log("DRAGING");
            GetComponent<SpriteRenderer>().sortingOrder = 16;
            GetComponent<Canvas>().sortingLayerName = "Attacker";
            if (owner == TurnSystem.Active_Player && movement > 0)
            {
                ChangeSlotColorinMovementRange();
                //Debug.Log(eventData.delta);
                //Debug.Log(rectTransform.anchoredPosition);
                rectTransform.anchoredPosition += eventData.delta;
                during_dragging = true;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<SpriteRenderer>().sortingOrder = 4;
        GetComponent<Canvas>().sortingLayerName = "Defender";
        if (interactable)
        {
            GameObject.Find("BattleField").GetComponent<BattleField>().ResetSlotsColor();
            if (owner == TurnSystem.Active_Player && movement > 0)
            {
                //Debug.Log("End Drag");
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
                //Debug.Log("Raycast obj" + eventData.pointerCurrentRaycast);
                if (eventData.pointerCurrentRaycast.gameObject == null)
                {
                    rectTransform.anchoredPosition = start_position;
                }
                else if (eventData.pointerCurrentRaycast.gameObject.tag != "BattleSlot")
                {
                    rectTransform.anchoredPosition = start_position;
                }
            }
            // ANIMACJA
            GetComponent<Animator>().SetBool("DisabledIcoAnim", false);
            UnmarkPosibleEnemyUnitsAsTargets();
            UnmarkPosibleSiegeTargets();
            // ZMIENNA DO UPDATE
            during_dragging = false;
        }
        else
        {
            // ANIMACJA
            GetComponent<Animator>().SetBool("DisabledIcoAnim", false);
            rectTransform.anchoredPosition = start_position;
            canvasGroup.alpha = 1f;
            // ZMIENNA DO UPDATE
            during_dragging = false;
            UnmarkPosibleSiegeTargets();
        }
    }

    public bool IfCavalry()
    {
        bool cavalry_bool = false;
        for (int i = 0; i<4; i++)
        {
            if (card.types[i] == "Cavalry")
            {
                Debug.Log("IfCavalry function = true");
                cavalry_bool = true;
            }
        }
        return cavalry_bool;
    }

    public void PutMaskColorToGreen()
    {
        AlfaMask.GetComponent<Image>().color = new Color32(0, 255, 0, 120);
    }

    public void PutMaskColorToRed()
    {
        AlfaMask.GetComponent<Image>().color = new Color32(255, 0, 0, 100);
    }

    public void ResetMaskColor()
    {
        AlfaMask.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
    }


    public void Update()
    {
        if (owner == TurnSystem.Active_Player)
        {
            if (movement > 0)
            {
                IcoBorderObject.GetComponent<Image>().color = new Color32(20, 220, 0, 255);
                GetComponent<Animator>().SetBool("ActiveIcoAnim", true);
            }
            else
            {
                IcoBorderObject.GetComponent<Image>().color = new Color32(20, 140, 0, 255);
                GetComponent<Animator>().SetBool("ActiveIcoAnim", false);
            }

        }
        else
        {
            GetComponent<Animator>().SetBool("ActiveIcoAnim", false);
        }
    }
}
