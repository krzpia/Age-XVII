using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public string Name;
    public string Nationality;
    public int Morale;
    public int Credits;
    public int Credits_Incrementor;
    public int Max_Credits;
    public bool drawed_this_turn = false;
    public Sprite Char_Image;
    public Sprite Coin_Image;
    public Sprite POL_Card_Bcg;
    public Sprite SWE_Card_Bcg;
    public Sprite POL_Card_Template;
    public Sprite SWE_Card_Template;
    public GameObject Card_Template;
    public GameObject Card_Instant_Template;
    public List<Card> card_units_types = new List<Card>();
    public List<Card_Instant> card_instant_types = new List<Card_Instant>();
    public List<GameObject> all_cards = new List<GameObject>();
    public List<GameObject> cards_on_hand = new List<GameObject>();
   
    public void Awake()
    {
        Morale = 20;
        Credits = 1;
        Credits_Incrementor = 2;
        Max_Credits = 12;

        // CREATING CARD UNITS
        for (int i=0; i<card_units_types.Count; i++)
        {
            Card_Template.GetComponent<CardsmDisplay>().card = card_units_types[i];
            Card_Template.GetComponent<DragAndPlayCard>().card = card_units_types[i];
            Card_Template.GetComponent<CardsmDisplay>().nameText.text = card_units_types[i].card_name;
            Card_Template.GetComponent<CardsmDisplay>().artworkImage.sprite = card_units_types[i].artwork;
            Card_Template.GetComponent<CardsmDisplay>().artauthorText.text = card_units_types[i].artwork_name;
            Card_Template.GetComponent<CardsmDisplay>().costText.text = card_units_types[i].cost.ToString();
            Card_Template.GetComponent<CardsmDisplay>().MattText.text = card_units_types[i].m_att.ToString();
            Card_Template.GetComponent<CardsmDisplay>().RattText.text = card_units_types[i].r_att.ToString();
            Card_Template.GetComponent<CardsmDisplay>().SattText.text = card_units_types[i].s_att.ToString();
            Card_Template.GetComponent<CardsmDisplay>().act_defText.text = card_units_types[i].max_def.ToString();
            Card_Template.GetComponent<CardsmDisplay>().max_defText.text = card_units_types[i].max_def.ToString();
            Card_Template.GetComponent<CardsmDisplay>().type1.text = card_units_types[i].types[0].ToString();
            Card_Template.GetComponent<CardsmDisplay>().type2.text = card_units_types[i].types[1].ToString();
            Card_Template.GetComponent<CardsmDisplay>().type3.text = card_units_types[i].types[2].ToString();
            Card_Template.GetComponent<CardsmDisplay>().type4.text = card_units_types[i].types[3].ToString();
            if (card_units_types[i].nation == "SWE")
            {
                Card_Template.GetComponent<CardsmDisplay>().CardBackGround.GetComponent<Image>().sprite = SWE_Card_Bcg;
                Card_Template.GetComponent<CardsmDisplay>().CardTemplateImage.GetComponent<Image>().sprite = SWE_Card_Template;
                Card_Template.GetComponent<CardsmDisplay>().CardArtBcg.GetComponent<Image>().color = new Color32(140, 180, 210, 255);
            }
            if (card_units_types[i].nation == "POL")
            {
                Card_Template.GetComponent<CardsmDisplay>().CardBackGround.GetComponent<Image>().sprite = POL_Card_Bcg;
                Card_Template.GetComponent<CardsmDisplay>().CardTemplateImage.GetComponent<Image>().sprite = POL_Card_Template;
                Card_Template.GetComponent<CardsmDisplay>().CardArtBcg.GetComponent<Image>().color = new Color32(200,180,100, 255);
            }
            Debug.Log("CREATING UNIT CARD" + card_units_types[i].name);
            GameObject CardSm = Instantiate (Card_Template, new Vector3(200, 300, 0), Quaternion.identity);
            /// STWORZONY PREFAB, DODAJE DO LISTY
            all_cards.Add(CardSm);
        }
        // CREATING CARD INSTANTS
        for (int i = 0; i < card_instant_types.Count; i++)
        {
            Card_Instant_Template.GetComponent<CardinstDisplay>().card = card_instant_types[i];
            Card_Instant_Template.GetComponent<DragAndPlayInstantCard>().card_inst = card_instant_types[i];
            Card_Instant_Template.GetComponent<CardinstDisplay>().nameText.text = card_instant_types[i].card_name;
            Card_Instant_Template.GetComponent<CardinstDisplay>().artworkImage.sprite = card_instant_types[i].artwork;
            Card_Instant_Template.GetComponent<CardinstDisplay>().artauthorText.text = card_instant_types[i].artwork_name;
            Card_Instant_Template.GetComponent<CardinstDisplay>().description.text = card_instant_types[i].description;
            Card_Instant_Template.GetComponent<CardinstDisplay>().costText.text = card_instant_types[i].cost.ToString();
            Card_Instant_Template.GetComponent<CardinstDisplay>().type1.text = card_instant_types[i].types[0].ToString();
            Card_Instant_Template.GetComponent<CardinstDisplay>().type2.text = card_instant_types[i].types[1].ToString();
            Card_Instant_Template.GetComponent<CardinstDisplay>().type3.text = card_instant_types[i].types[2].ToString();
            Card_Instant_Template.GetComponent<CardinstDisplay>().type4.text = card_instant_types[i].types[3].ToString();
            if (card_instant_types[i].nation == "SWE")
            {
                Card_Instant_Template.GetComponent<CardinstDisplay>().CardBackGround.GetComponent<Image>().sprite = SWE_Card_Bcg;
                Card_Instant_Template.GetComponent<CardinstDisplay>().CardTemplateImage.GetComponent<Image>().sprite = SWE_Card_Template;
                Card_Instant_Template.GetComponent<CardinstDisplay>().CardArtBcg.GetComponent<Image>().color = new Color32(140, 180, 210, 255);
            }
            if (card_instant_types[i].nation == "POL")
            {
                Card_Instant_Template.GetComponent<CardinstDisplay>().CardBackGround.GetComponent<Image>().sprite = POL_Card_Bcg;
                Card_Instant_Template.GetComponent<CardinstDisplay>().CardTemplateImage.GetComponent<Image>().sprite = POL_Card_Template;
                Card_Instant_Template.GetComponent<CardinstDisplay>().CardArtBcg.GetComponent<Image>().color = new Color32(200,180,100, 255);
            }
            Debug.Log("CREATING INSTANT CARD" + card_instant_types[i].name);
            GameObject CardInst = Instantiate(Card_Instant_Template, new Vector3(200, 300, 0), Quaternion.identity);
            /// STWORZONY PREFAB, DODAJE DO LISTY
            all_cards.Add(CardInst);
        }

    }

    public void GetCardintoHand(GameObject card_sm)
    {
        cards_on_hand.Add(card_sm);
    }

    public void RemoveCardFromHand(GameObject card_sm)
    {
        cards_on_hand.Remove(card_sm);
    }



}
