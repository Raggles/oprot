using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oprot.database.core
{
    public class DatabaseManager
    {
        private static readonly DatabaseManager _singleton = new DatabaseManager();
        public static DatabaseManager Singleton { get { return _singleton; } }

        private DatabaseContext _db = new DatabaseContext();

        /// <summary>
        /// Destroy the current context without saving changes, and create a new one
        /// </summary>
        public void RefreshDbContext()
        {
            _db = new DatabaseContext();
        }

       
        public void SaveChanges()
        {
            _db.SaveChanges();
        }
    }
}
