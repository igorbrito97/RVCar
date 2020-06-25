using System;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScenarioComponent : MonoBehaviour
{
    private int id;
    private string name;
    private string description;
    private int status;
    private List<KeyValuePair<int, string>> listScenarios;

    private VirtualObject objComponent;


    public ScenarioComponent()
    {

    }

    public ScenarioComponent(string name, string description, int status, VirtualObject objComponent)
    {
        this.Name = name;
        this.Description = description;
        this.Status = status;
        this.ListScenarios = null;
        this.ObjComponent = objComponent;
    }

    public ScenarioComponent(int id, string name, string description, int status, VirtualObject objComponent)
    {
        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Status = status;
        this.ListScenarios = null;
        this.ObjComponent = objComponent;
    }

    public ScenarioComponent(string name, string description, int status, List<KeyValuePair<int, string>> listScenarios,
        VirtualObject objComponent)
    {
        this.Name = name;
        this.Description = description;
        this.Status = status;
        this.ListScenarios = listScenarios;
        this.ObjComponent = objComponent;
    }

    public string Insert()
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        int result;
        string sql = @"insert into component 
                    (component_name,
                    component_description,
                    component_status,
                    objComp_id)
                    values ('$n','$d',$s,$o);";
    

        sql = sql.Replace("$n", this.Name);
        sql = sql.Replace("$d", this.Description);
        sql = sql.Replace("$s", this.Status + "");
        sql = sql.Replace("$o", this.objComponent.Id + "");

        Debug.Log("inserting, sql:" + sql);
        command.CommandText = sql;
        result = command.ExecuteNonQuery();
        Debug.Log("resultado query: " + result);
        if (result == 1)
        {
            if (this.ListScenarios != null)
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
        int result;
        string sql = @"update component set
                component_name = '$n',
                component_description = '$d',
                component_status = $s,
                objComp_id = $o
                where component_id = " + id;

        sql = sql.Replace("$n", this.Name);
        sql = sql.Replace("$d", this.Description);
        sql = sql.Replace("$s", this.Status + "");
        sql = sql.Replace("$o", this.objComponent.Id + "");

        Debug.Log("altering. " + sql);
        command.CommandText = sql;
        result = command.ExecuteNonQuery();
        Debug.Log("resultado query: " + result);
        if (result == 1)
        {
            string sql2 = @"delete from component_scenario where component_id = " + id;
            command.CommandText = sql2;
            result = command.ExecuteNonQuery();
            if (this.ListScenarios != null)
                return InsertListComponentScenarios(id) ? "Ok" : "Erro ao alterar!";
            else
                return "Ok";
        }


        return "Erro ao alterar!";
    }

    private bool InsertListComponentScenarios(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        bool success = true;
        for (int i = 0; i < this.ListScenarios.Count && success; i++)
        {
            string sql = @"insert into component_scenario
                    (component_id, scenario_id)
                    values ($c,$s);";
            sql = sql.Replace("$c", id + "");
            sql = sql.Replace("$s", this.ListScenarios[i].Key + "");

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
        string sql = @"update component set component_status = 0 where component_id = " + id;

        command.CommandText = sql;
        return command.ExecuteNonQuery() == 1;
    }

    public ScenarioComponent Search(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        ScenarioComponent comp = null;
        string sql = @"select * from component as comp inner join objComponent as obj where 
                        comp.objComp_id = obj.objComp_id and component_id = " + id;

        command.CommandText = sql;
        data = command.ExecuteReader();

        if (data.Read())
        {
            comp = new ScenarioComponent(
                Convert.ToInt32(data["component_id"]),
                data["component_name"].ToString(),
                data["component_description"].ToString(),
                Convert.ToInt32(data["component_status"]),
                new VirtualObject(
                    Convert.ToInt32(data["objComp_id"]),
                    data["objComp_name"].ToString(),
                    data["objComp_file"].ToString()
                )
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
            comp.listScenarios = lista;
        }
        data.Close();
        return comp;
    }

    public List<ScenarioComponent> SearchAll(String filter, bool status)
    {
        List<ScenarioComponent> list = new List<ScenarioComponent>();
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        String sql = @"select * from component as comp inner join objComponent as obj 
                    where comp.objComp_id = obj.objComp_id";

        if (!filter.Trim().Equals(""))
        {
            sql += " and component_name like '%$f%'";
            sql = sql.Replace("$f", filter);
            if (!status)
                sql += " and component_status = 1";
        }
        else if (!status)
            sql += " and component_status = 1";

        Debug.Log("SQL: " + sql);
        command.CommandText = sql;
        data = command.ExecuteReader();
        while (data.Read())
        {
            list.Add(new ScenarioComponent(
                Convert.ToInt32(data["component_id"]),
                data["component_name"].ToString(),
                data["component_description"].ToString(),
                Convert.ToInt32(data["component_status"]),
                new VirtualObject(
                    Convert.ToInt32(data["objComp_id"]),
                    data["objComp_name"].ToString(),
                    data["objComp_file"].ToString()
                )
            ));
        }
        data.Close();

        return list;
    }

    public List<ScenarioComponent> GetComponentsByScenario(int id)
    { 
        List<ScenarioComponent> list = new List<ScenarioComponent>();
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        String sql = @"select * from component_scenario as cs inner join component cp inner join objComponent as obj
                        where cs.component_id = cp.component_id and cp.objComp_id = obj.objComp_id
                        and scenario_id = " + id;
        
        command.CommandText = sql;
        data = command.ExecuteReader();
        while(data.Read())
        {
            list.Add(new ScenarioComponent(
                Convert.ToInt32(data["component_id"]),
                data["component_name"].ToString(),
                data["component_description"].ToString(),
                Convert.ToInt32(data["component_status"]),
                new VirtualObject(
                    Convert.ToInt32(data["objComp_id"]),
                    data["objComp_name"].ToString(),
                    data["objComp_file"].ToString()
                )
            ));
        }
        data.Close();
       
        return list;
    }

    public int GetMaxQuantityScenarioComponent(int comp_id, int sce_id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        int res = 1;
        string sql = @"select quantmax from compsce_quantity where comp_id = " + comp_id + " and sce_id = " + sce_id;

        command.CommandText = sql;
        data = command.ExecuteReader();

        if (data.Read())
        {
            res = Convert.ToInt32(data["quantmax"]);
        }
        data.Close();
        return res;
    }

    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public int Status { get => status; set => status = value; }
    public List<KeyValuePair<int, string>> ListScenarios { get => listScenarios; set => listScenarios = value; }
    public VirtualObject ObjComponent { get => objComponent; set => objComponent = value; }
}
