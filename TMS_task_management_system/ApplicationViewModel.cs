using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace TMS_task_management_system
{
    public class ApplicationViewModel : INotifyPropertyChanged 
    {

        public delegate void AccountHandler(Task task);
        // Определение события для AddCommand добавления задачи
        public event AccountHandler NotifyAddCommand;
        // Определение события для EditCommand изменения задачи
        public event AccountHandler NotifyEditCommand;
        // Определение события для DeleteCommand удаления задачи
        public event AccountHandler NotifyDeleteCommand;
        // Определение события для AddSubCommand добавлени подзадачи
        public event AccountHandler NotifyAddSubCommand;              

        //подключение к базе данных
        ApplicationContext db;
        //команда добавления новых заданий
        RelayCommand addCommand;
        //команда изменения задачи
        RelayCommand editCommand;
        //команда удаления задачи
        RelayCommand deleteCommand;
        //команда добавления подзадачи
        RelayCommand addsubCommand;

        Commands command = new Commands();

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
                  (addCommand = new RelayCommand((o) => NotifyAddCommand?.Invoke(command.AddCommand())
                  ));
            }
        }
        // команда редактирования
        public RelayCommand EditCommand
        {
            get
            {
                bool IsErrorTask = false;
                Task task = new Task();
                return editCommand ??
                  (editCommand = new RelayCommand((selectedItem) =>
                  {
                      task = command.EditCommand(selectedItem, ref IsErrorTask);
                      if (IsErrorTask == false)
                          NotifyEditCommand?.Invoke(task);
                  }));
                
                
            }
        }
        // команда удаления
        public RelayCommand DeleteCommand
        {
            get
            {

                bool IsErrorTask = false;
                Task task = new Task();
                return deleteCommand ??
                  (deleteCommand = new RelayCommand((selectedItem) =>
                  {
                      task = command.DeleteCommand(selectedItem, ref IsErrorTask);
                      if (IsErrorTask == false)
                          NotifyDeleteCommand?.Invoke(task);
                  }));
            }
        }

        // команда добавления подзадачи
        public RelayCommand AddSubCommand
        {
            
            get
            {

                bool IsErrorTask = false;
                Task task = new Task();
                return addsubCommand ??
                  (addsubCommand = new RelayCommand((selectedItem) =>
                  {
                      task = command.AddSubCommand(selectedItem, ref IsErrorTask);
                      if (IsErrorTask == false)
                          NotifyAddSubCommand?.Invoke(task);
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
