using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

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
                      TaskWindow taskWindow = new TaskWindow(new Task());
                      if (taskWindow.ShowDialog() == true)
                      {
                          Task task = taskWindow.Task;

                          bool plannedtime = int.TryParse(task.Planned_time_Task.ToString(), out int planned_time);
                          bool actualtime = int.TryParse(task.Actual_time_Task.ToString(), out int actual_time);
                          bool name = String.IsNullOrEmpty(task.Name_Task);
                          bool description = String.IsNullOrEmpty(task.Description_Task);
                          bool performers = String.IsNullOrEmpty(task.Performers_Task);

                          if (name != true && description != true && performers != true && task.Date_of_registration_Task != null
                            && plannedtime != false && actualtime != false)
                          {
                              db.Tasks.Add(task);
                              db.SaveChanges();
                              NotifyAddCommand?.Invoke(task);
                          }
                          else MessageBox.Show("Проверьте правильность вводимых данных", "Ввод неккоректных данных");
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



                    Task oldtask = new Task()
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
                    TaskWindow taskWindow = new TaskWindow(oldtask);


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


                            bool plannedtime = int.TryParse(task.Planned_time_Task.ToString(), out int planned_time);
                            bool actualtime = int.TryParse(task.Actual_time_Task.ToString(), out int actual_time);
                            bool name = String.IsNullOrEmpty(task.Name_Task);
                            bool description = String.IsNullOrEmpty(task.Description_Task);
                            bool performers = String.IsNullOrEmpty(task.Performers_Task);
                              
                            if (name != true && description != true && performers != true && task.Date_of_registration_Task != null
                             && plannedtime != false && actualtime != false)
                            {
                                db.Entry(task).State = EntityState.Modified;
                                db.SaveChanges();

                                NotifyEditCommand?.Invoke(task);
                            }

                            else MessageBox.Show("Проверьте правильность вводимых данных", "Ввод неккоректных данных");
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
                    //проверяем есть ли у задачи подзадачи, тем самым запрещая удалять
                    if (task.SubTasks.Count != 0) return;

                    //ищем задачу на удаление
                    var taskdelete = db.Tasks.Where(c => c.Id_Task == task.Id_Task).First();
                    db.Tasks.Remove(taskdelete);
                    db.SaveChanges();
                    NotifyDeleteCommand?.Invoke(task); 

                  }));
            }
        }

        // команда добавления подзадачи
        public RelayCommand AddSubCommand
        {
            get
            {
                return addsubCommand ??
                  (addsubCommand = new RelayCommand((selectedItem) =>
                  {
                      if (selectedItem == null) return;
                      // получаем выделенный объект
                      Task task = selectedItem as Task;
                      TaskWindow window = new TaskWindow(new Task());
                      if (window.ShowDialog() == true)
                      {
                          Task subtask = window.Task;

                          bool plannedtime = int.TryParse(subtask.Planned_time_Task.ToString(), out int planned_time);
                          bool actualtime = int.TryParse(subtask.Actual_time_Task.ToString(), out int actual_time);
                          bool name = String.IsNullOrEmpty(subtask.Name_Task);
                          bool description = String.IsNullOrEmpty(subtask.Description_Task);
                          bool performers = String.IsNullOrEmpty(subtask.Performers_Task);

                          if (name != true && description != true && performers != true && subtask.Date_of_registration_Task != null
                           && plannedtime != false && actualtime != false)
                          {
                              subtask.Ref_Task = task.Id_Task;
                              task.SubTasks.Add(subtask);
                              db.Tasks.Add(subtask);
                              db.SaveChanges();
                          }
                          else MessageBox.Show("Проверьте правильность вводимых данных", "Ввод неккоректных данных");
                      }
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
