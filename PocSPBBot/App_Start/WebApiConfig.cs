using EV.Cognitives.Services.Luis;
using EV.Cognitives.Services.TextAnalytics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Configuration;
using System.Web.Http;

namespace PocSPBBot
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {


            // Json settings
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Newtonsoft.Json.Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // TODO : not sure it's a good idea to init the service here 
            string key = ConfigurationManager.AppSettings["key:TextAnalytics"];
            TextEngine.Instance.Initialize(key);

            string luisKey = ConfigurationManager.AppSettings["Key:Luis"];
            string luisAppId= ConfigurationManager.AppSettings["AppId:Luis"];
            LuisEngine.Instance.Initialize(luisAppId, luisKey);

        }
    }
}
