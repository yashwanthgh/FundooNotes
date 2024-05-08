using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModelLayer.Response
{
    public class ResponseDataModel<T>
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
