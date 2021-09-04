using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateSticker_download : MonoBehaviour
{
    public int index=0;
    public string down_list="";
    public string str;
    GameObject aa = null;
    Button Btn_identity;

    public Sprite white;
    void Awake()
    {
        down_list = PlayerPrefs.GetString("List");
        Btn_identity = gameObject.GetComponent<Button>();
        Btn_identity.onClick.AddListener(Click_sticker);
        aa = transform.GetChild(2).gameObject;

        str = gameObject.name;
        string tmp="";

        for(int i = 3; i <= 5; i++)
        {
            tmp += str[i];
        }
        index = int.Parse(tmp);

        if (down_list[index] == '0')
        {
            aa.SetActive(true);
        }
        else
        {
            StartCoroutine(LoadingSticker());
        }
    }
    // Update is called once per frame
    public void Click_sticker()
    {
        string tmp = "";
        for(int i = 0; i < 354; i++)
        {
            if (i == index)
            {
                tmp += '1';
            }
            else
            {
                tmp += down_list[i];
            }
        }
        PlayerPrefs.SetString("List", tmp);

        aa.transform.GetChild(0).GetComponent<Image>().sprite = white;
        aa.GetComponent<Animator>().SetTrigger("Loading");

        down_list = PlayerPrefs.GetString("List");
        if (down_list[index] == '0')
        {
            aa.SetActive(true);
        }
        else
        {
            StartCoroutine(LoadingSticker());
        }

    }
    IEnumerator LoadingSticker()
    {
        yield return new WaitForSeconds(0.6f);
        aa.SetActive(false);
    }
}