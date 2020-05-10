using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCard : MonoBehaviour
{
    public GameObject Hand;
    public GameObject Opponent_Hand;
    public GameObject TurnSystem_Object;
    public Sprite SWEBcg;
    public Sprite POLBcg;
    public Image deck_image;
    private Player active_player;
    private Player opponent_player;
 
    public void ActualizateDeckImage()
    {
        if (TurnSystem.Active_Player.Nationality == "POL")
        {
            deck_image.sprite = POLBcg;
        }
        if(TurnSystem.Active_Player.Nationality == "SWE")
        {
            deck_image.sprite = SWEBcg;
        }
        // DODATKOWO ROOT ELEMENT TEZ MA TEN SAM SPRITE BO JAK JEST ANIMACJA TO 'ZOSTAJE POD SPODEM'
        GetComponent<Image>().sprite = deck_image.sprite;
    }

    public void DrawOneCard()
    {
         active_player = TurnSystem_Object.GetComponent<TurnSystem>().Get_Active_Player();
         //Debug.Log(active_player);
         if (active_player.drawed_this_turn == false)
         {
              if (active_player.cards_on_hand.Count <= 6)
              {
                  GameObject playerCard = Instantiate(active_player.all_cards[Random.Range(0, active_player.all_cards.Count)], new Vector2(0, 0), Quaternion.identity);
                  playerCard.transform.SetParent(Hand.transform, false);
                  active_player.GetCardintoHand(playerCard);
                  active_player.drawed_this_turn = true;
                  if (playerCard.CompareTag("Card Unit"))
                  {
                       playerCard.GetComponent<CardsmDisplay>().PutOwner(active_player);
                  }
                  else if (playerCard.CompareTag("Card Instant"))
                  {
                       playerCard.GetComponent<CardinstDisplay>().PutOwner(active_player);
                  }

                  //Debug.Log("DRAWs " + playerCard);
            }  
         }
    }

    public void Put_On_Hand(GameObject card_sm)
    {
        card_sm.transform.SetParent(Hand.transform, false);
        if (card_sm.CompareTag("Card Unit"))
        {
            card_sm.GetComponent<CardsmDisplay>().On_Awers();
        }
        else if (card_sm.CompareTag("Card Instant"))
        {
            card_sm.GetComponent<CardinstDisplay>().On_Awers();
        }
        
    }

    public void Put_On_Opponent_Hand(GameObject card_sm)
    {
        card_sm.transform.SetParent(Opponent_Hand.transform, false);
        if (card_sm.CompareTag("Card Unit"))
        {
            card_sm.GetComponent<CardsmDisplay>().On_Rewers();
        }
        else if (card_sm.CompareTag("Card Instant"))
        {
            card_sm.GetComponent<CardinstDisplay>().On_Rewers();
        }
    }

}
