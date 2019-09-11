﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Moq;

using System;

namespace Microsoft.VisualStudio.ProjectSystem
{
    internal static class IPhysicalProjectTreeStorageFactory
    {
        public static IPhysicalProjectTreeStorage Create()
        {
            return Mock.Of<IPhysicalProjectTreeStorage>();
        }

        public static IPhysicalProjectTreeStorage ImplementAddFolderAsync(Action<string> action)
        {
            var mock = new Mock<IPhysicalProjectTreeStorage>();
            mock.Setup(p => p.AddFolderAsync(It.IsAny<string>()))
                .ReturnsAsync(action);

            return mock.Object;
        }

        public static IPhysicalProjectTreeStorage ImplementCreateFolderAsync(Action<string> action)
        {
            var mock = new Mock<IPhysicalProjectTreeStorage>();
            mock.Setup(p => p.CreateFolderAsync(It.IsAny<string>()))
                .ReturnsAsync(action);

            return mock.Object;
        }
    }
}
