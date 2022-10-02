using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon
{
    public class ObjectResponse<T>
    {
        public bool Success { get; set; }
        private T _data;
        public T Data { get; set; }
        public string DataType { get; set; }
        public ObjectResponse()
        {
            DataType = typeof(T).AssemblyQualifiedName;
        }
    }
}
