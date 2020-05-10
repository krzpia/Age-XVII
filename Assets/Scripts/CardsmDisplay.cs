using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardsmDisplay : MonoBehaviour
{
    public Card card;
    public GameObject BigCard;
    public GameObject CardBackGround;
    public GameObject CardTemplateImage;
    public GameObject CardArtBcg;
    public Text nameText;
    public Text artauthorText;
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
    private List<Text> typeTextlist = new List<Text>();
    public Player owner;

    void Start()
    {
        typeTextlist.Add(type1);
        typeTextlist.Add(type2);
        typeTextlist.Add(type3);
        typeTextlist.Add(type4);
        nameText.text = card.card_name;
        artauthorText.text = card.artwork_name;
        for (int i = 0; i < card.types.Count; i++)
        {
            typeTextlist[i].text = card.types[i];
        }
        artworkImage.sprite = card.artwork;
        MattText.text = card.m_att.ToString();
        SattText.text = card.s_att.ToString();
        RattText.text = card.r_att.ToString();
        act_defText.text = card.act_def.ToString();
        max_defText.text = card.max_def.ToString();
        costText.text = card.cost.ToString();
    }

    public void PutOwner(Player player_owner_)
    {
        owner = player_owner_;
        //Debug.Log("OWNER: " + Player_Owner);
    }

    public void On_Awers()
    {
        //Debug.Log("AWERS");
        transform.rotation = Quaternion.Euler(0, 0, 0);
        CardBackGround.SetActive(false);
    }

    public void On_Rewers()
    {
        //Debug.Log("REWERS");
        transform.rotation = Quaternion.Euler(0, 0, 180);
        CardBackGround.SetActive(true);
    }
}
