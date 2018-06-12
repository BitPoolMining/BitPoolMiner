using System;
using System.Collections.Specialized;
using System.Net;
using Newtonsoft.Json;

namespace BitPoolMiner.Persistence.API.Base
{
    public class APIBase
    {
        /// <summary>
        /// Retrieve data from API, deserialize and parse
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        protected static T DownloadSerializedJSONData<T>(string url) where T : new()
        {
            using (var webClient = new WebClient())
            {
                var jsonData = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    jsonData = webClient.DownloadString(url);
                }
                catch (Exception e)
                {
                    throw new ApplicationException(string.Format("Error call API at {0}", url), e);
                }

                // If string with JSON data is not empty, deserialize it to class and return its instance 
                return !string.IsNullOrEmpty(jsonData) ? JsonConvert.DeserializeObject<T>(jsonData) : new T();
            }
        }

        /// <summary>
        /// Retrieve data from API, deserialize and parse
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        protected static T DownloadSerializedJSONData<T>(string url, NameValueCollection queryString) where T : new()
        {
            using (var webClient = new WebClient())
            {
                var jsonData = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    webClient.QueryString = queryString;
                    jsonData = webClient.DownloadString(url);
                }
                catch (Exception e)
                {
                    throw new ApplicationException(string.Format("Error call API at {0}", url), e);
                }

                // If string with JSON data is not empty, deserialize it to class and return its instance 
                return !string.IsNullOrEmpty(jsonData) ? JsonConvert.DeserializeObject<T>(jsonData) : new T();
            }
        }
    }
}
