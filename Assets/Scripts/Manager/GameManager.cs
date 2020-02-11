using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    private MySqlConnection con;
    private Psychologist psychologist;
    
    void Start () {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            //para teste conecta aqui, porem na versão final conecta ao logar e la faz isso e da set na conexão
            DatabaseController db = new DatabaseController();
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

    public MySqlConnection Con { get => con; set => con = value; }
    public Psychologist Psychologist { get => psychologist; set => psychologist = value; }

}
