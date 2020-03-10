using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configuration : MonoBehaviour
{

    private int id;
    private int option; // 0 - weather, 1 - scenario, 2 - component 
    private string name;
    private string description;
    private int status;

    public Configuration(int option)
    {
        this.option = option;
    }

    public Configuration(int option, string name, string description, int status)
    {
        this.option = option;
        this.name = name;
        this.description = description;
        this.status = status;
    }

    public Configuration(int id, int option, string name, string description,  int status)
    {
        this.id = id;
        this.option = option;
        this.name = name;
        this.description = description;
        this.status = status;
    }

    public string Insert()
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        int result;
        string sql;
        
        if(this.option == 0) 
        {
            sql = @"insert into weather 
                (weather_name,
                weather_description,
                weather_status)
                values ('$n','$d',$s);";
        }
        else if(this.option == 1)
        {
            sql = @"insert into scenario 
                (scenario_name,
                scenario_description,
                scenario_status)
                values ('$n','$d',$s);";
        }
        else if(this.option == 2)
        {
            sql = @"insert into component 
                (component_name,
                component_description,
                component_status)
                values ('$n','$d',$s);";
        }
        else 
            return "Erro ao inserir!";
        
        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$d", this.description);
        sql = sql.Replace("$s", this.status + "");

        Debug.Log("inserting, sql:" + sql);
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
        MySqlDataReader data;
        string sql;
        int result;

        if(this.option == 0)
        {
            sql = @"update weather set
                weather_name = '$n',
                weather_description = '$d',
                weather_status = $s
                where weather_id = " + id;
        }
        else if(this.option == 1)
        {
            sql = @"update scenario set
                scenario_name = '$n',
                scenario_description = '$d',
                scenario_status = $s
                where scenario_id = " + id;
        }
        else if(this.option == 2)
        {
            sql = @"update component set
                component_name = '$n',
                component_description = '$d',
                component_status = $s
                where component_id = " + id;
        }
        else 
            return "Erro ao alterar!";

        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$d", this.description);
        sql = sql.Replace("$s", this.status + "");

        Debug.Log("altering. " + sql);
        command.CommandText = sql;
        result = command.ExecuteNonQuery();
        Debug.Log("resultado query: " + result);
        if(result == 1)
        {
            return "Ok";
        }

        
        return "Erro ao alterar!";
    }

    public bool Delete(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        string sql;
        int result;

        if(this.option == 0)
        {
            sql = @"update weather set weather_status = 0 where weather_id = " + id;
        }
        else if(this.option == 1)
        {
            sql = @"update scenario set scenario_status = 0 where scenario_id = " + id;
        }
        else if(this.option == 2)
        {
            sql = @"update component set component_status = 0 where component_id = " + id;
        }
        else 
            return false;

        command.CommandText = sql;
        result = command.ExecuteNonQuery();
        
        return result == 1;
    }

    public Configuration Search(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        Configuration config = null;
        string sql;

        if(this.option == 0)
        {
            sql = "select * from weather where weather_id = " + id;    
            command.CommandText = sql;
            data = command.ExecuteReader();

            if (data.Read())
            {
                config = new Configuration(
                    Convert.ToInt32(data["weather_id"]),
                    0,
                    data["weather_name"].ToString(),
                    data["weather_description"].ToString(),
                    Convert.ToInt32(data["weather_status"])
                );
            }
            data.Close();   
        }
        else if(this.option == 1)
        {
            sql = "select * from scenario where scenario_id = " + id;
            command.CommandText = sql;
            data = command.ExecuteReader();

            if (data.Read())
            {
                config = new Configuration(
                    Convert.ToInt32(data["scenario_id"]),
                    1,
                    data["scenario_name"].ToString(),
                    data["scenario_description"].ToString(),
                    Convert.ToInt32(data["scenario_status"])
                );
            }
            data.Close(); 
        }
        else if(this.option == 2)
        {
            sql = "select * from component where component_id = " + id;
            command.CommandText = sql;
            data = command.ExecuteReader();

            if (data.Read())
            {
                config = new Configuration(
                    Convert.ToInt32(data["component_id"]),
                    2,
                    data["component_name"].ToString(),
                    data["component_description"].ToString(),
                    Convert.ToInt32(data["component_status"])
                );
            }
            data.Close(); 
        }
        return config;
    }

    public List<Configuration> SearchAll(String filter,bool status)
    {
        List<Configuration> list = new List<Configuration>();
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        String sql;

        if(this.option == 0)
        {
            sql = "select * from weather ";
            if(!filter.Trim().Equals(""))
            {
                sql+= "where weather_name like '%$f%'";
                sql = sql.Replace("$f",filter);
                if(!status)
                    sql +=" and weather_status = 1";
            }
            else if(!status)
                sql +="where weather_status = 1";

            Debug.Log("SQL: " + sql);
            command.CommandText = sql;
            data = command.ExecuteReader();
            while (data.Read())
            {
                list.Add(new Configuration(Convert.ToInt32(data["weather_id"]),
                0,
                data["weather_name"].ToString(),
                data["weather_description"].ToString(),
                Convert.ToInt32(data["weather_status"])));
            }
            data.Close();
        }
        else if(this.option == 1)
        {
            sql = "select * from scenario ";
            if(!filter.Trim().Equals(""))
            {
                sql+= "where scenario_name like '%$f%'";
                sql = sql.Replace("$f",filter);
                if(!status)
                    sql +=" and scenario_status = 1";
            }
            else if(!status)
                sql +="where scenario_status = 1";

            Debug.Log("SQL: " + sql);
            command.CommandText = sql;
            data = command.ExecuteReader();
            while (data.Read())
            {
                list.Add(new Configuration(Convert.ToInt32(data["scenario_id"]),
                1,
                data["scenario_name"].ToString(),
                data["scenario_description"].ToString(),
                Convert.ToInt32(data["scenario_status"])));
            }
            data.Close();
        }
        else if(this.option == 2)
        {
            sql = "select * from component ";
            if(!filter.Trim().Equals(""))
            {
                sql+= "where component_name like '%$f%'";
                sql = sql.Replace("$f",filter);
                if(!status)
                    sql +=" and component_status = 1";
            }
            else if(!status)
                sql +="where component_status = 1";

            Debug.Log("SQL: " + sql);
            command.CommandText = sql;
            data = command.ExecuteReader();
            while (data.Read())
            {
                list.Add(new Configuration(Convert.ToInt32(data["component_id"]),
                2,
                data["component_name"].ToString(),
                data["component_description"].ToString(),
                Convert.ToInt32(data["component_status"])));
            }
            data.Close();
        }

        return list;
    }

    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public int Status { get => status; set => status = value; }
}
