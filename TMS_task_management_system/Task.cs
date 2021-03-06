﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Windows;

namespace TMS_task_management_system
{
    public class Task : INotifyPropertyChanged 
    {

        //Имя задачи
        private string name_task;

        //Описание задачи
        private string description_task;

        //Исполнители задачи
        private string performers_task;

        //Дата регистрации задачи
        private string date_of_registration_task;

        //Статус задачи
        private string status_task;

        //Плановая трудоемкость задачи
        private int planned_time_task;

        //сколько всего фактическая трудоемкость задачи
        private int all_planned_time_task;

        //Фактическое время выполнения задачи
        private int actual_time_task;

        //сколько всего плановая трудоемкость задачи
        private int all_actual_time_task;

        //Дата завершения задачи
        private string date_of_completion_task;

        //id задачи, на который ссылается подзадача
        public int Ref_Task { get; set; }
        
        
      


        //коллекция подзадач
        [NotMapped]
        public ObservableCollection<Task> SubTasks { get; set; } = new ObservableCollection<Task>();

        //id задачи
        [Key]
        public int Id_Task { get; set; }

        public string Name_Task
        {
            get { return name_task; }
            set
            {
                
                if (String.IsNullOrWhiteSpace(value))
                {
                    throw new ApplicationException();

                }
                else
                    name_task = value;
 
            }
        }

        public string Description_Task
        {
            get { return description_task; }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    throw new ApplicationException();

                }
                else
                    description_task = value;
                
            }
        }

        public string Performers_Task
        {
            get { return performers_task; }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    throw new ApplicationException();

                }
                else
                    performers_task = value;
                
            }
        }

        public string Date_of_registration_Task
        {
            get { return date_of_registration_task; }
            set
            {
                date_of_registration_task = value;
                OnPropertyChanged("Date_of_registration_Task");
            }
        }

        public string Status_Task
        {
            get { return status_task; }
            set
            {
                status_task = value;
                OnPropertyChanged("Status_Task");
            }
        }

       

        public int Planned_time_Task
        {
            get { return planned_time_task; }
            set
            {
                planned_time_task = value;
                OnPropertyChanged("Planned_time_Task");
            }
        }

        public int All_Planned_time_Task
        {
            get { return all_planned_time_task; }
            set
            {
                all_planned_time_task = value;
                OnPropertyChanged("All_Planned_time_Task");
            }
        }

        public int Actual_time_Task
        {
            get { return actual_time_task; }
            set
            {
                actual_time_task = value;
                OnPropertyChanged("Actual_time_Task");
            }
        }

        public int All_Actual_time_Task
        {
            get { return all_actual_time_task; }
            set
            {
                all_actual_time_task = value;
                OnPropertyChanged("All_Actual_time_Task");
            }
        }

        public string Date_of_completion_Task
        {
            get { return date_of_completion_task; }
            set
            {
                date_of_completion_task = value;
                OnPropertyChanged("Date_of_completion_Task");
            }
        }

        
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
