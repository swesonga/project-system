﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Moq;

#nullable disable

namespace EnvDTE
{
    internal static class ProjectItemFactory
    {
        public static ProjectItem Create() => Mock.Of<ProjectItem>();
    }
}
