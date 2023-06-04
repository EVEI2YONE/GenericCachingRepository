using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCachingRepository.SharedCache
{
    //(A and B and C) or ((B and D and E) and (F or G or H))
    //Seq1 = (A and B and C)
    //Seq2a = (B and D and E)
    //Seq2b = (F or G or H)
    //Seq2 = ((B and D and E) and (F or G or H))
    //GridModel:GroupBy:OrderBy:Asc:Block: Hash => (A and B and C):((B and D and E) and (F or G or H))
    //GridModel:GroupBy:OrderBy:Asc:Block:(A or B or C)
    //GridModel:GroupBy:OrderBy:Asc:Block:(A)(B)(C) //Contains operation
    public interface IQueryCache : ICache
    {
    }

    public class QueryCache : Cache, IQueryCache
    {
        public QueryCache(int absoluteExpirationSeconds = 300, int slidingExpirationSeconds = 60, int expirationFrequencyScanSeconds = 1)
            : base (absoluteExpirationSeconds, slidingExpirationSeconds, expirationFrequencyScanSeconds)
        {
            Filters filters = new Filters();
        }
    }
}
