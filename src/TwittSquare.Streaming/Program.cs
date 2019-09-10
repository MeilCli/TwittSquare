using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwittSquare.Streaming {
    public class Program {
        public static void Main(string[] args) {
            var server = new StreamingServer();
            server.Run();
            while(true) {
                try {
                    Console.ReadKey();
                    server.WillStop = true;
                    return;
                } catch(Exception) { }
            }
        }
    }
}
