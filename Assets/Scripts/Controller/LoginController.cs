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
        inputFieldUsername.characterLimit = 60;
        inputFieldPassword.characterLimit = 45;

        inputFieldPassword.inputType = InputField.InputType.Password;
        errorText.gameObject.SetActive(false);

        
        inputFieldUsername.text = "";
        inputFieldPassword.text = "";
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
        Debug.Log("SQL: " + sql);
        try
        {
            GameObject database = new GameObject();    
            DatabaseController db = database.AddComponent<DatabaseController>();
            db.Connect();
            connection = db.GetConnection();
            Debug.Log("conecção: ");
            connection.Open();
            Debug.Log("abriuuuuuu");
            command = connection.CreateCommand();
            command.Connection = connection;
            command.CommandText = sql;
            
            Debug.Log("vamo le: " + sql);
            data = command.ExecuteReader();
            if (data.Read())
            {
                
                Debug.Log("leeeeeeeeeeeeeeu:");   
                psyc = new Psychologist(Convert.ToInt32(data["psyc_id"]),
                data["psyc_name"].ToString(),
                data["psyc_cpf"].ToString(),
                data["psyc_phone"].ToString(),
                data["psyc_email"].ToString(),
                Convert.ToChar(data["psyc_gender"]),
                data["psyc_password"].ToString(),
                Convert.ToInt32(data["psyc_status"]),
                data["psyc_crp"].ToString(),
                Convert.ToDateTime(data["psyc_birthday"]));

                data.Close();
                SceneManager.LoadScene("Menu");
            }
            else
            {
                data.Close();
                connection.Close();
                errorText.text = "Erro ao conectar no banco!";
                errorText.gameObject.SetActive(true);
                return;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    public void CloseApplication()
    {
        Application.Quit();
    }
}
