using System;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Scenario : MonoBehaviour
{
    private int id;
    private string name;
    private EnvironmentType environment;
    private string description;
    private int status;
    private List<KeyValuePair<int, string>> listComponents;
    private VirtualObject objScenario;

    public Scenario()
    {
        
    }

    public Scenario(int id)
    {
        this.id = id;
    }

    public Scenario(string name, EnvironmentType environment, string description, int status)
    {
        this.name = name;
        this.environment = environment;
        this.description = description;
        this.status = status;
        this.listComponents = null;
        this.objScenario = null;
    }

    public Scenario(int id, string name, EnvironmentType environment, string description, int status, VirtualObject objScenario)
    {
        this.id = id;
        this.name = name;
        this.environment = environment;
        this.description = description;
        this.status = status;
        this.listComponents = null;
        this.objScenario = objScenario;
    }


    public Scenario(string name, EnvironmentType environment, string description, int status, 
        List<KeyValuePair<int, string>> listComponents, VirtualObject objScenario)
    {
        this.name = name;
        this.environment = environment;
        this.description = description;
        this.status = status;
        this.listComponents = listComponents;
        this.objScenario = objScenario;
    }

    public string Insert()
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        int result;
        string sql = @"insert into scenario 
                            (scenario_name,
                            scenario_description,
                            env_id,
                            objSce_id,
                            scenario_status)
                    values ('$n','$d',$i,$o,$s);";
                    
        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$d", this.description);
        sql = sql.Replace("$i", this.environment.Id + "");
        sql = sql.Replace("$o", this.objScenario.Id + "");
        sql = sql.Replace("$s", this.status + "");

        command.CommandText = sql;
        result = command.ExecuteNonQuery();
        Debug.Log("resultado query: " + result);
        if(result == 1)
        {
            if (this.ListComponents != null)
            {
                int lastPk = GameManager.instance.GetMaxPK("scenario", "scenario_id");
                return InsertListScenarioComponent(lastPk) ? "Ok" : "Erro ao inserir!";
            }
            else
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
                        objSce_id = $o,
                        scenario_status = $s
                        where scenario_id = " + id;
        
        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$d", this.description);
        sql = sql.Replace("$i", this.environment.Id + "");
        sql = sql.Replace("$o", this.objScenario.Id + "");
        sql = sql.Replace("$s", this.status + "");

        Debug.Log("SWL ALTER: " + sql);
        command.CommandText = sql;
        result = command.ExecuteNonQuery();
        if(result == 1)
        {
            string sql2 = @"delete from component_scenario where scenario_id = " + id;
            command.CommandText = sql2;
            result = command.ExecuteNonQuery();
            if (this.ListComponents != null) // se component deleta todos os de component_scenario e insere novos
                return InsertListScenarioComponent(id) ? "Ok" : "Erro ao alterar!";
            else
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

     private bool InsertListScenarioComponent(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        bool success = true;
        for (int i = 0; i < this.ListComponents.Count && success; i++)
        {
            string sql = @"insert into component_scenario
                    (component_id, scenario_id)
                    values ($c,$s);";
            sql = sql.Replace("$c", this.ListComponents[i].Key + "");
            sql = sql.Replace("$s", id + "");

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

    public Scenario Search(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        Scenario scenario = null;
        string sql = @"select * from scenario as sce inner join environmentType as type inner join objScenario as obj
                     where sce.env_id = type.env_id and sce.objSce_id = obj.objSce_id and sce.scenario_id = " + id;

        command.CommandText = sql;
        data = command.ExecuteReader();

        if (data.Read())
        {
            scenario = new Scenario(
                Convert.ToInt32(data["scenario_id"]),
                data["scenario_name"].ToString(),
                new EnvironmentType(
                    Convert.ToInt32(data["env_id"]),
                    data["env_name"].ToString(),
                    data["env_description"].ToString(),
                    Convert.ToInt32(data["env_status"])
                ),
                data["scenario_description"].ToString(),
                Convert.ToInt32(data["scenario_status"]),
                new VirtualObject(
                    Convert.ToInt32(data["objSce_id"]),
                    data["objSce_name"].ToString(),
                    data["objSce_file"].ToString()
                )
            );
            data.Close();

            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            string sql2 =
                @"select comp.component_id, comp.component_name from component_scenario as sce inner join
                component as comp where comp.component_id = sce.component_id and scenario_id = " + id;
            command.CommandText = sql2;
            data = command.ExecuteReader();
            while (data.Read())
            {
                lista.Add(new KeyValuePair<int, string>(Convert.ToInt32(data["component_id"]), data["component_name"].ToString()));
            }
            scenario.ListComponents = lista;
        }       
        data.Close();
        
        return scenario;
    }

    public List<Scenario> SearchAll(string filter, bool status)
    {
        List<Scenario> list = new List<Scenario>();
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        String sql = @"select * from scenario as sce inner join environmentType as type inner join objScenario as obj
                     where sce.env_id = type.env_id and sce.objSce_id = obj.objSce_id";

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
                new EnvironmentType(
                    Convert.ToInt32(data["env_id"]),
                    data["env_name"].ToString(),
                    data["env_description"].ToString(),
                    Convert.ToInt32(data["env_status"])
                    ),
                data["scenario_description"].ToString(),
                Convert.ToInt32(data["scenario_status"]),
                new VirtualObject(
                    Convert.ToInt32(data["objSce_id"]),
                    data["objSce_name"].ToString(),
                    data["objSce_file"].ToString()
                )
            ));
        }
        data.Close();

        return list;
    }


        public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public EnvironmentType Environment { get => environment; set => environment = value; }
    public string Description { get => description; set => description = value; }
    public int Status { get => status; set => status = value; }
    public List<KeyValuePair<int, string>> ListComponents { get => listComponents; set => listComponents = value; }
    public VirtualObject ObjScenario { get => objScenario; set => objScenario = value; }
}
