using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class BehaviourCube : MonoBehaviour {

    public string URL;
    public Text info;

	// Use this for initialization
	IEnumerator Start () {
        info.text = "Start";
        if (File.Exists(Application.persistentDataPath + "TestTexture.jpg"))
        {
            info.text = "Don't need Downloaded";
            byte[] byteArray = File.ReadAllBytes(Application.persistentDataPath + "TestTexture.jpg");
            Texture2D texture = new Texture2D(8,8);
            texture.LoadImage(byteArray);
            this.GetComponent<Renderer>().material.mainTexture = texture;
        }
        else
        {
            info.text = "Downloading...";
            WWW www = new WWW(URL);
            yield return www;
            Texture2D texture = www.texture;
            this.GetComponent<Renderer>().material.mainTexture = texture;

            byte[] bytes = texture.EncodeToJPG();
            File.WriteAllBytes(Application.persistentDataPath + "TestTexture.jpg", bytes);

            info.text = "Downloaded.";
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
