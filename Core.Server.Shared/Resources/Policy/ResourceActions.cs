﻿using System;

namespace Core.Server.Shared.Resources
{
    [Flags]
    public enum ResourceActions : short
    {
        None = 0,
        Read = 1,
        Create = 2,
        Alter = 4,
        Delete = 8,
        All = 15
    }
}
