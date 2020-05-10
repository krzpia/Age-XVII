using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour, IDropHandler
{
    public GameObject card_unit;
    public int x;
    public int y;
    public int slot_id;
    public bool act_player_zone;

    public void Start()
    {

    }

    public void ChangeColorToRed()
    {
        GetComponent<Image>().color = new Color32(255, 100, 100, 50);
    }

    public void ChangeColorToGreen()
    {
        GetComponent<Image>().color = new Color32(100, 255, 100, 50);
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("On Drop");
        // LOGIKA UPUSZCZANIA PRZEDMIOTU NA DOWOLNYM SLOCIE (RUCH LUB ATAK)
        if (eventData.pointerDrag.tag == "Card Icon")
        {
            if (eventData.pointerDrag.transform.GetComponent<CardIco>().owner == TurnSystem.Active_Player &&
                eventData.pointerDrag.transform.GetComponent<CardIco>().movement > 0)
            {
                if (card_unit == null && transform.childCount == 0)
                {
                    //Debug.Log("PUT UNIT IF HAS ENOUGH MOVES");
                    SoundManagerScript.PlaySound("move_ico");
                    int distance_to_move = eventData.pointerDrag.transform.GetComponent<CardIco>().MovementDistanceCounter(x, y);
                    if (eventData.pointerDrag.transform.GetComponent<CardIco>().CheckMovementPosibility(distance_to_move))
                    {
                        eventData.pointerDrag.transform.GetComponent<CardIco>().ReduceMovement(distance_to_move);
                        eventData.pointerDrag.transform.GetComponent<CanvasGroup>().alpha = 1f;
                        eventData.pointerDrag.transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
                        PutUnit(eventData.pointerDrag);
                    }
                    else
                    {
                        SendUnittoOldSlot(eventData.pointerDrag);
                    }

                }
                else if (card_unit.GetComponent<CardIco>().owner == TurnSystem.Active_Player && transform.childCount > 0)
                {
                    //Debug.Log("FRIENDLY UNIT or MYSELF!");
                    SendUnittoOldSlot(eventData.pointerDrag);
                }
                else if (card_unit.GetComponent<CardIco>().owner != TurnSystem.Active_Player && transform.childCount > 0)
                {
                    if (eventData.pointerDrag.transform.GetComponent<CardIco>().has_attacked == false)
                    {
                        Debug.Log("ENEMY UNIT - ATTACK!");
                        GameObject Attacking_Unit = eventData.pointerDrag;
                        GameObject Defending_Unit = card_unit;
                        GameObject Battlefield_Object = GameObject.Find("BattleField");
                        Battlefield_Object.GetComponent<BattleField>().AttackEngine(Attacking_Unit, Defending_Unit);
                    }
                    else
                    {
                        Debug.Log("THIS UNIT HAS ATTACKED THIS TURN");
                    }

                }
                else
                {
                    SendUnittoOldSlot(eventData.pointerDrag);
                }
            }
        } 
    }


    public void SendUnittoOldSlot(GameObject unit_ico)
    {
        Debug.Log("Sending Unit " + unit_ico.name + " to Old Slot");
        unit_ico.GetComponent<RectTransform>().anchoredPosition = unit_ico.transform.GetComponent<CardIco>().start_position;
        unit_ico.transform.SetParent(unit_ico.transform.GetComponent<CardIco>().old_parent, false);
    }

    public GameObject PickUpUnit()
    {
        return card_unit;
    }

    public void ClearCardUnitVariable()
    {
        card_unit = null;
    }

    public void PutUnit(GameObject unit_ico)
    {
        //Debug.Log("PUTING UNIT " + unit_ico.name + "ON SLOT:" + x + "," + y);
        card_unit = unit_ico;
        card_unit.transform.SetParent(this.transform, false);
        card_unit.transform.GetComponent<CardIco>().slot_x = x;
        card_unit.transform.GetComponent<CardIco>().slot_y = y;
        card_unit.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
    }


    public void Update()
    {
        if (transform.childCount == 0 && card_unit != null)
        {
            //Debug.Log("This " + name + " dont have a child, but has occupied slot!. Clearing it!");
            card_unit = null;
        }

    }

}
