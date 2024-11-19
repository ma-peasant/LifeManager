using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeManager.Utils;

namespace LifeManager.Models
{
    public partial class  NoteBase : ObservableObject
    {
       
        [ObservableProperty]
        private string? _title;
        /// <summary>
        /// 所在的文件路径
        /// </summary>
        [ObservableProperty]
        private string? _path;

        [ObservableProperty]
        private bool? _isEnabled = false;
      
        [RelayCommand]
        public void RenameCommand(string newName)
        {
            FileUtils.RenameFile(this.Path, newName + ".txt");
            string directory = System.IO.Path.GetDirectoryName(this.Path);
            // 构造新文件的完整路径
            string newFilePath = System.IO.Path.Combine(directory, newName + ".txt");
            this.Path = newFilePath;
        }
    }
}
