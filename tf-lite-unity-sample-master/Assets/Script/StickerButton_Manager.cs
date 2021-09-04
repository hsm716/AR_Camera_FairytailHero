using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class StickerButton_Manager : MonoBehaviour
{

    [HideInInspector]
    public int[] click_list;
    
    [Header("스티커 버튼 오브젝트s")]
    public GameObject[] stickerObj_btns;

    //생성될 AR 오브젝트 ,by 황승민 ,2021/03/03
    public GameObject AR_Obj;


    [Header("UI 매니저")]
    public UI_Manager ui_m;
    [Header("게임 매니저")]
    public GameManager gameManager;
    [Header("FaceDetectSample 클래스")]
    public FaceDetectionSample fds;


    bool background = false;

    // 서버에서 Bundle(ar object)를 다운로드 진행상태등 관리하는 핸들
    AsyncOperationHandle<GameObject> handle;

    // 빈 오브젝트들 모음
    GameObject[] empty_list;


    // 즐겨찾기 상태 리스트
    [HideInInspector]
    public string my_list = "";
    [HideInInspector]
    public char[] my_list_;

    // 다운로드 상태 리스트
    [HideInInspector]
    public string down_list="";
    [HideInInspector]
    public char[] down_list_;

    [Header("** 다운로드 진행 이미지 **")]
    public Sprite downloading;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GetMyList();
        GetDownList();
    }
    private void Start()
    {
        click_list = new int[137];
        for (int i = 0; i < click_list.Length; i++)
        {
            click_list[i] = 0;
        }
        StartCoroutine(GetWebRequest());
    }

    // 즐겨찾기 값을 변경 후, 적용하는 함수
    public void SetMyList(int index)
    {
        my_list_ = my_list.ToCharArray();
        if (my_list_[index-217] == '1')
        {
            my_list_[index-217] = '0';
        }
        else
        {
            my_list_[index-217] = '1';
        }
        my_list = new string(my_list_);
        PlayerPrefs.SetString("MY", my_list);
        GetMyList();
    }
    public void GetMyList()
    {
        my_list = PlayerPrefs.GetString("MY");
        my_list_ = my_list.ToCharArray();


        // 현재 즐겨찾기 기능이 가능한 오브젝트들은 FUN,CUTE,ANIMAL(NEW)이기 때문에, 관련된 오브젝트들만 적용함.
        for (int i = 0; i < 137; i++) {
            if (my_list_[i] == '1')
            {
                ui_m.MyALL.transform.GetChild(i).gameObject.SetActive(true);
                GameObject[] objs = GameObject.FindGameObjectsWithTag("Fun");
                foreach(GameObject obj in objs)
                {
                    if (obj.name == ui_m.MyALL.transform.GetChild(i).gameObject.name)
                    {
                        obj.transform.GetChild(2).gameObject.SetActive(true);
                    }
                }
                objs = GameObject.FindGameObjectsWithTag("Cute");
                foreach (GameObject obj in objs)
                {
                    if (obj.name == ui_m.MyALL.transform.GetChild(i).gameObject.name)
                    {
                        obj.transform.GetChild(2).gameObject.SetActive(true);
                    }
                }
                objs = GameObject.FindGameObjectsWithTag("Animal");
                foreach (GameObject obj in objs)
                {
                    if (obj.name == ui_m.MyALL.transform.GetChild(i).gameObject.name)
                    {
                        obj.transform.GetChild(2).gameObject.SetActive(true);
                    }
                }

            }
            else
            {
                ui_m.MyALL.transform.GetChild(i).gameObject.SetActive(false);
                GameObject[] objs = GameObject.FindGameObjectsWithTag("Fun");
                foreach (GameObject obj in objs)
                {
                    if (obj.name == ui_m.MyALL.transform.GetChild(i).gameObject.name)
                    {
                        obj.transform.GetChild(2).gameObject.SetActive(false);
                    }
                }
                objs = GameObject.FindGameObjectsWithTag("Cute");
                foreach (GameObject obj in objs)
                {
                    if (obj.name == ui_m.MyALL.transform.GetChild(i).gameObject.name)
                    {
                        obj.transform.GetChild(2).gameObject.SetActive(false);
                    }
                }
                objs = GameObject.FindGameObjectsWithTag("Animal");
                foreach (GameObject obj in objs)
                {
                    if (obj.name == ui_m.MyALL.transform.GetChild(i).gameObject.name)
                    {
                        obj.transform.GetChild(2).gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    // 다운로드 리스트를 변경 및 갱신하는 함수
    public void SetDownList(int index, AsyncOperationHandle<GameObject> handle_)
    { 
        down_list_ = down_list.ToCharArray();
        if (down_list_[index] == '0')
        {

            StartCoroutine(Down_Sticker(handle_));
            down_list_[index] = '1';
        }
        down_list = new string(down_list_);
        PlayerPrefs.SetString("List",down_list);
        //GetDownList();
    }

    // 스티커 버튼이 클릭되면, 해당 버튼의 다운로드 이미지가 다운로드 진행상황에 맞게 애니매이션이 작동함.
    IEnumerator Down_Sticker(AsyncOperationHandle<GameObject> handle_)
    {
        Image down_back = gameManager.curBtnobj.transform.GetChild(gameManager.curBtnobj.transform.childCount-1).GetChild(0).GetComponent<Image>();
        down_back.sprite =downloading;
        while (true)
        {
            
            if (handle_.PercentComplete == 1f)
            {
                break;
            }
            else
            {
                down_back.fillAmount = handle_.PercentComplete;
                //Debug.Log("퍼센트 : " + down_back.fillAmount);
            }
            yield return null;
            //Debug.Log("다운 로드 상황 : " + handle.PercentComplete);
        }
        yield return null;
       GetDownList();
    }



    // 다운로드 리스트에 대한 내용을 통한 다운로드 이미지 비/활성화 여부
    public void GetDownList()
    {
        down_list = PlayerPrefs.GetString("List");
        down_list_ = down_list.ToCharArray();

        for (int i = 0; i < 354; i++)
        {
            if (down_list_[i] == '1')
            {
                GameObject[] objs = GameObject.FindGameObjectsWithTag("Fun");
                foreach (GameObject obj in objs)
                {
                    string tmp="";
                    tmp = obj.name.Substring(3, 3);
                    if(int.Parse(tmp) == i)
                        obj.transform.GetChild(obj.transform.childCount - 1).gameObject.SetActive(false);                   
                }
                objs = GameObject.FindGameObjectsWithTag("Cute");
                foreach (GameObject obj in objs)
                {
                    string tmp = "";
                    tmp = obj.name.Substring(3, 3);
                    if (int.Parse(tmp) == i)
                        obj.transform.GetChild(obj.transform.childCount - 1).gameObject.SetActive(false);
                }
                objs = GameObject.FindGameObjectsWithTag("Animal");
                foreach (GameObject obj in objs)
                {
                    string tmp = "";
                    tmp = obj.name.Substring(3, 3);
                    if (int.Parse(tmp) == i)
                        obj.transform.GetChild(obj.transform.childCount-1).gameObject.SetActive(false);
                }
                objs = GameObject.FindGameObjectsWithTag("Man");
                foreach (GameObject obj in objs)
                {
                    string tmp = "";
                    tmp = obj.name.Substring(3, 3);
                    if (int.Parse(tmp) == i)
                        obj.transform.GetChild(obj.transform.childCount - 1).gameObject.SetActive(false);
                }
                objs = GameObject.FindGameObjectsWithTag("Woman");
                foreach (GameObject obj in objs)
                {
                    string tmp = "";
                    tmp = obj.name.Substring(3, 3);
                    if (int.Parse(tmp) == i)
                        obj.transform.GetChild(obj.transform.childCount - 1).gameObject.SetActive(false);
                }

            }
            else
            {
                GameObject[] objs = GameObject.FindGameObjectsWithTag("Fun");
                foreach (GameObject obj in objs)
                {
                    string tmp = "";
                    tmp = obj.name.Substring(3, 3);
                    if (int.Parse(tmp) == i)
                        obj.transform.GetChild(obj.transform.childCount - 1).gameObject.SetActive(true);
                }
                objs = GameObject.FindGameObjectsWithTag("Cute");
                foreach (GameObject obj in objs)
                {

                    string tmp = "";
                    tmp = obj.name.Substring(3, 3);
                    if (int.Parse(tmp) == i)
                        obj.transform.GetChild(obj.transform.childCount - 1).gameObject.SetActive(true);
                }
                objs = GameObject.FindGameObjectsWithTag("Animal");
                foreach (GameObject obj in objs)
                {

                    string tmp = "";
                    tmp = obj.name.Substring(3, 3);
                    if (int.Parse(tmp) == i)
                        obj.transform.GetChild(obj.transform.childCount - 1).gameObject.SetActive(true);
                }
                objs = GameObject.FindGameObjectsWithTag("Man");
                foreach (GameObject obj in objs)
                {

                    string tmp = "";
                    tmp = obj.name.Substring(3, 3);
                    if (int.Parse(tmp) == i)
                        obj.transform.GetChild(obj.transform.childCount - 1).gameObject.SetActive(true);
                }
                objs = GameObject.FindGameObjectsWithTag("Woman");
                foreach (GameObject obj in objs)
                {

                    string tmp = "";
                    tmp = obj.name.Substring(3, 3);
                    if (int.Parse(tmp) == i)
                        obj.transform.GetChild(obj.transform.childCount - 1).gameObject.SetActive(true);
                }
            }
        }
    }


    // ccms에서 제공하는 정보 3가지 ( 다운 수, 사용 수, 행복도 수 )를 기반으로 랭킹을 적용하고 Hot 카테고리 버튼 배치에 영향을 줌.
    IEnumerator GetWebRequest()
    {
        UnityWebRequest request = new UnityWebRequest();
        using (request = UnityWebRequest.Get("https://www.ccms.kr/api/v1/arobject/?category=ARObject"))
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
                    click_list[index-217] = jtcs.result[i].usedCount;

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


                empty_list = GameObject.FindGameObjectsWithTag("Empty");
                foreach (GameObject empty in empty_list)
                {
                    empty.transform.SetAsLastSibling();
                }

            }
        }

    }

    // Json 관련 함수들
    string ObjectToJson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }
    T JsonToOject<T>(string jsonData)
    {
        return JsonUtility.FromJson<T>(jsonData);
    }
    //


    //오브젝트 버튼 클릭시 발생하는 함수, by 황승민, 2021/03/04 
    public void click_object(string ar_name)
    {
        // 최근 선택된 오브젝트( 스티커 버튼 오브젝트 )를 go에 담아서 이름을 통해 index를 추출함.
        GameObject go = EventSystem.current.currentSelectedGameObject;
        string go_index = go.name.Substring(3, 3);
        //

        // 스티커 버튼일 경우 curBtnobj에 최근 눌린 버튼정보를 대입함.
        if (go.CompareTag("Fun") || go.CompareTag("Cute") || go.CompareTag("Animal") || go.CompareTag("Man")|| go.CompareTag("Woman"))
        {
            gameManager.curBtnobj = EventSystem.current.currentSelectedGameObject;
        }
        // ar_name에 back이라는 이름이 속한 경우, background를 True로 하여 고정형 오브젝트임을 나타냄
        if (ar_name.Contains("back"))
        {
            background = true;
        }

        // 버튼이 여러번 눌려지는 것을 방지해서 isClick이 false일 때만, 버튼이 작동하게함.
        if (gameManager.isClick == false)
        {
            //https://www.ccms.kr/api/v1/arobject/jack/target/used
            gameManager.isClick = true;
            StartCoroutine(PatchWebRequest(EventSystem.current.currentSelectedGameObject.name));
            Debug.Log(EventSystem.current.currentSelectedGameObject.name);
            GameObject[] tmp = GameObject.FindGameObjectsWithTag("001");
            GameObject[] tmp2 = GameObject.FindGameObjectsWithTag("back_obj");
            for (int i = 0; i < tmp.Length; i++)
            {
                if (!ReferenceEquals(tmp[i], null))
                {
                    Addressables.ReleaseInstance(tmp[i]);
                }
                if (tmp[i] != null)
                {
                    Destroy(tmp[i]);
                }
            }
            for(int i = 0; i < tmp2.Length; i++)
            {
                if (!ReferenceEquals(tmp2[i], null))
                {
                    Addressables.ReleaseInstance(tmp2[i]);
                }
                if (tmp2[i] != null)
                {
                    Destroy(tmp2[i]);
                }
            }



            // 서버에서 Bundle파일을 다운로드 받아 생성하여, 오브젝트화 및 다운로드 진행상황 관리를 위해 Handle에 넣음.
            if (background) 
            {
                handle = Addressables.InstantiateAsync(ar_name);
            }
            else
            {
                handle = Addressables.InstantiateAsync(ar_name, new Vector3(0f, 0f, 0f), Quaternion.identity);
            }
            // index값과 handle을 매개변수로 넣어서, DownloadList값을 갱신 한다.
            SetDownList(int.Parse(go_index),handle);
            handle.Completed +=
            (AsyncOperationHandle<GameObject> obj) =>
            {
                if (obj.Result != null)
                {
                    gameManager.isClick = false;
                }
                AR_Obj = obj.Result;
                gameManager.curARobj = AR_Obj;
                // 오브젝트가 생성되면, 크기 설정을 위해 오브젝트 고유의 scale 값을 추출해서 사용한다.
                if (background)
                {
                    GameObject tmp_ = GameObject.FindGameObjectWithTag("001");
                    if (tmp_ != null)
                    {
                        gameManager.scaleX = tmp_.transform.localScale.x;
                        gameManager.scaleY = tmp_.transform.localScale.y; 
                    }
                }
                else
                {
                    if (AR_Obj != null)
                    {
                        gameManager.scaleX = AR_Obj.transform.localScale.x;
                        gameManager.scaleY = AR_Obj.transform.localScale.y;
                    }
                }
                GetMyList();
                string str_tmp = AR_Obj.name;
                str_tmp = str_tmp.Substring(5, 3);
                if (int.Parse(str_tmp) >= 217)
                {
                    if (my_list_[int.Parse(str_tmp) - 217] == '1')
                    {
                        ui_m.MyBtn_img.sprite = ui_m.isMySticker;
                    }
                    else
                    {
                        ui_m.MyBtn_img.sprite = ui_m.isNotMySticker;
                    }
                }
            };
           
        }


    }

    // ccms서버에 기재된 오브젝트의 사용 수(다운로드 수)를 증가하는 작업. 
    IEnumerator PatchWebRequest(string sticker_name)
    {
        string tmp = sticker_name.Substring(3,3);
        Debug.Log(tmp);
        UnityWebRequest request = new UnityWebRequest();
        string requestBodyString = "baaaaaaaaaaaaad";
        // Convert Json body string into a byte array
        byte[] requestBodyData = System.Text.Encoding.UTF8.GetBytes(requestBodyString);
        // Specify that our method is of type 'patch'
        using (request = UnityWebRequest.Put("https://www.ccms.kr/api/v1/arobject/arobj-"+ tmp+"/target/used", requestBodyData))
        {
            request.method = "PATCH";
            yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
               // Debug.Log(request.downloadHandler.text);
            }

        }

    }
}
