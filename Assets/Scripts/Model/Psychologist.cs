using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Psychologist : MonoBehaviour {

    private int id;
    private string name;
    private string cpf;
    private string phone;
    private string email;
    private char gender;
    private string password;
    private int status;
    private string crp;
    private DateTime birthday;

    public Psychologist()
    {
    }

    public Psychologist(int id)
    {
        this.id = id;
    }

    public Psychologist(string name, string cpf, string phone, string email, char gender, string password, int status, string crp, DateTime birthday)
    {
        this.name = name;
        this.cpf = cpf;
        this.phone = phone;
        this.email = email;
        this.gender = gender;
        this.password = password;
        this.status = status;
        this.crp = crp;
        this.birthday = birthday;
    }

    public Psychologist(int id, string name, string cpf, string phone, string email, char gender, string password, int status, string crp, DateTime birthday)
    {
        this.id = id;
        this.name = name;
        this.cpf = cpf;
        this.phone = phone;
        this.email = email;
        this.gender = gender;
        this.password = password;
        this.status = status;
        this.crp = crp;
        this.birthday = birthday;
    }

    public string Insert()
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        int result;
        string sql = @"insert into psychologist 
                            (psyc_name,
                            psyc_cpf,
                            psyc_email,
                            psyc_phone,
                            psyc_birthday,
                            psyc_gender,
                            psyc_crp,
                            psyc_status,
                            psyc_password)
                    values ('$n','$c','$e','$p','$b','$g','$r', $s ,'$w');";
                    
        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$c", this.cpf);
        sql = sql.Replace("$e", this.email);
        sql = sql.Replace("$p", this.phone);
        sql = sql.Replace("$b", this.birthday.Year + "-" + this.birthday.Month + "-" + this.birthday.Day);
        sql = sql.Replace("$g", this.gender + "");
        sql = sql.Replace("$r", this.crp);
        sql = sql.Replace("$s", this.status + "");
        sql = sql.Replace("$w", this.password);
        
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
        int result;
        string sql = @"update psychologist set 
                        psyc_name = '$n',
                        psyc_cpf = '$c',
                        psyc_email = '$e',
                        psyc_phone = '$p',
                        psyc_birthday = '$b',
                        psyc_gender = '$g',
                        psyc_crp = '$r',
                        psyc_status = $s,
                        psyc_password = '$w'
                        where psyc_id = " + id;

        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$c", this.cpf);
        sql = sql.Replace("$e", this.email);
        sql = sql.Replace("$p", this.phone);
        sql = sql.Replace("$b", this.birthday.Year + "-" + this.birthday.Month + "-" + this.birthday.Day);
        sql = sql.Replace("$g", this.gender + "");
        sql = sql.Replace("$r", this.crp);
        sql = sql.Replace("$s", this.status + "");
        sql = sql.Replace("$w", this.password);

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
        string sql = @"update psychologist set psyc_status = 0 where psyc_id = " + id;
        int result;

        command.CommandText = sql;
        result = command.ExecuteNonQuery();

        return result == 1;
    }

    public Psychologist Search(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        Psychologist psyc = null;
        string sql = "select * from psychologist where psyc_id = " + id;

        command.CommandText = sql;
        data = command.ExecuteReader();

        if (data.Read())
        {
            psyc = new Psychologist(
                Convert.ToInt32(data["psyc_id"]),
                data["psyc_name"].ToString(),
                data["psyc_cpf"].ToString(),
                data["psyc_phone"].ToString(),
                data["psyc_email"].ToString(),
                Convert.ToChar(data["psyc_gender"]),
                data["psyc_password"].ToString(),
                Convert.ToInt32(data["psyc_status"]),
                data["psyc_crp"].ToString(),
                Convert.ToDateTime(data["psyc_birthday"]));
        }
                
        data.Close();
        return psyc;
    }

    public List<Psychologist> SearchAll(string filter,bool status)
    {
        Debug.Log("Pesquisa, filtro: " + filter + " stat: " + status);
        List<Psychologist> list = new List<Psychologist>();
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        String sql = "select * from psychologist ";

        if(!filter.Trim().Equals(""))
        {
            sql+= "where psyc_name like '%$f%'";
            sql = sql.Replace("$f",filter);
            if(!status)
                sql +=" and psyc_status = 1";
        }
        else if(!status)
            sql +="where psyc_status = 1";
         

        Debug.Log("SQL: " + sql);
        command.CommandText = sql;
        data = command.ExecuteReader();
        while (data.Read())
        {
            list.Add(new Psychologist(Convert.ToInt32(data["psyc_id"]),
                data["psyc_name"].ToString(),
                data["psyc_cpf"].ToString(),
                data["psyc_phone"].ToString(),
                data["psyc_email"].ToString(),
                Convert.ToChar(data["psyc_gender"]),
                data["psyc_password"].ToString(),
                Convert.ToInt32(data["psyc_status"]),
                data["psyc_crp"].ToString(),
                Convert.ToDateTime(data["psyc_birthday"])));
        }
        data.Close();

        return list;
    }

    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public string Cpf { get => cpf; set => cpf = value; }
    public string Phone { get => phone; set => phone = value; }
    public string Email { get => email; set => email = value; }
    public char Gender { get => gender; set => gender = value; }
    public string Password { get => password; set => password = value; }
    public int Status { get => status; set => status = value; }
    public string Crp { get => crp; set => crp = value; }
    public DateTime Birthday { get => birthday; set => birthday = value; }
}
