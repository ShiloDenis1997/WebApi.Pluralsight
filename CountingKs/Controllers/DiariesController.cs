using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CountingKs.Data;
using CountingKs.Filters;
using CountingKs.Models;
using CountingKs.Services;

namespace CountingKs.Controllers
{
    [CountingKsAuthorize]
    public class DiariesController : BaseApiController
    {
        private readonly ICountingKsIdentityService _identityService;

        public DiariesController(ICountingKsRepository repository, ICountingKsIdentityService identityService) 
            : base(repository)
        {
            _identityService = identityService;
        }

        public IEnumerable<DiaryModel> Get()
        {
            var username = _identityService.CurrentUser;
            return Repository.GetDiaries(username)
                .OrderBy(d => d.CurrentDate)
                .Take(10)
                .ToList()
                .Select(ModelFactory.Create);
        }

        public HttpResponseMessage Get(DateTime diaryId)
        {
            var username = _identityService.CurrentUser;
            var result = Repository.GetDiary(username, diaryId);

            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, ModelFactory.Create(result));
        }
    }
}
