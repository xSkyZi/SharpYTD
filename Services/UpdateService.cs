using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpYTDWPF.Services
{

    public interface IUpdateService
    {
        void CheckFiles();
    }
    public class UpdateService : IUpdateService
    {
        public async void CheckFiles()
        {
            string baseDir = Directory.GetCurrentDirectory();

            if (!File.Exists(baseDir + "\\yt-dlp.exe"))
            {
                DialogResult result = MessageBox.Show("yt-dlp.exe is missing! Would you like to get it automatically downloaded? (Warning, yt-dlp is required for the application to work. If you don't want the app to automatically download, download the latest release of yt-dlp.exe from https://github.com/yt-dlp/yt-dlp/releases and place it alongside application files)", "SharpYTD", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) DownloadYtdlp();
            }

            if (!File.Exists(baseDir + "\\ffmpeg.exe"))
            {
                DialogResult result = MessageBox.Show("ffmpeg.exe is missing! Would you like to get it automatically downloaded? (Warning, FFmpeg is required for the application to work. If you don't want the app to automatically download, download the latest release of FFmpeg from https://github.com/ffbinaries/ffbinaries-prebuilt/releases/tag/v6.1 and place the content of the zip file alongside application files)", "SharpYTD", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes) DownloadFFmpeg();
            }
        }

        public async void DownloadYtdlp()
        {
            try
            {
                await YoutubeDLSharp.Utils.DownloadYtDlp();
                MessageBox.Show("Finished downloading yt-dlp!", "SharpYTD", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while downloading yt-dlp!", "SharpYTD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async void DownloadFFmpeg()
        {
            try
            {
                await YoutubeDLSharp.Utils.DownloadFFmpeg();
                MessageBox.Show("Finished downloading FFmpeg!", "SharpYTD", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while downloading FFmpeg!", "SharpYTD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
