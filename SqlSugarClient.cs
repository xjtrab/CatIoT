using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CAT_IoT
{
    public class SqlSugarDBClient
    {
        /// <summary>
        /// Create SqlSugarClient
        /// </summary>
        /// <returns></returns>
        private SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = System.Environment.GetEnvironmentVariable($"SQLCONNECTIONSTRING"),
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            }); ;
            //Print sql
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };
            return db;
        }

        public class roboticarm
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int id { get; set; }
            public string cdid { get; set; }
            public int robotrunningstatus { get; set; }
            public DateTime EventEnqueuedUtcTime { get; set; }
        }

        public class machiningcenter_mcd
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int id { get; set; }
            public string ConnectionDeviceId { get; set; }
            public DateTime EventEnqueuedUtcTime { get; set; }
            public int mcdrunningstatus { get; set; }
        }

        public class machiningcenter_pama
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int id { get; set; }
            public string ConnectionDeviceId { get; set; }
            public DateTime EventEnqueuedUtcTime { get; set; }
            public int pamarunningstatus { get; set; }
        }

        public class statusall
        {
            /// <summary>
            /// ConnectionDeviceId
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public string id { get; set; }

            /// <summary>
            /// 1,机械臂  2,MCD 3,pama
            /// </summary>
            public int type { get; set; }
            /// <summary>
            /// ref=MachiningType.cs
            /// </summary>
            public int status { get; set; }

            /// <summary>
            /// 最后一次上传的时间
            /// </summary>
            public DateTime lasttime { get; set; }
        }

        public List<roboticarm> GetRoboticarmList()
        {
            var db = GetInstance();
            var list = db.Queryable<roboticarm>()
                .PartitionBy(x => x.cdid)
                .Take(1).OrderBy(st => st.id, OrderByType.Desc)
                .ToList();
            return list;
        }

        public List<machiningcenter_mcd> GetmcdList()
        {
            var db = GetInstance();
            var list = db.Queryable<machiningcenter_mcd>()
                .PartitionBy(x => x.ConnectionDeviceId)
                .Take(1).OrderBy(st => st.id, OrderByType.Desc)
                .ToList();
            return list;
        }

        public List<machiningcenter_pama> GetpamaList()
        {
            var db = GetInstance();
            var list = db.Queryable<machiningcenter_pama>()
                .PartitionBy(x => x.ConnectionDeviceId)
                .Take(1).OrderBy(st => st.id, OrderByType.Desc)
                .ToList();
            return list;
        }

        public List<statusall> GetStatusall()
        {
            var db = GetInstance();
            var list = db.Queryable<statusall>().ToList();
            return list;
        }

        public void AddOrUpdate(List<statusall> list)
        {
            var db = GetInstance();
            foreach (var item in list)
            {
                db.Saveable<statusall>(item).ExecuteReturnEntity();

            }

        }
    }
}
