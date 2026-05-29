using System.Collections.Generic;

namespace FlowCore.Core.Constants
{
    public static class SystemRoles
    {
        public const string Manager = "Manager";
        public const string HR = "HR";
        public const string CEO = "CEO";

        public static readonly List<string> CanApproveLeaveRequests = new()
        {
            Manager,
            HR,
            CEO
        };
    }
}