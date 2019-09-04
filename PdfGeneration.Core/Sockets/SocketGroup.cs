using System.Collections.Generic;

namespace PdfGeneration.Core.Sockets
{
    public class SocketGroup
    {
        public string Name { get; set; }
        public List<string> Connections { get; set; }
    }
}