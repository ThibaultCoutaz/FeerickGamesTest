using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ScriptTest : MonoBehaviour {

    [System.Serializable]
    public struct Objects
    {
        public MeshRenderer tex;
        public string URL;
    }

    public Objects[] listObjects;

    IEnumerator Start()
    {

        //info.text = "Start";
        for (int i = 0; i < listObjects.Length; i++)
        {
            if (File.Exists(Application.persistentDataPath + i + "Texture.jpg"))
            {
                byte[] byteArray = File.ReadAllBytes(Application.persistentDataPath + i + "Texture.jpg");
                Debug.LogError("Save :: Array Bytes = " + Convert.ToBase64String(byteArray));
                Texture2D texture = new Texture2D(8, 8);
                texture.LoadImage(byteArray);
                listObjects[i].tex.material.mainTexture = texture;
            }
            else
            {
                WWW www = new WWW(listObjects[i].URL);
                yield return www;
                Texture2D texture = www.texture;
                listObjects[i].tex.material.mainTexture = texture;

                byte[] bytes = texture.EncodeToJPG();
                File.WriteAllBytes(Application.persistentDataPath + i + "Texture.jpg", bytes);
            }
        }
    }
}
