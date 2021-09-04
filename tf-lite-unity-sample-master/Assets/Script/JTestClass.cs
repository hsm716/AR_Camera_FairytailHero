using System.Collections.Generic;
using UnityEngine;

// Json 관련 클래스 및 필드

// ccms 서버에서 다루는 데이터에 대해서 정의함.

[System.Serializable]
public class S3Data
{
    public string fieldname;
    public string originalname;
    public string encoding;
    public string mimetype;
    public int size;
    public string bucket;
    public string key;
    public string acl;
    public string contentType;
    public string contentDisposition;
    public string storageClass;
    public string serverSideEncryption;
    public string metadata;
    public string location;
    public string etag;
}

[System.Serializable]
public class ObjectData
{
    public int averageScore;
    public int downloadCount;
    public int usedCount;
    public string _id;
    public string name;
    public string category;
    public List<S3Data> s3Info;

    public string modifiedManager;
    public string content;
    public string created;
    public string modified;
    public int objectId;
}

[System.Serializable]
public class JTestClass
{
    public int statusCode;
    public string status;
    public string message;
    public List<ObjectData> result;




    public JTestClass() { }

    public void Print()
    {
        /*        Debug.Log("statusCode = " + statusCode);
                Debug.Log("status = " + status);
                Debug.Log("message = " + message);
                for (int i = 0; i < result.Count; i++)
                {
                    Debug.Log("averageScore = " + result[i].averageScore);
                    Debug.Log("downloadCount = " + result[i].downloadCount);
                    Debug.Log("usedCount = " + result[i].usedCount);
                    Debug.Log("_id = " + result[i]._id);
                    Debug.Log("name = " + result[i].name);
                    Debug.Log("categori = " + result[i].category);
          *//*          for (int j = 0; j < result[0].s3Info.Count; j++)
                    {
                        Debug.Log("fieldname = " + result[i].s3Info[j].fieldname);
                        Debug.Log("originalname = " + result[i].s3Info[j].originalname);
                        Debug.Log("encoding = " + result[i].s3Info[j].encoding);
                        Debug.Log("mimetype = " + result[i].s3Info[j].mimetype);
                        Debug.Log("size = " + result[i].s3Info[j].size);
                        Debug.Log("bucket = " + result[i].s3Info[j].bucket);
                        Debug.Log("key = " + result[i].s3Info[j].key);
                        Debug.Log("acl = " + result[i].s3Info[j].acl);
                        Debug.Log("contentType = " + result[i].s3Info[j].contentType);
                        Debug.Log("contentDisposition = " + result[i].s3Info[j].contentDisposition);
                        Debug.Log("storageClass = " + result[i].s3Info[j].storageClass);
                        Debug.Log("serverSideEncryption = " + result[i].s3Info[j].serverSideEncryption);
                        Debug.Log("metadata = " + result[i].s3Info[j].metadata);
                        Debug.Log("location = " + result[i].s3Info[j].location);
                        Debug.Log("etag = " + result[i].s3Info[j].etag);
                    }*//*

                    Debug.Log("modifiedManager = " + result[i].modifiedManager);
                    Debug.Log("content = " + result[i].content);
                    Debug.Log("created = " + result[i].created);
                    Debug.Log("modified = " + result[i].modified);
                    Debug.Log("objectId = " + result[i].objectId);
                }



            }*/
    }
}
