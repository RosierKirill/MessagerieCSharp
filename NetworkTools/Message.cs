using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTools
{
    public class Message
    {
        public String Id {  get; internal set; }

        public String Data { get; internal set; }

        public Message( String id, string data ) 
        { 
            Id = id;
            Data = data;
        }
    }
}
