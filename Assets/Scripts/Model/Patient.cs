using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patient : MonoBehaviour
{

    private int id;
    private string name;
    private string cpf;
    private DateTime birthday;
    private string phone;
    private string email;
    private string note;
    private char gender;
    private bool status;

    public Patient()
    {
    }

    public Patient(string name, string cpf, DateTime birthday, string phone, string email, string note, char gender, bool status)
    {
        this.name = name;
        this.cpf = cpf;
        this.birthday = birthday;
        this.phone = phone;
        this.email = email;
        this.note = note;
        this.gender = gender;
        this.status = status;
    }

    public Patient(int id, string name, string cpf, DateTime birthday, string phone, string email, string note, char gender, bool status)
    {
        this.id = id;
        this.name = name;
        this.cpf = cpf; 
        this.birthday = birthday;
        this.phone = phone;
        this.email = email;
        this.note = note;
        this.gender = gender;
        this.status = status;
    }

    public string Insert()
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        int result;
        string sql = @"insert into patient 
                            (pat_name,
                            pat_cpf,
                            pat_email,
                            pat_phone,
                            pat_birthday,
                            pat_gender,
                            pat_note,
                            pat_status)
                    values ('$n','$c','$e','$p','$b','$g','$o', $s);";
        
        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$c", this.cpf);
        sql = sql.Replace("$e", this.email);
        sql = sql.Replace("$p", this.phone);
        sql = sql.Replace("$b", this.birthday + "");
        sql = sql.Replace("$g", this.gender + "");
        sql = sql.Replace("$r", this.note);
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
        MySqlDataReader data;
        int result;
        string sql = @"update patient set 
                        (pat_name = '$n',
                        pat_cpf = '$c',
                        pat_email = '$e',
                        pat_phone = '$p',
                        pat_birthday = '$b',
                        pat_gender = '$g',
                        pat_note = '$o',
                        pat_status = $s)
                        where pat_id = " + id;

        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$c", this.cpf);
        sql = sql.Replace("$e", this.email);
        sql = sql.Replace("$p", this.phone);
        sql = sql.Replace("$b", this.birthday + "");
        sql = sql.Replace("$g", this.gender + "");
        sql = sql.Replace("$r", this.note);
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

    public bool Delete(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        string sql = @"update patient set pat_status = false where pat_id = " + id;
        int result;

        command.CommandText = sql;
        result = command.ExecuteNonQuery();
        
        return result == 1;
    }

    public Patient Search(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        Patient patient = null;
        string sql = "select * from patient where pat_id = " + id;

        command.CommandText = sql;
        data = command.ExecuteReader();

        if (data.Read())
        {
            patient = new Patient(
                Convert.ToInt32(data["pat_id"]),
                data["pat_name"].ToString(),
                data["pat_cpf"].ToString(),
                Convert.ToDateTime(data["pat_birthday"].ToString()),
                data["pat_phone"].ToString(),
                data["pat_email"].ToString(),
                data["pat_note"].ToString(),
                Convert.ToChar(data["pat_gender"]),
                Convert.ToBoolean(data["pat_status"]));
        }
        data.Close();
        return patient;
    }

    public List<Patient> SearchAll()
    {
        return null;
    }

    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public string Cpf { get => cpf; set => cpf = value; }
    public DateTime Birthday { get => birthday; set => birthday = value; }
    public string Phone { get => phone; set => phone = value; }
    public string Email { get => email; set => email = value; }
    public string Note { get => note; set => note = value; }
    public char Gender { get => gender; set => gender = value; }
    public bool Status { get => status; set => status = value; }
}
