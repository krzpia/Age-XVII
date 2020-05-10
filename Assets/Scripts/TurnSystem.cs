using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystem : MonoBehaviour
{

    public Text TurnIndicator;
    public Image Char_Image;
    public Text Player_Name;
    public Text Player_Morale;
    public Image Op_Char_Image;
    public Text Op_Player_Name;
    public Text Op_Morale;
    public Image Coin_Image;
    public Text Credits;
    public Text Credits_Increment;
    public GameObject DrawDeck;
    public GameObject PlayerHand;
    public GameObject OpponentHand;
    public GameObject PlayerGraveYard;
    public GameObject OpponentGraveYard;
    public GameObject PlayerUsedCards;
    public GameObject OpponentUsedCards;
    public GameObject BattleField_Obj;
    public Player Player1;
    public Player Player2;
    public static Player Active_Player;
    public static Player Op_Player;
    public bool Player1Turn;
    public bool Player2Turn;
    public int TurnCounter;
    private int PhaseCounter;

    // Start is called before the first frame update
    void Start()
    {
        Player1Turn = true;
        Player2Turn = false;
        TurnCounter = 0;
        PhaseCounter = 0;
        Active_Player = Player1;
        Op_Player = Player2;
        Debug.Log("ACTIVE PLAYER " + Active_Player);
        Debug.Log("STARTING DRAW CARD SCRIPT, DRAWING 5 CARDS");
        for (int i = 0; i < 5; i++)
        {
            GameObject playerCard = Instantiate(Active_Player.all_cards[Random.Range(0, Active_Player.all_cards.Count)], new Vector2(0, 0), Quaternion.identity);
            DrawDeck.GetComponent<DrawCard>().Put_On_Hand(playerCard);
            Active_Player.GetCardintoHand(playerCard);
            if (playerCard.CompareTag("Card Unit"))
            {
                playerCard.GetComponent<CardsmDisplay>().PutOwner(Active_Player);
            }
            else if (playerCard.CompareTag("Card Instant"))
            {
                playerCard.GetComponent<CardinstDisplay>().PutOwner(Active_Player);
            }
            //Debug.Log("DRAWING " + playerCard);
        }
        for (int i = 0; i < 5; i++)
        {
            GameObject playerCard = Instantiate(Op_Player.all_cards[Random.Range(0, Op_Player.all_cards.Count)], new Vector2(0, 0), Quaternion.identity);
            DrawDeck.GetComponent<DrawCard>().Put_On_Opponent_Hand(playerCard);
            Op_Player.GetCardintoHand(playerCard);
            if (playerCard.CompareTag("Card Unit"))
            {
                playerCard.GetComponent<CardsmDisplay>().PutOwner(Op_Player);
            }
            else if (playerCard.CompareTag("Card Instant"))
            {
                playerCard.GetComponent<CardinstDisplay>().PutOwner(Op_Player);
            }
            //Debug.Log("DRAWING " + playerCard);
        }
        Debug.Log("5 CARDS DRAWED");
        StartCoroutine("DrawAnimationWait");
        DrawDeck.GetComponent<DrawCard>().ActualizateDeckImage();
        OrganizeHand(PlayerHand);

    }

    IEnumerator DrawAnimationWait()
    {
        //Debug.Log("BEF");
        GetComponent<Button>().interactable = false;
        DisableInteracionofCardsonHand();
        DrawDeck.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(1.5f);
        DrawDeck.GetComponent<Animation>().Stop();
        DrawDeck.GetComponent<DrawCard>().DrawOneCard();
        GetComponent<Button>().interactable = true;
        OrganizeHand(PlayerHand);
        EnableInteracionofCardsonHand();
        //Debug.Log("AFT");
    }

    public void OnClick()
    {
        SoundManagerScript.PlaySound("click");
        NextTurn();
    }

    public void DisableInteracionofCardsonHand()
    {
        //Debug.Log("DISABLING INTERACTION");
        List<GameObject> all_cards = GetListofAllCardsinHand();
        //Debug.Log(all_cards.Count);
        foreach (GameObject card in all_cards)
        {
            if (card.CompareTag("Card Unit"))
            {
                card.GetComponent<DragAndPlayCard>().interactable = false;
            }
            else if (card.CompareTag("Card Instant"))
            {
                card.GetComponent<DragAndPlayInstantCard>().interactable = false;
            }
        }
    }

    public void EnableInteracionofCardsonHand()
    {
        //Debug.Log("ENABLING INTERACTION");
        List<GameObject> all_cards = GetListofAllCardsinHand();
        //Debug.Log(all_cards.Count);
        foreach (GameObject card in all_cards)
        {
            if (card.CompareTag("Card Unit"))
            {
                card.GetComponent<DragAndPlayCard>().interactable = true;
            }
            else if (card.CompareTag("Card Instant"))
            {
                card.GetComponent<DragAndPlayInstantCard>().interactable = true;
            }
        }
    }

    public List<GameObject> GetListofAllCardsinHand()
    {
        List<GameObject> all_cards = new List<GameObject>();
        foreach (Transform cardsm in PlayerHand.transform)
        {
            all_cards.Add(cardsm.gameObject);
        }
        return all_cards;
    }

    public void OrganizeHand(GameObject container)
    {
        List<Transform> Temporary_Hand_List = new List<Transform>();
        foreach (Transform card_in_cont in container.transform)
        {
            Temporary_Hand_List.Add(card_in_cont);
        }
        foreach (Transform cardinthl in Temporary_Hand_List)
        {
            cardinthl.SetParent(container.transform, false);
        }
    }

    public void ChangeHands(GameObject player_cont, GameObject opponent_cont)
    {
        List<Transform> Temporary_Hand_List = new List<Transform>();
        List<Transform> Temporary_OppH_List = new List<Transform>();
        //Debug.Log(player_cont.transform.childCount);
        foreach (Transform cardsminhand in player_cont.transform)
        {
            Temporary_Hand_List.Add(cardsminhand);
        }
        //foreach (Transform cardsminhand in PlayerHand.transform)
        //{
        //    Destroy(cardsminhand.parent);
        //}
        foreach (Transform cardinopp in opponent_cont.transform)
        {
            Temporary_OppH_List.Add(cardinopp);
        }
        //foreach (Transform cardinopp in OpponentHand.transform)
        //{
        //    Destroy(cardinopp.parent);
        //}
        foreach (Transform cardinthl in Temporary_Hand_List)
        {
            cardinthl.SetParent(opponent_cont.transform, false);
            if (cardinthl.CompareTag("Card Unit"))
            {
                cardinthl.GetComponent<CardsmDisplay>().On_Rewers();
            }
            else if (cardinthl.CompareTag("Card Instant"))
            {
                cardinthl.GetComponent<CardinstDisplay>().On_Rewers();
            }
        }
        Temporary_Hand_List.Clear();
        foreach (Transform cardinopl in Temporary_OppH_List)
        {
            cardinopl.SetParent(player_cont.transform, false);
            if (cardinopl.CompareTag("Card Unit"))
            {
                cardinopl.GetComponent<CardsmDisplay>().On_Awers();
            }
            else if(cardinopl.CompareTag("Card Instant"))
            {
                cardinopl.GetComponent<CardinstDisplay>().On_Awers();
            }
        }
        Temporary_OppH_List.Clear();

    }

    public void ChangeContainers(GameObject player_cont, GameObject opponent_cont)
    {
        List<Transform> Temporary_Hand_List = new List<Transform>();
        List<Transform> Temporary_OppH_List = new List<Transform>();
        //Debug.Log(player_cont.transform.childCount);
        foreach (Transform cardsminhand in player_cont.transform)
        {
            Temporary_Hand_List.Add(cardsminhand);
        }
        //foreach (Transform cardsminhand in PlayerHand.transform)
        //{
        //    Destroy(cardsminhand.parent);
        //}
        foreach (Transform cardinopp in opponent_cont.transform)
        {
            Temporary_OppH_List.Add(cardinopp);
        }
        //foreach (Transform cardinopp in OpponentHand.transform)
        //{
        //    Destroy(cardinopp.parent);
        //}
        foreach (Transform cardinthl in Temporary_Hand_List)
        {
            cardinthl.SetParent(opponent_cont.transform, false);
        }
        Temporary_Hand_List.Clear();
        foreach (Transform cardinopl in Temporary_OppH_List)
        {
            cardinopl.SetParent(player_cont.transform, false);
        }
        Temporary_OppH_List.Clear();

    }


    public void NextTurn()
    {
        if (Player1Turn)
        {
            // 1. OBLICZAM KARNE PUNKTY MORALE ZA POZYCJE JEDNOSTEK
            BattleField_Obj.GetComponent<BattleField>().ComputeMorale();
            // 1.5 ZMIENIAM KOLOT BORDER IMAGE ICON GRACZA
            List<GameObject> act_player_units = BattleField_Obj.GetComponent<BattleField>().GetListofActivePlayerUnitsIconsInBattlefield();
            foreach (GameObject act_player_unit in act_player_units)
            {
                act_player_unit.GetComponent<CardIco>().IcoBorderObject.GetComponent<Image>().color = new Color32(190, 50, 50, 255);
            }
            // 2. ZAMIENIAM GRACZY
            Player1Turn = false;
            Player2Turn = true;
            Active_Player = Player2;
            Op_Player = Player1;
            PhaseCounter += 1;
            // 3. ZAMIENIAM OBIEKTY NA PLANSZY
            ChangeHands(PlayerHand, OpponentHand);
            ChangeContainers(PlayerGraveYard, OpponentGraveYard);
            ChangeHands(PlayerUsedCards, OpponentUsedCards);
            DrawDeck.GetComponent<DrawCard>().ActualizateDeckImage();
            // 4. REPOZYCJONUJE JEDNOSTKI NA BATTLEFIELD i RESETUJE MOVEMENT oraz HAS ATTACKED JEDNOSTEK
            BattleField_Obj.GetComponent<BattleField>().NextTurn();
            // 5. MODYFIKUJE ZMIENNE NOWEGO AKTYWNEGO GRACZA
            Active_Player.drawed_this_turn = false;
            Active_Player.Credits = Active_Player.Credits_Incrementor;
            Active_Player.Credits_Incrementor++;
            if (Active_Player.Credits_Incrementor >= Active_Player.Max_Credits)
            {
                Active_Player.Credits_Incrementor = Active_Player.Max_Credits;
            }
            // 6. CIAGNE 1 KARTE
            StartCoroutine("DrawAnimationWait");
            // 7. ORGANIZUJE KARTY NA RECE
            OrganizeHand(PlayerHand);


        }
        else
        {
            // 1. OBLICZAM KARNE PUNKTY MORALE ZA POZYCJE JEDNOSTEK
            BattleField_Obj.GetComponent<BattleField>().ComputeMorale();
            // 1.5 ZMIENIAM KOLOT BORDER IMAGE ICON GRACZA
            List<GameObject> act_player_units = BattleField_Obj.GetComponent<BattleField>().GetListofActivePlayerUnitsIconsInBattlefield();
            foreach (GameObject act_player_unit in act_player_units)
            {
                act_player_unit.GetComponent<CardIco>().IcoBorderObject.GetComponent<Image>().color = new Color32(190, 50, 50, 255);
            }
            // 2. ZAMIENIAM GRACZY
            Player1Turn = true;
            Player2Turn = false;
            Active_Player = Player1;
            Op_Player = Player2;
            PhaseCounter += 1;
            // 3. ZAMIENIAM OBIEKTY NA PLANSZY
            ChangeHands(PlayerHand, OpponentHand);
            ChangeContainers(PlayerGraveYard, OpponentGraveYard);
            ChangeHands(PlayerUsedCards, OpponentUsedCards);
            DrawDeck.GetComponent<DrawCard>().ActualizateDeckImage();
            // 4. REPOZYCJONUJE JEDNOSTKI NA BATTLEFIELD i RESETUJE MOVEMENT JEDNOSTEK
            BattleField_Obj.GetComponent<BattleField>().NextTurn();
            // 5. MODYFIKUJE ZMIENNE NOWEGO AKTYWNEGO GRACZA
            Active_Player.drawed_this_turn = false;
            Active_Player.Credits = Active_Player.Credits_Incrementor;
            Active_Player.Credits_Incrementor++;
            if (Active_Player.Credits_Incrementor >= Active_Player.Max_Credits)
            {
                Active_Player.Credits_Incrementor = Active_Player.Max_Credits;
            }
            // 6. CIAGNE 1 KARTE
            StartCoroutine("DrawAnimationWait");
            // 7. ORGANIZUJE KARTY NA RECE
            OrganizeHand(PlayerHand);
        }

        if (PhaseCounter > 1)
        {
            TurnCounter += 1;
            PhaseCounter = 0;
        }
    }

    public Player Get_Active_Player()
    {
        if (Player1Turn)
        {
            return Player1;
        }
        else
        {
            return Player2;
        }
    }

    public Player Get_Opponent_Player()
    {
        if (Player1Turn)
        {
            return Player2;
        }
        else
        {
            return Player1;
        }
    }
    // Update is called once per frame
    void Update()
    {
        TurnIndicator.text = "Turn " + (TurnCounter + 1).ToString();
        Char_Image.sprite = Active_Player.Char_Image;
        Player_Name.text = Active_Player.Name;
        Player_Morale.text = Active_Player.Morale.ToString();
        Op_Morale.text = Op_Player.Morale.ToString();
        Op_Char_Image.sprite = Op_Player.Char_Image;
        Op_Player_Name.text = Op_Player.Name;
        Credits.text = Active_Player.Credits.ToString();
        Credits_Increment.text = Active_Player.Credits_Incrementor.ToString();
        Coin_Image.sprite = Active_Player.Coin_Image;
    }
}
