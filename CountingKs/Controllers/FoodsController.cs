using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;
using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Filters;
using CountingKs.Models;

namespace CountingKs.Controllers
{
    [CountingKsAuthorize(perUser:false)]
    public class FoodsController : BaseApiController
    {
        private const int PAGE_SIZE = 50;

        public FoodsController(ICountingKsRepository repository)
            :base(repository){}

        public object Get(bool includeMeasures = true, int page = 0)
        {
            IQueryable<Food> query;
            if (includeMeasures)
            {
                query = Repository.GetAllFoodsWithMeasures();
            }
            else
            {
                query = Repository.GetAllFoods();
            }

            var baseQuery = query.OrderBy(f => f.Description);

            var totalCount = baseQuery.Count();
            int totalPages = (int)Math.Ceiling((double) totalCount / PAGE_SIZE);

            var helper = new UrlHelper(Request);
            string prevPage = page > 0 ? helper.Link("Foods", new {includeMeasures = includeMeasures, page = page - 1}) : string.Empty;
            string nextPage = page < totalPages - 1 ? helper.Link("Foods", new { includeMeasures = includeMeasures, page = page + 1 }) : string.Empty;

            var results = baseQuery.Skip(page * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .ToList()
                .Select(ModelFactory.Create);

            return new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PrevPage = prevPage,
                NextPage = nextPage,
                Results = results
            };
        }

        public FoodModel Get(int foodid)
        {
            return ModelFactory.Create(Repository.GetFood(foodid));
        }
    }
}
