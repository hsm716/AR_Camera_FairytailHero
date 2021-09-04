using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Xml;
using UnityEngine.UI;
using System.IO;
using VoxelBusters.ReplayKit;

public class UI_Manager : MonoBehaviour
{
    [Header("스티커 메뉴 스크롤뷰")]
    //스티커 메뉴 스크롤뷰,by 황승민, 21/03/03
    public GameObject scroll_list;
    public GameObject scroll_my;
    public GameObject scroll_hot;
    public GameObject scroll_fun;
    public GameObject scroll_cute;
    public GameObject scroll_special;
    public GameObject scroll_spring;
    public GameObject scroll_summer;
    public GameObject scroll_man;
    public GameObject scroll_woman;
    public GameObject scroll_animal;

    // UI 오브젝트들.. , by 황승민, 21/03/05

    [Header("UI 메인 캔버스")]
    public GameObject main_canvas;

    [Header("약관동의 관련 UIs")]
    public GameObject agreePanel;
    public GameObject warning;
    public GameObject agree_ok;

    [Header("스티커 메뉴 관련")]
    public GameObject sticker_menu;
    public GameObject touch_img;
    public GameObject object_main;
    public GameObject object_main_sticker;
    public Image menu_img;
    public Sprite sp_MenuOn;
    public Sprite sp_MenuOff;

    [Header("카메라 촬영버튼 관련")]
    public GameObject Capture;
    public Sprite recoding_capture_img;
    public Sprite orgin_capture_img;


    [Header("촬영모드 (사진/비디오) 전환버튼 관련")]
    public GameObject Record;
    public Sprite recoding_orgin_img;
    public Sprite recording_available_img;

    [Header("동화히어로 관련 링크 버튼")]
    public GameObject url; // 홈페이지 링크
    public GameObject help; // 유튜브 링크
    public GameObject Ad_arBtn; //ar컨텐츠 플레이스토어
    public GameObject Ad_Btn; // 2d컨텐츠 플레이스토어 

    [Header("녹화 및 저장 관련 UIs")]
    public GameObject rec_canvas;
    public GameObject rec;
    public GameObject record_help;
    public GameObject SavingCanvas;
    public GameObject touch_screen;



    [Header("하단 BAR 및 카메라 Cover")]
    public GameObject bottom_bar;
    public GameObject cameraCover1;
    public GameObject cameraCover2;

    [Header("감정분석 관련 UIs")]
    public RawImage picked_picture; // 갤러리에서 선택된 이미지를 렌더링해줄 이미지 오브젝트
    public GameObject showPicture_Pannel; // 감정분석관련 패널
    public Texture2D texture; // 선택된 이미지
    public GameObject play_emo; // 감정분석 시작버튼
    public GameObject back_home; // 뒤로가기 버튼

    // *** 설정 변수들 *** //
    bool saving = false; // saving 변수의 On/off 값을 통해 Save 진행 유무 결정
    private bool clicked_screen = false; // 녹화 중 화면이 클릭됨의 유무를 통해 촬영 중단을 결정함
    public bool is_record = false;


    public int rec_mode = 0; //짝수 : 사진 // 홀수 : 동영상 //, by 황승민, 21/03/29 

    int count1;// 녹화 저장시 딜레이 설정 => 긴 영상은 저장하는 시간을 보장해줘야 해주기 때문
    int count2;
    int time; // 타이머 시간

    // 사진/비디오 촬영 후, 이미지 저장시 설정 값들
    int resWidth; //가로길이
    int resHeight; //세로길이
    string path; //저장할 경로

    int width;
    int height;

    //help 버튼 시 불러오는 Url 주소 반환 값 저장을 위한 변수, by 황승민, 21/03/05
    string url__;
    string url___;

    [HideInInspector]
    public string curPickImg = "";
    public static string path_str_tmp; //경로 + 파일명을 저장하기위한 문자열
    public static string path_str_save; //경로 + 파일명을 저장하기위한 문자열



   

    GameObject tmp; // 임시 오브젝트 그릇

    // 녹화 관련 코루틴을 변수에 담아서 중단 및 진행을 관리함.
    IEnumerator rec_coroutine;







    [Header("FaceDetectionSample 클래스")]
    public FaceDetectionSample fds;







    [Header("오디오 매니저")]
    public ANAExample audioManager;





    [Header("갤러리 버튼")]
    public GameObject gallery;

    [Header("전면/후면 전환 버튼")]
    public GameObject cameraSwitch;


    [Header("1:1/9:16/Full 비율 전환 관련 UIs")]
    public GameObject ratioSwitch;
    public GameObject ratioPanel;
    public GameObject ratio_1e1;
    public GameObject ratio_16e9;
    public GameObject ratio_full;


    [Header("설정 관련 UIs")]
    public GameObject setting;
    public GameObject settingPanel;
    [Space]
    public GameObject gridSwitch;
    [Space]
    public GameObject timerSwitch;
    public Text time_txt;



    [Header("즐겨찾기 관련 UIs")]
    public Image MyBtn_img; // 버튼 이미지오브젝트
    public Sprite isMySticker; // On 이미지
    public Sprite isNotMySticker; // Off 이미지
    public GameObject MyBtn; // 버튼 오브젝트
    public GameObject MyALL; // My카테고리 오브젝트들을 담고있는 부모오브젝트 


    [Header("카테고리 text 스크롤뷰")]
    public GameObject Categori;

    [Header("게임 매니저")]
    public GameObject gm;
    GameManager gm_data;

    [Header("스티커 버튼 매니저")]
    public StickerButton_Manager SBM;




    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Home))
            {
                //home button
            }
            else if (Input.GetKey(KeyCode.Escape))
            {
                Click_Camera();
            }
            else if (Input.GetKey(KeyCode.Menu))
            {
                //menu button
            }
        }

        // 동화컨텐츠 링크가 존재하는 구 스티커 오브젝트들을 위한 코드
        tmp = GameObject.FindGameObjectWithTag("001");
        if (tmp != null)
        {
            Ad_arBtn.SetActive(true);
            Ad_Btn.SetActive(true);
        }
        //

        // 현재 선택된 AR오브젝트가 있다면,..
        if (gm_data.curARobj != null)
        {
            // 현재 AR오브젝트가 배경고정 및 얼굴추적 오브젝트라면, 
            if (gm_data.curARobj.CompareTag("back_obj") || gm_data.curARobj.CompareTag("001"))
            {
                // 메뉴가 활성화 되어있을 때, 즐겨찾기 버튼 활성화
                if (sticker_menu.activeSelf == true)
                {
                    if (gm_data.curARobj.name.Contains("fun") || gm_data.curARobj.name.Contains("cute") || gm_data.curARobj.name.Contains("ani"))
                    {
                        MyBtn.SetActive(true);
                    }
                    else
                    {
                        MyBtn.SetActive(false);
                    }
                }
                else
                {
                    MyBtn.SetActive(false);
                }

            }
            else
            {
                MyBtn.SetActive(false);
            }
        }
        //


        // 비디오 촬영을 중단할 때, 영상을 저장하는 부분
        if (clicked_screen)
        {

            //프리뷰를 저장하기 위한 딜레이count(너무 긴 영상인 경우 딜레이 없이 촬영종료시 저장이 안될 수 있음
            if (!saving)
            {
                count1++;
                if (count1 == 30)
                {
                    count1 = 0;
                    //save.SetActive(true);
                    if (ReplayKitManager.IsPreviewAvailable())
                    {
                        saving = true;
                        ReplayKitManager.SavePreview((error) =>
                        {
                            ;
                        });
                    }
                }
            }

            if (saving)
            {
                count2++;
                if (count2 >= 15)
                {

                    count2 = 0;
                    saving = false;
                    clicked_screen = false;
                    SavingCanvas.SetActive(false);
                    main_canvas.SetActive(true);
                    rec_canvas.SetActive(false);

                }
            }
        }



    }
    void Start()
    {
        gm_data = gm.GetComponent<GameManager>();
        count1 = 0;
        count2 = 0;
        resHeight = Screen.height;
        resWidth = Screen.width;
        path = Application.persistentDataPath + "/ScreenShot/";
        NativeGallery.RequestPermission(NativeGallery.PermissionType.Read); // 캡쳐를 위한 권한
        NativeGallery.RequestPermission(NativeGallery.PermissionType.Write);
        //
        StartCoroutine(GetRequest());

        sticker_menu.SetActive(false);
        scroll_hot.SetActive(true);


        ////////////////////////////////////////////////////////

        // UI 배치 컨트롤
        #region
        width = Screen.width;
        height = Screen.height;

        
        bottom_bar.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.145f);
        bottom_bar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);

        Capture.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.15f, width * 0.15f);
        Capture.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, width * 0.15f);

        object_main.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.2f, height * 0.075f);
        object_main.GetComponent<RectTransform>().anchoredPosition = new Vector2(width * 0.2f, height * 0.065f);
        object_main_sticker.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1 * height * 0.065f);
        touch_img.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.18f, height * 0.07f);
        touch_img.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1 * width * 0.2f * 0.33f, height * 0.065f);

        url.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.13f, width * 0.13f);
        url.GetComponent<RectTransform>().GetComponent<RectTransform>().anchoredPosition = new Vector2(-width * 0.02f, 0f);

        
        Record.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.080f, height * 0.03f);
        Record.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1 * width * 0.205f, height * 0.048f);
        
        gallery.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.100f, width * 0.100f);
        gallery.GetComponent<RectTransform>().anchoredPosition = new Vector2(-width * 0.055f, height * 0.038f);
        

        cameraSwitch.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.065f, width * 0.065f);
        cameraSwitch.GetComponent<RectTransform>().anchoredPosition = new Vector2(-width * 0.055f, -height * 0.175f);

        ratioSwitch.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.06f, height * 0.05f);
        ratioSwitch.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -height * 0.170f);

        setting.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.065f, width * 0.065f);
        setting.GetComponent<RectTransform>().anchoredPosition = new Vector2(width * 0.055f, -height * 0.175f);

        settingPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.32f, height * 0.115f);
        if (0.48f <= (width / height) && (width /height) <= 0.5f)
        {
            settingPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(width * 0.04f, -height * 0.260f);
            ratioPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -height * 0.260f);
        }
        else
        {
            settingPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(width * 0.04f, -height * 0.285f);
            ratioPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -height * 0.285f);
        }
       

        timerSwitch.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.069f, height * 0.0349f);
        timerSwitch.GetComponent<RectTransform>().anchoredPosition = new Vector2(-width * 0.073f, 0f);

        gridSwitch.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.069f, height * 0.0349f);
        gridSwitch.GetComponent<RectTransform>().anchoredPosition = new Vector2(width*0.063f, 0f);


        ratioPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.52f, height * 0.115f);
        

        ratio_1e1.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.069f, height * 0.0549f);
        ratio_1e1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-width * 0.170f, 0f);

        ratio_16e9.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.069f, height * 0.0549f);
        ratio_16e9.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);

        ratio_full.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.069f, height * 0.0549f);
        ratio_full.GetComponent<RectTransform>().anchoredPosition = new Vector2(width * 0.17f, 0f);


        help.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.09f, height * 0.06f);
        help.GetComponent<RectTransform>().GetComponent<RectTransform>().anchoredPosition = new Vector2(width * 0.09f, -height * 0.04f);

        sticker_menu.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.45f);
        sticker_menu.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, height * 0.053f);

        scroll_list.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.375f);
        scroll_list.GetComponent<RectTransform>().anchoredPosition = new Vector2(width * 0.05f, height * 0.20f);


        scroll_my.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.375f);
        scroll_my.GetComponentInChildren<GridLayoutGroup>().cellSize = new Vector2(width * 0.12f, width * 0.12f);
        scroll_my.GetComponentInChildren<GridLayoutGroup>().spacing = new Vector2(width * 0.075f, width * 0.075f);


        scroll_hot.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.375f);
        scroll_hot.GetComponentInChildren<GridLayoutGroup>().cellSize = new Vector2(width * 0.12f, width * 0.12f);
        scroll_hot.GetComponentInChildren<GridLayoutGroup>().spacing = new Vector2(width * 0.075f, width * 0.075f);
     


        scroll_fun.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.375f);
        scroll_fun.GetComponentInChildren<GridLayoutGroup>().cellSize = new Vector2(width * 0.12f, width * 0.12f);
        scroll_fun.GetComponentInChildren<GridLayoutGroup>().spacing = new Vector2(width * 0.075f, width * 0.075f);
     


        scroll_cute.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.375f);
        scroll_cute.GetComponentInChildren<GridLayoutGroup>().cellSize = new Vector2(width * 0.12f, width * 0.12f);
        scroll_cute.GetComponentInChildren<GridLayoutGroup>().spacing = new Vector2(width * 0.075f, width * 0.075f);

        scroll_special.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.375f);
        scroll_special.GetComponentInChildren<GridLayoutGroup>().cellSize = new Vector2(width * 0.12f, width * 0.12f);
        scroll_special.GetComponentInChildren<GridLayoutGroup>().spacing = new Vector2(width * 0.075f, width * 0.075f);

        scroll_spring.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.375f);
        scroll_spring.GetComponentInChildren<GridLayoutGroup>().cellSize = new Vector2(width * 0.12f, width * 0.12f);
        scroll_spring.GetComponentInChildren<GridLayoutGroup>().spacing = new Vector2(width * 0.075f, width * 0.075f);

        scroll_summer.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.375f);
        scroll_summer.GetComponentInChildren<GridLayoutGroup>().cellSize = new Vector2(width * 0.12f, width * 0.12f);
        scroll_summer.GetComponentInChildren<GridLayoutGroup>().spacing = new Vector2(width * 0.075f, width * 0.075f);

        scroll_man.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.375f);
        scroll_man.GetComponentInChildren<GridLayoutGroup>().cellSize = new Vector2(width * 0.12f, width * 0.12f);
        scroll_man.GetComponentInChildren<GridLayoutGroup>().spacing = new Vector2(width * 0.075f, width * 0.075f);

        scroll_woman.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.375f);
        scroll_woman.GetComponentInChildren<GridLayoutGroup>().cellSize = new Vector2(width * 0.12f, width * 0.12f);
        scroll_woman.GetComponentInChildren<GridLayoutGroup>().spacing = new Vector2(width * 0.075f, width * 0.075f);

        scroll_animal.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.375f);
        scroll_animal.GetComponentInChildren<GridLayoutGroup>().cellSize = new Vector2(width * 0.12f, width * 0.12f);
        scroll_animal.GetComponentInChildren<GridLayoutGroup>().spacing = new Vector2(width * 0.075f, width * 0.075f);




        //cover를 이용해서 카메라 비율에 따른 레터박스 크기 변경, by 황승민, 21/03/24

        if (0.48f <= (width / height) && (width / height) <= 0.5f)
        {
            cameraCover1.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.258f);
            cameraCover2.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.258f);
        }
        else
        {

            cameraCover1.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.275f);
            cameraCover2.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.275f);
        }

        cameraCover1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, height*0.001f);
        cameraCover2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, height*0.001f);


        rec.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.05f, height * 0.02f);
        rec.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1 * width * 0.4f,  height * 0.17f);

        touch_screen.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 2, height * 2);


        record_help.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.35f, height * 0.2f);
        record_help.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1 * width * 0.25f, height * 0.35f);

        play_emo.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.35f, height * 0.15f);
        play_emo.GetComponent<RectTransform>().anchoredPosition = new Vector2(width * 0.1f, height * 0.02f);


        back_home.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.35f, height * 0.15f);
        back_home.GetComponent<RectTransform>().anchoredPosition = new Vector2(-width * 0.1f, height * 0.02f);




        warning.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.95f, height * 0.55f);
        warning.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f,0f);

        agree_ok.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.225f, height * 0.10f);
        agree_ok.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -height * 0.15f);


        Categori.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.9f, height * 0.06f);
        Categori.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, height * 0.375f);

        MyBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.065f, width * 0.065f);
        MyBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-width* 0.05f, height * 0.305f);

        #endregion
    }




    private int LoadXML(DownloadHandler handler)
    {
        TextAsset textAsset = new TextAsset(handler.text);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text.Substring(textAsset.text.IndexOf("\n")));//System.Environment.NewLine)));
        /*
			XML 페이지에서 
			<Notice>
				<Sun>
					<play>on</play>
					<play>off</play> 둘중 하나
				</Sun>
			</Notice>
		*/
        XmlNodeList nodes = xmlDoc.SelectNodes("camera/help");
        //Debug.Log("THIS: " + nodes[0].SelectSingleNode("link").InnerText);
        url__ = nodes[0].SelectSingleNode("link").InnerText;

        nodes = xmlDoc.SelectNodes("camera/n00/link");
        url___ = nodes[0].InnerText;
        // on 이면 성공 , off 면 서비스 하지 않음.
        //if (nodes[0].SelectSingleNode("link").InnerText.Equals("on"))
        //	return 2;
        //		else
        // off 면 <alert>서비스 실패 문구</alert> 출력
        //			m_text.GetComponent<Text>().text = nodes[0].SelectSingleNode("alert").InnerText;
        return 1;
    }
    //##버튼 이벤트 함수들##//
    #region
    public void Click_MyBtn()
    {
        // 현재 오브젝트의 이름에서 명시된, Index값을 추출!
        string tmp = gm_data.curARobj.name;
        tmp = tmp.Substring(5, 3);

        

        // 해당 Index의 MyList값이 1이 라면, 0으로 변경하며 이미지들 또한 그에 맞게 변경 
        if (SBM.my_list_[int.Parse(tmp)-217] == '1')
        {
            MyBtn_img.sprite = isNotMySticker;
            MyALL.transform.GetChild(int.Parse(tmp) - 217).gameObject.SetActive(false);
        }
        else
        {
            MyBtn_img.sprite = isMySticker;
            MyALL.transform.GetChild(int.Parse(tmp) - 217).gameObject.SetActive(true);
        }
        // 새로운 List값을 설정하는 함수
        SBM.SetMyList(int.Parse(tmp));
        
        // 새로 설정된 값을 Update함.
        SBM.GetMyList();

    }
    // 비디오 촬영 중, 카메라 화면을 터치입력시 촬영종료
    public void StopRecording_touch()
    {
        clicked_screen = true;
        ReplayKitManager.StopRecording();
        StartCoroutine(Active_SavingCanvas());
    }


    // 스티커 메뉴 클릭 시,...
    public void Click_sticker()
    {
        if (sticker_menu.activeSelf == false)
        {
            bottom_bar.SetActive(false);
            Record.SetActive(false);
            gallery.SetActive(false);
            object_main.SetActive(false);
            menu_img.sprite = sp_MenuOn;
            sticker_menu.SetActive(true);
            touch_img.SetActive(false);
        }
        else
        {
            menu_img.sprite = sp_MenuOff;
            sticker_menu.SetActive(false);
            touch_img.SetActive(true);
        }

    }

    // 동화히어로 홈페이지 링크 버튼 클릭 시,...
    public void Click_link()
    {
        Application.OpenURL(url___);
    }



    // 사진/비디오 촬영모드 전환 버튼..
    public void Click_vedio()
    {   
        rec_mode += 1;
        if (rec_mode % 2==1)
        {
            is_record = true;
            record_help.SetActive(true);
            fds.timer_mode = 0;
            fds.timer.sprite = fds.noTimer;
            time_txt.text = "";
            Capture.GetComponent<Image>().sprite = recoding_capture_img;
            Record.GetComponent<Image>().sprite = recording_available_img;
        }
        else
        {
        
            is_record = false;
            record_help.SetActive(false);
            Capture.GetComponent<Image>().sprite = orgin_capture_img;
            Record.GetComponent<Image>().sprite = recoding_orgin_img;
        }
        
    }
        
    // 처음 보여지는 서비스 이용약관에 동의 버튼을 누를 시, 패널 사라지고 서비스 이용가능 하게 함, by 황승민 , 04/07
    public void Click_AgreeBtn()
    {
        agreePanel.SetActive(false);
    }
    // Help(유튜브 링크) 버튼 클릭 시 작동 함수, by 황승민, 2021/03/05
    public void Click_help()
    {
        /*
		if (url.Equals("https://www.youtube.com/channel/UCqG6kFwLdHtYnjFtD7i6Bew"))
		{
			not_exit.SetActive(true);
			not = true;
		}
		else
			Application.OpenURL(url);
            */
        Application.OpenURL(url__);
    }


    // 카메라 촬영 화면 클릭 시, 활성화된 스티커메뉴 비활성화
    public void Click_Camera()
    {
        GameObject[] panels = GameObject.FindGameObjectsWithTag("top_panel");
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }


        sticker_menu.SetActive(false);
        bottom_bar.SetActive(true);
        Record.SetActive(true);
        gallery.SetActive(true);
        object_main.SetActive(true);
    }


    // 촬영 버튼 클릭 시, 촬영 시작
    public void Click_Capture()
    {
        // UI들이 들어있는 main_canvas를 비활성화 
        main_canvas.SetActive(false);
        
        // 타이머 시간을 출력시켜주는 Text Object 비활성화
        time_txt.gameObject.SetActive(true);
        
        // 그리드는 비활성화 처리
        fds.grid.gameObject.SetActive(false);


        rec_coroutine = Capture_screen();
        // 사진 촬영하는 코루틴 호출
        StartCoroutine(rec_coroutine);
    }

    // 상단 UI 패널 활성화시, 이미 활성화 되있던 패널들은 비활성화 처리.
    public void Click_topBtn(GameObject btn)
    {
        if (btn.activeSelf == true)
        {
            btn.SetActive(false);
            return;
        }

        GameObject[] panels = GameObject.FindGameObjectsWithTag("top_panel");
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
        btn.SetActive(true);
    }

    // 갤러리 버튼 클릭 시, 이미지를 불러오는 작업.
    public void PickImage(int maxSize)
    {

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            char ch = '/';
            string[] spstring = path.Split(ch);
            for (int i = spstring.Length - 1; i < spstring.Length; i++)
            {
                //불러온 사진이름에서 사용된 스티커 분류해내는 작업
                char cutCh = '(';
                string tmp_ = spstring[i].Split(cutCh)[0];
                curPickImg = tmp_;
                Debug.Log(curPickImg);
            }


            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                texture = new Texture2D(maxSize, maxSize, TextureFormat.Alpha8, true);
                texture = NativeGallery.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                picked_picture.texture = texture;
                picked_picture.GetComponent<AspectRatioFitter>().aspectRatio = (float)texture.width / texture.height;

                showPicture_Pannel.SetActive(true);
            }
        }, "Select a PNG image", "image/jpg");
        Debug.Log("Permission result: " + permission);
    }
    #endregion
    
    // 코루틴 함수들
    #region


    // 비디오 촬영 시작 
    public IEnumerator Rec_start_go()
    {
        if (ReplayKitManager.IsPreviewAvailable())
            ReplayKitManager.Discard();
        ReplayKitManager.StartRecording(true);
        yield return new WaitForSeconds(0f);
    }

    //XML 페이지 정보 요청, by 황승민, 2021/03/05
    IEnumerator GetRequest()
    {
        yield return new WaitForEndOfFrame();

        // XML 페이지 가져오기
        UnityWebRequest www = UnityWebRequest.Get("http://mobilefrontier.cafe24.com/donghwahero/access/donghwa_camera.xml");

        yield return www.SendWebRequest();

        int isLoading = 0; // 0 : Network Error , 1 : Error , 2 : Succes
        if (www.isNetworkError || www.isHttpError)
        {
            isLoading = 0;
            //			m_text.GetComponent<Text>().text = "인터넷에 연결되어 있지 않습니다.\n네트워크 상태를 확인해 주십시요.";
            Debug.Log(www.error);
        }
        else
        {
            // XML 페이지 파싱
            isLoading = LoadXML(www.downloadHandler);
            //Debug.Log("Success! : " + www.downloadHandler.text);
        }
    }

    // 설정된 시간에 따른 타이머 작동 
    IEnumerator Timer_start(int sec)
    {
        for (int i = 0; i < sec; i++)
        {
            time -= 1;
            yield return new WaitForSeconds(1f);
            time_txt.text = "" + time;
        }
        time_txt.text = "";
        time_txt.gameObject.SetActive(false);
    }

    // 촬영 버튼 클릭 시, 사진/비디오 촬영 작업
    IEnumerator Capture_screen()
    {
        yield return new WaitForEndOfFrame();

        // 비디오 촬영
        if (is_record)
        {
            rec_canvas.SetActive(true);
            StartCoroutine(Rec_start_go());
            yield return new WaitForEndOfFrame();
            audioManager.CameraShot();
        }
        else // 사진 촬영
        {

            // 타이머 시간값 적용
            time_txt.gameObject.SetActive(true);
            time = fds.timer_mode;
            time_txt.text = "" + time;
            StartCoroutine(Timer_start(time));
            if (time == 0)
            {
                yield return new WaitForSeconds(time);
            }
            else
            {
                yield return new WaitForSeconds(time + 1);
            }
            time_txt.gameObject.SetActive(false);
            //

            // 찰칵음 출력
            audioManager.CameraShot();
            //

            yield return new WaitForEndOfFrame();

            // 저장될 경로에 폴더가 없으면 생성
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                Directory.CreateDirectory(path);
            }
            //


            // 현재 오브젝트가 없으면 공백, 아니면 오브젝트 이름 저장
            string tmp="";
            if (gm_data.curARobj != null)
            {
                tmp = gm_data.curARobj.name;    
            }
            if (tmp == null)
            {
                tmp = "";
            }
            //

            // 파일 저장 시, 오브젝트 이름도 같이 저장해서 인덱스를 추출 가능하게 해줌.
            path_str_save = path + tmp + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".jpg";
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            screenShot.Apply();

            NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(screenShot, "동화히어로 카메라", path_str_save);
            Debug.Log("Permission result : " + permission);

            Destroy(screenShot);
            main_canvas.SetActive(true);
            if (fds.cur_ratio == FaceDetectionSample.Aspect_ratio.nine_by_sixteen)
            {
                cameraCover1.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.14f);
                cameraCover1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, height * 0.001f);

                cameraCover2.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.14f);
                cameraCover2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, height * 0.001f);
            }
            else if (fds.cur_ratio == FaceDetectionSample.Aspect_ratio.full_screen)
            {
                cameraCover1.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0f);
                cameraCover1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, height * 0.001f);

                cameraCover2.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0f);
                cameraCover2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, height * 0.001f);
            }

            fds.grid.gameObject.SetActive(fds.grid_onoff);
        }
    }

    // 저장 canvas 활성화 
    IEnumerator Active_SavingCanvas()
    {
        yield return new WaitForSeconds(0.5f);
        SavingCanvas.SetActive(true);
    }
    #endregion


}