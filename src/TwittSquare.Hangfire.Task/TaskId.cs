using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwittSquare.Hangfire.Task {
    public class TaskId {

        public const string DeleteExpirationLoginId = "DeleteExpirationLogin";
        public const string UserTaskCheckId = "UserTaskCheckId";

        public static string GetUserHomeTimelineId(long userId) {
            return $"UserHomeTimelineId-{userId}";
        }

        public static string GetUserUserTimelineId(long userId) {
            return $"UserUserTimelineId-{userId}";
        }

        public static string GetUserFollowingId(long userId) {
            return $"UserFollowingId-{userId}";
        }
    }
}
