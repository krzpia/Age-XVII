using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public GameObject Big_Card;
    public GameObject Big_Card_Template_Bcg;
    public Sprite SWE_template;
    public Sprite POL_template;
    public Text nameText;
    public Text artauthorText;
    public Text descriptionText;
    public Text commentText;
    public Text type1;
    public Text type2;
    public Text type3;
    public Text type4;
    public Image artworkImage;
    public Text costText;
    public Text MattText;
    public Text RattText;
    public Text SattText;
    public Text act_defText;
    public Text max_defText;
    private List<Text> typeTextlist;// = new List<Text>();
    private GameObject CardZoomed;

    public void Start()
    {
        //Big_Card.SetActive(false);
        //Debug.Log("Start Display Big Card Script");
    }

    public void Show_Big_Card(Card card, int act_def, int m_att, int r_att, int s_att)
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
        commentText.text = card.add_info;
        costText.text = card.cost.ToString();
        MattText.text = m_att.ToString();
        RattText.text = r_att.ToString();
        SattText.text = s_att.ToString();
        act_defText.text = act_def.ToString();
        max_defText.text = card.max_def.ToString();
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
