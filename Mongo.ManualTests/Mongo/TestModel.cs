using System;
using Common.Mongo.Models;

namespace Mongo.ManualTests.Mongo
{
    public class TestModel : PartitionedDocument<Guid>
    {
        public string TestStringProp { get; set; }

        public int TestCompanyId { get; set; }
    }
}
