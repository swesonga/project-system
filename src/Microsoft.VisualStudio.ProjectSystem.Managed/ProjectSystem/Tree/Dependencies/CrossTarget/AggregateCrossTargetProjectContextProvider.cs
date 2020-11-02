﻿// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Composition;

namespace Microsoft.VisualStudio.ProjectSystem.Tree.Dependencies.CrossTarget
{
    /// <summary>
    ///     Creates <see cref="AggregateCrossTargetProjectContext"/> instances based on the
    ///     current <see cref="UnconfiguredProject"/>.
    /// </summary>
    [Export(typeof(AggregateCrossTargetProjectContextProvider))]
    [ProjectSystemContract(ProjectSystemContractScope.UnconfiguredProject, ProjectSystemContractProvider.Private, Cardinality = ImportCardinality.ExactlyOne)]
    internal class AggregateCrossTargetProjectContextProvider
    {
        private readonly IUnconfiguredProjectCommonServices _commonServices;
        private readonly IActiveConfiguredProjectsProvider _activeConfiguredProjectsProvider;
        private readonly ITargetFrameworkProvider _targetFrameworkProvider;

        [ImportingConstructor]
        public AggregateCrossTargetProjectContextProvider(
            IUnconfiguredProjectCommonServices commonServices,
            IActiveConfiguredProjectsProvider activeConfiguredProjectsProvider,
            ITargetFrameworkProvider targetFrameworkProvider)
        {
            _commonServices = commonServices;
            _activeConfiguredProjectsProvider = activeConfiguredProjectsProvider;
            _targetFrameworkProvider = targetFrameworkProvider;
        }

        /// <summary>
        ///     Creates a <see cref="AggregateCrossTargetProjectContext"/>.
        /// </summary>
        /// <returns>
        ///     The created <see cref="AggregateCrossTargetProjectContext"/>.
        /// </returns>
        public async Task<AggregateCrossTargetProjectContext> CreateProjectContextAsync()
        {
            // Get the set of active configured projects ignoring target framework.
#pragma warning disable CS0618 // Type or member is obsolete
            ImmutableDictionary<string, ConfiguredProject>? configuredProjectsMap = await _activeConfiguredProjectsProvider.GetActiveConfiguredProjectsMapAsync();
#pragma warning restore CS0618 // Type or member is obsolete

            if (configuredProjectsMap == null)
            {
                throw new InvalidOperationException("There are no active configured projects.");
            }

            ProjectConfiguration activeProjectConfiguration = _commonServices.ActiveConfiguredProject.ProjectConfiguration;
            ImmutableArray<TargetFramework>.Builder targetFrameworks = ImmutableArray.CreateBuilder<TargetFramework>(initialCapacity: configuredProjectsMap.Count);
            TargetFramework activeTargetFramework = TargetFramework.Empty;

            foreach ((string tfm, ConfiguredProject configuredProject) in configuredProjectsMap)
            {
                ProjectProperties projectProperties = configuredProject.Services.ExportProvider.GetExportedValue<ProjectProperties>();
                ConfigurationGeneral configurationGeneralProperties = await projectProperties.GetConfigurationGeneralPropertiesAsync();
                TargetFramework targetFramework = await GetTargetFrameworkAsync(tfm, configurationGeneralProperties);

                targetFrameworks.Add(targetFramework);

                if (activeTargetFramework.Equals(TargetFramework.Empty) &&
                    configuredProject.ProjectConfiguration.Equals(activeProjectConfiguration))
                {
                    activeTargetFramework = targetFramework;
                }
            }

            bool isCrossTargeting = !(configuredProjectsMap.Count == 1 && string.IsNullOrEmpty(configuredProjectsMap.First().Key));

            return new AggregateCrossTargetProjectContext(
                isCrossTargeting,
                targetFrameworks.MoveToImmutable(),
                configuredProjectsMap,
                activeTargetFramework,
                _targetFrameworkProvider);
        }

        private async Task<TargetFramework> GetTargetFrameworkAsync(
            string shortOrFullName,
            ConfigurationGeneral configurationGeneralProperties)
        {
            if (string.IsNullOrEmpty(shortOrFullName))
            {
                object? targetObject = await configurationGeneralProperties.TargetFrameworkMoniker.GetValueAsync();

                if (targetObject == null)
                {
                    return TargetFramework.Empty;
                }

                shortOrFullName = targetObject.ToString();
            }

            return _targetFrameworkProvider.GetTargetFramework(shortOrFullName) ?? TargetFramework.Empty;
        }
    }
}
