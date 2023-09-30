using GenericCachingRepository.SourceCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCachingRepository.SharedCache
{
    /*
        (A and B and C) or ((B and D and E) and (F or G or H))
        Seq1 = (A and B and C)
        Seq2a = (B and D and E)
        Seq2b = (F or G or H)
        Seq2 = ((B and D and E) and (F or G or H))
        GridModel:GroupBy:OrderBy:Asc:Block: Hash => (A and B and C):((B and D and E) and (F or G or H))
        GridModel:GroupBy:OrderBy:Asc:Block:(A or B or C)
        GridModel:GroupBy:OrderBy:Asc:Block:(A)(B)(C) //Contains operation
    */

    /* PARTIAL STRING SEARCH
        declare @UserInput varchar(1000) = '1'
    
        select * from Table_1
        where cast(Col_2 as varchar(100) like '%'+@UserInput+'%'
        order by Col_1 asc, Col2 desc
     */

    public interface IQueryCache : ICache
    {
        public void SaveCacheQueryResultReferences<T>(string queryKey, IEnumerable<T> list) where T : class;
        public Task<IEnumerable<T>> LoadCacheQueryResults<T>(DbSet<T> dbset, string queryKey) where T : class;
        public long GetQueryCount<T>(string whereClause) where T : class;
        public void SetQueryCount<T>(string whereClause, long count) where T : class;
    }

    public class QueryCache : Cache, IQueryCache
    {
        public QueryCache(int absoluteExpirationSeconds = 300, int slidingExpirationSeconds = 60, int expirationFrequencyScanSeconds = 1)
            : base (absoluteExpirationSeconds, slidingExpirationSeconds, expirationFrequencyScanSeconds)
        {}

        public void SetQueryCount<T>(string whereClause, long count) where T : class
            => cache.Set<long>($"{typeof(T).Name}:count:{whereClause}", count);

        public long GetQueryCount<T>(string whereClause) where T : class
            => cache.Get<long>($"{typeof(T).Name}:count:{whereClause}");

        public async Task<IEnumerable<T>> LoadCacheQueryResults<T>(DbSet<T> dbset, string queryKey) where T : class
        {
            //fetch reference list
            var referenceIDs = this.Get<List<object[]>>($"{typeof(T).Name}:{queryKey}");
            var list = new List<T>();

            if (referenceIDs != null)
            {
                foreach (var referenceID in referenceIDs)
                {
                    //fetch from cache
                    var key = CacheKeyHelper.GetKeyIds<T>(referenceID);
                    var item = this.Get<T>(key);
                    //check db set
                    if(item == null)
                        item = await dbset.FindAsync(referenceID);

                    if(item != null)
                    {
                        //update cache
                        this.Remove<T>(key);
                        this.Add<T>(key, item);
                        //build reference list
                        list.Add(item);
                    }

                }
            }

            return list;
        }

        public void SaveCacheQueryResultReferences<T>(string queryKey, IEnumerable<T> list) where T : class
        {
            var referenceIDs = new List<object[]>();

            foreach(var item in list)
            {
                //store in cache
                this.Add(CacheKeyHelper.GetKey(item), item);
                //build reference list
                referenceIDs.Add(CacheKeyHelper.GetIds(item));
            }

            //store reference list
            this.Add<List<object[]>>($"{typeof(T).Name}:{queryKey}", referenceIDs);
        }
    }
}
