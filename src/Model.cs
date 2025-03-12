using System.Windows;
using Microsoft.Data.Sqlite;

namespace ActivAndZen.Model;


public static class Settings
{
    public static readonly string DatabaseFile = "aaz.db";
    public static readonly string SqlFile = "aaz.sql";
}

public struct Client
{
    public int Id;
    public int IsActive;
    public string Name;
}

public struct Employee
{
    public int Id;
    public int IsActive;
    public int ClientId;
    public string FirstName;
    public string LastName;
    public string Email;
    public string Phone;
    public string SpecialNote;
}

public struct PastSlots
{
    public DateTime dateTime;
}

public abstract class TableQueryBase
{
    protected string TableName;

    public TableQueryBase(string tableName)
    {
        TableName = tableName;
    }

    protected int _count;
    public int Count
    {
        get => _count;
    }

    public int Exec(bool is_SELECT, string query, params SqliteParameter [] parameters) 
    {
        try {
            using (SqliteConnection connection = new SqliteConnection($"Data Source={Settings.DatabaseFile}")) 
            {
                connection.Open();

                using (SqliteCommand cmd = new SqliteCommand(query, connection)) 
                {

                    if (parameters != null && parameters.Length > 0) {
                        cmd.Parameters.AddRange(parameters);
                    }

                    if (is_SELECT)
                    {
                        Clear();
                        
                        using (SqliteDataReader dataReader = cmd.ExecuteReader()) 
                        {
                            while (dataReader.Read()) {
                                GetLine(dataReader);
                                _count++;
                            }
                        }
                        return 0;
                    }

                    return cmd.ExecuteNonQuery();
                }
            }
        }
        catch(SqliteException sqlerr) {
            MessageBox.Show($"SQLite Error: {sqlerr.Message}");
            return -1;
        }
        catch(Exception err) {
            MessageBox.Show($"General Error: {err.Message}");
            return -1;
        } 
    }

    public void SelectAll()
    {
        OnSelectAll();
        Exec(true, $"SELECT * FROM {TableName};");
    }

    protected abstract void GetLine(SqliteDataReader dataReader);
    protected abstract void Clear();

    protected abstract void OnSelectAll();
}

// -------------------------------------
// Classes Generated with Powershell
// --------------------------------------

public class Clients : TableQueryBase
{
    private List<int>? Ids;
    private List<int>? IsActives;
    private List<string>? Names;

    public Clients() : base("clients") {}


    public string GetNameFromId(int id)
    {
        Names = new();
        Exec(true, $"SELECT * FROM {TableName} WHERE id = {id}");
        return Names[0];
    }

    protected override void OnSelectAll()
    {
        Ids = new();
        IsActives = new();
        Names = new();
    }

    public ActivAndZen.Model.Client this[int index]
    {
        get
        {
            if (index > _count || index < 0)
                throw new IndexOutOfRangeException("Index invalide.");
            return new ActivAndZen.Model.Client {
                Id = Ids != null ? Ids[index] : -1,
                IsActive = IsActives != null ? IsActives[index] : -1,
                Name = Names != null ? Names[index] : ""
            };
        }
    }

    protected override void GetLine(SqliteDataReader dataReader)
    {
        if (Ids != null) Ids.Add(dataReader.GetInt32(0));
        if (IsActives != null) IsActives.Add(dataReader.GetInt32(1));
        if (Names != null) Names.Add(dataReader.GetString(2));
    }

    protected override void Clear()
    {
        if (Ids != null) Ids.Clear();
        if (IsActives != null) IsActives.Clear();
        if (Names != null) Names.Clear();
    }
}

public class Employees : TableQueryBase
{
    private List<int>? Ids;
    private List<int>? IsActives;
    private List<int>? ClientIds;
    private List<string>? FirstNames;
    private List<string>? LastNames;
    private List<string>? Emails;
    private List<string>? Phones;
    private List<string>? SpecialNotes;

    public Employees() : base("employees") {}

    public void SelectPossibles(string substr) {
        Ids = new();
        IsActives = new();
        ClientIds = new();
        FirstNames = new();
        LastNames = new();
        
        string firstName = substr;
        string lastName = substr;

        int space_index = substr.IndexOf(' ');
        if (space_index > 0) {
            firstName = substr.Substring(0, space_index);
            lastName = substr.Substring(space_index);
        }

        Exec(true, $"SELECT * FROM {TableName} WHERE LOWER(first_name) LIKE '%{firstName}%' OR LOWER(last_name) LIKE '%{lastName}%'");
    }

    protected override void OnSelectAll() 
    {
        Ids = new();
        IsActives = new();
        ClientIds = new();
        FirstNames = new();
        LastNames = new();
        Emails = new();
        Phones = new();
        SpecialNotes = new();
    }

    public ActivAndZen.Model.Employee this[int index]
    {
        get
        {
            if (index > _count || index < 0)
                throw new IndexOutOfRangeException("Index invalide.");
            return new ActivAndZen.Model.Employee {
                Id = Ids != null ? Ids[index] : -1,
                IsActive = IsActives != null ? IsActives[index] : -1,
                ClientId = ClientIds != null ? ClientIds[index] : -1,
                FirstName = FirstNames != null ? FirstNames[index] : "",
                LastName = LastNames != null ? LastNames[index] : "",
                Email = Emails != null ? Emails[index] : "",
                Phone = Phones != null ? Phones[index] : "",
                SpecialNote = SpecialNotes != null ? SpecialNotes[index] : ""
            };
        }
    }

    protected override void GetLine(SqliteDataReader dataReader)
    {
        if (Ids != null) Ids.Add(dataReader.GetInt32(0));
        if (IsActives != null) IsActives.Add(dataReader.GetInt32(1));
        if (ClientIds != null) ClientIds.Add(dataReader.GetInt32(2));
        if (FirstNames != null) FirstNames.Add(dataReader.GetString(3));
        if (LastNames != null) LastNames.Add(dataReader.GetString(4));
        if (Emails != null) Emails.Add(dataReader.GetString(5));
        if (Phones != null) Phones.Add(dataReader.GetString(6));
        if (SpecialNotes != null) SpecialNotes.Add(dataReader.GetString(7));
    }

    protected override void Clear()
    {
        if (Ids != null) Ids.Clear();
        if (IsActives != null) IsActives.Clear();
        if (ClientIds != null) ClientIds.Clear();
        if (FirstNames != null) FirstNames.Clear();
        if (LastNames != null) LastNames.Clear();
        if (Emails != null) Emails.Clear();
        if (Phones != null) Phones.Clear();
        if (SpecialNotes != null) SpecialNotes.Clear();
    }
}
