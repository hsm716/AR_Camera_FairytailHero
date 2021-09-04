using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;

public class EmotionBase : BaseImagePredictor<float>
{
    // output의 크기 만큼 동적 할당, by 황승민, 2021_0312 
    public float[,] output0 = new float[1, 2];
    public float max = -1000f;
    public int max_idx = 0;
    public string result_str;
    public float Degree_a = -1000f;
    public float Degree_b = -1000f;

    // 생성자, by 황승민, 2021_0312
    public EmotionBase(string modelPath,bool useGPU = true) : base(modelPath, useGPU)
    {
    }


    public override void Invoke(Texture inputTex)
    {
        Debug.Log("tensor input Info " + interpreter.GetInputTensorInfo(0));
        Debug.Log("tensor output Info " + interpreter.GetOutputTensorInfo(0));

        // Input값을 넣어줍니다 , by 황승민, 2021_0312
        ToTensor(inputTex, input0);
        interpreter.SetInputTensorData(0, input0);

        interpreter.Invoke();

        // output값을 반환받습니다, by 황승민, 2021_0312
        interpreter.GetOutputTensorData(0, output0);
    }
    public void GetResults()
    {
        Degree_a = -1000f;
        Degree_b = -1000f;


        for (int i = 0; i < 2; i++)
        {
            float a = output0[0, i];
            if (max <= a)
            {
                max = a;
                max_idx = i;
            }
            double b = System.Math.Round(a, 6);

            if (i == 0)
            {
                Degree_a = (float)b;
            }
            else if(i==1)
            {
                Degree_b = (float)b;
            }

        }
        Debug.Log("" + "화남 :" + ". " + Degree_a + "\n");
        Debug.Log("" + "행복 : " + ". " + Degree_b + "\n");

        float max2;
        max2=Mathf.Max(Degree_a, Degree_b);

        if (max2 == Degree_a)
        {
            result_str = "화남";
            Debug.Log("감정 결과 : 화남");
        }
        else if (max2 == Degree_b)
        {
            result_str = "행복";
            Debug.Log("감정 결과 : 행복");
        }
    }

}
