﻿using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace TMS_task_management_system
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        ApplicationContext db;
        RelayCommand addCommand;
        RelayCommand editCommand;
        RelayCommand deleteCommand;
        IEnumerable<Task> tasks;

        public IEnumerable<Task> Tasks
        {
            get { return tasks; }
            set
            {
                tasks = value;
                OnPropertyChanged("Tasks");
            }
        }

        public ApplicationViewModel()
        {
            db = new ApplicationContext();
            db.Tasks.Load();
            Tasks = db.Tasks.Local.ToBindingList();
        }
        // команда добавления
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand((o) =>
                  {
                      TaskWindow phoneWindow = new TaskWindow(new Task());
                      if (phoneWindow.ShowDialog() == true)
                      {
                          Task task = phoneWindow.Task;
                          db.Tasks.Add(task);
                          db.SaveChanges();
                      }
                  }));
            }
        }
        // команда редактирования
        public RelayCommand EditCommand
        {
            get
            {
                return editCommand ??
                  (editCommand = new RelayCommand((selectedItem) =>
                  {
                      if (selectedItem == null) return;
                      // получаем выделенный объект
                      Task task = selectedItem as Task;

                      Task vm = new Task()
                      {
                          Id_Task = task.Id_Task,
                          Name_Task = task.Name_Task,
                          Description_Task = task.Description_Task,
                          Performers_Task = task.Performers_Task,
                          Date_of_registration_Task = task.Date_of_registration_Task,
                          Status_Task = task.Status_Task,
                          Planned_time_Task = task.Planned_time_Task,
                          Actual_time_Task = task.Actual_time_Task,
                          Date_of_completion_Task = task.Date_of_completion_Task
                      };
                      TaskWindow taskWindow = new TaskWindow(vm);


                      if (taskWindow.ShowDialog() == true)
                      {
                          // получаем измененный объект
                          task = db.Tasks.Find(taskWindow.Task.Id_Task);
                          if (task != null)
                          {
                              task.Name_Task = taskWindow.Task.Name_Task;
                              task.Description_Task = taskWindow.Task.Description_Task;
                              task.Performers_Task = taskWindow.Task.Performers_Task;
                              task.Date_of_registration_Task = taskWindow.Task.Date_of_registration_Task;
                              task.Status_Task = taskWindow.Task.Status_Task;
                              task.Planned_time_Task = taskWindow.Task.Planned_time_Task;
                              task.Actual_time_Task = taskWindow.Task.Actual_time_Task;
                              task.Date_of_completion_Task = taskWindow.Task.Date_of_completion_Task;

                              db.Entry(task).State = EntityState.Modified;
                              db.SaveChanges();
                          }
                      }
                  }));
            }
        }
        // команда удаления
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand((selectedItem) =>
                  {
                      if (selectedItem == null) return;
                      // получаем выделенный объект
                      Task task = selectedItem as Task;
                      db.Tasks.Remove(task);
                      db.SaveChanges();
                  }));
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