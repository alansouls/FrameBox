using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameBox.Core.Inbox.Options;

internal static class InternalInboxOptions
{
    public static int MaxInboxRetryCount { get; set; } = 3;
}
