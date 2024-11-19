using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeManager.Tables
{
    public class ToDoItem : Table
    {
        private bool _isChecked;
        private string _content = "";

        private string _createDate = "";
        public ToDoItem()
        {
        }

        [Column("todo_ischecked")]
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value != _isChecked)
                {
                    //SetProperty内部会自动触发OnPropertyChanged事件
                    SetProperty(ref _isChecked, value);
                }

            }
        }
        [Column("todo_content")]
        public string Content
        {
            get => _content;
            set
            {
                if (value != _content)
                {

                    SetProperty(ref _content, value);
                }

            }
        }
        [Column("todo_createdate")]
        public string CreateDate
        {
            get => _createDate;
            set
            {
                if (value != _createDate)
                {

                    SetProperty(ref _createDate, value);
                }

            }
        }
    }
}
