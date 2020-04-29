using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Session : MonoBehaviour {
    int id;
    string name;
    string description;
    Stage stage;
    Patient patient;
    int status;
    int isPublic;

    public Session()
    {

    }

    public Session(string name, string description, Stage stage, Patient patient, int status, int isPublic)
    {
        this.name = name;
        this.description = description;
        this.stage = stage;
        this.patient = patient;
        this.status = status;
        this.isPublic = isPublic;
    }

    public Session(int id, string name, string description, Stage stage, Patient patient, int status, int isPublic)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.stage = stage;
        this.patient = patient;
        this.status = status;
        this.isPublic = isPublic;
    }

    public string Insert()
    {
        // MySqlCommand command = GameManager.instance.Con.CreateCommand();
        // int result;
        // string sql = @"insert into session 
        //                     (session_name,
        //                     session_description,
        //                     session_time,
        //                     session_scenario,
        //                     session_weather,
        //                     session_status)
        //             values ('$n','$d',$t,$s,$w,$i);";

        // sql = sql.Replace("$n", this.name);
        // sql = sql.Replace("$d", this.description);
        // sql = sql.Replace("$t", this.time + "");
        // sql = sql.Replace("$s", this.scenario.Id + "");
        // sql = sql.Replace("$w", this.weather.Id + "");
        // sql = sql.Replace("$i", this.status + "");

        // Debug.Log("inserting, sql:" + sql);
        // command.CommandText = sql;
        // result = command.ExecuteNonQuery();
        // Debug.Log("resultado query: " + result);
        // if(result == 1)
        // {
        //     if(this.components.Count > 0)
        //     {
        //         int lastPk = GameManager.instance.GetMaxPK("session","session_id");
        //         return InsertListSessionComponent(lastPk) ? "Ok" : "Erro ao inserir componentes na sessão!";
        //     }
        //     else
        //         return "Ok";
        // }


        return "Erro ao inserir!";
    }

    private bool InsertListSessionComponent(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        bool success = true;
    //     for (int i = 0; i < this.components.Count && success; i++)
    //     {
    //         string sql = @"insert into session_component
    //                 (session_id, component_id)
    //                 values ($s,$c);";
    //         sql = sql.Replace("$s", id + "");
    //         sql = sql.Replace("$c",  this.components[i].Id + "");

    //         try
    //         {
    //             command.CommandText = sql;
    //             success = command.ExecuteNonQuery() == 1;
    //         }
    //         catch (MySqlException ex)
    //         {
    //             return false;
    //         }
    //     }
        return success;
    }

    public string Alter(int id)
    {
    //     MySqlCommand command = GameManager.instance.Con.CreateCommand();
    //     MySqlDataReader data;
    //     int result;
    //     string sql = @"update session set 
    //                     session_name = '$n',
    //                     session_description = '$d',
    //                     session_time = $t,
    //                     session_scenario = $s,
    //                     session_weather = $w,
    //                     session_status = $i
    //                     where session_id = " + id;

    //     sql = sql.Replace("$n", this.name);
    //     sql = sql.Replace("$d", this.description);
    //     sql = sql.Replace("$t", this.time + "");
    //     sql = sql.Replace("$s", this.scenario.Id + "");
    //     sql = sql.Replace("$w", this.weather.Id + "");
    //     sql = sql.Replace("$i", this.status + "");

    //     Debug.Log("altering. " + sql);
    //     command.CommandText = sql;
    //     result = command.ExecuteNonQuery();
    //     Debug.Log("resultado query: " + result);
    //     if(result == 1)
    //     {
    //         sql = @"delete from session_component where session_id = " + id;
    //         command.CommandText = sql;
    //         command.ExecuteNonQuery();
    //         if(this.components.Count > 0)
    //             return InsertListSessionComponent(id) ? "Ok" : "Erro ao alterar componentes da sessão!";

    //         return "Ok";
    //     }

        return "Erro ao alterar!";
    }

    public bool Delete(int id)
    {
        int result = 1;
        // MySqlCommand command = GameManager.instance.Con.CreateCommand();
        // string sql = @"update session set session_status = 0 where session_id = " + id;

        // command.CommandText = sql;
        // result = command.ExecuteNonQuery();

        return result == 1;
    }

    public Session Search(int id)
    {
        Session session = null;
    //     MySqlCommand command = GameManager.instance.Con.CreateCommand();
    //     MySqlDataReader data;
    //     string sql = @"select * from session inner join scenario inner join weather where session_scenario = scenario_id and
    //             session_weather = weather_id and session_id = " + id;

    //     command.CommandText = sql;
    //     data = command.ExecuteReader();

    //     if (data.Read())
    //     {
    //         session = new Session(Convert.ToInt32(data["session_id"]),
    //             data["session_name"].ToString(),
    //             data["session_description"].ToString(),
    //             new Configuration(Convert.ToInt32(data["weather_id"]),
    //                 0,
    //                 data["weather_name"].ToString(),
    //                 data["weather_description"].ToString(),
    //                 Convert.ToInt32(data["weather_status"])
    //             ), //weather 0
    //             Convert.ToInt32(data["session_time"]),
    //             new Configuration(Convert.ToInt32(data["scenario_id"]),
    //                 1,
    //                 data["scenario_name"].ToString(),
    //                 data["scenario_description"].ToString(),
    //                 Convert.ToInt32(data["scenario_status"])
    //             ), //scenario 1
    //             new List<Configuration>(),
    //             Convert.ToInt32(data["session_status"])
    //         );
    //         data.Close();
    //         List<Configuration> lista = new List<Configuration>();
    //         sql = @"select * from session_component as ses inner join component as comp where
    //             ses.component_id = comp.component_id and ses.session_id = " + id;
    //         command.CommandText = sql;
    //         data = command.ExecuteReader();
    //         while(data.Read())
    //         {
    //             lista.Add(new Configuration(Convert.ToInt32(data["component_id"]),
    //                 2,
    //                 data["component_name"].ToString(),
    //                 data["component_description"].ToString(),
    //                 Convert.ToInt32(data["component_status"])
    //             ));
    //         }
    //         session.components = lista;
    //     }
    //     data.Close();
        return session;
    }

    public List<Session> SearchAll(string filter, bool status)
    {
        List<Session> lista = new List<Session>();
    //     MySqlCommand command = GameManager.instance.Con.CreateCommand();
    //     MySqlDataReader data;
    //     String sql = @"select * from session inner join scenario inner join weather where session_scenario = scenario_id and
    //             session_weather = weather_id ";
    //     if(!filter.Trim().Equals(""))
    //     {
    //         sql+="and session_name like '%f%'";
    //         sql = sql.Replace("%f",filter);
    //         if(!status)
    //             sql +=" and session_status = 1";
    //     }
    //     else if(!status)
    //         sql += "and session_status = 1";

    //     command.CommandText = sql;
    //     data = command.ExecuteReader();
    //     while(data.Read())
    //     {
    //         lista.Add(new Session(Convert.ToInt32(data["session_id"]),
    //             data["session_name"].ToString(),
    //             data["session_description"].ToString(),
    //             new Configuration(Convert.ToInt32(data["weather_id"]),
    //                 0,
    //                 data["weather_name"].ToString(),
    //                 data["weather_description"].ToString(),
    //                 Convert.ToInt32(data["weather_status"])
    //             ), //weather 0
    //             Convert.ToInt32(data["session_time"]),
    //             new Configuration(Convert.ToInt32(data["scenario_id"]),
    //                 1,
    //                 data["scenario_name"].ToString(),
    //                 data["scenario_description"].ToString(),
    //                 Convert.ToInt32(data["scenario_status"])
    //             ), //scenario 1
    //             new List<Configuration>(),
    //             Convert.ToInt32(data["session_status"])
    //         ));
    //     }
    //     data.Close();
        return lista;
    }

    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public Stage Stage { get => stage; set => stage = value; }
    public Patient Patient { get => patient; set => patient = value; }
    public int Status { get => status; set => status = value; }
    public int IsPublic { get => isPublic; set => isPublic = value; }
}