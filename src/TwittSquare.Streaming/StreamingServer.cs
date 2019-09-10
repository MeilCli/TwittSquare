using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwittSquare.Core.Twitter.Model;

namespace TwittSquare.Streaming {

    public class StreamingServer {

        public bool IsRunning { get; private set; }
        public bool WillStop { get; set; }
        private Dictionary<long,UserStream> userStreams = new Dictionary<long,UserStream>();

        public void Run() {
            Console.Out.WriteLine("Start Server");
            Task.Run(async ()=> {
                IsRunning = true;
                while(true) {
                    if(WillStop) {
                        WillStop = false;
                        disConnect();
                        break;
                    }
                    checkConnect();
                    await Task.Delay(1000 * 60 * 1);
                }
                IsRunning = false;
            });
        }

        private void checkConnect() {
            foreach(var userStream in userStreams) {
                if(userStream.Value.IsConnect == false) {
                    Console.Out.WriteLine("Reconnect");
                    try {
                        userStream.Value.DisConnect();
                        userStream.Value.Connect();
                    }catch(Exception e) {
                        Console.Out.WriteLine(e.Message);
                    }
                }
            }
            try {
                using(var context = new TwitterContext()) {
                    foreach(var token in context.Tokens) {
                        if(userStreams.ContainsKey(token.UserId) == false) {
                            userStreams[token.UserId] = new UserStream(token.UserId);
                            userStreams[token.UserId].Connect();
                        }
                    }
                }
            } catch(Exception e) {
                Console.Out.WriteLine(e.Message);
            }
        }

        private void disConnect() {
            foreach(var userStream in userStreams) {
                try {
                    userStream.Value.DisConnect();
                }catch(Exception e) {
                    Console.Out.WriteLine(e.Message);
                }
            }
        }

    }
}
