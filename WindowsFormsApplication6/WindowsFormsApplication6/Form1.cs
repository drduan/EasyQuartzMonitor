using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using SqlSugar;

namespace WindowsFormsApplication6
{

/// <summary>
/// https://www.w3cschool.cn/quartz_doc/quartz_doc-hm9x2dki.html
/// </summary>
public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {

        RunProgram().GetAwaiter().GetResult();
        //GetStudentList();
    }

    public List<Queue> GetStudentList()
    {
        var db = GetInstance();
        var list = db.SqlQueryable<Queue>("select * from mis_queue").ToPageList(1, 2);//Search
        string msg = JsonConvert.SerializeObject(list);
        return list;
    }


    /// <summary>
    /// Create SqlSugarClient
    /// </summary>
    /// <returns></returns>
    private SqlSugarClient GetInstance()
    {
        SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = Config.ConnectionString,
            DbType = SqlSugar.DbType.Oracle,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        });
        //Print sql
        db.Aop.OnLogExecuting = (sql, pars) =>
        {
            Console.WriteLine(sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
            Console.WriteLine();
        };
        return db;
    }


    private static async Task RunProgram()
    {
        try
        {
            // Grab the Scheduler instance from the Factory
            NameValueCollection props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);
            IScheduler scheduler = await factory.GetScheduler();

            // and start it off
            await scheduler.Start();


            IJobDetail job = JobBuilder.Create<HelloJob>()
                             .WithIdentity("job1", "group1")
                             .Build();

            ITrigger trigger = TriggerBuilder.Create()
                               .WithIdentity("trigger1", "group1")
                               .StartNow()
                               .WithSimpleSchedule(x => x
                                                   .WithIntervalInSeconds(10)
                                                   .WithRepeatCount(10))
                               .Build();

            // Tell quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
            // some sleep to show what's happening
            await Task.Delay(TimeSpan.FromSeconds(60));


            // and last shut down the scheduler when you are ready to close your program
            await scheduler.Shutdown();
        }
        catch (SchedulerException se)
        {
            await Console.Error.WriteLineAsync(se.ToString());
        }
    }

}



//[SugarColumn("student")]
public class Queue
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public string Id {
        get;
        set;
    }
    public string lsh {
        get;
        set;
    }
    public string billnumber {
        get;
        set;
    }
    public string billtype {
        get;
        set;
    }
    public string cksj {
        get;
        set;
    }


}



}
