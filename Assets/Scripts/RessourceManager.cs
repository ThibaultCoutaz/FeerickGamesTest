using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using System.Threading;

public class RessourceManager : MonoBehaviour {

    public Text info;

    [System.Serializable]
    public struct Objects
    {
        public MeshRenderer tex;
        public string URL;
        [HideInInspector]
        public bool needSave;
        [HideInInspector]
        public string pathSave;
        [HideInInspector]
        public byte[] saveData;
        [HideInInspector]
        public bool DataReady;
    }

    public Objects[] listObjects;
    private WWW www;
    private Thread listThread;
    private Texture2D tmpTex;
    bool ThreadFree = true;
    private List<int> indexObjectsWaiting;

    // Use this for initialization
    IEnumerator Start () {

        tmpTex = new Texture2D(8, 8);

        indexObjectsWaiting = new List<int>();

        for (int i=0; i< listObjects.Length; i++)
        {
            listObjects[i].DataReady = false;
            listObjects[i].pathSave = Application.persistentDataPath + i + ".jpg"; //Still have to find a better way because if you change the order in the inspector or just change the place of one element he will take the texture of the old one
            if (File.Exists(listObjects[i].pathSave))
            {
                //info.text = "Dont Need to Download";
                listObjects[i].needSave = false;

                if (ThreadFree)
                {
                    //Debug.LogError("Start Thread Load pour " + i);
                    int tmpI = i; //Because before I create this Tmp value the i++ was acting before the value was send to the thread so it was everytime i+1 that the thread receive
                    listThread = new Thread(() => LoadTexture(tmpI));
                    listThread.Start();
                    ThreadFree = false;
                }
                else
                {
                    indexObjectsWaiting.Add(i);
                    //Debug.LogError("Texture " + i + " is waiting for Thread (Size = " + indexObjectsWaiting.Count + ")");
                }
            }
            else
            {
                //info.text = "Downloading";
                //Debug.LogError("Start Thread Save pour " + i);
                www = new WWW(listObjects[i].URL);
                yield return www;

                listObjects[i].saveData = www.texture.EncodeToJPG();

                listObjects[i].needSave = true;

                if (ThreadFree)
                {
                    int tmpI = i;
                    listThread = new Thread(() => SaveTexture(tmpI));
                    listThread.Start();
                    ThreadFree = false;
                }
                else
                {
                    indexObjectsWaiting.Add(i);
                    //Debug.LogError("Texture " + i + " is waiting for Thread (Size = " + indexObjectsWaiting.Count + ")");
                }
            }

        }	
    }
	
	// Update is called once per frame
	void Update () {
        
        for(int i=0; i< listObjects.Length; i++)
        {
            if (listObjects[i].DataReady)
            {
                tmpTex.LoadImage(listObjects[i].saveData);
                Debug.LogError(listObjects[i].tex.name);
                listObjects[i].tex.material.mainTexture = tmpTex;
                listObjects[i].DataReady = false;
                tmpTex = new Texture2D(8, 8);
                //info.text = "Object"+i;
            }
        }
    }

    private void SaveTexture(int _i)
    {
        //Debug.LogError("Save Texture " + _i);
        listObjects[_i].needSave = false;

        //byte[] bytes = listTex[_i].EncodeToJPG();
        File.WriteAllBytes(listObjects[_i].pathSave, listObjects[_i].saveData);
        LoadTexture(_i);
    }

    private void LoadTexture(int _i)
    {
        //Debug.LogError("Load "+_i);
        listObjects[_i].saveData = File.ReadAllBytes(listObjects[_i].pathSave);
        listObjects[_i].DataReady = true;

        if (indexObjectsWaiting.Count > 0)
        {
            //Debug.LogError("encore un");
            int tmpI = indexObjectsWaiting[0];
            indexObjectsWaiting.RemoveAt(0);
            if (listObjects[tmpI].needSave)
            {
                SaveTexture(tmpI);
            }
            else
            {
                LoadTexture(tmpI);
            }
        }
        else
        {
            ThreadFree = true;
        }
    }
}
