using System;
using System.Web.Http;
using CountingKs.Data;
using CountingKs.Models;

namespace CountingKs.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        private readonly Lazy<ModelFactory> _modelFactory;

        public BaseApiController(ICountingKsRepository repository)
        {
            Repository = repository;
            _modelFactory = new Lazy<ModelFactory>(() => new ModelFactory(Request, Repository));
        }

        protected ModelFactory ModelFactory => _modelFactory.Value;
        protected ICountingKsRepository Repository { get; }
    }
}
