using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelAccessLayer
{
    public class MainResponse
    {
            /// <summary>
            /// Status
            /// </summary>
            public string Status { get; set; }
            /// <summary>
            /// Code
            /// </summary>
            public int Code { get; set; }
            /// <summary>
            /// Message
            /// </summary>
            public string Message { get; set; }
            /// <summary>
            /// Reurn data
            /// </summary>
            public object Data { get; set; }

            /// <summary>
            /// Default Contructor
            /// </summary>
            public MainResponse()
            {
                this.Status = "";
                this.Code = 200;
                this.Message = "";
                this.Data = null;
            }
        }
}
