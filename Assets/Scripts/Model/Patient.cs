using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patient : MonoBehaviour {

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
