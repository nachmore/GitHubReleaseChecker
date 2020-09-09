using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace GitHubReleaseChecker
{
  public class ReleaseChecker
  {

    //https://api.github.com/repos/microsoft/PowerToys/releases/latest

    private string _GITHUB_RELEASES_API = "https://api.github.com/repos/{0}/{1}/releases/{2}";

    private readonly HttpClient _httpClient = new HttpClient();

    public string AccountName { get; set; }
    public string Repository { get; set; }

    public ReleaseChecker(string accountName, string repository)
    {
      AccountName = accountName;
      Repository = repository;
    }

    public string GetReleaseApiUrl(string releaseId = "latest")
    {
      return string.Format(_GITHUB_RELEASES_API, AccountName, Repository, releaseId);
    }

    public async Task<GitHubRelease> GetReleaseAsync(string releaseId = "latest")
    {
      var apiUrl = GetReleaseApiUrl(releaseId);

      _httpClient.DefaultRequestHeaders.Accept.Clear();
      _httpClient.DefaultRequestHeaders.Add("User-Agent", "GitHub Release Checker");

      var jsonString = await _httpClient.GetStringAsync(apiUrl);

      return GitHubRelease.CreateFromJson(jsonString);
    }
  }
}
