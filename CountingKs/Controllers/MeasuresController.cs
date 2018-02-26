using System.Collections.Generic;
using System.Linq;
using CountingKs.Data;
using CountingKs.Models;

namespace CountingKs.Controllers
{
    public class MeasuresController : BaseApiController
    {
        public MeasuresController(ICountingKsRepository repository)
            :base(repository){}

        public IEnumerable<MeasureModel> Get(int foodid)
        {
            return Repository.GetMeasuresForFood(foodid)
                .ToList()
                .Select(ModelFactory.Create);
        }

        public MeasureModel Get(int foodid, int id)
        {
            var result = Repository.GetMeasure(id);

            if (result.Food.Id == foodid)
            {
                return ModelFactory.Create(result);
            }

            return null;
        }
    }
}
