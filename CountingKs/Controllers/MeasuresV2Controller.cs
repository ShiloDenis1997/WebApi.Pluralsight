using System.Collections.Generic;
using System.Linq;
using CountingKs.Data;
using CountingKs.Models;

namespace CountingKs.Controllers
{
    public class MeasuresV2Controller : BaseApiController
    {
        public MeasuresV2Controller(ICountingKsRepository repository)
            :base(repository){}

        public IEnumerable<MeasureV2Model> Get(int foodid)
        {
            return Repository.GetMeasuresForFood(foodid)
                .ToList()
                .Select(ModelFactory.Create2);
        }

        public MeasureV2Model Get(int foodid, int id)
        {
            var result = Repository.GetMeasure(id);

            if (result.Food.Id == foodid)
            {
                return ModelFactory.Create2(result);
            }

            return null;
        }
    }
}
