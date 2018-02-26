using System.Collections.Generic;
using System.Linq;
using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Models;

namespace CountingKs.Controllers
{
    public class FoodsController : BaseApiController
    {
        public FoodsController(ICountingKsRepository repository)
            :base(repository){}

        public IEnumerable<FoodModel> Get(bool includeMeasures = true)
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

            return query.OrderBy(f => f.Description)
                .Take(25)
                .ToList()
                .Select(ModelFactory.Create);
        }

        public FoodModel Get(int foodid)
        {
            return ModelFactory.Create(Repository.GetFood(foodid));
        }
    }
}
