using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeManager.Utils;

namespace LifeManager.Models
{
    //基础数据类的同时是一个动态绑定的对象
    public partial class  NoteBase : ObservableObject
    {
       
        [ObservableProperty]
        private string? _title;             //文件名
        /// <summary>
        /// 所在的文件路径
        /// </summary>
        [ObservableProperty]
        private string? _path;           //文件路径

        [ObservableProperty]
        private bool? _isEnabled = false;  //双击修改文件名需要用到的
      
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
