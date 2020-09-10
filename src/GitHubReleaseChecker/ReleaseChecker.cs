using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
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
      if (string.IsNullOrWhiteSpace(Repository))
        throw new ArgumentException($"{nameof(Repository)} cannot be null or white space as it is used by GitHub to allow API calls through. See https://developer.github.com/v3/#user-agent-required");

      var apiUrl = GetReleaseApiUrl(releaseId);

      _httpClient.DefaultRequestHeaders.Accept.Clear();

      // https://developer.github.com/v3/#user-agent-required
      _httpClient.DefaultRequestHeaders.Add("User-Agent", Repository);

      var jsonString = await _httpClient.GetStringAsync(apiUrl);

      return GitHubRelease.CreateFromJson(jsonString);
    }

    private Timer _updateTimer;

    public delegate void OnUpdateDetected(GitHubRelease newVersion);

    private class CheckForUpdateParams
    {
      public string CurrentVersion { get; set; }
      public OnUpdateDetected Callback { get; set; }
    }

    private const int _MIN_UPDATE_CHECK_INTERVAL = 60 * 60 * 1000;

    public void MonitorForUpdates(string currentVersion, OnUpdateDetected updateCallback, int timerIntervalMs = 24 * 60 * 60 * 1000)
    {
      if (timerIntervalMs != Timeout.Infinite && timerIntervalMs < _MIN_UPDATE_CHECK_INTERVAL)
        throw new ArgumentException($"{nameof(timerIntervalMs)} ({timerIntervalMs}) must be less than {_MIN_UPDATE_CHECK_INTERVAL}ms in order to avoid throttling limits");

      if (updateCallback == null)
        throw new ArgumentException($"{nameof(updateCallback)} cannot be null");

      if (_updateTimer != null)
        _updateTimer.Dispose();

      var args = new CheckForUpdateParams()
      {
        CurrentVersion = currentVersion,
        Callback = updateCallback
      };

      _updateTimer = new Timer(CheckForUpdate, args, 0, timerIntervalMs);
    }

    private async void CheckForUpdate(object state)
    {
      if (!(state is CheckForUpdateParams))
        throw new ArgumentException($"{nameof(state)} must be of type {nameof(CheckForUpdateParams)} (current is {state.GetType().Name})");

      var args = (CheckForUpdateParams)state;

      var release = await GetReleaseAsync();

      if (args.CurrentVersion != release.Version)
        args.Callback(release);
    }
  }
}
