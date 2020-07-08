﻿// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.VisualStudio.ProjectSystem.Tree.Dependencies.Models;
using Microsoft.VisualStudio.ProjectSystem.Tree.Dependencies.Snapshot;
using Moq;

namespace Microsoft.VisualStudio.ProjectSystem.VS
{
    internal static class DependenciesSnapshotFactory
    {
        public static DependenciesSnapshot Implement(
            Dictionary<ITargetFramework, TargetedDependenciesSnapshot>? dependenciesByTarget = null,
            DiagnosticLevel? maximumVisibleDiagnosticLevel = null,
            ITargetFramework? activeTarget = null,
            MockBehavior mockBehavior = MockBehavior.Default)
        {
            var mock = new Mock<DependenciesSnapshot>(mockBehavior);

            if (dependenciesByTarget != null)
            {
                mock.Setup(x => x.DependenciesByTargetFramework).Returns(dependenciesByTarget.ToImmutableDictionary());
            }

            if (maximumVisibleDiagnosticLevel.HasValue)
            {
                mock.Setup(x => x.MaximumVisibleDiagnosticLevel).Returns(maximumVisibleDiagnosticLevel.Value);
            }

            if (activeTarget != null)
            {
                mock.Setup(x => x.ActiveTargetFramework).Returns(activeTarget);
            }

            return mock.Object;
        }
    }
}
