using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Session : MonoBehaviour {
    int id;
    string name;
    string description;
    Configuration weather;
    int time;
    Configuration scenario;
    List<Configuration> components;
    int status;

    public Session()
    {

    }

    public Session(string name, string description, Configuration weather, int time, Configuration scenario, List<Configuration> components, int status)
    {
        this.name = name;
        this.description = description;
        this.weather = weather;
        this.time = time;
        this.scenario = scenario;
        this.components = components;
        this.status = status;
    }

    public Session(int id, string name, string description, Configuration weather, int time, Configuration scenario, List<Configuration> components, int status)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.weather = weather;
        this.time = time;
        this.scenario = scenario;
        this.components = components;
        this.status = status;
    }

    public string Insert()
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        int result;
        string sql = @"insert into session 
                            (session_name,
                            session_description,
                            session_time,
                            session_scenario,
                            session_weather,
                            session_status)
                    values ('$n','$d',$t,$s,$w,$st);";
        
        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$d", this.description);
        sql = sql.Replace("$t", this.time + "");
        sql = sql.Replace("$s", this.scenario.Id + "");
        sql = sql.Replace("$w", this.weather.Id + "");
        sql = sql.Replace("$st", this.status + "");

        //INSERIR LIST COMPONENT

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
        int result;
        string sql = @"update session set 
                        session_name = '$n',
                        session_description = '$d',
                        session_time = $t,
                        session_scenario = $s,
                        session_weather = $w,
                        session_status = $st
                        where session_id = " + id;

        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$d", this.description);
        sql = sql.Replace("$t", this.time + "");
        sql = sql.Replace("$s", this.scenario.Id + "");
        sql = sql.Replace("$w", this.weather.Id + "");
        sql = sql.Replace("$st", this.status + "");

        //LIST COMPONENT

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
        string sql = @"update session set session_status = 0 where session_id = " + id;
        int result;

        command.CommandText = sql;
        result = command.ExecuteNonQuery();
        
        return result == 1;
    }

    public Session Search(int id)
    {
        Session session = new Session();
        return session;
    }

    public List<Session> SearchAll(string filter, bool status)
    {
        List<Session> lista = new List<Session>();
        return lista;
    }

    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public Configuration Weather { get => weather; set => weather = value; }
    public int Time { get => time; set => time = value; }
    public Configuration Scenario { get => scenario; set => scenario = value; }
    public List<Configuration> Components { get => components; set => components = value; }
    public int Status { get => status; set => status = value; }
}