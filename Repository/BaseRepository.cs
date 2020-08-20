using System.Data;
using System.Data.SQLite;
using Dapper;

namespace CartRepository
{
    public interface IBaseRepository
    {
        IDbConnection GetConnection();
    }

    public class BaseRepository : IBaseRepository
    {
        protected readonly string _connectionString;

        public BaseRepository(string conn)
        {
            _connectionString = conn;
        }

        public IDbConnection GetConnection()
        {
            var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            return conn;
        }

        public void CreateDatabase()
        {
            using var db = GetConnection();
            db.Execute(
                @"create table Cart
                      (
                         Id                                 integer primary key AUTOINCREMENT,
                         IsDeleted                           boolean not null,
                         LastUpdated                         datetime not null
                      )");
            db.Execute(
                @"create table CartProduct
                      (
                         Id                                 integer primary key AUTOINCREMENT,
                         CartId                           integer not null,
                         ProductId                           integer not null,
                         Count                           integer not null,
                         Cost                           float not null,
                         IsBonusPoints                           boolean not null,
                         FOREIGN KEY(CartId) REFERENCES Cart(Id)
                      )");
            db.Execute(
                @"create table CartHook
                      (
                         Id                                 integer primary key AUTOINCREMENT,
                         CartId                           integer not null,
                         Url                         varchar(255) not null,
                         FOREIGN KEY(CartId) REFERENCES Cart(Id)
                      )");
        }
    }
}