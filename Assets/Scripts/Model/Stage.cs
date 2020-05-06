using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    int id;
    string name;
    string description;
    Configuration weather;
    int time;
    Configuration scenario;
    List<Configuration> components;
    int status;

    public Stage()
    {

    }

    public Stage(string name, string description, Configuration weather, int time, Configuration scenario, List<Configuration> components, int status)
    {
        this.name = name;
        this.description = description;
        this.weather = weather;
        this.time = time;
        this.scenario = scenario;
        this.components = components;
        this.status = status;
    }

    public Stage(int id, string name, string description, Configuration weather, int time, Configuration scenario, List<Configuration> components, int status)
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
        string sql = @"insert into stage 
                            (stage_name,
                            stage_description,
                            stage_time,
                            scenario_id,
                            weather_id,
                            stage_status)
                    values ('$n','$d',$t,$s,$w,$i);";
        
        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$d", this.description);
        sql = sql.Replace("$t", this.time + "");
        sql = sql.Replace("$s", this.scenario.Id + "");
        sql = sql.Replace("$w", this.weather.Id + "");
        sql = sql.Replace("$i", this.status + "");

        Debug.Log("inserting, sql:" + sql);
        command.CommandText = sql;
        result = command.ExecuteNonQuery();
        Debug.Log("resultado query: " + result);
        if(result == 1)
        {
            if(this.components.Count > 0)
            {
                int lastPk = GameManager.instance.GetMaxPK("stage","stage_id");
                return InsertListStageComponent(lastPk) ? "Ok" : "Erro ao inserir componentes na sessão!";
            }
            else
                return "Ok";
        }

        
        return "Erro ao inserir!";
    }

    private bool InsertListStageComponent(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        bool success = true;
        for (int i = 0; i < this.components.Count && success; i++)
        {
            string sql = @"insert into stage_component
                    (stage_id, component_id)
                    values ($s,$c);";
            sql = sql.Replace("$s", id + "");
            sql = sql.Replace("$c",  this.components[i].Id + "");

            try
            {
                command.CommandText = sql;
                success = command.ExecuteNonQuery() == 1;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }
        return success;
    }

    public string Alter(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        int result;
        string sql = @"update stage set 
                        stage_name = '$n',
                        stage_description = '$d',
                        stage_time = $t,
                        scenario_id = $s,
                        weather_id = $w,
                        stage_status = $i
                        where stage_id = " + id;

        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$d", this.description);
        sql = sql.Replace("$t", this.time + "");
        sql = sql.Replace("$s", this.scenario.Id + "");
        sql = sql.Replace("$w", this.weather.Id + "");
        sql = sql.Replace("$i", this.status + "");

        Debug.Log("altering. " + sql);
        command.CommandText = sql;
        result = command.ExecuteNonQuery();
        Debug.Log("resultado query: " + result);
        if(result == 1)
        {
            sql = @"delete from stage_component where stage_id = " + id;
            command.CommandText = sql;
            command.ExecuteNonQuery();
            if(this.components.Count > 0)
                return InsertListStageComponent(id) ? "Ok" : "Erro ao alterar componentes da sessão!";

            return "Ok";
        }

        return "Erro ao alterar!";
    }

    public bool Delete(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        string sql = @"update stage set stage_status = 0 where stage_id = " + id;
        int result;

        command.CommandText = sql;
        result = command.ExecuteNonQuery();
        
        return result == 1;
    }

    public Stage Search(int id)
    {
        Stage stage = null;
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        string sql = @"select * from stage as st inner join scenario as sce inner join weather as wea where st.scenario_id = sce.scenario_id and
                st.weather_id = wea.weather_id and stage_id = " + id;

        command.CommandText = sql;
        data = command.ExecuteReader();

        if (data.Read())
        {
            stage = new Stage(Convert.ToInt32(data["stage_id"]),
                data["stage_name"].ToString(),
                data["stage_description"].ToString(),
                new Configuration(Convert.ToInt32(data["weather_id"]),
                    0,
                    data["weather_name"].ToString(),
                    data["weather_description"].ToString(),
                    Convert.ToInt32(data["weather_status"])
                ), //weather 0
                Convert.ToInt32(data["stage_time"]),
                new Configuration(Convert.ToInt32(data["scenario_id"]),
                    1,
                    data["scenario_name"].ToString(),
                    data["scenario_description"].ToString(),
                    Convert.ToInt32(data["scenario_status"])
                ), //scenario 1
                new List<Configuration>(),
                Convert.ToInt32(data["stage_status"])
            );
            data.Close();
            List<Configuration> lista = new List<Configuration>();
            sql = @"select * from stage_component as st inner join component as comp where
                st.component_id = comp.component_id and st.stage_id = " + id;
            command.CommandText = sql;
            data = command.ExecuteReader();
            while(data.Read())
            {
                lista.Add(new Configuration(Convert.ToInt32(data["component_id"]),
                    2,
                    data["component_name"].ToString(),
                    data["component_description"].ToString(),
                    Convert.ToInt32(data["component_status"])
                ));
            }
            stage.components = lista;
        }
        data.Close();
        return stage;
    }

    public List<Stage> SearchAll(string filter, bool status)
    {
        List<Stage> lista = new List<Stage>();
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        String sql = @"select * from stage as st inner join scenario as sce inner join weather as wea where st.scenario_id = sce.scenario_id and
                st.weather_id = wea.weather_id ";
        if(!filter.Trim().Equals(""))
        {
            sql+="and stage_name like '%%f%'";
            sql = sql.Replace("%f",filter);
            if(!status)
                sql +=" and stage_status = 1";
        }
        else if(!status)
            sql += "and stage_status = 1";

        command.CommandText = sql;
        data = command.ExecuteReader();
        while(data.Read())
        {
            lista.Add(new Stage(Convert.ToInt32(data["stage_id"]),
                data["stage_name"].ToString(),
                data["stage_description"].ToString(),
                new Configuration(Convert.ToInt32(data["weather_id"]),
                    0,
                    data["weather_name"].ToString(),
                    data["weather_description"].ToString(),
                    Convert.ToInt32(data["weather_status"])
                ), //weather 0
                Convert.ToInt32(data["stage_time"]),
                new Configuration(Convert.ToInt32(data["scenario_id"]),
                    1,
                    data["scenario_name"].ToString(),
                    data["scenario_description"].ToString(),
                    Convert.ToInt32(data["scenario_status"])
                ), //scenario 1
                new List<Configuration>(),
                Convert.ToInt32(data["stage_status"])
            ));
        }
        data.Close();
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
