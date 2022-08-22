using System;
using Newtonsoft.Json;

namespace EZTracAdminRSC.Models
{
    public class ApiResponse<T>
    {
      [JsonProperty("data")]
      public T Data { get; set; }
    }
}