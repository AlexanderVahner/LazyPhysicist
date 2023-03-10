using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.DB
{
    public class DBRecord
    {
        public long? rowid;

        private SQLiteService dbService;
        public DBRecord(SQLiteService dbService, string tableName)
        {
            this.dbService = dbService;
            TableName = tableName;
        }
        public int SetValue(string fieldName, object value)
        {
            return rowid != null ? dbService.SetRecordValue(TableName, (int)rowid, fieldName, value) : 0;
        }
        public string TableName { get; protected set; }

        
    }
}
