using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class LoadData : MonoBehaviour
{
    public int[] click_list;
    public GameObject[] stickerObj_btns;
    void Start()
    {
        click_list = new int[354];
        for(int i = 0; i < click_list.Length; i++)
        {
            click_list[i] = 0;
        }
        StartCoroutine(GetWebRequest());
        //StartCoroutine(PatchWebRequest());

        //StartCoroutine(PatchWebRequest());
    }
    IEnumerator PatchWebRequest()
    {
        UnityWebRequest request = new UnityWebRequest();
        string requestBodyString = "baaaaaaaaaaaaad";
        // Convert Json body string into a byte array
        byte[] requestBodyData = System.Text.Encoding.UTF8.GetBytes(requestBodyString);
        // Specify that our method is of type 'patch'
        WWWForm form = new WWWForm();
        form.AddField("score", 96);
        //request = UnityWebRequest.Post("https://www.ccms.kr/api/v1/arobject/fun_050/target/score", form);

        //using (request = UnityWebRequest.Put("https://www.ccms.kr/api/v1/arobject/fun_050/target/score", requestBodyData))
        using (request = UnityWebRequest.Post("https://www.ccms.kr/api/v1/arobject/ARObj_219/target/score", form))
        {
            request.method = "PATCH";
            
            yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            
        }
        
    }
    IEnumerator GetWebRequest()
    {
        UnityWebRequest request = new UnityWebRequest();
        using (request = UnityWebRequest.Get("https://www.ccms.kr/api/v1/arobject/?category=ARObject"))
        //*"))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
               // Debug.Log(request.downloadHandler.text);
                Byte[] results = request.downloadHandler.data;
                JTestClass jtcs = new JTestClass();
                jtcs = JsonToOject<JTestClass>(request.downloadHandler.text);
                jtcs.Print();
                for (int i = 0; i < jtcs.result.Count; i++)
                {
                 //   Debug.Log(jtcs.result[i].name);
                    string tmp = jtcs.result[i].name.Substring(6, 3);
                 //   Debug.Log(jtcs.result[i].usedCount);
                    int index = int.Parse(tmp);
                //    Debug.Log("인덱스 : " + index);
                //    Debug.Log("사용수 : "+ jtcs.result[i].usedCount);
                    click_list[index] = jtcs.result[i].usedCount;

                }
                int[] rankings = Enumerable.Repeat(0, click_list.Length).ToArray(); //모두 1로 초기화

                //② 처리: RANK
                for (int i = 0; i < click_list.Length; i++)
                {
                    rankings[i] = -1; //1등으로 초기화, 순위 배열을 매 회전마다 1등으로 초기화
                    for (int j = 0; j < click_list.Length; j++)
                    {
                        if (click_list[i] <= click_list[j]) //현재(i)와 나머지(j) 비교
                        {
                            rankings[i]++;         //RANK: 나보다 큰 점수가 나오면 순위 1증가
                        }
                    }
                }

                //③ 출력
                for (int i = 0; i < click_list.Length; i++)
                {
                    stickerObj_btns[i].transform.SetSiblingIndex(rankings[i]);
                }
            }
        }
    }
    /* 

        }*/
    string ObjectToJson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }

    T JsonToOject<T>(string jsonData)
    {
        return JsonUtility.FromJson<T>(jsonData);
    }
}
        /*    void Start()
            {
                StartCoroutine(DownloadFile());
            }

            IEnumerator DownloadFile()
            {
                var uwr = new UnityWebRequest("https://www.ccms.kr/api/v1/arobject/1 ", UnityWebRequest.kHttpVerbGET);
                string path = Path.Combine(Application.persistentDataPath, "unity2d.json");
                uwr.downloadHandler = new DownloadHandlerFile(path);
                yield return uwr.SendWebRequest();
                if (uwr.isNetworkError || uwr.isHttpError)
                    Debug.LogError(uwr.error);
                else
                    Debug.Log("File successfully downloaded and saved to " + path);
            }*/
        /*    public UnityEngine.UI.Image _img;

            void Start()
            {
                Download("https://s.pstatic.net/static/www/mobile/edit/2021/0402/mobile_145640368109.png");
            }

            public void Download(string url)
            {
                StartCoroutine(LoadFromWeb(url));
            }

            IEnumerator LoadFromWeb(string url)
            {
                UnityWebRequest wr = new UnityWebRequest(url);
                DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
                wr.downloadHandler = texDl;
                yield return wr.SendWebRequest();
                if (!(wr.isNetworkError || wr.isHttpError))
                {
                    Texture2D t = texDl.texture;
                    Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height),
                                             Vector2.zero, 1f);
                    _img.sprite = s;
                }
            }*/
