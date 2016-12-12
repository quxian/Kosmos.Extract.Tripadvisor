using Kosmos.Extract.tripadvisor.Model;
using Kosmos.Extract.Tripadvisor.DbContext;
using Kosmos.Singleton;
using StringExtensionForYongsheng;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Kosmos.Extract.Tripadvisor
{
    public static class ExtractResultCache
    {
        private static object _lock = new object();
        private static Task _task;

        public static ConcurrentBag<string> Result { get; set; }
        static ExtractResultCache()
        {
            Result = new ConcurrentBag<string>();

            _task = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromMinutes(2));
                        using (var dbContxt = new AppDbContext())
                        {
                            CacheToDb(dbContxt);
                        }
                    }
                    catch (Exception e)
                    {
                        SingleHttpClient.PostException(e);
                    }
                }
            });

        }

        public static void CacheToDb(AppDbContext dbContext)
        {
            lock (_lock)
            {
                var result = Result.Select(x => new ExtractResult
                {
                    HashCode = Guid.NewGuid().ToString("N"),
                    Result = x,
                    ExtractData = DateTime.Now
                });
                dbContext.ExtractResults.AddRange(result);
                dbContext.SaveChanges();
                Result = new ConcurrentBag<string>();
            }
        }
    }
}