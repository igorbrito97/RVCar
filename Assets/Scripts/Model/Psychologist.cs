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
    private string gender;
    private string password;
    private bool status;
    private string crp;
    private DateTime birthday;

    public Psychologist()
    {
    }

    public Psychologist(int id, string name, string cpf, string phone, string email, string gender, string password, bool status, string crp, DateTime birthday)
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

    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public string Cpf { get => cpf; set => cpf = value; }
    public string Phone { get => phone; set => phone = value; }
    public string Email { get => email; set => email = value; }
    public string Gender { get => gender; set => gender = value; }
    public string Password { get => password; set => password = value; }
    public bool Status { get => status; set => status = value; }
    public string Crp { get => crp; set => crp = value; }
    public DateTime Birthday { get => birthday; set => birthday = value; }
}
