using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MySql.Data.MySqlClient;
using System;

public class LoginController : MonoBehaviour {

    private MySqlConnection connection;
    private string sql;

    [SerializeField] InputField inputFieldUsername;
    [SerializeField] InputField inputFieldPassword;
    [SerializeField] Text errorText;

	void Start () {
        inputFieldPassword.inputType = InputField.InputType.Password;
        errorText.gameObject.SetActive(false);
	}

    public void SignIn()
    {
        MySqlCommand command;
        MySqlDataReader data;
        Psychologist psyc = new Psychologist();
        string login = inputFieldUsername.text;
        string password = inputFieldPassword.text;
       

        if (login.Trim().Length < 3 || password.Trim().Length < 4)
        {
            errorText.gameObject.SetActive(true);
            return;
        }

        sql = @"select * from psychologist where psyc_email = '$a' and psyc_password = '$b'";

        sql = sql.Replace("$a", login);
        sql = sql.Replace("$b", password);
        try
        {
            DatabaseController db = new DatabaseController();
            db.Connect();
            connection = db.GetConnection();
            connection.Open();
            command = connection.CreateCommand();
            command.CommandText = sql;
            data = command.ExecuteReader();
            if (data.Read())
            {
                psyc = new Psychologist(Convert.ToInt32(data["psyc_id"]),
                    data["psyc_name"].ToString(),
                    data["psyc_cpf"].ToString(),
                    data["psyc_rg"].ToString(),
                    data["psyc_phone"].ToString(),
                    data["psyc_email"].ToString(),
                    data["psyc_gender"].ToString(),
                    data["psyc_password"].ToString(),
                    Convert.ToDateTime(data["psyc_birthday"].ToString()));

                data.Close();
                SceneManager.LoadScene("Scenario1");
            }
            else
            {
                data.Close();
                connection.Close();
                errorText.gameObject.SetActive(true);
                return;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void LoadManager()
    {// abre form de cadastro
        System.Diagnostics.Process.Start("D:/Igor/RProjeto/RVCarManager/bin/Debug/RVCarManager.exe");
    }

    public void CloseApplication()
    {
        Application.Quit();
    }
}
