using System;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeatherType : MonoBehaviour
{
    private int id;
    private string name;

    public WeatherType()
    {

    }

    public WeatherType(int id)
    {
        this.id = id;
    }

    public WeatherType(int id, string name)
    {
        this.id = id;
        this.name = name;
    }

    public List<WeatherType> SearchAll()
    {
        List<WeatherType> list = new List<WeatherType>();
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        String sql = "select * from weatherType ";

        command.CommandText = sql;
        data = command.ExecuteReader();
        while (data.Read())
        {
            list.Add(new WeatherType(
                Convert.ToInt32(data["weatherType_id"]),
                data["weatherType_name"].ToString()
            ));
        }
        data.Close();

        return list;
    }


    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }

}
