using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeManager.Tables
{
    public class PayItem : Table
    {
        private double _amount;
        private string _content;
        private string _date;
        private bool _type; //支出false/收入true。   
        public PayItem()
        {
        }

        [Column("pay_amount")]
        public double Amount {
            get => _amount;
            set
            {
                if (value != _amount)
                {

                    SetProperty(ref _amount, value);
                }

            }
        }
      
        [Column("pay_content")]
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
        [Column("pay_date")]
        public string Date
        {
            get => _date;
            set
            {
                if (value != _date)
                {

                    SetProperty(ref _date, value);
                }

            }
        }

        [Column("pay_type")]
        public bool Type
        {
            get => _type;
            set
            {
                if (value != _type)
                {

                    SetProperty(ref _type, value);
                }

            }
        }
    }
}
