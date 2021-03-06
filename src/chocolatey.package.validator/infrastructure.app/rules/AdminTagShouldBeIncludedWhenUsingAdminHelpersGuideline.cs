﻿// Copyright © 2015 - Present RealDimensions Software, LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
//
// You may obtain a copy of the License at
//
// 	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace chocolatey.package.validator.infrastructure.app.rules
{
    using System.IO;
    using System.Linq;
    using infrastructure.rules;
    using NuGet;

    public class AdminTagShouldBeIncludedWhenUsingAdminHelpersGuideline : BasePackageRule
    {
        public override string ValidationFailureMessage { get { return
@"This package uses a helper function that requires administrative permissions, the tag 'admin' should also be in the nuspec. [More...](https://github.com/chocolatey/package-validator/wiki/AdminTagShouldBeUsedWhenUsingAdminHelper) 
  * **NOTE:** This becomes a Requirement starting on 1 February 2016."; } }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            var valid = true;

            var files = package.GetFiles().or_empty_list_if_null();

            foreach (var packageFile in files.or_empty_list_if_null())
            {
                string extension = Path.GetExtension(packageFile.Path).to_lower();
                if (extension != ".ps1" && extension != ".psm1") continue;

                var contents = packageFile.GetStream().ReadToEnd().to_lower();

                if (contents.Contains("install-chocolateypackage") ||
                    contents.Contains("start-chocolateyprocessasadmin") ||
                    contents.Contains("install-chocolateyinstallpackage") ||
                    contents.Contains("install-chocolateyenvironmentvariable") ||
                    contents.Contains("install-chocolateyexplorermenuitem") ||
                    contents.Contains("install-chocolateyfileassociation"))
                {
                    valid = package.Tags.to_string().Split(' ').Any(tag => tag.ToLower() == "admin");
                }
            }

            return valid;
        }
    }
}
