using System.Collections.Generic;

namespace Common.Mongo.Models
{
    public class PagedResultEntity<T> : PagedResultBaseEntity
        where T : class
    {
        public PagedResultEntity() => Results = new List<T>();

        public List<T> Results { get; set; }
    }
}