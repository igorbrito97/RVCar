using System;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentType : MonoBehaviour
{
    private int id;
    private string name;
    private string description;
    private int status;

    public EnvironmentType()
    {

    }

    public EnvironmentType(int id)
    {
        this.id = id;
    }
    
    public EnvironmentType(string name, string description, int status)
    {
        this.name = name;
        this.description = description;
        this.status = status;
    }

    public EnvironmentType(int id, string name, string description, int status)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.status = status;
    }

    public string Insert()
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        int result;
        string sql = @"insert into environmentType 
                            (env_name,
                            env_description,
                            env_status)
                    values ('$n','$d',$s);";
                    
        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$d", this.description);
        sql = sql.Replace("$s", this.status + "");

        command.CommandText = sql;
        result = command.ExecuteNonQuery();
        Debug.Log("resultado query: " + result);
        if(result == 1)
        {
            return "Ok";
        }

        
        return "Erro ao inserir!";
    }

    public string Alter(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        int result;
        string sql = @"update environmentType set
                        env_name = '$n',
                        env_description = '$d',
                        env_status = $s 
                        where env_id = " + id;
        
        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$d", this.description);
        sql = sql.Replace("$s", this.status + "");

        Debug.Log("SWL ALTER: " + sql);
        command.CommandText = sql;
        result = command.ExecuteNonQuery();
        if(result == 1)
        {
            return "Ok";
        }

        return "Erro ao alterar!";
    }

    public bool Delete(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        string sql = @"update environmentType set env_status = 0 where env_id = " + id;
        int result;

        command.CommandText = sql;
        result = command.ExecuteNonQuery();

        return result == 1;
    }

    public EnvironmentType Search(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        EnvironmentType env = null;
        string sql = "select * from environmentType where env_id = " + id;

        command.CommandText = sql;
        data = command.ExecuteReader();

        if (data.Read())
        {
            env = new EnvironmentType(
                        Convert.ToInt32(data["env_id"]),
                        data["env_name"].ToString(),
                        data["env_description"].ToString(),
                        Convert.ToInt32(data["env_status"])
            );
        }
                
        data.Close();
        return env;
    }

    public List<EnvironmentType> SearchAll(string filter, bool status)
    {
        List<EnvironmentType> list = new List<EnvironmentType>();
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        String sql = "select * from environmentType ";

        if(!filter.Trim().Equals(""))
        {
            sql+= "where env_name like '%$f%'";
            sql = sql.Replace("$f",filter);
            if(!status)
                sql +=" and env_status = 1";
        }
        else if(!status)
            sql +="where env_status = 1";
         

        Debug.Log("SQL: " + sql);
        command.CommandText = sql;
        data = command.ExecuteReader();
        while (data.Read())
        {
            list.Add(new EnvironmentType(
                Convert.ToInt32(data["env_id"]),
                data["env_name"].ToString(),
                data["env_description"].ToString(),
                Convert.ToInt32(data["env_status"])));
        }
        data.Close();

        return list;
    }

    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public int Status { get => status; set => status = value; }

}
