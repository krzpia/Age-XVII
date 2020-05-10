using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigCardInstDisplay : MonoBehaviour
{
    public GameObject Big_Card;
    public GameObject Big_Card_Template_Bcg;
    public Sprite SWE_template;
    public Sprite POL_template;
    public Text nameText;
    public Text artauthorText;
    public Text descriptionText;
    public Text type1;
    public Text type2;
    public Text type3;
    public Text type4;
    public Image artworkImage;
    public Text costText;
    private List<Text> typeTextlist;// = new List<Text>();
    private GameObject CardZoomed;

    public void Start()
    {
        //Big_Card.SetActive(false);
        //Debug.Log("Start Display Big Card Script");
    }

    public void Show_Big_Card_Instant(Card_Instant card)
    {
        Big_Card.SetActive(true);
        typeTextlist = new List<Text>();
        typeTextlist.Add(type1);
        typeTextlist.Add(type2);
        typeTextlist.Add(type3);
        typeTextlist.Add(type4);
        nameText.text = card.card_name;
        artworkImage.sprite = card.artwork;
        artauthorText.text = card.artwork_name;
        descriptionText.text = card.description;
        costText.text = card.cost.ToString();
        for (int i = 0; i < typeTextlist.Count; i++)
        {
            typeTextlist[i].text = card.types[i];
        }
        if (card.nation == "SWE")
        {
            Big_Card_Template_Bcg.GetComponent<Image>().sprite = SWE_template;
        }
        if (card.nation == "POL")
        {
            Big_Card_Template_Bcg.GetComponent<Image>().sprite = POL_template;
        }
        //Debug.Log("SHOWING :" + card.card_name);
        GameObject zoomfield = GameObject.Find("ZoomField");
        CardZoomed = Instantiate(Big_Card, new Vector3(200, 300, 0), Quaternion.identity);
        CardZoomed.transform.SetParent(zoomfield.transform, false);
    }

    public void End_Show_Big_Card()
    {
        Big_Card.SetActive(false);
        Destroy(CardZoomed);
    }

}
