using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace LifeManager.Tables
{
    public abstract class Table : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        [SQLite.Column("id")]
        public int Id { get; set; }

    }
}
