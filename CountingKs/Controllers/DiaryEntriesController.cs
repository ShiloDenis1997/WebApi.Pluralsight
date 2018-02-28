using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CountingKs.Data;
using CountingKs.Models;
using CountingKs.Services;

namespace CountingKs.Controllers
{
    public class DiaryEntriesController : BaseApiController
    {
        private readonly ICountingKsIdentityService _identityService;

        public DiaryEntriesController(ICountingKsRepository repository, ICountingKsIdentityService identityService)
            : base(repository)
        {
            _identityService = identityService;
        }

        public IEnumerable<DiaryEntryModel> Get(DateTime diaryid)
        {
            var result = Repository.GetDiaryEntries(CurrentUser, diaryid);

            return result.ToList()
                .Select(ModelFactory.Create);
        }

        public HttpResponseMessage Get(DateTime diaryId, int id)
        {
            var result = Repository.GetDiaryEntry(CurrentUser, diaryId, id);

            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, ModelFactory.Create(result));
        }

        public HttpResponseMessage Post(DateTime diaryId, [FromBody]DiaryEntryModel model)
        {
            try
            {
                var entity = ModelFactory.Parse(model);

                if (entity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not read diary entry from body");
                }

                var diary = Repository.GetDiary(_identityService.CurrentUser, diaryId);
                if (diary == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                if (diary.Entries.Any(e => e.Measure.Id == entity.Measure.Id))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Duplicates are not allowed");
                }

                diary.Entries.Add(entity);
                if (Repository.SaveAll())
                {
                    return Request.CreateResponse(HttpStatusCode.Created, ModelFactory.Create(entity));
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not save to database");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Delete(DateTime diaryid, int id)
        {
            try
            {
                if (Repository.GetDiary(CurrentUser, diaryid).Entries.Any(e => e.Id == id) == false)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                if (Repository.DeleteDiaryEntry(id) && Repository.SaveAll())
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Cannot delete diary entry from database");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPatch]
        [HttpPut]
        public HttpResponseMessage Patch(DateTime diaryid, int id, [FromBody] DiaryEntryModel model)
        {
            try
            {
                var entity = Repository.GetDiaryEntry(CurrentUser, diaryid, id);
                if (entity == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                var parsedModel = ModelFactory.Parse(model);
                if (parsedModel == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                if (entity.Quantity != parsedModel.Quantity)
                {
                    entity.Quantity = parsedModel.Quantity;
                    if (Repository.SaveAll())
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        private string CurrentUser => _identityService.CurrentUser;
    }
}
