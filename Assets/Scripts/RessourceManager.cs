using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
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
        public string pathSave;
        [HideInInspector]
        public byte[] saveData;
        [HideInInspector]
        public bool DataReady;
    }

    public Objects[] listObjects;
    private WWW www;
    private Texture2D[] listTex;
    private Thread listThread;
    private Texture2D tmpTex;

    // Use this for initialization
    IEnumerator Start () {

        tmpTex = new Texture2D(8, 8);

        listTex = new Texture2D[listObjects.Length];

        for (int i=0; i< listObjects.Length; i++)
        {
            listObjects[i].DataReady = false;
            listObjects[i].pathSave = Application.persistentDataPath + i + ".jpg"; //Still have to find a better way because if you change the order in the inspector or just change the place of one element he will take the texture of the old one
            if (File.Exists(listObjects[i].pathSave))
            {
                info.text = "Dont Need to Download";
                int tmpI = i; //Because before I create this Tmp value the i++ was acting before the value was send to the thread so it was everytime i+1 that the thread receive
                listThread = new Thread(() => LoadTexture(tmpI)); 
                listThread.Start();
            }
            else
            {
                info.text = "Downloading";
                www = new WWW(listObjects[i].URL);
                yield return www;
                listTex[i] = www.texture;

                int tmpI = i;
                listThread = new Thread(() => SaveTexture(tmpI)); 
                listThread.Start();
            }

        }	
    }
	
	// Update is called once per frame
	void Update () {
        if (listObjects[0].DataReady)
        {
            tmpTex.LoadImage(listObjects[0].saveData);
            listObjects[0].tex.material.mainTexture = tmpTex;
            listObjects[0].DataReady = false;
        }
        if (listObjects[1].DataReady)
        {
            tmpTex.LoadImage(listObjects[1].saveData);
            listObjects[1].tex.material.mainTexture = tmpTex;
            listObjects[1].DataReady = false;
        }
    }

    private void SaveTexture(int _i)
    {
        byte[] bytes = listTex[_i].EncodeToJPG();
        File.WriteAllBytes(listObjects[_i].pathSave, bytes);
        LoadTexture(_i);
    }

    private void LoadTexture(int _i)
    {
        listObjects[_i].saveData = File.ReadAllBytes(listObjects[_i].pathSave);
        listObjects[_i].DataReady = true;
    }
}
