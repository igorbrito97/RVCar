﻿using System;
using UnityEngine;
using MySql.Data.MySqlClient;

public class DatabaseController : MonoBehaviour {

    public static DatabaseController instance;
    private MySqlConnection connection;
    private string sql;

    public void Connect()
    {
        try
        {
            if (instance == null)
            {
                instance = this;
                sql = @"Server=localhost;
                        Database=rvcardb;
                        Port=3306;
                        User ID=iguinho;
                        Password=rvcar2020;
                        CharSet=utf8";

                connection = new MySqlConnection(sql);
            }
            else
            {
                //Destroy(gameObject); // Herdado do MonoBehaviour
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public MySqlConnection GetConnection()
    {
        return connection;
    }
}
