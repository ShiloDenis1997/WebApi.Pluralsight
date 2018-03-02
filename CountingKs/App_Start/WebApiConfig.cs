﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using CountingKs.Filters;
using CountingKs.Services;
using Newtonsoft.Json.Serialization;
using WebApiContrib.Formatting.Jsonp;

namespace CountingKs
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "Foods",
                routeTemplate: "api/nutrition/foods/{foodid}",
                defaults: new { controller = "Foods", foodid = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "Measures",
                routeTemplate: "api/nutrition/foods/{foodid}/measures/{id}",
                defaults: new { controller = "Measures", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "Diaries",
                routeTemplate: "api/user/diaries/{diaryid}",
                defaults: new { controller = "Diaries", diaryid = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DiaryEntries",
                routeTemplate: "api/user/diaries/{diaryid}/entries/{id}",
                defaults: new { controller = "DiaryEntries", id= RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DiarySummary",
                routeTemplate: "api/user/diaries/{diaryid}/summary",
                defaults: new { controller = "DiarySummary" }
            );

            config.Routes.MapHttpRoute(
                name: "Token",
                routeTemplate: "api/token",
                defaults: new { controller = "Token" }
            );

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            CreateMediaTypes(jsonFormatter);


            // Add support for JSONP
            var formatter = new JsonpMediaTypeFormatter(jsonFormatter);
            config.Formatters.Insert(0, formatter);

            // Replace the Controller Configuration
            config.Services.Replace(typeof(IHttpControllerSelector), new CountingKsControllerSelector(config));


#if !DEBUG
            config.Filters.Add(new RequireHttpsAttribute());
#endif
        }

        private static void CreateMediaTypes(JsonMediaTypeFormatter jsonFormatter)
        {
            var mediaTypes = new string[]
            {
                "application/vnd.countingks.food.v1+json",
                "application/vnd.countingks.measure.v1+json",
                "application/vnd.countingks.measure.v2+json",
                "application/vnd.countingks.diary.v1+json",
                "application/vnd.countingks.diaryEntry.v1+json"
            };

            foreach (var mediaType in mediaTypes)
            {
                jsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            }
        }
    }
}