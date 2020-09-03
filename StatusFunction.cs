using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SqlSugar;
using static CAT_IoT.SqlSugarDBClient;

namespace CAT_IoT
{
    public static class StatusFunction
    {
        [FunctionName("StatusFunction")]
        public static void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            SqlSugarDBClient sqlSugarDBClient = new SqlSugarDBClient();
            var dbStatusall = sqlSugarDBClient.GetStatusall();
            var addSatusAllList = new List<statusall>();
            var updateSatusAllList = new List<statusall>();
            int StopMinutes = int.Parse(System.Environment.GetEnvironmentVariable($"StopLong") ?? "-1");
            var result = sqlSugarDBClient.GetRoboticarmList();
            foreach (var item in result)
            {
                if (item.cdid != null)
                {
                    var dbsatus = dbStatusall.Where(x => x.id == item.cdid && x.type == (int)MachiningType.Roboticarm).FirstOrDefault();
                    int status = (int)item.robotrunningstatus;
                    if (StopMinutes != -1 && (DateTime.UtcNow - item.EventEnqueuedUtcTime).TotalMinutes > StopMinutes)
                    {
                        status = (int)RunningStatus.Stop;
                    }
                    if (dbsatus != null)
                    {
                        //update
                        updateSatusAllList.Add(new statusall { id = item.cdid, lasttime = item.EventEnqueuedUtcTime, status = status, type = (int)MachiningType.Roboticarm });
                    }
                    else
                    {
                        addSatusAllList.Add(new statusall { id = item.cdid, lasttime = item.EventEnqueuedUtcTime, status = status, type = (int)MachiningType.Roboticarm });
                    }
                }
            }

            var resultmcd = sqlSugarDBClient.GetmcdList();
            foreach (var item in resultmcd)
            {
                if (item.ConnectionDeviceId != null)
                {
                    var dbsatus = dbStatusall.Where(x => x.id == item.ConnectionDeviceId && x.type == (int)MachiningType.MCD).FirstOrDefault();
                    int status = (int)item.mcdrunningstatus;
                    if (StopMinutes != -1 && (DateTime.UtcNow - item.EventEnqueuedUtcTime).TotalMinutes > StopMinutes)
                    {
                        status = (int)RunningStatus.Stop;
                    }
                    if (dbsatus != null)
                    {
                        //update
                        updateSatusAllList.Add(new statusall { id = item.ConnectionDeviceId, lasttime = item.EventEnqueuedUtcTime, status = status, type = (int)MachiningType.MCD });
                    }
                    else
                    {
                        addSatusAllList.Add(new statusall { id = item.ConnectionDeviceId, lasttime = item.EventEnqueuedUtcTime, status = status, type = (int)MachiningType.MCD });
                    }
                }
            }

            var resultpama = sqlSugarDBClient.GetpamaList();
            foreach (var item in resultpama)
            {
                if (item.ConnectionDeviceId != null)
                {
                    var dbsatus = dbStatusall.Where(x => x.id == item.ConnectionDeviceId && x.type == (int)MachiningType.Pama).FirstOrDefault();

                    int status = (int)item.pamarunningstatus;
                    if (StopMinutes != -1 && (DateTime.UtcNow - item.EventEnqueuedUtcTime).TotalMinutes > StopMinutes)
                    {
                        status = (int)RunningStatus.Stop;
                    }
                    if (dbsatus != null)
                    {
                        //update
                        updateSatusAllList.Add(new statusall { id = item.ConnectionDeviceId, lasttime = item.EventEnqueuedUtcTime, status = status, type = (int)MachiningType.Pama });
                    }
                    else
                    {
                        addSatusAllList.Add(new statusall { id = item.ConnectionDeviceId, lasttime = item.EventEnqueuedUtcTime, status = status, type = (int)MachiningType.Pama });
                    }
                }
            }

            if (addSatusAllList.Count > 0 || updateSatusAllList.Count > 0)
            {
                sqlSugarDBClient.AddOrUpdate(addSatusAllList);
                sqlSugarDBClient.AddOrUpdate(updateSatusAllList);
            }
        }
    }
}
