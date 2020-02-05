using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Psychologist : MonoBehaviour {

    private int id;
    private string name;
    private string cpf;
    private string rg;
    private string phone;
    private string email;
    private string gender;
    private string password;
    private DateTime birthday;

    public Psychologist()
    {
    }

    public Psychologist(int id, string name, string cpf, string rg, string phone, string email, string gender, string password, DateTime birthday)
    {
        this.Id = id;
        this.Name = name;
        this.Cpf = cpf;
        this.Rg = rg;
        this.Phone = phone;
        this.Email = email;
        this.Gender = gender;
        this.Password = password;
        this.Birthday = birthday;
    }

    public int Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
        }
    }

    public string Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
        }
    }

    public string Cpf
    {
        get
        {
            return cpf;
        }

        set
        {
            cpf = value;
        }
    }

    public string Rg
    {
        get
        {
            return rg;
        }

        set
        {
            rg = value;
        }
    }

    public string Phone
    {
        get
        {
            return phone;
        }

        set
        {
            phone = value;
        }
    }

    public string Email
    {
        get
        {
            return email;
        }

        set
        {
            email = value;
        }
    }

    public string Gender
    {
        get
        {
            return gender;
        }

        set
        {
            gender = value;
        }
    }

    public string Password
    {
        get
        {
            return password;
        }

        set
        {
            password = value;
        }
    }

    public DateTime Birthday
    {
        get
        {
            return birthday;
        }

        set
        {
            birthday = value;
        }
    }
}
