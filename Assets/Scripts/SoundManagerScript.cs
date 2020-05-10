using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip click_snd, melee_att_snd, position_att_snd, siege_att_snd, play_card_snd, move_ico_snd;
    static AudioSource audio_src;

    public void Start()
    {
        click_snd = Resources.Load<AudioClip>("click");
        melee_att_snd = Resources.Load<AudioClip>("melee");
        position_att_snd = Resources.Load<AudioClip>("position");
        siege_att_snd = Resources.Load<AudioClip>("siege");
        play_card_snd = Resources.Load<AudioClip>("play_card");
        move_ico_snd = Resources.Load<AudioClip>("move_ico");

        audio_src = GetComponent<AudioSource>();

    }

    public static void PlaySound(string clip)
    {
        switch (clip){
            case "click":
                audio_src.PlayOneShot(click_snd);
                break;
            case "melee":
                audio_src.PlayOneShot(melee_att_snd);
                break;
            case "position":
                audio_src.PlayOneShot(position_att_snd);
                break;
            case "siege":
                audio_src.PlayOneShot(siege_att_snd);
                break;
            case "play_card":
                audio_src.PlayOneShot(play_card_snd);
                break;
            case "move_ico":
                audio_src.PlayOneShot(move_ico_snd);
                break;
        }
    }
}
