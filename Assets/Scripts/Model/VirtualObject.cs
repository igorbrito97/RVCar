using System;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualObject : MonoBehaviour
{
    private int id;
    private string name;
    private string file;

    public VirtualObject()
    {
    }

    public VirtualObject(int id, string name, string file)
    {
        this.id = id;
        this.name = name;
        this.file = file;
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
                data["objComp_name"].ToString(),
                data["objComp_file"].ToString()
            ));
        }
        data.Close();

        return list;
    }

     public VirtualObject SearchComponent(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        VirtualObject obj = null;
        string sql = "select * from objComponent where objComp_id = " + id;

        command.CommandText = sql;
        data = command.ExecuteReader();

        if (data.Read())
        {
            obj = new VirtualObject(
                Convert.ToInt32(data["objComp_id"]),
                data["objComp_name"].ToString(),
                data["objComp_file"].ToString()
            );
        }
                
        data.Close();
        return obj;
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
                data["objSce_name"].ToString(),
                data["objSce_file"].ToString()
            ));
        }
        data.Close();

        return list;
    }

    public VirtualObject SearchScenario(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        VirtualObject obj = null;
        string sql = "select * from objScenario where objSce_id = " + id;

        command.CommandText = sql;
        data = command.ExecuteReader();

        if (data.Read())
        {
            obj = new VirtualObject(
                Convert.ToInt32(data["objSce_id"]),
                data["objSce_name"].ToString(),
                data["objSce_file"].ToString()
            );
        }
                
        data.Close();
        return obj;
    }
    
    public List<VirtualObject> SearchAllCar()
    {
        
        List<VirtualObject> list = new List<VirtualObject>();
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        String sql = "select * from objCar ";

        command.CommandText = sql;
        data = command.ExecuteReader();
        while (data.Read())
        {
            list.Add(new VirtualObject(
                Convert.ToInt32(data["objCar_id"]),
                data["objCar_name"].ToString(),
                data["objCar_file"].ToString()
            ));
        }
        data.Close();
        return list;
    }

    public VirtualObject SearchCar(int id)
    {
        VirtualObject obj = null;
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        string sql = "select * from objCar where objCar_id = " + id;

        command.CommandText = sql;
        data = command.ExecuteReader();

        if (data.Read())
        {
            obj = new VirtualObject(
                Convert.ToInt32(data["objCar_id"]),
                data["objCar_name"].ToString(),
                data["objCar_file"].ToString()
            );
        }
                
        data.Close();
        return obj;
    }


    
    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public string File { get => file; set => name = value; }
}
