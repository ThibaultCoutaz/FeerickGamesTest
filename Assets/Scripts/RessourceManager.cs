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

    public Objects listObjects;
    private WWW www;
    private Thread listThread;
    private Texture2D texture;

    // Use this for initialization
    IEnumerator Start () {

        texture = new Texture2D(8, 8);

        //www = new WWW[listObjects.Length];


        //for (int i=0; i< listObjects.Length; i++)
        //{
            listObjects.DataReady = false;
            listObjects.pathSave = Application.persistentDataPath + /*i +*/ "Texture.jpg"; //Still have to find a better way because if you change the order in the inspector or just change the place of one element he will take the texture of the old one
            if (File.Exists(listObjects.pathSave))
            {
                info.text = "Dont Need to Download";
                listThread = new Thread(() => LoadTexture()); //Pour les paramétres plus tard
                listThread.Start();
            }
            else
            {
                info.text = "Downloading";
                www = new WWW(listObjects.URL);
                yield return www;
                texture = www.texture;

                listThread = new Thread(() => SaveTexture()); //Pour les paramétres plus tard
                listThread.Start();
            }

        //}	
    }
	
	// Update is called once per frame
	void Update () {
        if (listObjects.DataReady)
        {
            texture.LoadImage(listObjects.saveData);
            listObjects.tex.material.mainTexture = texture;
        }
    }

    private void SaveTexture()
    {
        info.text = "Texture sauvegardé";
        byte[] bytes = texture.EncodeToJPG();
        File.WriteAllBytes(listObjects.pathSave, bytes);
        LoadTexture();
    }

    private void LoadTexture()
    {
        //info.text = "Loading Texture";
        listObjects.saveData = File.ReadAllBytes(listObjects.pathSave);
        listObjects.DataReady = true;
    }
}
