﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

using Xunit;

#nullable disable

namespace Microsoft.VisualStudio.ProjectSystem
{
    public sealed class SetDiffTests
    {
        // TODO explore when before/after inputs have duplicate items

        [Theory]
        [InlineData(new int[] {},        new int[] {1, 2, 3}, new int[] {1, 2, 3}, new int[] {})]
        [InlineData(new int[] {1, 2, 3}, new int[] {},        new int[] {},        new int[] {1, 2, 3})]
        [InlineData(new int[] {1, 2, 3}, new int[] {1, 2, 3}, new int[] {},        new int[] {})]
        [InlineData(new int[] {3, 2, 1}, new int[] {1, 2, 3}, new int[] {},        new int[] {})]
        [InlineData(new int[] {1, 2},    new int[] {2, 3},    new int[] {3},       new int[] {1})]
        public void ProducesCorrectDiff(int[] before, int[] after, int[] added, int[] removed)
        {
            var diff = new SetDiff<int>(before, after);

            var actualAdded = new HashSet<int>(diff.Added);
            var actualRemoved = new HashSet<int>(diff.Removed);

            Assert.True(actualAdded.SetEquals(added));
            Assert.True(actualRemoved.SetEquals(removed));
        }

        [Fact]
        public void Constructor_WithNullValues_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new SetDiff<int>(null, new[] {1, 2, 3}));
            Assert.Throws<ArgumentNullException>(() => new SetDiff<int>(new[] {1, 2, 3}, null));
        }
    }
}
