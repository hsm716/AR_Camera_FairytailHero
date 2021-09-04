using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;
using System.IO;
using UnityEngine.Networking;
using UnityEditor;

// Tensorflow Lite with Unity 연동, by 황승민, 2021_0312
public class EmotionTest : MonoBehaviour
{
    [Header("모델 파일")]
    // tflite파일들을 Group화 해서 에디터에서 선택 가능하게 해줍니다. (default로는 model.tflite)선택 가능케 함, by 황승민, 2021_0312
    [SerializeField, FilePopup("*.tflite")] string fileName = "model.tflite";


    [Header("감정분석 관련 UIs")]
    public GameObject showPicturePannel;
    public GameObject result_pannel;
    // output내용들을 출력시킬 View, by 황승민, 2021_0312
    [SerializeField] Text outputTextView = null;


    // RawImage에 사진을 등록해놓고 버튼을 누르면 분석 및 결과값 출력하게함, by 황승민, 2021_0312
    //[SerializeField] Button checkButton = null;

    [Header("선택된 이미지를 보여주는 이미지 오브젝트")]
    // RawImage에 Input할 사진을 넣어줌, by 황승민, 2021_0312
    [SerializeField] private RawImage imageTexture;


    // output뷰에 삽입할 string, by 황승민, 2021_0312
    string str = "";

    float max = -1000f;
    int max_idx=0;
    private EmotionBase eBase;
    
    [Header("UI 매니저")]
    public UI_Manager ui_m;

    [Header("UI 매니저")]
    public Image emotion_img;
    public Image emotion_back_img;
    public GameObject emotion_txt;
    public GameObject happyPoint_txt;
    public Sprite angry;
    public Sprite happy;

    static Texture2D result;
    void Start()
    {
        result_pannel.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width*0.85f, Screen.height*0.33f);

        emotion_img.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width * 0.3f, Screen.height * 0.15f);
        emotion_img.GetComponent<RectTransform>().anchoredPosition = new Vector2(-Screen.width * 0.12f, -Screen.height * 0.05f);

        emotion_back_img.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width * 0.30f, Screen.height * 0.20f);
        emotion_back_img.GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width * 0.22f, -Screen.height * 0.02f);
        emotion_txt.GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width * 0.22f, 0f);
        happyPoint_txt.GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width * 0.22f, -Screen.height*0.13f);
        // tflite 파일의 경로를 지정함 , by 황승민, 2021_0312
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        // ShowBase 클래스 변수를 생성 및 모델파일 경로를 매개변수로한 생성자 호출, by 황승민, 2021_0312
        eBase = new EmotionBase(path,true);

        // 버튼에 OnButtonClick 이벤트 부착, by 황승민, 2021_0312
        //checkButton.onClick.AddListener(OnButtonClick);
    }


    // 이미지 GrayScale하는 작업.
    Texture2D ConvertToGrayscale(Texture2D texture)
    {
        Color32[] pixels = texture.GetPixels32();
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Color32 pixel = pixels[x + y * texture.width];
                int p = ((256 * 256 + pixel.r) * 256 + pixel.b) * 256 + pixel.g;
                int b = p % 256;
                p = Mathf.FloorToInt(p / 256);
                int g = p % 256;
                p = Mathf.FloorToInt(p / 256);
                int r = p % 256;
                float l = (0.2126f * r / 255f) + 0.7152f * (g / 255f) + 0.0722f * (b / 255f);
                Color c = new Color(l, l, l, 1);
                texture.SetPixel(x, y, c);
            }
        }

        texture.Apply(false, true);
        return texture;
    }
    // 이미지를 수정가능하게 복사해주는 작업.
    Texture2D duplicateTexture(Texture2D source,int targetWidth,int targetHeight)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText = ScaleTexture(readableText, 48, 48);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);

        return readableText;
    }

    // 이미지 크기값을 변경해주는 작업.
    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();

        return result;

    }


    // Button 클릭 시 이벤트 내용, by 황승민, 2021_0312
    public void OnButtonClick()
    {
        result_pannel.SetActive(true);
        string usedSticker = ui_m.curPickImg;
        outputTextView.text = "";
        str = "";
        eBase.max_idx = 0;
        eBase.max = -1000f;


        Texture2D tmp_texture = duplicateTexture(imageTexture.texture as Texture2D,48,48);


        // sBase클래스의 Invoke 함수를 매개변수로 imageTexture.texture값을 Input으로 넣어 호출, by 황승민, 2021_0312
        tmp_texture = ConvertToGrayscale(tmp_texture);
        //imageTexture.texture = tmp_texture;

        eBase.Invoke(tmp_texture);

        // sBase클래스의 GetResults를 통해 output0값을 업데이트 시킵니다, by 황승민, 2021_0312
        eBase.GetResults();

        switch (eBase.result_str)
        {
            case "화남":
                emotion_img.sprite = angry;
                str = "화남";
                break;
            case "행복":
                emotion_img.sprite = happy;
                str = "행복";
                break;
            case "슬픔":
                emotion_img.sprite = null;
                str = "슬픔";
                break;
            case "놀람":
                emotion_img.sprite = null;
                str = "놀람";
                break;
        }

        outputTextView.text = str;
        int tmp_value__ = (int)(eBase.Degree_b * 100);
        int tmp_value = tmp_value__;
        happyPoint_txt.GetComponent<Text>().text = "행복도 : " + tmp_value + "점";
        StartCoroutine(PatchWebRequest(tmp_value,usedSticker));
    }
    private void OnDestroy()
    {
        eBase?.Dispose();
    }
    // 뒤로가기 버튼
    public void backback()
    {
        Destroy(ui_m.texture);
        str = "";
        eBase.max_idx = 0;
        eBase.max = -1000f;
        result_pannel.SetActive(false);
        showPicturePannel.SetActive(false);
    }

    // 행복도 값을 ccms서버에 넘겨주는 작업.
    public IEnumerator PatchWebRequest(int score,string sticker_name)
    {
        string tmp="";
        for(int i = 5; i < 8; i++)
        {
            tmp += sticker_name[i];
        }
        UnityWebRequest request = new UnityWebRequest();
        string requestBodyString = "baaaaaaaaaaaaad";
        // Convert Json body string into a byte array
        byte[] requestBodyData = System.Text.Encoding.UTF8.GetBytes(requestBodyString);
        // Specify that our method is of type 'patch'
        
        // body가 될 부분을 form으로 생성해서 관리해줌.
        WWWForm form = new WWWForm();
        form.AddField("score", score);
   
        //request = UnityWebRequest.Post("https://www.ccms.kr/api/v1/arobject/fun_050/target/score", form);

        //using (request = UnityWebRequest.Put("https://www.ccms.kr/api/v1/arobject/fun_050/target/score", requestBodyData))

        // body를 주소와 함께 보내려면 UnityWebRequest Post 방식으로 보내야함.
        using (request = UnityWebRequest.Post("https://www.ccms.kr/api/v1/arobject/arobj-"+tmp+"/target/score", form))
        {
            // ccms에서 지정한 요청 방식은 PATCH이므로 행복도 값을 Request하는 방식을 PATCH라고 명시 해줘야 함.
            request.method = "PATCH";
            
            yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }

        }

    }
}
