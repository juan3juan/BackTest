using DataContract;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AllocationEngine
{
    public class StrategyService : IStrategy
    {
        public Dictionary<string, int> ExecuteStrategy(StrategyDataContract dataContract)
        {
            var client = new RestClient("https://localhost:44332/api/AllocationEngine");
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var request = new RestRequest("Strategy", Method.POST);

            //request.AddJsonBody(JsonConvert.SerializeObject(dataContract));
            request.AddJsonBody(dataContract);

            Dictionary<string, int> result = null;
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = JsonConvert.DeserializeObject<Dictionary<string, int>>(response.Content);
                return result; // raw content as string
            }
            return result;
        }

    }
}
