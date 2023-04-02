using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Models
{

    public class DatabaseSettings : IDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string PostCollectionName { get; set; }
        public string CommentsCollectionName { get; set; }
    }

        public interface IDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string PostCollectionName { get; set; }
        string CommentsCollectionName { get; set; }
    }
}