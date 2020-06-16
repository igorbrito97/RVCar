using System;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualObject : MonoBehaviour
{
    private int id;
    private string name;

    public VirtualObject()
    {
    }

    public VirtualObject(int id, string name)
    {
        this.id = id;
        this.name = name;
    }
    public List<VirtualObject> SearchAllComponent()
    {
        List<VirtualObject> list = new List<VirtualObject>();
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        String sql = "select * from objComponent ";

        command.CommandText = sql;
        data = command.ExecuteReader();
        while (data.Read())
        {
            list.Add(new VirtualObject(
                Convert.ToInt32(data["objComp_id"]),
                data["objComp_name"].ToString()
            ));
        }
        data.Close();

        return list;
    }

    public List<VirtualObject> SearchAllScenario()
    {
        List<VirtualObject> list = new List<VirtualObject>();
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        String sql = "select * from objScenario ";

        command.CommandText = sql;
        data = command.ExecuteReader();
        while (data.Read())
        {
            list.Add(new VirtualObject(
                Convert.ToInt32(data["objSce_id"]),
                data["objSce_name"].ToString()
            ));
        }
        data.Close();

        return list;
    }


    
    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
}
