using System;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Data.Sqlite;

namespace ActivAndZen.Model;

// Queries
// GetAllEmployees
// GetPossibleEmployees
// GetEmployeeFromId
// GetClientNameFromId

public static class Settings
{
    public static readonly string DatabaseFile = "aaz.db";
    public static readonly string SqlFile = "aaz.sql";
}

public class Clients
{
    public int Count = 0;
    public List<int> id = new();
    public List<bool> is_active = new();
    public List<string> name = new();
}

public class Employee
{
    public int id = 0;
    public bool is_active = false;
    public int client_id = 0;
    public string first_name = String.Empty;
    public string last_name = String.Empty;
    public string email = String.Empty;
    public string phone = String.Empty;
    public string special_notes = String.Empty;
}

public class Employees
{
    public int Count = 0;
    public List<int> id = new();
    public List<bool> is_active = new();
    public List<int> client_id = new();
    public List<string> first_name = new();
    public List<string> last_name = new();

    /// <summary>
    /// Sort by first occurence of filter in the full name.
    /// </summary>
    public void SortOrderByFilter(string filter)
    {
        if (string.IsNullOrEmpty(filter) || this.Count == 0) return;

        int[] indices = new int[this.Count];
        int[] sortKeys = new int[this.Count];

        for (int i = 0; i < this.Count; i++)
        {
            string full_name = $"{this.first_name[i]} {this.last_name[i]}";
            sortKeys[i] = full_name.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase);
            indices[i] = i;
        }

        Array.Sort(sortKeys, indices);

        Span<int> idSpan = CollectionsMarshal.AsSpan(id);
        Span<bool> isActiveSpan = CollectionsMarshal.AsSpan(is_active);
        Span<int> clientIdSpan = CollectionsMarshal.AsSpan(client_id);
        Span<string> firstNameSpan = CollectionsMarshal.AsSpan(first_name);
        Span<string> lastNameSpan = CollectionsMarshal.AsSpan(last_name);

        bool[] visited = new bool[this.Count];

        for (int i = 0; i < this.Count; i++)
        {
            if (visited[i]) continue;

            int current = i;
            var tempId = idSpan[i];
            var tempActive = isActiveSpan[i];
            var tempClient = clientIdSpan[i];
            var tempFirst = firstNameSpan[i];
            var tempLast = lastNameSpan[i];

            while (!visited[current])
            {
                int next = indices[current];
                visited[current] = true;

                if (next == i) break; // Fin de la boucle de permutation

                idSpan[current] = idSpan[next];
                isActiveSpan[current] = isActiveSpan[next];
                clientIdSpan[current] = clientIdSpan[next];
                firstNameSpan[current] = firstNameSpan[next];
                lastNameSpan[current] = lastNameSpan[next];

                current = next;
            }

            idSpan[current] = tempId;
            isActiveSpan[current] = tempActive;
            clientIdSpan[current] = tempClient;
            firstNameSpan[current] = tempFirst;
            lastNameSpan[current] = tempLast;
        }
    }
}

public static class Queries
{
    
    public static void Compile()
    {
        Employees a = GetPossibleEmployees("!null!");
        string b = GetClientNameFromId(1);
    }
    /// <summary>
    /// Get basic infos about every employees
    /// </summary>
    public static Employees GetAllEmployees()
    {
        string query = "SELECT id, is_active, client_id, first_name, last_name FROM employees";
        using (var connection = new SqliteConnection($"Data Source={Settings.DatabaseFile}"))
        {
            connection.Open();
            using (var cmd = new SqliteCommand(query, connection))
            using (var reader = cmd.ExecuteReader())
            {
                Employees result = new();
                while (reader.Read()) 
                {
                    result.id.Add(reader.GetInt32(0));
                    result.is_active.Add(reader.GetBoolean(1));
                    result.client_id.Add(reader.GetInt32(2));
                    result.first_name.Add(reader.GetString(3));
                    result.last_name.Add(reader.GetString(4));
                }
                result.Count = result.id.Count;
                return result;
            }
        }
    }

    public static Employees GetPossibleEmployees(string filter)
    {
        if (string.IsNullOrWhiteSpace(filter)) return new Employees();

        filter = filter.ToLower();

        string first = $"%{filter}%";
        string last = $"%{filter}%";

        int space_index = filter.IndexOf(' ');

        StringBuilder query = new("SELECT id, is_active, client_id, first_name, last_name FROM employees WHERE LOWER(first_name)");

        bool should_cmp_fullName = true;

        if (space_index > 0) {
            first = filter.Substring(0, space_index).Trim();
            last = filter.Substring(space_index + 1).Trim();

            if (last.Length == 0) {
                should_cmp_fullName = false;
                query.Append(" = @first");
            } else {
                last += "%";
                query.Append(" = @first AND LOWER(last_name) LIKE @last");
            }
        } else {
            query.Append(" LIKE @first OR LOWER(last_name) LIKE @last");
        }
        
        query.Append(';');

        using var connection = new SqliteConnection($"Data Source={Settings.DatabaseFile}");
        connection.Open();

        using var cmd = new SqliteCommand(query.ToString(), connection);
        cmd.Parameters.AddWithValue("@first", first);
        if (should_cmp_fullName) cmd.Parameters.AddWithValue("@last", last);

        using var reader = cmd.ExecuteReader();

        Employees result = new();
        while (reader.Read()) 
        {
            result.id.Add(reader.GetInt32(0));
            result.is_active.Add(reader.GetBoolean(1));
            result.client_id.Add(reader.GetInt32(2));
            result.first_name.Add(reader.GetString(3));
            result.last_name.Add(reader.GetString(4));
        }
        result.Count = result.id.Count;

        return result;
    }


    public static Employee GetEmployeeFromId(int id)
    {
        string query = $"SELECT * FROM employees WHERE id = {id}";
        using (var connection = new SqliteConnection($"Data Source={Settings.DatabaseFile}"))
        {
            connection.Open();
            using (var cmd = new SqliteCommand(query, connection))
            using (var reader = cmd.ExecuteReader())
            {
                Employee result = new();
                while (reader.Read()) 
                {
                    result.id = reader.GetInt32(0);
                    result.is_active = reader.GetBoolean(1);
                    result.client_id = reader.GetInt32(2);
                    result.first_name = reader.GetString(3);
                    result.last_name = reader.GetString(4);
                    result.email = reader.GetString(5);
                    result.phone = reader.GetString(6);
                    result.special_notes = reader.GetString(7);
                }
                return result;
            }
        }
    }

    public static string GetClientNameFromId(int client_id)
    {
        string query = $"SELECT name FROM clients WHERE id = {client_id}";
        using (var connection = new SqliteConnection($"Data Source={Settings.DatabaseFile}"))
        {
            connection.Open();
            using (var cmd = new SqliteCommand(query, connection))
            using (var reader = cmd.ExecuteReader())
            {
                string result = "";
                while (reader.Read()) 
                {
                    result = reader.GetString(0);
                }
                return result;
            }
        }
    }
}
