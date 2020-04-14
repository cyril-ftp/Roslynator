﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Roslynator.FileSystem
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class FileSystemFinderResult
    {
        internal FileSystemFinderResult(in NamePart part, Match match, bool isDirectory = false)
        {
            Match = match;
            Part = part;
            IsDirectory = isDirectory;
        }

        public string Path => Part.Path;

        public NamePart Part { get; }

        public Match Match { get; }

        public bool IsDirectory { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Path}";
    }
}
