﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voat.Data;
using Voat.Domain.Models;
using Voat.Domain.Query;

namespace Voat.Tests.QueryTests
{
    [TestClass]
    public class QueryDomainObjectsTests
    {
        [TestMethod]
        [TestCategory("Query"), TestCategory("Query.DomainObjects")]
        public async Task Query_DomainObjects()
        {
            //just make sure sql compiles 

            var options = new SearchOptions();
            QueryDomainObject q = null;
            IEnumerable<DomainReferenceDetails> results;

            options.Sort = Domain.Models.SortAlgorithm.Hot;
            q = new QueryDomainObject(Domain.Models.DomainType.Set, options);
            results = await q.ExecuteAsync();
            q = new QueryDomainObject(Domain.Models.DomainType.Subverse, options);
            results = await q.ExecuteAsync();

            //Active pattern has an additional join and group clause
            options.Sort = Domain.Models.SortAlgorithm.Active;
            q = new QueryDomainObject(Domain.Models.DomainType.Set, options);
            results = await q.ExecuteAsync();
            q = new QueryDomainObject(Domain.Models.DomainType.Subverse, options);
            results = await q.ExecuteAsync();

            options.Sort = Domain.Models.SortAlgorithm.New;
            q = new QueryDomainObject(Domain.Models.DomainType.Set, options);
            results = await q.ExecuteAsync();
            q = new QueryDomainObject(Domain.Models.DomainType.Subverse, options);
            results = await q.ExecuteAsync();

        }

        [TestMethod]
        [TestCategory("Query"), TestCategory("Query.DomainObjects")]
        public async Task Query_User_DomainObjects()
        {
            ////just make sure sql compiles 

            //var options = new SearchOptions();
            //QueryDomainObject q = null;
            //IEnumerable<DomainReferenceDetails> results;

            //options.Sort = Domain.Models.SortAlgorithm.Hot;
            //q = new QueryUserSub(Domain.Models.DomainType.Set, options);
            //results = await q.ExecuteAsync();
            //q = new QueryDomainObject(Domain.Models.DomainType.Subverse, options);
            //results = await q.ExecuteAsync();

            ////Active pattern has an additional join and group clause
            //options.Sort = Domain.Models.SortAlgorithm.Active;
            //q = new QueryDomainObject(Domain.Models.DomainType.Set, options);
            //results = await q.ExecuteAsync();
            //q = new QueryDomainObject(Domain.Models.DomainType.Subverse, options);
            //results = await q.ExecuteAsync();

            //options.Sort = Domain.Models.SortAlgorithm.New;
            //q = new QueryDomainObject(Domain.Models.DomainType.Set, options);
            //results = await q.ExecuteAsync();
            //q = new QueryDomainObject(Domain.Models.DomainType.Subverse, options);
            //results = await q.ExecuteAsync();


        }

    }
}
