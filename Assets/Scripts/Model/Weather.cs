using System;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weather : MonoBehaviour
{
    private int id;
    private string name;
    private int info;
    private WeatherType type;
    private string description;
    private int status;

    public Weather()
    {

    }

    public Weather(int id)
    {
        this.id = id;
    }

    public Weather(string name, int info, WeatherType type, string description, int status)
    {
        this.name = name;
        this.info = info;
        this.type = type;
        this.description = description;
        this.status = status;
    }

    public Weather(int id, string name, int info, WeatherType type, string description, int status)
    {
        this.id = id;
        this.name = name;
        this.info = info;
        this.type = type;
        this.description = description;
        this.status = status;
    }

    public string Insert()
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        int result;
        string sql = @"insert into weather 
                            (weather_name,
                            weather_description,
                            weatherType_id,
                            weather_info,
                            weather_status)
                    values ('$n','$d',$t,$i,$s);";
                    
        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$d", this.description);
        sql = sql.Replace("$t", this.type.Id + "");
        sql = sql.Replace("$i", this.info + "");
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
        string sql = @"update weather set
                        weather_name = '$n',
                        weather_description = '$d',
                        weatherType_id = $t,
                        weather_info = $i,
                        weather_status = $s
                        where weather_id = " + id;
        
        sql = sql.Replace("$n", this.name);
        sql = sql.Replace("$d", this.description);
        sql = sql.Replace("$t", this.type.Id + "");
        sql = sql.Replace("$i", this.info + "");
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
        string sql = @"update weather set weather_status = 0 where weather_id = " + id;
        int result;

        command.CommandText = sql;
        result = command.ExecuteNonQuery();

        return result == 1;
    }

    public Weather Search(int id)
    {
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        Weather weather = null;
        string sql = @"select * from weather as wea inner join weatherType as type
                     where  wea.weatherType_id = type.weatherType_id and wea.weather_id = " + id;

        command.CommandText = sql;
        data = command.ExecuteReader();

        if (data.Read())
        {
            weather = new Weather(
                Convert.ToInt32(data["weather_id"]),
                data["weather_name"].ToString(),
                Convert.ToInt32(data["weather_info"]),
                new WeatherType(
                    Convert.ToInt32(data["weatherType_id"]),
                    data["weatherType_name"].ToString()
                ),
                data["weather_description"].ToString(),
                Convert.ToInt32(data["weather_status"])
            );
        }
                
        data.Close();
        return weather;
    }

    public List<Weather> SearchAll(string filter, bool status)
    {
        List<Weather> list = new List<Weather>();
        MySqlCommand command = GameManager.instance.Con.CreateCommand();
        MySqlDataReader data;
        String sql = @"select * from weather as wea inner join weatherType as type
                    where wea.weatherType_id = type.weatherType_id ";

        if(!filter.Trim().Equals(""))
        {
            sql+= " and weather_name like '%$f%'";
            sql = sql.Replace("$f",filter);
            if(!status)
                sql +=" and weather_status = 1";
        }
        else if(!status)
            sql +=" and weather_status = 1";
         

        Debug.Log("SQL: " + sql);
        command.CommandText = sql;
        data = command.ExecuteReader();
        while (data.Read())
        {
            list.Add(new Weather(
                Convert.ToInt32(data["weather_id"]),
                data["weather_name"].ToString(),
                Convert.ToInt32(data["weather_info"]),
                new WeatherType(
                    Convert.ToInt32(data["weatherType_id"]),
                    data["weatherType_name"].ToString()
                ),
                data["weather_description"].ToString(),
                Convert.ToInt32(data["weather_status"])
            ));
        }
        data.Close();

        return list;
    }

    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public int Info { get => info; set => info = value; }
    public WeatherType Type { get => type; set => type = value; }
    public string Description { get => description; set => description = value; }
    public int Status { get => status; set => status = value; }
}
