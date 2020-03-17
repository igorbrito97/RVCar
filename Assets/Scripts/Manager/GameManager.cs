using MySql.Data.MySqlClient;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public MySqlConnection con;
    public Psychologist psychologist;
    
    void Start () {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            //para teste conecta aqui, porem na versão final conecta ao logar e la faz isso e da set na conexão            
            GameObject database = new GameObject();    
            DatabaseController db = database.AddComponent<DatabaseController>();
            db.Connect();
            con = db.GetConnection();
            con.Open();
        }
        else
        {
            // DestroyImmediate(gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
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
