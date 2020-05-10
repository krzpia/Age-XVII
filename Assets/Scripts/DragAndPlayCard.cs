using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndPlayCard : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 start_drag_position;
    public Card card;
    public GameObject BigCard;
    public GameObject self;
    public static bool during_dragging = false;
    private bool leftMouseDown;
    public bool interactable;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        start_drag_position = rectTransform.anchoredPosition;
        interactable = true;
    }
    
    private bool CheckIfOwnerIsActivePlayer()
    {
        if (TurnSystem.Active_Player == GetComponent<CardsmDisplay>().owner)
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
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (CheckIfOwnerIsActivePlayer())
            {
                //Debug.Log("Right Button Pressed on the " + card.card_name + " owned by: " + GetComponent<CardsmDisplay>().owner);
                BigCard.GetComponent<CardDisplay>().Show_Big_Card(card, card.act_def, card.m_att, card.r_att, card.s_att);
            }
            else
            {
                //Debug.Log("Right Button Pressed on the opponnent`s card");
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        BigCard.GetComponent<CardDisplay>().End_Show_Big_Card();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (leftMouseDown)
        {
            if (interactable && during_dragging == false)
            {
                if (CheckIfOwnerIsActivePlayer())
                {
                    // ZAZNACZAM KOLORY SLOTOW GDY MAM WYSTARCZAJACO KREDYTOW
                    if (card.cost <= TurnSystem.Active_Player.Credits)
                    {
                        GameObject.Find("BattleField").GetComponent<BattleField>().MarkGreenActPlayerSlots();
                    }
                    else
                    {
                        GameObject.Find("BattleField").GetComponent<BattleField>().MarkRedActPlayerSlots();
                    }
                    GameObject.Find("BattleField").GetComponent<BattleField>().MarkRedOppPlayerSlots();
                    // ZACZYNAM CIAGNAC KARTE
                    canvasGroup.blocksRaycasts = false;
                    canvasGroup.alpha = .6f;
                    start_drag_position = rectTransform.anchoredPosition;
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (leftMouseDown)
        {
            if (interactable)
            {
                if (CheckIfOwnerIsActivePlayer())
                {
                    //Debug.Log("TRYING TO PLAY A CARD");
                    GameObject.Find("BattleField").GetComponent<BattleField>().ResetSlotsColor();
                    if (eventData.pointerCurrentRaycast.gameObject == null)
                    {
                        rectTransform.anchoredPosition = start_drag_position;
                    }
                    else if (eventData.pointerCurrentRaycast.gameObject.CompareTag("BattleSlot") && CompareTag("Card Unit"))
                    {
                        //Debug.Log("PLAY A CARD - put in battle slot!");
                        if (eventData.pointerCurrentRaycast.gameObject.GetComponent<CardSlot>().act_player_zone)
                        {
                            if (card.cost <= TurnSystem.Active_Player.Credits)
                            {
                                // PLAYING A CARD:
                                // 0. PLAY SOUND
                                SoundManagerScript.PlaySound("play_card");
                                // 1. INSTATIANTE AND ADD VARIABLES
                                GameObject CardUnitIcon = Instantiate(card.card_icon_template, new Vector2(0, 0), Quaternion.identity);
                                CardUnitIcon.transform.GetComponent<CardIco>().BigCardTemplate = BigCard;
                                CardUnitIcon.transform.GetComponent<CardIco>().artworkImage.sprite = card.artwork;
                                CardUnitIcon.transform.GetComponent<CardIco>().card = card;
                                CardUnitIcon.transform.GetComponent<CardIco>().card = card;
                                CardUnitIcon.transform.GetComponent<CardIco>().old_parent = eventData.pointerCurrentRaycast.gameObject.transform;
                                // 2. ADD OWNER
                                CardUnitIcon.transform.GetComponent<CardIco>().owner = TurnSystem.Active_Player;
                                // 3. PUT CARD UNIT ICO INTO BATTLEFIELD SLOT 
                                eventData.pointerCurrentRaycast.gameObject.transform.GetComponent<CardSlot>().PutUnit(CardUnitIcon);
                                // 4. DEACTIVATING CARDSM GAMEOBJECT AND PUTING INTO PLAYERUSEDCARDS Container
                                self.SetActive(false);
                                self.transform.SetParent(GameObject.Find("PlayerUsedCards").transform, false);
                                // 5. REMOVING FROM PLAYER CLASS OBJECT INTERNAL LIST
                                TurnSystem.Active_Player.RemoveCardFromHand(this.gameObject);
                                // 6. PAY COST
                                TurnSystem.Active_Player.Credits -= card.cost;
                            }
                            else
                            {
                                //Debug.Log("NO CREDITS - go back to hand!");
                                rectTransform.anchoredPosition = start_drag_position;
                            }

                        }
                        else
                        {
                            //Debug.Log("OTHER - go back to hand!");
                            rectTransform.anchoredPosition = start_drag_position;
                        }
                    }
                    else
                    {
                        //Debug.Log("OTHER OBJETCT - go back to hand!");
                        rectTransform.anchoredPosition = start_drag_position;
                    }
                    canvasGroup.blocksRaycasts = true;
                    canvasGroup.alpha = 1f;
                    during_dragging = false;
                }
            }
        }
        OrganizeHand();
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (leftMouseDown)
        {
            if (interactable)
            {
                if (CheckIfOwnerIsActivePlayer())
                {
                    rectTransform.anchoredPosition += eventData.delta;
                    during_dragging = true;
                }
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

    public List<GameObject> GetListofAllCardsinHand()
    {
        List<GameObject> all_cards = new List<GameObject>();
        foreach (Transform cardsm in GameObject.Find("PlayerHand").transform)
        {
            all_cards.Add(cardsm.gameObject);
        }
        return all_cards;
    }

    public void OrganizeHand()
    {
        List<Transform> Temporary_Hand_List = new List<Transform>();
        foreach (Transform card_in_cont in GameObject.Find("PlayerHand").transform)
        {
            Temporary_Hand_List.Add(card_in_cont);
        }
        foreach (Transform cardinthl in Temporary_Hand_List)
        {
            cardinthl.SetParent(GameObject.Find("Temp_Unit_Container").transform, false);
        }
        foreach (Transform cardinthl in Temporary_Hand_List)
        {
            cardinthl.SetParent(GameObject.Find("PlayerHand").transform, false);
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


