using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Xml;

//버튼 클릭시 오브젝트에 맞는 광고url로 가게 하는 스크립트, by 황승민, 2021/03/06

public class AD_TEST : MonoBehaviour
{
    public GameManager gm_data;

    //광고 주소들
    static int n_size = 20;
    string[] url = new string[n_size];
    /*
    string n00 = "https://fairytalehero.modoo.at/"; //메인홈페이지
    string n01 = "https://fairytalehero.modoo.at/?link=5cyuvc3o"; //개미와배짱이
    string n02 = "https://fairytalehero.modoo.at/?link=1cbu54f6"; //엄지공주
    string n03 = "https://fairytalehero.modoo.at/?link=9l5qd9b7"; //어린왕자
    string n04 = "https://fairytalehero.modoo.at/?link=bslg5v5z"; //해와 달이 된 오누이
    string n05 = "";
    string n06 = "https://fairytalehero.modoo.at/?link=ao9v9xyy"; //잭과 콩나무
    string n07 = "https://fairytalehero.modoo.at/?link=chbkzrj1"; //똥일기
    string n08 = "https://fairytalehero.modoo.at/?link=d8xibt6c"; //동생이태어났어요
    string n09 = "https://fairytalehero.modoo.at/?link=90j6s6py"; //피노키오
    string n10 = "https://fairytalehero.modoo.at/?link=9ko5x2nm"; //흥부와놀부
    string n11 = "https://fairytalehero.modoo.at/?link=2qgjnq1c"; //헨젤과 그레텔
    string n12 = "https://fairytalehero.modoo.at/?link=4kenfjkk"; //금도끼은도끼
    string n13 = "https://fairytalehero.modoo.at/?link=9xvxokfo"; //정말정말 좋아해
    string n14 = "https://fairytalehero.modoo.at/?link=b5shrp85"; //만약에만약에
    string n15 = "https://fairytalehero.modoo.at/?link=2bltn7kc"; //장화 신은 고양이
    string n16 = "https://fairytalehero.modoo.at/?link=dmchcrz1"; //대단해대단해
    string n17 = "https://fairytalehero.modoo.at/?link=bh960mc8"; //수박씨는 안 먹을테야
    string n18 = "https://fairytalehero.modoo.at/?link=dtkl4px5"; //아낌없이 주는 나무
    string n19 = "https://fairytalehero.modoo.at/?link=ewiqspxr"; //어쩌면 어쩌면
    */

    //광고에 맞는 스프라이트들
    public Sprite s01;
    public Sprite s02;
    public Sprite s03;
    public Sprite s04;
    public Sprite s05;
    public Sprite s06;
    public Sprite s07;
    public Sprite s08;
    public Sprite s09;
    public Sprite s10;
    public Sprite s11;
    public Sprite s12;
    public Sprite s13;
    public Sprite s14;
    public Sprite s15;
    public Sprite s16;
    public Sprite s17;
    public Sprite s18;
    public Sprite s19;
    public Sprite s20;
    public Sprite s21;
    public Sprite s22;
    public Sprite s23;
    public Sprite s24;
    public Sprite s25;
    public Sprite s26;
    public Sprite s27;
    public Sprite s28;
    public Sprite s29;
    public Sprite s30;
    public Sprite s31;
    public Sprite s32;
    public Sprite s33;
    public Sprite s34;
    public Sprite s35;
    public Sprite s36;
    public Sprite s37;
    public Sprite s38;
    public Sprite s39;
    public Sprite s40;
    public Sprite s41;
    public Sprite s42;
    public Sprite s43;
    public Sprite s44;
    public Sprite s45;
    public Sprite s46;
    public Sprite s47;
    public Sprite s48;
    public Sprite s49;

    //AR앱이 있을시 추가로 보여줄 오브젝트
    public GameObject ar_ad;

    //AR앱이 있는지 판단하는 배열
    static bool[] tf_array = new bool[50];


    Text my_text; //동화 제목을 담을 텍스트

    string my_url = ""; //동화 url을 담을 문자열

    UnityEngine.UI.Image my_icon; // 동화 아이콘을 담을 이미지

    //오브젝트를 교체 했는지 알아보기 위한 변수들
    public GameObject me;
    GameObject body;
    string before_name = "";
    string after_name = "";

    bool is_change = false;
    bool ad_on = false;
    int count = 0;

    int check_num = 0;

    Vector3 scale_tmp;

    void Start()
    {
        my_icon = GetComponent<UnityEngine.UI.Image>();
        my_text = GetComponentInChildren<Text>();

        scale_tmp = me.transform.localScale;
        me.transform.localScale = Vector3.zero;

        tf_array[1] = true;
        tf_array[2] = true;
        tf_array[3] = true;
        tf_array[4] = true;
        tf_array[5] = true;
        tf_array[6] = true;
        tf_array[9] = true;
        tf_array[12] = true;
        tf_array[45] = true;
        tf_array[39] = true;
        StartCoroutine(GetRequest());

    }


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
        gameObject.SetActive(false);
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
        nodes = xmlDoc.SelectNodes("camera");
        for (int i = 0; i < n_size; i++)
        {
            string n;
            if (i < 10)
                n = "n0" + i.ToString();
            else
                n = "n" + i.ToString();

            //Debug.Log(i + " : " + nodes[0].SelectSingleNode(n + "/link").InnerText);

            url[i] = nodes[0].SelectSingleNode(n + "/link").InnerText;
        }
        return 1;
    }

    // Update is called once per frame
    void Update()
    {


        body = gm_data.curARobj;
        if (body != null)
        {
            after_name = body.name;

            //오브젝트 변경 후 초기화
            if (after_name != before_name)
            {
                me.transform.localScale = Vector3.zero;
                ad_on = false;
                is_change = true;
                before_name = after_name;
            }

            //오브젝트가 변경 된 후 시간 측정
            if (is_change)
            {
                count++;
            }

            //경과후 버튼 활성화
            if (count >= 30)
            {
                change();
                me.transform.localScale = scale_tmp;
                count = 0;
                is_change = false;
                ad_on = true;
            }
        }
        if (my_icon.sprite == null)
        {
            my_icon.transform.localScale = Vector3.zero;
        }
    }

    //현재 오브젝트에 따라서 url변경

    void change()
    {

        char c0 = body.name[2];
        char c1 = body.name[3];


        check_num = c0 - '0';
        check_num *= 10;
        check_num += c1 - '0';
        //Debug.Log(check_num);
        
        if (check_num <=216 &&tf_array[check_num])
            ar_ad.SetActive(true);

        if (c0 == '0')
            switch (c1)
            {
                case '1': my_url = url[1]; my_icon.sprite = s01; my_text.text = "개미와배짱이"; break;
                case '2': my_url = url[2]; my_icon.sprite = s02; my_text.text = "엄지공주"; break;
                case '3': my_url = url[3]; my_icon.sprite = s03; my_text.text = "어린왕자"; break;
                case '4': my_url = url[4]; my_icon.sprite = s04; my_text.text = "해와달이된.."; break;
                case '5': my_url = "https://fairytalehero.modoo.at/?link=3j0hkq8v"; my_icon.sprite = s05; my_text.text = "탈것"; break;
                case '6': my_url = url[6]; my_icon.sprite = s06; my_text.text = "잭과 콩나무"; break;
                case '7': my_url = url[7]; my_icon.sprite = s07; my_text.text = "똥일기"; break;
                case '8': my_url = url[8]; my_icon.sprite = s08; my_text.text = "동생이태어났.."; break;
                case '9': my_url = url[9]; my_icon.sprite = s09; my_text.text = "피노키오"; break;
                default: my_url = url[0]; break;
            }
        else if (c0 == '1')
            switch (c1)
            {
                case '0': my_url = url[10]; my_icon.sprite = s10; my_text.text = "흥부와놀부"; break;
                case '1': my_url = url[11]; my_icon.sprite = s11; my_text.text = "핸젤과 그레텔"; break;
                case '2': my_url = url[12]; my_icon.sprite = s12; my_text.text = "금도끼은도끼"; break;
                case '3': my_url = url[13]; my_icon.sprite = s13; my_text.text = "정말정말좋아.."; break;
                case '4': my_url = url[14]; my_icon.sprite = s14; my_text.text = "만약에만약에"; break;
                case '5': my_url = url[15]; my_icon.sprite = s15; my_text.text = "장화신은고양.."; break;
                case '6': my_url = url[16]; my_icon.sprite = s16; my_text.text = "대단해대단해"; break;
                case '7': my_url = url[17]; my_icon.sprite = s17; my_text.text = "수박씨는안먹.."; break;
                case '8': my_url = url[18]; my_icon.sprite = s18; my_text.text = "아낌없이주는.."; break;
                case '9': my_url = url[19]; my_icon.sprite = s19; my_text.text = "어쩌면어쩌면"; break;
                default: my_url = url[0]; break;
            }
        else if (c0 == '2')
            switch (c1)
            {
                case '0': my_url = "https://fairytalehero.modoo.at/?link=2nk0zomg"; my_icon.sprite = s20; my_text.text = "치아교육"; break;
                case '1': my_url = "https://fairytalehero.modoo.at/?link=mphgd875"; my_icon.sprite = s21; my_text.text = "다문화체험"; break;
                case '2': my_url = "https://fairytalehero.modoo.at/?link=1sjqytjv"; my_icon.sprite = s22; my_text.text = "타다자동차"; break;
                case '3': my_url = "https://fairytalehero.modoo.at/?link=dj08eztv"; my_icon.sprite = s23; my_text.text = "전통놀이탐험"; break;
                case '4': my_url = "https://fairytalehero.modoo.at/?link=cw1ryefk"; my_icon.sprite = s24; my_text.text = "직업체험"; break;
                case '5': my_url = "https://fairytalehero.modoo.at/?link=m748xjcl"; my_icon.sprite = s25; my_text.text = "타다비행기"; break;
                case '6': my_url = "https://fairytalehero.modoo.at/?link=3nei98u9"; my_icon.sprite = s26; my_text.text = "실내안전교육"; break;
                case '7': my_url = "https://fairytalehero.modoo.at/?link=66sk0d8x"; my_icon.sprite = s27; my_text.text = "아기키우기"; break;
                case '8': my_url = "https://fairytalehero.modoo.at/?link=cilp5w8e"; my_icon.sprite = s28; my_text.text = "무지개"; break;
                case '9': my_url = "https://fairytalehero.modoo.at/?link=2xuz5ey0"; my_icon.sprite = s29; my_text.text = "공사중"; break;
                default: my_url = url[0]; break;
            }
        else if (c0 == '3')
            switch (c1)
            {
                case '0': my_url = "https://fairytalehero.modoo.at/?link=dapxk1uc"; my_icon.sprite = s30; my_text.text = "숫자놀이"; break;
                case '1': my_url = "https://fairytalehero.modoo.at/?link=dcx8g1jd"; my_icon.sprite = s31; my_text.text = "백설공주"; break;
                case '2': my_url = "https://fairytalehero.modoo.at/?link=1gb7060w"; my_icon.sprite = s32; my_text.text = "강아지랑나랑"; break;
                case '3': my_url = "https://fairytalehero.modoo.at/?link=8q5z21er"; my_icon.sprite = s33; my_text.text = "색칠하기"; break;
                case '4': my_url = "https://fairytalehero.modoo.at/?link=ezfe5765"; my_icon.sprite = s34; my_text.text = "동물놀이"; break;
                case '5': my_url = "https://fairytalehero.modoo.at/?link=79n48hsg"; my_icon.sprite = s35; my_text.text = "인어공주"; break;
                case '6': my_url = "https://fairytalehero.modoo.at/?link=2akska7n"; my_icon.sprite = s36; my_text.text = "아기돼지삼.."; break;
                case '7': my_url = "https://fairytalehero.modoo.at/?link=a48klksk"; my_icon.sprite = s37; my_text.text = "빨간머리홍.."; break;
                case '8': my_url = "https://fairytalehero.modoo.at/?link=c9cs7286"; my_icon.sprite = s38; my_text.text = "사이좋게반.."; break;
                case '9': my_url = "https://fairytalehero.modoo.at/?link=7huzq02w"; my_icon.sprite = s39; my_text.text = "빨강모자"; break;
                default: my_url = url[0]; break;
            }
        else if (c0 == '4')
            switch (c1)
            {
                case '0': my_url = "https://fairytalehero.modoo.at/?link=d4tiwg90"; my_icon.sprite = s40; my_text.text = "참잘했어요"; break;
                case '1': my_url = "https://fairytalehero.modoo.at/?link=1oljlwg2"; my_icon.sprite = s41; my_text.text = "우리몸은내.."; break;
                case '2': my_url = "https://fairytalehero.modoo.at/?link=exdj4xp3"; my_icon.sprite = s42; my_text.text = "내가이다음.."; break;
                case '3': my_url = "https://fairytalehero.modoo.at/?link=7z1zks6h"; my_icon.sprite = s43; my_text.text = "병원은무섭.."; break;
                case '4': my_url = "https://fairytalehero.modoo.at/?link=hmr3fxpi"; my_icon.sprite = s44; my_text.text = "늑대와일곱.."; break;
                case '5': my_url = "https://fairytalehero.modoo.at/?link=f0a1pmwn"; my_icon.sprite = s45; my_text.text = "미운아기오리"; break;
                case '6': my_url = "https://fairytalehero.modoo.at/?link=6dl4p7q2"; my_icon.sprite = s46; my_text.text = "참아름다운말"; break;
                case '7': my_url = "https://fairytalehero.modoo.at/?link=2fewk4rd"; my_icon.sprite = s47; my_text.text = "즐거운유치원"; break;
                case '8': my_url = "https://fairytalehero.modoo.at/?link=kzdotn3e"; my_icon.sprite = s48; my_text.text = "성냥팔이소녀"; break;
                case '9': my_url = "https://fairytalehero.modoo.at/?link=4848ovd1"; my_icon.sprite = s49; my_text.text = "과일주수만.."; break;
                default: my_url = url[0]; break;
            }
        else
        {
            my_url = url[0];
            my_icon.sprite = null;
        }
    }

    public void click_ad()
    {
        Application.OpenURL(my_url);
    }
}
