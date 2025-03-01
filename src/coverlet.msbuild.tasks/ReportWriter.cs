﻿// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using Coverlet.Core;
using Coverlet.Core.Abstractions;

namespace Coverlet.MSbuild.Tasks
{
  internal class ReportWriter
  {
    private readonly string _coverletMultiTargetFrameworksCurrentTFM;
    private readonly string _directory;
    private readonly string _output;
    private readonly IReporter _reporter;
    private readonly IFileSystem _fileSystem;
    private readonly ISourceRootTranslator _sourceRootTranslator;
    private readonly CoverageResult _result;

    public ReportWriter(string coverletMultiTargetFrameworksCurrentTFM, string directory, string output,
                        IReporter reporter, IFileSystem fileSystem, CoverageResult result, ISourceRootTranslator sourceRootTranslator)
        => (_coverletMultiTargetFrameworksCurrentTFM, _directory, _output, _reporter, _fileSystem, _result, _sourceRootTranslator) =
            (coverletMultiTargetFrameworksCurrentTFM, directory, output, reporter, fileSystem, result, sourceRootTranslator);

    public string WriteReport()
    {
      string filename = Path.GetFileName(_output);

      string separatorPoint = string.IsNullOrEmpty(_coverletMultiTargetFrameworksCurrentTFM) ? "" : ".";

      if (filename == string.Empty)
      {
        // empty filename for instance only directory is passed to CoverletOutput c:\reportpath
        // c:\reportpath\coverage.reportedextension
        filename = $"coverage.{_coverletMultiTargetFrameworksCurrentTFM}{separatorPoint}{_reporter.Extension}";
      }
      else if (Path.HasExtension(filename))
      {
        // filename with extension for instance c:\reportpath\file.ext
        // we keep user specified name
        filename = $"{Path.GetFileNameWithoutExtension(filename)}{separatorPoint}{_coverletMultiTargetFrameworksCurrentTFM}{Path.GetExtension(filename)}";
      }
      else
      {
        // filename without extension for instance c:\reportpath\file
        // c:\reportpath\file.reportedextension
        filename = $"{filename}{separatorPoint}{_coverletMultiTargetFrameworksCurrentTFM}.{_reporter.Extension}";
      }

      string report = Path.Combine(_directory, filename);
      _fileSystem.WriteAllText(report, _reporter.Report(_result, _sourceRootTranslator));
      return report;
    }
  }
}
