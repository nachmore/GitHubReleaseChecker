using GitHubReleaseChecker;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GitHubReleaseCheckerTests
{
  public class Tests
  {

    private Dictionary<string, string> _accounts = new Dictionary<string, string>()
    {
      {"nachmore", "AmazonChimeHelper" },
      {"Studio3T", "robomongo" }
    };

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ValidateApiUrlLatestRelease()
    {
      var account = GitHubTestAccount.Get();
      var checker = new ReleaseChecker(account.Name, account.Repository);

      var url = checker.GetReleaseApiUrl();

      Assert.AreEqual(url, "https://api.github.com/repos/nachmore/AmazonChimeHelper/releases/latest");
    }

    [Test]
    public void ValidateApiUrlSpecificRelease()
    {
      var account = GitHubTestAccount.Get();
      var checker = new ReleaseChecker(account.Name, account.Repository);

      var url = checker.GetReleaseApiUrl(account.Release);

      Assert.AreEqual(url, "https://api.github.com/repos/nachmore/AmazonChimeHelper/releases/23205966");
    }

    [Test]
    public void TestCheckerConstruction()
    {
      var account = GitHubTestAccount.Get();
      var checker = new ReleaseChecker(account.Name, account.Repository);

      Assert.AreEqual(checker.AccountName, account.Name);
      Assert.AreEqual(checker.Repository, account.Repository);
    }

    [Test]
    public async Task TestEmptyGetRelease()
    {
      var account = GitHubTestAccount.Get();
      var checker = new ReleaseChecker(account.Name, account.Repository);
      var latest = await checker.GetReleaseAsync();

      Assert.IsNotNull(latest);
    }

    [Test]
    public async Task TestSpecificGetRelease()
    {
      var account = GitHubTestAccount.Get();
      var checker = new ReleaseChecker(account.Name, account.Repository);
      var release = await checker.GetReleaseAsync(account.Release);

      Assert.IsNotNull(release);
      Assert.IsFalse(release.PreRelease);
      Assert.AreEqual(release.Name, "Chime Helper 1.2");
      Assert.AreEqual(release.HtmlUrl, "https://github.com/nachmore/AmazonChimeHelper/releases/tag/v1.2");
      Assert.AreEqual(release.Assets.Length,  1);
      Assert.IsNotEmpty(release.ReleaseNotes);
    }

    [Test]
    public async Task TestMultipleAssets()
    {
      var account = GitHubTestAccount.Get();
      var checker = new ReleaseChecker(account.Name, account.Repository);
      var release = await checker.GetReleaseAsync(account.Release);

      Assert.IsNotNull(release);
      Assert.IsNotNull(release.Assets);

      Assert.AreEqual(release.Assets.Length, 4);

      var asset = release.Assets[2];

      Assert.AreEqual(asset.Name, "robo3t-1.4.0-windows-x86_64-12e54cc.exe");
      Assert.AreEqual(asset.Size, 19019000);
      Assert.AreEqual(asset.DownloadUrl, "https://github.com/Studio3T/robomongo/releases/download/v1.4.0/robo3t-1.4.0-windows-x86_64-12e54cc.exe");
    }

    [Test]
    public void TestNullAccount()
    {
      var account = GitHubTestAccount.Get();
      var checker = new ReleaseChecker(account.Name, account.Repository);

      Assert.IsNull(checker.AccountName);
      Assert.IsNotNull(checker.Repository);

      Assert.ThrowsAsync<System.Net.Http.HttpRequestException>(() => checker.GetReleaseAsync());
    }

    [Test]
    public void TestNullRepository()
    {
      var account = GitHubTestAccount.Get();
      var checker = new ReleaseChecker(account.Name, account.Repository);

      Assert.IsNotNull(checker.AccountName);
      Assert.IsNull(checker.Repository);

      Assert.ThrowsAsync<System.ArgumentException>(() => checker.GetReleaseAsync());
    }

    [Test]
    public void TestNullRelease()
    {
      var account = GitHubTestAccount.Get();
      var checker = new ReleaseChecker(account.Name, account.Repository);

      Assert.IsNotNull(checker.AccountName);
      Assert.IsNotNull(checker.Repository);

      Assert.ThrowsAsync<System.Net.Http.HttpRequestException>(() => checker.GetReleaseAsync(account.Release));
    }

    [Test]
    public void TestUpdateDetection()
    {
      string versionToCheckFor = "v1";

      var account = GitHubTestAccount.Get();
      var checker = new ReleaseChecker(account.Name, account.Repository);

      GitHubRelease latestRelease = null;

      checker.MonitorForUpdates(
        versionToCheckFor,
        (release) => { latestRelease = release; },
        System.Threading.Timeout.Infinite
      );

      var timeToSleep = 5000;

      while (latestRelease == null && timeToSleep > 0)
      {
        Thread.Sleep(1000);
        timeToSleep -= 1000;
      }

      Assert.IsNotNull(latestRelease);
      Assert.AreNotEqual(latestRelease.Version, versionToCheckFor);
    }

    [Test]
    public void TestInvalidUpdateDetectionInterval()
    {
      var account = GitHubTestAccount.Get();
      var checker = new ReleaseChecker(account.Name, account.Repository);

      GitHubRelease latestRelease = null;

      Assert.Throws<ArgumentException>(() =>
      {
        checker.MonitorForUpdates(
          "v1",
          (release) => { },
          0
        );
      });
    }

    [Test]
    public void TestNullUpdateDetectionCallback()
    {
      var account = GitHubTestAccount.Get();
      var checker = new ReleaseChecker(account.Name, account.Repository);

      GitHubRelease latestRelease = null;

      Assert.Throws<ArgumentException>(() =>
      {
        checker.MonitorForUpdates(
          "v1",
          null,
          0
        );
      });
    }
  }
}