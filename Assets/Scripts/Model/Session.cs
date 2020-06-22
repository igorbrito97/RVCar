using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Session : MonoBehaviour {
    private int id;
    private string name;
    string description;
    private Psychologist psychologist;
    private Patient patient;
    private Scenario scenario;
    private Weather weather;
    private List<KeyValuePair<int, string>> listComponents;
    private int status;
    private int isPublic;


    public Session()
    {
    }
    public Session(Psychologist psychologist)
    {
        this.psychologist = psychologist;
    }

    public Session(string name, string description, Psychologist psychologist, Patient patient, Scenario scenario, Weather weather, int status, int isPublic)
    {
        this.name = name;
        this.description = description;
        this.psychologist = psychologist;
        this.patient = patient;
        this.scenario = scenario;
        this.weather = weather;
        this.status = status;
        this.isPublic = isPublic;
        this.listComponents = null;
    }

    public Session(int id, string name, string description, Psychologist psychologist, Patient patient, Scenario scenario, Weather weather, int status, int isPublic)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.psychologist = psychologist;
        this.patient = patient;
        this.scenario = scenario;
        this.weather = weather;
        this.status = status;
        this.isPublic = isPublic;
        this.listComponents = null;
    }public Session(string name, string description, Psychologist psychologist, Patient patient, Scenario scenario, Weather weather, int status, int isPublic, List<KeyValuePair<int, string>> listComponents)
    {
        this.name = name;
        this.description = description;
        this.psychologist = psychologist;
        this.patient = patient;
        this.scenario = scenario;
        this.weather = weather;
        this.status = status;
        this.isPublic = isPublic;
        this.listComponents = listComponents;
    }

    public Session(int id, string name, string description, Psychologist psychologist, Patient patient, Scenario scenario, Weather weather, int status, int isPublic, List<KeyValuePair<int, string>> listComponents)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.psychologist = psychologist;
        this.patient = patient;
        this.scenario = scenario;
        this.weather = weather;
        this.status = status;
        this.isPublic = isPublic;
        this.listComponents = listComponents;
    }

    public string Insert()
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        int result;
        string sql = @"insert into session 
                            (psychologist_id,
                            patient_id,
                            weather_id,
                            scenario_id,
                            session_name,
                            session_description,
                            session_status,
                            session_public)
                    values ($ps,$pt,$we,$sce,'$n','$d',$st,$ip);";

        sql = sql.Replace("$ps", this.psychologist.Id + "");
        sql = sql.Replace("$pt", this.patient.Id + "");
        sql = sql.Replace("$we", this.weather.Id + "");
        sql = sql.Replace("$sce", this.scenario.Id + "");
        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$d", this.description);
        sql = sql.Replace("$st", this.status + "");
        sql = sql.Replace("$ip", this.isPublic + "");

        Debug.Log("inserting, sql:" + sql);
        command.CommandText = sql;
        result = command.ExecuteNonQuery();
        Debug.Log("resultado query: " + result);
        if(result == 1)
        {
            if (this.ListComponents != null)
            {
                int lastPk = GameManager.instance.GetMaxPK("session", "session_id");
                return InsertListSessionComponent(lastPk) ? "Ok" : "Erro ao inserir!";
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
        string sql = @"update session set 
                        psychologist_id = $ps,
                        patient_id = $pt,
                        weather_id = $we,
                        scenario_id = $sce,
                        session_name = '$n',
                        session_description = '$d',
                        session_status = $status,
                        session_public = $ip
                        where session_id = " + id;

        sql = sql.Replace("$ps", this.psychologist.Id + "");
        sql = sql.Replace("$pt", this.patient.Id + "");
        sql = sql.Replace("$we", this.weather.Id + "");
        sql = sql.Replace("$sce", this.scenario.Id + "");
        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$d", this.description);
        sql = sql.Replace("$status", this.status + "");
        sql = sql.Replace("$ip", this.isPublic + "");

        Debug.Log("altering. " + sql);
        command.CommandText = sql;
        result = command.ExecuteNonQuery();
        Debug.Log("resultado query: " + result);
        if(result == 1)
        {
            string sql2 = @"delete from session_component where session_id = " + id;
            command.CommandText = sql2;
            result = command.ExecuteNonQuery();
            if (this.ListComponents != null)
                return InsertListSessionComponent(id) ? "Ok" : "Erro ao alterar!";
            else
                return "Ok";
        }

        return "Erro ao alterar!";
    }

    public bool Delete(int id)
    {
        int result = 1;
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        string sql = @"update session set session_status = 0 where session_id = " + id;

        command.CommandText = sql;
        result = command.ExecuteNonQuery();

        return result == 1;
    }

    private bool InsertListSessionComponent(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        bool success = true;
        for (int i = 0; i < this.ListComponents.Count && success; i++)
        {
            string sql = @"insert into session_component
                    (session_id, component_id)
                    values ($s,$c);";
            sql = sql.Replace("$s", id + "");
            sql = sql.Replace("$c", this.ListComponents[i].Key + "");

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

    public Session Search(int id) //nao busca tudo do stage porque tem scenario, weather
    {
        Debug.Log("SEARCHZADA" + id);
        Session session = null;
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        string sql = @"select * from session as ses inner join psychologist inner join weather wea inner join patient inner join scenario as sce 
                inner join weatherType as wt inner join environmentType as et where 
                psychologist_id = psyc_id and ses.weather_id = wea.weather_id and patient_id = pat_id and 
                sce.scenario_id = ses.scenario_id and wt.weatherType_id = wea.weatherType_id and et.env_id = sce.env_id and session_id = " + id;

        command.CommandText = sql;
        data = command.ExecuteReader();

        if (data.Read())
        {
            session = new Session(Convert.ToInt32(data["session_id"]),
                data["session_name"].ToString(),
                data["session_description"].ToString(),
                new Psychologist(Convert.ToInt32(data["psyc_id"]),
                    data["psyc_name"].ToString(),
                    data["psyc_cpf"].ToString(),
                    data["psyc_phone"].ToString(),
                    data["psyc_email"].ToString(),
                    Convert.ToChar(data["psyc_gender"]),
                    data["psyc_password"].ToString(),
                    Convert.ToInt32(data["psyc_status"]),
                    data["psyc_crp"].ToString(),
                    Convert.ToDateTime(data["psyc_birthday"])
                ),
                new Patient(Convert.ToInt32(data["pat_id"]),
                    data["pat_name"].ToString(),
                    data["pat_cpf"].ToString(),
                    Convert.ToDateTime(data["pat_birthday"].ToString()),
                    data["pat_phone"].ToString(),
                    data["pat_email"].ToString(),
                    data["pat_note"].ToString(),
                    Convert.ToChar(data["pat_gender"]),
                    Convert.ToInt32(data["pat_status"])
                ),
                new Scenario(
                    Convert.ToInt32(data["scenario_id"]),
                    data["scenario_name"].ToString(),
                    new EnvironmentType(
                        Convert.ToInt32(data["env_id"]),
                        data["env_name"].ToString()
                    ),
                    data["scenario_description"].ToString(),
                    Convert.ToInt32(data["scenario_status"])
                ),
                new Weather(
                    Convert.ToInt32(data["weather_id"]),
                    data["weather_name"].ToString(),
                    Convert.ToInt32(data["weather_info"]),
                    new WeatherType(
                        Convert.ToInt32(data["weatherType_id"]),
                        data["weatherType_name"].ToString()
                    ),
                    data["weather_description"].ToString(),
                    Convert.ToInt32(data["weather_status"])
                ),
                Convert.ToInt32(data["session_status"]),
                Convert.ToInt32(data["session_public"])
            );
            data.Close();

            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            string sql2 =
                @"select comp.component_id, comp.component_name from session_component as ses inner join
                component as comp where comp.component_id = ses.component_id and session_id = " + id;
            command.CommandText = sql2;
            data = command.ExecuteReader();
            while (data.Read())
            {
                lista.Add(new KeyValuePair<int, string>(Convert.ToInt32(data["component_id"]), data["component_name"].ToString()));
            }
            session.ListComponents = lista;
        }
        data.Close();
        
        return session;
    }

    //buscar todos nao precisa do join com stage porque nao aparece na tabela, só a psyc/pat, no search ele pega todas as infos
    public List<Session> SearchAll(string filter, bool status)
    {
        List<Session> lista = new List<Session>();
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        //pego todas as sessoes que forem publicas ou que forem privadas e minhas (com o filtro e desativadas) 
        string sql = @"select * from session inner join psychologist inner join patient where (session_public = 1 or psychologist_id = " + this.psychologist.Id + 
            ") and psychologist_id = psyc_id and patient_id = pat_id";
        if(!filter.Trim().Equals(""))
        {
            sql+=" and session_name like '%%f%'";
            sql = sql.Replace("%f",filter);
            if(!status)
                sql +=" and session_status = 1";
        }
        else if(!status)
            sql += " and session_status = 1";
        Debug.Log("sql: " + sql);
        command.CommandText = sql;
        data = command.ExecuteReader();
        while(data.Read())
        {
            lista.Add(new Session(Convert.ToInt32(data["session_id"]),
                data["session_name"].ToString(),
                data["session_description"].ToString(),
                new Psychologist(Convert.ToInt32(data["psyc_id"]),
                    data["psyc_name"].ToString(),
                    data["psyc_cpf"].ToString(),
                    data["psyc_phone"].ToString(),
                    data["psyc_email"].ToString(),
                    Convert.ToChar(data["psyc_gender"]),
                    data["psyc_password"].ToString(),
                    Convert.ToInt32(data["psyc_status"]),
                    data["psyc_crp"].ToString(),
                    Convert.ToDateTime(data["psyc_birthday"])),
                new Patient(Convert.ToInt32(data["pat_id"]),
                    data["pat_name"].ToString(),
                    data["pat_cpf"].ToString(),
                    Convert.ToDateTime(data["pat_birthday"].ToString()),
                    data["pat_phone"].ToString(),
                    data["pat_email"].ToString(),
                    data["pat_note"].ToString(),
                    Convert.ToChar(data["pat_gender"]),
                    Convert.ToInt32(data["pat_status"])
                ),
                new Scenario(Convert.ToInt32(data["scenario_id"])),
                new Weather(Convert.ToInt32(data["weather_id"])),
                Convert.ToInt32(data["session_status"]),
                Convert.ToInt32(data["session_public"])
            ));
        }
        data.Close();
        return lista;
    }

    public List<Session> SearchAllByPat(string filter) //buscar pelo paciente na hora da execução
    {
        List<Session> lista = new List<Session>();
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        //pego todas as sessoes que forem publicas ou que forem privadas e minhas
        string sql = @"select * from session inner join patient where (session_public = 1 or psychologist_id = " + this.psychologist.Id + 
            ") and patient_id = pat_id and session_status = 1";
        if(!filter.Trim().Equals(""))
        {
            sql+=" and pat_name like '%%f%'";
            sql = sql.Replace("%f",filter);
        }

        command.CommandText = sql;
        data = command.ExecuteReader();
        while(data.Read())
        {
            lista.Add(new Session(Convert.ToInt32(data["session_id"]),
                data["session_name"].ToString(),
                data["session_description"].ToString(),
                new Psychologist(Convert.ToInt32(data["psyc_id"])),
                new Patient(Convert.ToInt32(data["pat_id"]),
                    data["pat_name"].ToString(),
                    data["pat_cpf"].ToString(),
                    Convert.ToDateTime(data["pat_birthday"].ToString()),
                    data["pat_phone"].ToString(),
                    data["pat_email"].ToString(),
                    data["pat_note"].ToString(),
                    Convert.ToChar(data["pat_gender"]),
                    Convert.ToInt32(data["pat_status"])
                ),
                new Scenario(Convert.ToInt32(data["scenario_id"])),
                new Weather(Convert.ToInt32(data["weather_id"])),
                Convert.ToInt32(data["session_status"]),
                Convert.ToInt32(data["session_public"])
            ));
        }
        data.Close();
        return lista;
    }
    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public Psychologist Psychologist { get => psychologist; set => psychologist = value; }
    public Patient Patient { get => patient; set => patient = value; }
    public Scenario Scenario { get => scenario; set => scenario = value; }
    public Weather Weather { get => weather; set => weather = value; }
    public int Status { get => status; set => status = value; }
    public int IsPublic { get => isPublic; set => isPublic = value; }
    public List<KeyValuePair<int, string>> ListComponents { get => listComponents; set => listComponents = value; }
}