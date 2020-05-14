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
    private List<KeyValuePair<int, string>> listScenarios; //only for component

    public Configuration(int option)
    {
        this.option = option;
    }
    public Configuration(int id,int option)
    {
        this.id = id;
        this.option = option;
    }

    public Configuration(int option, string name, string description, int status)
    {
        this.option = option;
        this.name = name;
        this.description = description;
        this.status = status;
        this.listScenarios = null;
    }

    public Configuration(int id, int option, string name, string description, int status)
    {
        this.id = id;
        this.option = option;
        this.name = name;
        this.description = description;
        this.status = status;
        this.listScenarios = null;
    }
    public Configuration(int option, string name, string description, int status, List<KeyValuePair<int, string>> listScenarios) //component
    {
        this.option = option;
        this.name = name;
        this.description = description;
        this.status = status;
        this.listScenarios = listScenarios;
    }

    public Configuration(int id, int option, string name, string description, int status, List<KeyValuePair<int, string>> listScenarios)//component
    {
        this.id = id;
        this.option = option;
        this.name = name;
        this.description = description;
        this.status = status;
        this.listScenarios = listScenarios;
    }

    public string Insert()
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        int result;
        string sql;

        if (this.option == 0)
        {
            sql = @"insert into weather 
                (weather_name,
                weather_description,
                weather_status)
                values ('$n','$d',$s);";
        }
        else if (this.option == 1)
        {
            sql = @"insert into scenario 
                (scenario_name,
                scenario_description,
                scenario_status)
                values ('$n','$d',$s);";
        }
        else if (this.option == 2)
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
        if (result == 1)
        {
            if (this.option == 2 && this.listScenarios != null)
            {
                int lastPk = GameManager.instance.GetMaxPK("component", "component_id");
                return InsertListComponentScenarios(lastPk) ? "Ok" : "Erro ao inserir!";
            }
            else
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

        if (this.option == 0)
        {
            sql = @"update weather set
                weather_name = '$n',
                weather_description = '$d',
                weather_status = $s
                where weather_id = " + id;
        }
        else if (this.option == 1)
        {
            sql = @"update scenario set
                scenario_name = '$n',
                scenario_description = '$d',
                scenario_status = $s
                where scenario_id = " + id;
        }
        else if (this.option == 2)
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
        if (result == 1)
        {
            if (this.option == 2 && this.listScenarios != null) // se component deleta todos os de component_scenario e insere novos
            {
                string sql2 = @"delete from component_scenario where component_id = " + id;
                command.CommandText = sql2;
                result = command.ExecuteNonQuery();
                if(result > 0)
                    return InsertListComponentScenarios(id) ? "Ok" : "Erro ao alterar!";
            }
            else
                return "Ok";
        }


        return "Erro ao alterar!";
    }

    private bool InsertListComponentScenarios(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        bool success = true;
        for (int i = 0; i < this.listScenarios.Count && success; i++)
        {
            string sql = @"insert into component_scenario
                    (component_id, scenario_id)
                    values ($c,$s);";
            sql = sql.Replace("$c", id + "");
            sql = sql.Replace("$s", this.listScenarios[i].Key + "");

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

    public bool Delete(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        string sql;
        int result;

        if (this.option == 0)
        {
            sql = @"update weather set weather_status = 0 where weather_id = " + id;
        }
        else if (this.option == 1)
        {
            sql = @"update scenario set scenario_status = 0 where scenario_id = " + id;
        }
        else if (this.option == 2)
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

        if (this.option == 0)
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
        else if (this.option == 1)
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
        else if (this.option == 2)
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
                data.Close();
                List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
                string sql2 =
                @"select sce.scenario_id, sce.scenario_name from component_scenario as comp inner join
                scenario as sce where comp.scenario_id = sce.scenario_id and component_id = " + id;
                command.CommandText = sql2;
                data = command.ExecuteReader();
                while (data.Read())
                {
                    lista.Add(new KeyValuePair<int, string>(Convert.ToInt32(data["scenario_id"]), data["scenario_name"].ToString()));
                }
                config.listScenarios = lista;
            }
            data.Close();
        }
        return config;
    }

    public List<Configuration> SearchAll(String filter, bool status)
    {
        List<Configuration> list = new List<Configuration>();
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        String sql;

        if (this.option == 0)
        {
            sql = "select * from weather ";
            if (!filter.Trim().Equals(""))
            {
                sql += "where weather_name like '%$f%'";
                sql = sql.Replace("$f", filter);
                if (!status)
                    sql += " and weather_status = 1";
            }
            else if (!status)
                sql += "where weather_status = 1";

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
        else if (this.option == 1)
        {
            sql = "select * from scenario ";
            if (!filter.Trim().Equals(""))
            {
                sql += "where scenario_name like '%$f%'";
                sql = sql.Replace("$f", filter);
                if (!status)
                    sql += " and scenario_status = 1";
            }
            else if (!status)
                sql += "where scenario_status = 1";

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
        else if (this.option == 2)
        {
            sql = "select * from component ";
            if (!filter.Trim().Equals(""))
            {
                sql += "where component_name like '%$f%'";
                sql = sql.Replace("$f", filter);
                if (!status)
                    sql += " and component_status = 1";
            }
            else if (!status)
                sql += "where component_status = 1";

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

    public List<Configuration> GetComponentsByScenario(int id)
    { 
        List<Configuration> list = new List<Configuration>();
        if(this.option == 2)
        {
            MySqlCommand command = GameManager.instance.Con.CreateCommand();
            MySqlDataReader data;
            String sql = @"select * from component_scenario as cs inner join component cp where cs.component_id = cp.component_id and scenario_id = " + id;
            command.CommandText = sql;
            data = command.ExecuteReader();
            while(data.Read())
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
    public List<KeyValuePair<int, string>> ListScenarios { get => listScenarios; set => listScenarios = value; }
}
