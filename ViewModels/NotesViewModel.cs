using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LifeManager.Message;
using LifeManager.Models;
using LifeManager.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace LifeManager.ViewModels
{
    public partial class NotesViewModel : ViewModelBase
    {
        private ObservableCollection<NoteBase> _noteTitles;
        public NotesViewModel()
        {
            // We can use this to add some items for the designer. 
            // You can also use a DesignTime-ViewModel
            if (Design.IsDesignMode)
            {
                
            }
            this.IsEditing = false;
            NoteTitles  = new ObservableCollection<NoteBase>();
            LoadData();
            WeakReferenceMessenger.Default.Register<IsAllMessage>(this, (r, m) =>
            {
                LoadData();
            });
        }

        private void LoadData()
        {
            //从指定文件夹读取日记
            NoteTitles.Clear();
            List<string> files =  FileUtils.GetAllTxtFiles();
            foreach (string file in files) {
                NoteBase noteBase = new NoteBase()
                {
                    Path = new FileInfo(file).FullName,
                    Title = new FileInfo(file).Name.Replace(".txt", ""),
                };
                noteBase.PropertyChanged += NoteBase_PropertyChanged;
                NoteTitles.Add(noteBase);
            }
            if (NoteTitles.Count > 0)
            {
                this.Content = FileUtils.ReadTxtFile(NoteTitles[0].Path);
            }
        }

        private void NoteBase_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NoteBase.Path))
            {
                this.Content = FileUtils.ReadTxtFile((sender as NoteBase).Path);
            }
        }

        public ObservableCollection<NoteBase> NoteTitles
        {
            get { return _noteTitles; }
            set { SetProperty(ref _noteTitles, value);} 
        }

        [ObservableProperty]
        public string? _content;

        [ObservableProperty]
        public NoteBase? selectNote;

        [ObservableProperty]
        public bool? isEditing;

        [RelayCommand]
        public void SaveContent()
        {
            FileUtils.SaveTextToFile(this.Content, this.SelectNote.Path);
        }

        [RelayCommand]
        public void NewFile()
        {
            // 获取当前日期并创建对应的目录路径
            string directoryPath = FileUtils.savePath + @$"\{Common.SelectedDateTime}";
            // 查找已有文件中是否存在重复的名称，自动生成新的 Note 名
            int noteNumber = 1;
            string newNoteTitle;

            do
            {
                newNoteTitle = $"Note-{noteNumber}";
                noteNumber++;
            }
            while (NoteTitles.Any(note => note.Title.Equals(newNoteTitle, StringComparison.OrdinalIgnoreCase)));

            // 设置文件路径和标题
            string newNotePath = @$"{directoryPath}\{newNoteTitle}.txt";

            // 向 NoteTitles 添加新项
            NoteTitles.Add(new NoteBase()
            {
                Path = newNotePath,
                Title = newNoteTitle,
            });

            // 确保目录存在
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            // 创建文件
            FileUtils.CreateFile(newNotePath);
        }
    }
}
