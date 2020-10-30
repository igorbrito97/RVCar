using MySql.Data.MySqlClient;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public static MySqlConnection con;
    public static Psychologist psychologist;
    
    void Start () {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            //para teste conecta aqui, porem na versão final: não tem GameManager no Menu, só no Login - conecta ao logar e la faz isso e da set na conexão       
            /*GameObject database = new GameObject();    
            DatabaseController db = database.AddComponent<DatabaseController>();
            db.Connect();
            con = db.GetConnection();
            con.Open();
            psychologist = new Psychologist().Search(1);
            */

            //deixa a rv desligada, liga somente quando começar a sessão, e quando voltar tem que desligar de novo
            DisableVR();
            /*Debug.Log("LAOADED: "+ XRSettings.loadedDeviceName);
            foreach(string i in XRSettings.supportedDevices)
                Debug.Log("! = "+ i);
                */
        }
        else
        {
            // DestroyImmediate(gameObject);
        }
    }

    public void EnableVR()
    {
        Debug.Log("ENABLED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        StartCoroutine(ChangeVRConfig("Oculus",true));
    }

    public void DisableVR()
    {
        StartCoroutine(ChangeVRConfig("",false));
    }

    IEnumerator ChangeVRConfig(string deviceName, bool flag)
    {
        XRSettings.LoadDeviceByName(deviceName);
        yield return null;
        XRSettings.enabled = flag;
        Debug.Log("CHANGINGGGGGGGGGGGGGGGGGGGGGGGG: " + deviceName + " " + flag);
        Debug.Log("LAOADED2: "+ XRSettings.loadedDeviceName);
        Debug.Log("!!!!!!!!!!!!: "+ XRSettings.supportedDevices[1]);
        
    }

    public int GetMaxPK(string table, string key)
    {
        MySqlCommand command = con.CreateCommand();
        MySqlDataReader data;
        string sql = "select max(" + key + ") from " + table;
        int maxPK = -1;

        command.CommandText = sql;
        data = command.ExecuteReader();

        if (data.Read())
        {
            maxPK = Convert.ToInt32(data[0]);
        }

        data.Close();

        return maxPK;
    }

    public MySqlConnection Con { get => con; set => con = value; }
    public Psychologist Psychologist { get => psychologist; set => psychologist = value; }

}
