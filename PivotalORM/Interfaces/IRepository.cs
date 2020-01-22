using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PivotalORM
{
    public interface IRepository
    {
        T Get<T>(byte[] id);
        IEnumerable<T> Get<T>(string queryName, params object[] parameters);
        IEnumerable<T> GetBySql<T>(string wherePredicate);
        IEnumerable<T> GetBySql<T>(string wherePredicate, int top);
        T GetSingleItem<T>(string queryName, params object[] parameters);
        T GetSingleOrDefault<T>(string queryName, params object[] parameters);
        void Insert(object obj);
        void InsertRange(IEnumerable objects);
        void Update(object obj);
        IEnumerable<T> GetAll<T>();
        void Delele<T>(T obj);
    }
}
