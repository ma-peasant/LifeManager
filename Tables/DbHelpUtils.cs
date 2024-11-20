using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeManager.Tables
{
    public class DbHelpUtils
    {
        const string dbName = "LifeManager.db";
        static readonly string databasePath = System.IO.Path.Combine(Environment.CurrentDirectory, dbName);
        static readonly SQLiteAsyncConnection db = new SQLiteAsyncConnection(databasePath);
        /// <summary>
        /// 创建数据库
        /// </summary>
        public static async Task InitDbAsync()
        {
            // Get an absolute path to the database file
            //表可以新增字段， 修改和删除字段没有用
            //await db.CreateTableAsync<Record>();
            //await db.CreateTableAsync<ConsumeType>();
            await db.CreateTableAsync<PayItem>();
            await db.CreateTableAsync<ToDoItem>();
        }

        public static SQLiteAsyncConnection GetSQLiteAsyncConnection()
        {
            return db;
        }

        public static async Task AddRecordAsync<T>(T table)
        {
            await db.InsertAsync(table);
        }

        public static async Task DeleteRecordAsync<T>(T table)
        {
            await db.DeleteAsync(table);
        }

        public static async Task UpdateRecordAsync<T>(T table)
        {
            await db.UpdateAsync(table);
        }

        //where T:new() ,  约束来确保类型T有一个无参数的构造函数，这是SQLite-net的要求。
        public static async Task<List<T>> QueryRecordAsync<T>() where T : new()
        {
            return await db.Table<T>().ToListAsync();
        }


        public static async Task<List<T>> ExecuteSqlAsync<T>(string sql) where T : new()
        {
            List<T> result = await db.QueryAsync<T>(sql);
            return result;
        }
    }
}
