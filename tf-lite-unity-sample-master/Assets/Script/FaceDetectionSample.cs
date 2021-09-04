using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;

/// <summary>
/// BlazeFace from MediaPile
/// https://github.com/google/mediapipe
/// https://viz.mediapipe.dev/demo/face_detection
/// </summary>
/// 


public class FaceDetectionSample : MonoBehaviour
{
    
    bool front_back_mode = false; //false : 후면, true : 전면, by 황승민, 21/03/19
    
    
    [Header("모델 파일")]
    //.ftlite 확장자 파일들을 팝업화 시켜 놓음, facemodelFile을 지정 함, by 황승민, 210310
    [SerializeField, FilePopup("*.tflite")] string faceModelFile = "coco_ssd_mobilenet_quant.tflite";

    [Header("카메라뷰 이미지오브젝트")]
    // 디바이스 카메라를 통해 보여지는 화면을 지정, by 황승민, 210310
    [SerializeField] RawImage cameraView = null;

    [Header("카메라 비율 설정")]
    [SerializeField] AspectRatioFitter fit;



    [Header("WebCamTexture : 디바이스 카메라 정보")]
    // 디바이스 카메라 관련 정보를 담는 변수, by 황승민, 210310
    public WebCamTexture webcamTexture;
    // FaceDetect 클래스 변수, by 황승민, 210310
    FaceDetect faceDetect;
    // FaceDetect 클래스 안에 있는 내부 클래스 Result에 대한 리스트 변수, by 황승민, 210310
    List<FaceDetect.Result> results;

    // 그리기 도구와 같은 역할을 하는 클래스 변수, by 황승민, 210310
    PrimitiveDraw draw;
    // 디바이스 카메라화면을 출력시킬 RawImage판(?)의 꼭짓점 위치 정보, by 황승민, 210310
    Vector3[] rtCorners = new Vector3[4];
    // camera 이름 저장 변수, by 황승민, 210310
    string cameraName;

    [Header("UI 매니저")]
    public UI_Manager ui_m;
    [Header("게임 매니저")]
    public GameObject gameManager_obj;
    GameManager gamemanager;

    //AR오브젝트가 생성되고 최초 한번만 scale값을 저장하기 위한 bool 변수, by 황승민, 21/03/24
    public bool scanScale = false;

    //전면카메라 거울모드를 위한 rotation 변경 값, by 황승민, 21/03/24
    float angle_;

    float scaleX;
    float scaleY;

    WebCamDevice[] devices;

    private bool camAvailable;
    private Texture defaultBackground;

    public enum Aspect_ratio { one_by_one, full_screen, nine_by_sixteen};


    [Header("화면 비율 관련")]
    public Aspect_ratio cur_ratio;
    public Image screen_ratio;
    public Sprite white_viewSwitch;
    [Space]
    public Image img_1e1;
    public Image img_16e9;
    public Image img_full;
    [Space]
    public Sprite sp_1e1_On;
    public Sprite sp_16e9_On;
    public Sprite sp_full_On;
    [Space]
    public Sprite sp_1e1_Off;
    public Sprite sp_16e9_Off;
    public Sprite sp_full_Off;

    [Header("그리드 관련")]
    public bool grid_onoff;
    public RawImage grid;
    public Sprite grid_On;
    public Sprite grid_Off;
    public Text gridSwitch_txt;

    [Header("타이머 관련")]
    public int timer_mode = 0; // 0: NO,  3: three, 7: seven
    public Image timer;
    public Text timeSwitch_txt;
    public Sprite noTimer;
    public Sprite threeTimer;
    public Sprite sevenTimer;

    [Header("설정 및 전후면 전환 이미지")]
    public Sprite gray_viewSwitch;
    public Sprite white_setting;
    public Sprite gray_setting;


    float width;
    float height;
    void Awake()
    {
        width = Screen.width;
        height = Screen.height;
        cur_ratio = Aspect_ratio.full_screen;
        defaultBackground = cameraView.texture;
        devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.Log("No camera detected");
            camAvailable = false;
            return;
        }
        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                webcamTexture = new WebCamTexture(devices[i].name, 640, 1136,30);
            }
        }
        if (webcamTexture == null)
        {
            Debug.Log("Unable to find back camera");
            return;
        }
        // 복사시킬 파일 경로를 detectionPath 변수에 담아줍니다, by 황승민, 210310
        string detectionPath = Path.Combine(Application.streamingAssetsPath, faceModelFile);

        // FaceDetect 클래스의 detectionPath를 매개변수로 하는 생성자를 호출함, by 황승민, 210310
        faceDetect = new FaceDetect(detectionPath);
   
        gamemanager = gameManager_obj.GetComponent<GameManager>();

        camAvailable = true;

        float ratio = (float)webcamTexture.width / (float)webcamTexture.height;
        cameraView.rectTransform.localScale = new Vector3(ratio, ratio, 1f);
        fit.enabled = false;
        screen_ratio.sprite = sp_full_On;
        cur_ratio = Aspect_ratio.full_screen;
        webcamTexture.filterMode = FilterMode.Trilinear;
        cameraView.transform.rotation = Quaternion.Euler(angle_, 0f,0);
        // 디바이스 카메라 작동 On, by 황승민, 210310
        webcamTexture.Play();
        // 디바이스 카메라 정보를, cameraView의 texture에 넣어줌으로써 카메라가 비추는 화면을 볼 수 있게 해줌, by 황승민, 210310
        cameraView.texture = webcamTexture;
        select_1e1();

        Debug.Log($"Starting camera: {cameraName}");

        // PrimitDraw클래스의 생성자를 통해서 그리는 것을 보여주게 하는 메인 카메라 등록 및 레이어 설정을 하게함., by 황승민, 210310
        draw = new PrimitiveDraw(Camera.main, gameObject.layer);
    }


    // 전면, 후면 변경 함수, by 황승민, 21/03/24
    public void SwitchView()
    {

        if (front_back_mode == false) //후면 -> 전면
        {
            front_back_mode = true;
        }
        else//전면 -> 후면
        {
            front_back_mode = false;
        }
        if (front_back_mode)
        {
            cameraName = devices[1].name;
            angle_ = -180f;
        }
        else
        {
            cameraName = devices[0].name;
            angle_ = 0f;
        }
        cameraView.transform.rotation = Quaternion.Euler(angle_, 0f, 0f);
        webcamTexture = new WebCamTexture(cameraName, 640, 1136, 30);
        webcamTexture.filterMode = FilterMode.Trilinear;

        webcamTexture.Play();
        cameraView.texture = webcamTexture;
    }
    void OnDestroy()
    {
        webcamTexture?.Stop();
        faceDetect?.Dispose();
        draw?.Dispose();
    }

    void Update()
    {
        if (!camAvailable)
            return;

        // faceDetect의 Invoke함수를 지속적으로 호출하여 Detect한 정보를 업데이트, by 황승민, 210310
        faceDetect.Invoke(webcamTexture);

        // 카메라의 뷰의 material을 faceDetect의 transformMat으로 지속적 업데이트, by 황승민, 210310
        cameraView.material = faceDetect.transformMat;
        // 카메라 뷰의 rectTransform, 즉 크기 및 위치 정보를 지속적 업데이트, by 황승민, 210310
        cameraView.rectTransform.GetWorldCorners(rtCorners);

        scaleX = gamemanager.scaleX;
        scaleY = gamemanager.scaleY;

        // faceDetect한 결과 값을 results에 받아옴, by 황승민, 210310
        var results = faceDetect.GetResults();
        if (results == null) return;

        //faceDetect한 내용을 바탕으로한 results 값을 그려줌, by 황승민, 210310
        DrawResults(results);
    }
    public void select_1e1()
    {
        // 1:1 비율은 ratio값을 1로 설정한다 => Aspect Ratio fitter 컴포넌트를 이용하므로.
        float ratio = 1f;

        //cover를 이용해서 카메라 비율에 따른 레터박스 크기 변경, by 황승민, 21/03/24
        if ( 0.48f <= (width/height) && (width / height) <= 0.5f)
        {
            ui_m.settingPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(width * 0.04f, -height * 0.260f);
            ui_m.ratioPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -height * 0.260f);
            ui_m.cameraCover1.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.258f);
            ui_m.cameraCover2.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.258f);
        }
        else
        {
            ui_m.settingPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(width * 0.04f, -height * 0.285f);
            ui_m.ratioPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -height * 0.285f);
            ui_m.cameraCover1.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.277f);
            ui_m.cameraCover2.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.277f);
        }
        ui_m.cameraCover1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, height * 0.001f);
        ui_m.cameraCover2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, height * 0.001f);

        ui_m.bottom_bar.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.125f);

        ui_m.cameraSwitch.GetComponent<RectTransform>().anchoredPosition = new Vector2(-width * 0.055f, -height * 0.175f);

        ui_m.ratioSwitch.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -height * 0.170f);

        ui_m.setting.GetComponent<RectTransform>().anchoredPosition = new Vector2(width * 0.055f, -height * 0.175f);

        ui_m.setting.GetComponent<Image>().sprite = gray_setting;
        ui_m.cameraSwitch.GetComponent<Image>().sprite = gray_viewSwitch;

        ui_m.rec.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.05f, height * 0.02f);
        ui_m.rec.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1 * width * 0.4f, height * 0.17f);

        cameraView.rectTransform.localScale = new Vector3(ratio, ratio, 1f);
        grid.rectTransform.localScale = new Vector3(ratio, ratio, 1f);
        fit.enabled = true;
        screen_ratio.sprite = sp_1e1_Off;
        cur_ratio = Aspect_ratio.one_by_one;

        webcamTexture = new WebCamTexture(cameraName, 640, 1136, 30);

        cameraView.transform.rotation = Quaternion.Euler(angle_, 0f,0f);
        webcamTexture.filterMode = FilterMode.Trilinear;


        webcamTexture.Play();
        cameraView.texture = webcamTexture;
        ui_m.ratioPanel.SetActive(false);
        img_full.sprite = sp_full_Off;
        img_1e1.sprite = sp_1e1_On;
        img_16e9.sprite = sp_16e9_Off;
    }
    public void select_16e9()
    {
        // WebCamTexture의 너비 높이 값을 이용해서 ratio 값을 구한다.
        float ratio = (float)webcamTexture.width / (float)webcamTexture.height;

        ui_m.setting.GetComponent<Image>().sprite = gray_setting;
        ui_m.cameraSwitch.GetComponent<Image>().sprite = gray_viewSwitch;

        //cover를 이용해서 카메라 비율에 따른 레터박스 크기 변경, by 황승민, 21/03/24
        ui_m.cameraCover1.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.14f);
        ui_m.cameraCover1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, height * 0.001f);

        ui_m.cameraCover2.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.14f);
        ui_m.cameraCover2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, height * 0.001f);

        ui_m.cameraSwitch.GetComponent<RectTransform>().anchoredPosition = new Vector2(-width * 0.055f, -height * 0.075f);
        ui_m.ratioSwitch.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -height * 0.070f);
        ui_m.setting.GetComponent<RectTransform>().anchoredPosition = new Vector2(width * 0.055f, -height * 0.075f);

        ui_m.ratioPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -height * 0.145f);
        ui_m.settingPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(width * 0.04f, -height * 0.145f);

        ui_m.rec.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.05f, height * 0.02f);
        ui_m.rec.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1 * width * 0.4f, height * 0.32f);


        //ratio 값을 토대로 카메라뷰의 크기를 설정하고, 그에 맞게 그리드 크기도 결정해줌.
        cameraView.rectTransform.localScale = new Vector3(ratio, ratio, 1f);
        grid.rectTransform.localScale = new Vector3(1f, ratio, 1f);
        fit.enabled = false;
        screen_ratio.sprite = sp_16e9_Off;
        cur_ratio = Aspect_ratio.nine_by_sixteen;

        webcamTexture = new WebCamTexture(cameraName, 640, 1136, 30);
        cameraView.transform.rotation = Quaternion.Euler(angle_, 0f, 0f);
        webcamTexture.filterMode = FilterMode.Trilinear;


        webcamTexture.Play();
        cameraView.texture = webcamTexture;
        ui_m.ratioPanel.SetActive(false);
        img_full.sprite = sp_full_Off;
        img_1e1.sprite = sp_1e1_Off;
        img_16e9.sprite = sp_16e9_On;
    }
    public void select_full()
    {
        fit.enabled = false;
        float ratio = (float)webcamTexture.width / (float)webcamTexture.height;


        ui_m.setting.GetComponent<Image>().sprite = white_setting;
        ui_m.cameraSwitch.GetComponent<Image>().sprite =white_viewSwitch;


        //cover를 이용해서 카메라 비율에 따른 레터박스 크기 변경, by 황승민, 21/03/24
        ui_m.cameraCover1.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0f);
        ui_m.cameraCover1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, height * 0.001f);

        ui_m.cameraCover2.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0f);
        ui_m.cameraCover2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, height * 0.001f);

        ui_m.cameraSwitch.GetComponent<RectTransform>().anchoredPosition = new Vector2(-width * 0.055f, -height * 0.075f);
        ui_m.ratioSwitch.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -height * 0.070f);
        

        ui_m.setting.GetComponent<RectTransform>().anchoredPosition = new Vector2(width * 0.055f, -height * 0.075f);
        //ui_m.bottom_bar.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0f);

        ui_m.settingPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(width * 0.04f, -height * 0.145f);
        ui_m.ratioPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -height * 0.145f);

        ui_m.rec.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.05f, height * 0.02f);
        ui_m.rec.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1 * width * 0.4f, height * 0.42f);



        cameraView.rectTransform.localScale = new Vector3(ratio*1.47f, ratio*1.47f, 1f);
        grid.rectTransform.localScale = new Vector3(1f, ratio*1.47f, 1f);
        screen_ratio.sprite = sp_full_On;
        cur_ratio = Aspect_ratio.full_screen;

        webcamTexture = new WebCamTexture(cameraName, 640, 1136, 30);

        cameraView.transform.rotation = Quaternion.Euler(angle_, 0f, 0f);
        webcamTexture.filterMode = FilterMode.Trilinear;


        webcamTexture.Play();
        cameraView.texture = webcamTexture;
        ui_m.ratioPanel.SetActive(false);
        img_full.sprite = sp_full_On;
        img_1e1.sprite = sp_1e1_Off;
        img_16e9.sprite = sp_16e9_Off;
    }
    public void Switch_ratio()
    {
        if (ui_m.ratioPanel.activeSelf == true)
        {
            ui_m.ratioPanel.SetActive(false);
        }
        else
        {
            ui_m.ratioPanel.SetActive(true);
        }
    }
    void DrawResults(List<FaceDetect.Result> results)
    {
        Vector3 min = rtCorners[0];
        Vector3 max = rtCorners[2];



        // 오브젝트의 거리와 Detect된 얼굴의 Position 값과 비교해서 가장 가까운 얼굴 위치로 추적하게 설정함. => 하나의 얼굴에만 고정하는 효과.
        GameObject obj = GameObject.FindGameObjectWithTag("001");
        int nearIndex=0;
        float nearestDist=100000;
        //draw.color = Color.green;
        for (int i = 0; i < results.Count; i++)
        {
            var result_ = results[i];

            Rect rect_ = MathTF.Lerp(min, max, result_.rect, true);
            float x = rect_.x * 100;
            float y = rect_.y *100;
            float dist = Mathf.Sqrt((obj.transform.position.x - x)* (obj.transform.position.x - x) + (obj.transform.position.y - y)* (obj.transform.position.y - y));
            if(nearestDist > dist)
            {
                nearestDist = dist;
                nearIndex = i;
            }

        }
        
            var result = results[nearIndex];

            Rect rect = MathTF.Lerp(min, max, result.rect, true);

            //생성된 AR오브젝트를 찾아 저장, by 황승민, 21/03/24
            //GameObject back_obj = GameObject.FindGameObjectWithTag("back_obj");
            obj = GameObject.FindGameObjectWithTag("001");

            //AR오브젝트가 사람얼굴 detect rect의 position, height width 값을 통해 오브젝트 크기 위치 조절, by 황승민, 21/03/24
            Vector3 updatePosition;
            Vector3 updateScale;
            Vector3 updateRotation;
            if (obj.name.Contains("fun") || obj.name.Contains("cute") || obj.name.Contains("ani") || obj.name.Contains("동화아이콘"))
            {
                updateRotation = new Vector3(0, 0, Input.acceleration.x * 90);
                if (front_back_mode)
                {
                    updatePosition = new Vector3(-1 * (rect.position.x + rect.width * 0.5f), -1 * (rect.position.y + rect.height * 0.5f), obj.transform.position.z);
                    updateScale = new Vector3(scaleX * rect.width * 0.5f, -1f * (scaleY * rect.height * 0.5f), 1f);
                }
                else
                {
                    updatePosition = new Vector3((rect.position.x + rect.width * 0.5f), (rect.position.y + rect.height * 0.5f), obj.transform.position.z);
                    updateScale = new Vector3(scaleX * rect.width * 0.5f, (scaleY * rect.height) * 0.5f, 1f);
                }
            //AR오브젝트의 변화가 자연스럽게 변화시키기 위해, 구면선형보간을 이용하여 오브젝트의 값을 변화시킴
            obj.transform.localRotation = Quaternion.Slerp(obj.transform.localRotation, Quaternion.Euler(updateRotation), 0.2f);
            }
            else
            {
                if (front_back_mode)
                {
                    updatePosition = new Vector3(-1 * (rect.position.x + rect.width * 0.5f), -1 * (rect.position.y), obj.transform.position.z);
                    updateScale = new Vector3(scaleX * rect.width * 6f, -1f * (scaleY * rect.height * 6f), 1f);
                }
                else
                {
                    updatePosition = new Vector3((rect.position.x + rect.width * 0.5f), (rect.position.y), obj.transform.position.z);
                    updateScale = new Vector3(scaleX * rect.width * 6f, (scaleY * rect.height * 6f), 1f);
                }

            }


            //AR오브젝트의 변화가 자연스럽게 변화시키기 위해, 구면선형보간을 이용하여 오브젝트의 값을 변화시킴
            //ex) Slerp(최초위치, 도착위치, 속도) , by 황승민, 21/03/24

            obj.transform.position = Vector3.Slerp(obj.transform.position, updatePosition, 0.2f);
            obj.transform.localScale = Vector3.Slerp(obj.transform.localScale, updateScale, 0.2f);


        
        //draw.Apply();
    }


    // 그리드 설정
    public void Switch_Grid()
    {
        if (grid.gameObject.activeSelf == false)
        {
            ui_m.gridSwitch.GetComponent<Image>().sprite = grid_On;
            gridSwitch_txt.color = new Color(1f, 1f, 1f, 1f);
            grid_onoff = true;
            grid.gameObject.SetActive(true);
            ui_m.settingPanel.SetActive(false);
        }
        else
        {
            ui_m.gridSwitch.GetComponent<Image>().sprite = grid_Off;
            gridSwitch_txt.color = new Color(0.421f, 0.421f, 0.421f, 1f);
            grid_onoff = false;
            grid.gameObject.SetActive(false);
            ui_m.settingPanel.SetActive(false);
        }
    }

    // 타이머 설정
    public void Set_Timer()
    {
        // 비디오 촬영은 타이머 x
        if (ui_m.is_record)
        {
            timer_mode = 0;
            ui_m.time_txt.text = "";
            timer.sprite = noTimer;
            ui_m.time_txt.gameObject.SetActive(false);
            return;
        }
        else
        {
            // 타이머 모드별 설정
            switch (timer_mode)
            {
                case 0:
                    timeSwitch_txt.color = new Color(1f, 1f, 1f,1f);
                    timer.sprite = threeTimer;
                    timer_mode = 3;
                    ui_m.time_txt.text = "" + 3;
                    break;
                case 3:
                    
                    timer.sprite = sevenTimer;
                    timer_mode = 7;
                    ui_m.time_txt.text = "" + 7;
                    break;
                case 7:
                    timeSwitch_txt.color = new Color(0.421f, 0.421f, 0.421f, 1f);
                    timer.sprite = noTimer;
                    timer_mode = 0;
                    ui_m.time_txt.text = "";
                    break;

            }

            
            ui_m.time_txt.gameObject.SetActive(false);
        }
    }

}
