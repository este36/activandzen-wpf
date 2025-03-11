using System;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Data.Sqlite;

namespace ActivAndZen.Model;

public static class Model
{
    private static readonly string Settings = "Data Source=aaz.db";

    public static List<T> Ask<T>(string query, Action<List<T>, SqliteDataReader> getline)
    {
        List<T> values = new List<T>();
        
        try {
            using (SqliteConnection connection = new SqliteConnection(Settings)) {
                connection.Open();

                using (SqliteCommand cmd = new SqliteCommand(query, connection)) {
                    using (SqliteDataReader dataReader = cmd.ExecuteReader()) {
                        while (dataReader.Read()) {
                            getline(values, dataReader);
                        }
                    }
                }
            }
            return values;
        }
        catch(SqliteException sqlerr) {
            MessageBox.Show($"SQLite Error: {sqlerr.Message}");
            return new List<T>();
        }
        catch(Exception err) {
            MessageBox.Show($"General Error: {err.Message}");
            return new List<T>();
        }
    }
}