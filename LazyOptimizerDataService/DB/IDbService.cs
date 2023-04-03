using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizerDataService.DB
{
    public interface IDbService : IDisposable
    {
        void Select<T>(IList<T> destination, string sqlRequest, IEnumerable<object> parameters = null);
        DbDataReader Select(string sqlRequest, IEnumerable<object> parameters = null);
        void SelectSingle<T>(out T destination, string sqlRequest, IEnumerable<object> parameters = null);
        int Execute(string sqlRequest, IEnumerable<object> parameters = null);
        object GetValue(string sqlRequest, IEnumerable<object> parameters = null);
        int SetValue(string tableName, long rowId, string fieldName, object value);
        int LastInsertedRowId();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        bool Connected { get; set; }
    }
}
