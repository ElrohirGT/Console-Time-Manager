using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;
using Dapper;
using Console_Time_Manager.Models;
using System.Runtime.InteropServices;

namespace Console_Time_Manager
{
    public class SQLiteAccesLayer
    {
        public static string ConnectionString
        {
            get {
                string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "TimeManagerDatabase.db");
                return $"Data Source='{path}'";
            }
        }
        public static List<T> Get<T>(string sql, DynamicParameters param = null)
        {
            SQLiteConnection cnn = new SQLiteConnection(ConnectionString);
            cnn.Open();
            sql ??= "";
            var output = cnn.Query<T>(sql, param ?? new DynamicParameters());
            cnn.Close();
            return output.AsList();
        }
        public static Tuple<int,long> Query<T>(string sql, T data)
        {
            SQLiteConnection cnn = new SQLiteConnection(ConnectionString);
            cnn.Open();
            int output = cnn.Execute(sql, data);
            Tuple<int, long> result = new Tuple<int, long>(output, cnn.LastInsertRowId);
            cnn.Close();
            return result;
        }
    }
}
