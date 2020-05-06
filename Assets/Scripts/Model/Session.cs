using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Session : MonoBehaviour {
    int id;
    string name;
    string description;
    Psychologist psychologist;
    Stage stage;
    Patient patient;
    int status;
    int isPublic;

    public Session()
    {
    }
    public Session(Psychologist psychologist)
    {
        this.psychologist = psychologist;
    }

    public Session(string name, string description, Psychologist psychologist, Stage stage, Patient patient, int status, int isPublic)
    {
        this.name = name;
        this.description = description;
        this.psychologist = psychologist;
        this.stage = stage;
        this.patient = patient;
        this.status = status;
        this.isPublic = isPublic;
    }

    public Session(int id, string name, string description, Psychologist psychologist, Stage stage, Patient patient, int status, int isPublic)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.psychologist = psychologist;
        this.stage = stage;
        this.patient = patient;
        this.status = status;
        this.isPublic = isPublic;
    }

    public string Insert()
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        int result;
        string sql = @"insert into session 
                            (psychologist_id,
                            patient_id,
                            stage_id,
                            session_name,
                            session_description,
                            session_status,
                            session_public)
                    values ($ps,$pt,$stage,'$n','$d',$status,$ip);";

        sql = sql.Replace("$ps", this.psychologist.Id + "");
        sql = sql.Replace("$pt", this.patient.Id + "");
        sql = sql.Replace("$stage", this.stage.Id + "");
        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$d", this.description);
        sql = sql.Replace("$status", this.status + "");
        sql = sql.Replace("$ip", this.isPublic + "");

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
                        psychologist_id = $ps,
                        patient_id = $pt,
                        stage_id = $stage,
                        session_name = '$n',
                        session_description = '$d',
                        session_status = $status,
                        session_public = $ip
                        where session_id = " + id;

        sql = sql.Replace("$ps", this.psychologist.Id + "");
        sql = sql.Replace("$pt", this.patient.Id + "");
        sql = sql.Replace("$stage", this.stage.Id + "");
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

    public Session Search(int id) //nao busca tudo do stage porque tem scenario, weather
    {
        Session session = null;
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        string sql = @"select * from session as ses inner join psychologist inner join stage as sta inner join patient where psychologist_id = psyc_id 
            and ses.stage_id = sta.stage_id and patient_id = pat_id and session_id = " + id;

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
                new Stage(Convert.ToInt32(data["stage_id"]),
                    data["stage_name"].ToString(),
                    data["stage_description"].ToString()
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
                Convert.ToInt32(data["session_status"]),
                Convert.ToInt32(data["session_public"])
            );
        }
        data.Close();
        return session;
    }

    //buscar todos nao precisa do join com stage/pat porque nao aparece na tabela, só a psyc, no search ele pega todas as infos
    public List<Session> SearchAll(string filter, bool status)
    {
        Debug.Log("buscando: "+ this.psychologist.Id);
        List<Session> lista = new List<Session>();
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        //pego todas as sessoes que forem publicas ou que forem privadas e minhas (com o filtro e desativadas) 
        string sql = @"select * from session inner join psychologist where (session_public = 1 or psychologist_id = " + this.psychologist.Id + 
            ") and psychologist_id = psyc_id";
        if(!filter.Trim().Equals(""))
        {
            sql+=" and session_name like '%f%'";
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
                new Stage(Convert.ToInt32(data["stage_id"])),
                new Patient(Convert.ToInt32(data["patient_id"])),
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
    public Stage Stage { get => stage; set => stage = value; }
    public Patient Patient { get => patient; set => patient = value; }
    public int Status { get => status; set => status = value; }
    public int IsPublic { get => isPublic; set => isPublic = value; }
}