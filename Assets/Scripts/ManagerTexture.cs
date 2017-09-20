using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Threading;

public class ManagerTexture : MonoBehaviour {

    /// <summary>
    /// Script qui n'est plus utilisé et avec beaucoup d'erreurs , juste utilisé pour exemple pour certaines parties du code
    /// </summary>
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
    private List<Objects> listObjectsManage;

    public Text info;
    public int nbThread;

    #region Private Value
    private Thread[] listThread;
    //private Texture2D[] useTexThread;
    private Texture2D tmpTexture;
    private WWW[] www;
    private int indexTexture = 0;
    private int i;
    private Objects tmp;
    #endregion

    IEnumerator Start()
    {
        info.text = "Init";
        listThread = new Thread[nbThread];
        //useTexThread = new Texture2D[nbThread];
        listObjectsManage = new List<Objects>();

        www = new WWW[listObjects.Length];

        for(i = 0; i < listObjects.Length; i++)
        {
            listObjects[i].pathSave = Application.persistentDataPath + indexTexture + "Texture.jpg";
            listObjects[i].DataReady = false;
            listObjectsManage.Add(listObjects[i]);
        }

        tmpTexture = new Texture2D(8, 8);

        for (i = 0; i < nbThread; i++)
        {
            info.text = "Downloading Texture "+i;
            //Seulement dans le cas ou le nb de thread = le nombre dimage a gerer .
            www[i] = new WWW(listObjects[i].URL);
            yield return www[i];
            
            listThread[i] = new Thread(() => ThreadManagePic(indexTexture,i));
            listThread[i].Start();
            indexTexture++;


        }
    }

    // Use this for initialization
    //   IEnumerator Start () {
    //       info.text = "Start";
    //       for (int i = 0; i < listObjects.Length; i++)
    //       {
    //           if (File.Exists(Application.persistentDataPath + i + "Texture.jpg"))
    //           {
    //               info.text = "Don't need Downloaded";
    //               byte[] byteArray = File.ReadAllBytes(Application.persistentDataPath + i + "Texture.jpg");
    //               Texture2D texture = new Texture2D(8, 8);
    //               texture.LoadImage(byteArray);
    //               listObjects[i].tex.material.mainTexture = texture;
    //           }
    //           else
    //           {
    //               info.text = "Downloading...";
    //               WWW www = new WWW(listObjects[i].URL);
    //               yield return www;
    //               Texture2D texture = www.texture;
    //               listObjects[i].tex.material.mainTexture = texture;

    //               byte[] bytes = texture.EncodeToJPG();
    //               File.WriteAllBytes(Application.persistentDataPath + i + "Texture.jpg", bytes);

    //               info.text = "Downloaded.";
    //           }
    //       }
    //}

    // Update is called once per frame

    void Update () {
        if(listObjectsManage.Count > 0)
        {
            for(int i=0; i < listObjectsManage.Count; i++)
            {
                Debug.LogError(listObjectsManage[i].DataReady+" -- "+ listObjects[i].DataReady);
                if (listObjectsManage[i].DataReady)
                {
                    Debug.LogError("loading texture "+i);
                    tmp = listObjectsManage[i];
                    tmpTexture.LoadImage(tmp.saveData);
                    tmp.tex.material.mainTexture = tmpTexture;
                    tmp.DataReady = false;
                }
            }
        }
    }

    private void ThreadManagePic(int index,int indexThread)
    {
        if (File.Exists(listObjects[index].pathSave))
        {
            Debug.LogError("image " + index + " exsiste");
            listObjectsManage.ToArray()[index].saveData = File.ReadAllBytes(listObjects[index].pathSave);
            //useTexThread[indexThread].LoadImage(byteArray);
            //listObjects[index].tex.material.mainTexture = useTexThread[indexThread];
            listObjectsManage.ToArray()[index].DataReady = true;
        }
        else
        {
            Debug.LogError("image " + index + " n'exsiste pas");
        }
    }
    
}
