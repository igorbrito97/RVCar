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
        }
        else
        {
            // DestroyImmediate(gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public MySqlConnection Con
    {
        get
        {
            return con;
        }

        set
        {
            con = value;
        }
    }

    public Psychologist Psychologist
    {
        get
        {
            return psychologist;
        }

        set
        {
            psychologist = value;
        }
    }
}
