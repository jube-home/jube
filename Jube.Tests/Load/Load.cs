/* Copyright (C) 2022-present Jube Holdings Limited.
 *
 * This file is part of Jube™ software.
 *
 * Jube™ is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License 
 * as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * Jube™ is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty  
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

 * You should have received a copy of the GNU Affero General Public License along with Jube™. If not, 
 * see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Jube.Test.Load;

public class Load
{
    [Theory]
    [InlineData("https://localhost:5001/api/invoke/EntityAnalysisModel/90c425fd-101a-420b-91d1-cb7a24a969cc",10000,100000,false)]
    public async void LoadTest(string uriString,int httpTimeout, int iteration,bool async)
    {
        var random = new Random();
        var uri = async ? new Uri(uriString + "/async") : new Uri(uriString);
        
        var stringTemplate = Helpers.ReadFileContents("Load/Mock.json");
        
        var httpClientHandler = new HttpClientHandler();
        httpClientHandler.MaxConnectionsPerServer = 30;
        httpClientHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

        using var client = new HttpClient(httpClientHandler);
        client.Timeout = TimeSpan.FromMilliseconds(httpTimeout);

        for (var i = 0; i < iteration; i++)
        {
            var tasks = new List<Task<HttpResponseMessage>>();
            for (var j = 0; j < random.NextInt64(10,httpClientHandler.MaxConnectionsPerServer); j++)
            {
                var stringReplaced = stringTemplate.Replace("[@AccountId@]", random.NextInt64(1, 1000).ToString());
                stringReplaced = stringReplaced.Replace("[@TxnId@]", i.ToString());
            
                var stringContent = new StringContent(
                    stringReplaced,
                    Encoding.UTF8,
                    "application/json");
            
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = uri,
                    Content = stringContent
                };

                var task = client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
                tasks.Add(task);    
            }
            Task.WaitAll(tasks.ToArray());

            if (async)
            {
                var tasksCallback = new List<Task<HttpResponseMessage>>();
                foreach (var task in tasks.Where(task => task.Result.StatusCode != HttpStatusCode.NotFound))
                {
                    var jObject = JObject.Parse(await task.Result.Content.ReadAsStringAsync());
                
                    var uriCallback = new Uri($"https://localhost:5001/api/invoke/EntityAnalysisModel/Callback/{jObject.SelectToken("entityAnalysisModelInstanceEntryGuid").Value<string>()}");
                
                    var requestCallback = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = uriCallback
                    };
                
                    var taskCallback = client.SendAsync(requestCallback, HttpCompletionOption.ResponseContentRead);
                    tasksCallback.Add(taskCallback);
                }
                Task.WaitAll(tasksCallback.ToArray());
            }
            
            Thread.Sleep(0);
        }
    }
}