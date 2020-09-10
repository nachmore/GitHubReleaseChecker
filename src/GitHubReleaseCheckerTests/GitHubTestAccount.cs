using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GitHubReleaseCheckerTests
{
  class GitHubTestAccount
  {

    private static Dictionary<string, GitHubTestAccount> _testAccounts = new Dictionary<string, GitHubTestAccount>()
    {
      { "null_account", new GitHubTestAccount() { Name = null, Repository = "whatever" } },
      { "null_repo", new GitHubTestAccount() { Name = "nachmore", Repository = null } },
      { "null_release", new GitHubTestAccount() { Name = "nachmore", Repository = "AmazonChimeHelper", Release = null } },
      { "nachmore_latest", new GitHubTestAccount() {Name = "nachmore", Repository = "AmazonChimeHelper" } },
      { "nachmore_specific_release", new GitHubTestAccount() {Name = "nachmore", Repository = "AmazonChimeHelper", Release = "23205966" } },
      { "specific_release_multi_asset", new GitHubTestAccount() {Name = "Studio3T", Repository = "robomongo", Release = "30663288" } },
    };

    private static Dictionary<string, GitHubTestAccount> _testToAccountMatrix = new Dictionary<string, GitHubTestAccount>()
    {
      { nameof(Tests.ValidateApiUrlLatestRelease), _testAccounts["nachmore_latest"] },
      { nameof(Tests.ValidateApiUrlSpecificRelease), _testAccounts["nachmore_specific_release"] },
      { nameof(Tests.TestCheckerConstruction), _testAccounts["nachmore_latest"] },
      { nameof(Tests.TestEmptyGetRelease), _testAccounts["nachmore_latest"] },
      { nameof(Tests.TestSpecificGetRelease), _testAccounts["nachmore_specific_release"] },
      { nameof(Tests.TestMultipleAssets), _testAccounts["specific_release_multi_asset"] },
      { nameof(Tests.TestNullAccount), _testAccounts["null_account"] },
      { nameof(Tests.TestNullRepository), _testAccounts["null_repo"] },
      { nameof(Tests.TestNullRelease), _testAccounts["null_release"] },
      { nameof(Tests.TestUpdateDetection), _testAccounts["nachmore_latest"] },
      { nameof(Tests.TestInvalidUpdateDetectionInterval), _testAccounts["nachmore_latest"] },
      { nameof(Tests.TestNullUpdateDetectionCallback), _testAccounts["nachmore_latest"] },
    };

    public string Name { get; private set; }
    public string Repository { get; private set; }
    public string Release { get; private set; }

    public static GitHubTestAccount Get([CallerMemberName] string test = null)
    {
      return _testToAccountMatrix[test];
    }
  }
}
