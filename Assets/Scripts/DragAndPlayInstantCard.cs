using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndPlayInstantCard : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Card_Instant card_inst;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 start_drag_position;
    public GameObject BigCard;
    public bool interactable;
    private bool leftMouseDown;
    public static bool during_dragging = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        start_drag_position = rectTransform.anchoredPosition;
        interactable = true;
    }

    private bool CheckIfOwnerIsActivePlayer()
    {
        if (TurnSystem.Active_Player == GetComponent<CardinstDisplay>().owner)
        {
            //Debug.Log("OWNED BY ACTIVE PLAYER");
            return true;
        }
        else
        {
            //Debug.Log("OWNED BY OTHER PLAYER");
            return false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("CLICK");
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (CheckIfOwnerIsActivePlayer())
            {
                //Debug.Log("Right Button Pressed on the " + card_inst.card_name + " owned by: " + GetComponent<CardinstDisplay>().owner);
                BigCard.GetComponent<BigCardInstDisplay>().Show_Big_Card_Instant(card_inst);
            }
            else
            {
                //Debug.Log("Right Button Pressed on the opponnent`s card");
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        BigCard.GetComponent<BigCardInstDisplay>().End_Show_Big_Card();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (interactable)
        {
            if (leftMouseDown && CheckIfOwnerIsActivePlayer())
            {
                if (card_inst.cost <= TurnSystem.Active_Player.Credits)
                {
                    // ZAZNACZAM KOLORY SLOTOW GDY MAM WYSTARCZAJACO KREDYTOW i W ZALEZNOSCI OD TYPU EFEKTU
                    if (GetComponent<CardinstDisplay>().card.effect_type == "GLOBAL")
                    {
                        GameObject.Find("BattleField").GetComponent<BattleField>().MarkGreenActPlayerSlots();
                        GameObject.Find("BattleField").GetComponent<BattleField>().MarkGreenOppPlayerSlots();
                    }
                    else if (GetComponent<CardinstDisplay>().card.effect_type == "PL_AREA")
                    {
                        GameObject.Find("BattleField").GetComponent<BattleField>().MarkGreenActPlayerSlots();
                        GameObject.Find("BattleField").GetComponent<BattleField>().MarkRedOppPlayerSlots();
                    }
                    else if (GetComponent<CardinstDisplay>().card.effect_type == "OP_AREA")
                    {
                        GameObject.Find("BattleField").GetComponent<BattleField>().MarkRedActPlayerSlots();
                        GameObject.Find("BattleField").GetComponent<BattleField>().MarkGreenOppPlayerSlots();
                    }
                    else if (GetComponent<CardinstDisplay>().card.effect_type == "PL_UNIT")
                    {
                        GameObject.Find("BattleField").GetComponent<BattleField>().MarkGreenActPlayerUnits();
                    }
                    else if (GetComponent<CardinstDisplay>().card.effect_type == "OP_UNIT")
                    {
                        GameObject.Find("BattleField").GetComponent<BattleField>().MarkGreenOppPlayerUnits();
                    }
                    else
                    {
                        Debug.LogWarning("Instant Card, out of StartDrag posibilities");
                    }
                }
                else 
                {
                    Debug.Log("NO CREDITS to play INSTANT");
                    GameObject.Find("BattleField").GetComponent<BattleField>().MarkRedActPlayerSlots();
                    GameObject.Find("BattleField").GetComponent<BattleField>().MarkRedOppPlayerSlots();
                }
                // ZACZYNAM CIAGNAC KARTE
                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = .6f;
                start_drag_position = rectTransform.anchoredPosition;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (interactable)
        {
            if (leftMouseDown && CheckIfOwnerIsActivePlayer())
            {
                Debug.Log("TRYING TO PLAY INSTANT CARD");
                GameObject.Find("BattleField").GetComponent<BattleField>().ResetSlotsColor();
                GameObject.Find("BattleField").GetComponent<BattleField>().ResetUnitsColor();
                if (eventData.pointerCurrentRaycast.gameObject == null)
                {
                    rectTransform.anchoredPosition = start_drag_position;
                }
                else if (eventData.pointerCurrentRaycast.gameObject.CompareTag("BattleSlot") && card_inst.effect_type == "PL_AREA")
                {
                    Debug.Log("PLAYING CARD, DECODING INSTANT EFFECT");
                    // 0. PLAY SOUND
                    // 1. ADDING EFFECT
                    if (card_inst.duration == 0)
                    {
                        // INSTANT EFFECT
                        List<GameObject> act_player_units = GameObject.Find("BattleField").GetComponent<BattleField>().GetListofActivePlayerUnitsIconsInBattlefield();
                        foreach (GameObject act_player_unit in act_player_units)
                        {
                            act_player_unit.GetComponent<CardIco>().m_att_mod += card_inst.m_att_mod;
                            act_player_unit.GetComponent<CardIco>().r_att_mod += card_inst.r_att_mod;
                            act_player_unit.GetComponent<CardIco>().s_att_mod += card_inst.s_att_mod;
                            act_player_unit.GetComponent<CardIco>().act_def_mod += card_inst.def_mod;
                            act_player_unit.GetComponent<CardIco>().max_def_mod += card_inst.max_def_mod;
                            if (card_inst.heal_to_max_def_mod)
                            {
                                act_player_unit.GetComponent<CardIco>().HealUnit();
                            }
                            if (card_inst.renew_movement_mod)
                            {
                                act_player_unit.GetComponent<CardIco>().movement = act_player_unit.GetComponent<CardIco>().max_movement;
                            }
                            if (card_inst.renew_if_attacked_mod)
                            {
                                act_player_unit.GetComponent<CardIco>().has_attacked = false;
                            }
                        }
                    }
                    else
                    {
                        // DURATION PUTO TO TURN SYSTEM EFFECTS LIST or something..
                    }
                    // 4. DEACTIVATING CARDSM GAMEOBJECT AND PUTING INTO PLAYERUSEDCARDS Container
                    gameObject.SetActive(false);
                    transform.SetParent(GameObject.Find("PlayerUsedCards").transform, false);
                    // 5. REMOVING FROM PLAYER CLASS OBJECT INTERNAL LIST
                    TurnSystem.Active_Player.RemoveCardFromHand(this.gameObject);
                    // 6. PAY COST
                    TurnSystem.Active_Player.Credits -= card_inst.cost;
                }
                else
                {
                    Debug.LogWarning("Instant Card, out of EndDrag posibilities");
                    rectTransform.anchoredPosition = start_drag_position;
                }
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = 1f;
                during_dragging = false;
            }
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (interactable)
        {
            if (leftMouseDown && CheckIfOwnerIsActivePlayer())
            {
                rectTransform.anchoredPosition += eventData.delta;
                during_dragging = true;
            }
        }
    }


    public void OnHoverEnter()
    {
        if (during_dragging == false)
        {
            if (interactable)
            {
                if (CheckIfOwnerIsActivePlayer())
                {
                    GetComponent<Animator>().SetBool("Uppered", true);
                }
            }
        }
        
    }

    public void OnHoverExit()
    {
        if (interactable)
        {
            if (CheckIfOwnerIsActivePlayer())
            {
                GetComponent<Animator>().SetBool("Uppered", false);
            }
        }
    }


    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            leftMouseDown = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            leftMouseDown = false;
        }
    }
}
