using System;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Scenario : MonoBehaviour
{
    private int id;
    private string name;
    private string file;
    private EnvironmentType environment;
    private string description;
    private int status;

    public Scenario()
    {
        
    }

    public Scenario(string name, string file, EnvironmentType environment, string description, int status)
    {
        this.name = name;
        this.file = file;
        this.environment = environment;
        this.description = description;
        this.status = status;
    }

     public Scenario(int id, string name, EnvironmentType environment, string description, int status)
    {
        this.id = id;
        this.name = name;
        this.file = "";
        this.environment = environment;
        this.description = description;
        this.status = status;
    }

    public Scenario(int id, string name, string file, EnvironmentType environment, string description, int status)
    {
        this.id = id;
        this.name = name;
        this.file = file;
        this.environment = environment;
        this.description = description;
        this.status = status;
    }
    public string Insert()
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        int result;
        string sql = @"insert into scenario 
                            (scenario_name,
                            scenario_description,
                            scenario_file,
                            env_id,
                            scenario_status)
                    values ('$n','$d','$f',$i,$s);";
                    
        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$d", this.description);
        sql = sql.Replace("$f", this.file);
        sql = sql.Replace("$i", this.environment.Id + "");
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
        string sql = @"update scenario set
                        scenario_name = '$n',
                        scenario_description = '$d',
                        env_id = $i,
                        scenario_file = '$f',
                        scenario_status = $s
                        where scenario_id = " + id;
        
        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$d", this.description);
        sql = sql.Replace("$i", this.environment.Id + "");
        sql = sql.Replace("$f", this.file);
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
        string sql = @"update scenario set scenario_status = 0 where scenario_id = " + id;
        int result;

        command.CommandText = sql;
        result = command.ExecuteNonQuery();

        return result == 1;
    }

    public Scenario Search(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        Scenario scenario = null;
        string sql = @"select * from scenario as sce inner join environmentType as type
                     where sce.env_id = type.env_id and sce.scenario_id = " + id;

        command.CommandText = sql;
        data = command.ExecuteReader();

        if (data.Read())
        {
            scenario = new Scenario(
                Convert.ToInt32(data["scenario_id"]),
                data["scenario_name"].ToString(),
                data["scenario_file"].ToString(),
                new EnvironmentType(
                    Convert.ToInt32(data["env_id"]),
                    data["env_name"].ToString(),
                    data["env_description"].ToString(),
                    Convert.ToInt32(data["env_status"])
                ),
                data["scenario_description"].ToString(),
                Convert.ToInt32(data["scenario_status"])
            );
        }       
        data.Close();
        return scenario;
    }

    public List<Scenario> SearchAll(string filter, bool status)
    {
        List<Scenario> list = new List<Scenario>();
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        String sql = @"select * from scenario as sce inner join environmentType as type
                     where sce.env_id = type.env_id";

        if(!filter.Trim().Equals(""))
        {
            sql+= " and scenario_name like '%$f%'";
            sql = sql.Replace("$f",filter);
            if(!status)
                sql +=" and scenario_status = 1";
        }
        else if(!status)
            sql +=" and scenario_status = 1";
         

        Debug.Log("SQL: " + sql);
        command.CommandText = sql;
        data = command.ExecuteReader();
        while (data.Read())
        {
            list.Add(new Scenario(
                Convert.ToInt32(data["scenario_id"]),
                data["scenario_name"].ToString(),
                data["scenario_file"].ToString(),
                new EnvironmentType(
                    Convert.ToInt32(data["env_id"]),
                    data["env_name"].ToString(),
                    data["env_description"].ToString(),
                    Convert.ToInt32(data["env_status"])
                    ),
                data["scenario_description"].ToString(),
                Convert.ToInt32(data["scenario_status"])
            ));
        }
        data.Close();

        return list;
    }


        public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public string File { get => file; set => file = value; }
    public EnvironmentType Environment { get => environment; set => environment = value; }
    public string Description { get => description; set => description = value; }
    public int Status { get => status; set => status = value; }
}
