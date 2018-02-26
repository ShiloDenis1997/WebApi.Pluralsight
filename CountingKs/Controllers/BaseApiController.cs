using System;
using System.Web.Http;
using CountingKs.Data;
using CountingKs.Models;

namespace CountingKs.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        private readonly ICountingKsRepository _repository;
        private readonly Lazy<ModelFactory> _modelFactory;

        public BaseApiController(ICountingKsRepository repository)
        {
            _repository = repository;
            _modelFactory = new Lazy<ModelFactory>(() => new ModelFactory(Request));
        }

        protected ModelFactory ModelFactory => _modelFactory.Value;
        protected ICountingKsRepository Repository => _repository;
    }
}
