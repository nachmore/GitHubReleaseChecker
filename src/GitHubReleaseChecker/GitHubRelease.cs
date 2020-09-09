using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GitHubReleaseChecker
{
  public class GitHubRelease
  {
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; }

    [JsonPropertyName("tag_name")]
    public string Version { get; set; }

    [JsonPropertyName("prerelease")]
    public bool PreRelease { get; set; }

    [JsonPropertyName("assets")]
    public GitHubReleaseAsset[] Assets {get; set;}

    [JsonPropertyName("body")]
    public string ReleaseNotes { get; set; }

    internal static GitHubRelease CreateFromJson(string jsonString)
    {
      var rv = JsonSerializer.Deserialize<GitHubRelease>(jsonString);

      return rv;
    }

    public void LaunchDownload(int asset_id = -1)
    {
      // we have no way of knowing which asset to download, so if none is specified and there
      // is more than 1 then just ignore the download request. If debugging, throw and exception
      // to make things obvious to our caller.
      if (Assets.Length > 1 && asset_id == -1)
      {
        if (System.Diagnostics.Debugger.IsAttached)
        {
          throw new ArgumentException($"When there are more than 1 assets attahed to a release (in this case: {Assets.Length}) you must specify the id of the one to download");
        }

        return;
      }

      var asset = Assets[asset_id == -1 ? 0 : asset_id];

      var downloadProcess = new Process();

      downloadProcess.StartInfo.UseShellExecute = true;
      downloadProcess.StartInfo.FileName = asset.DownloadUrl;

      downloadProcess.Start();
    }
  }
}