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
                              task.All_Actual_time_Task = task.Actual_time_Task;
                              task.All_Planned_time_Task = task.Planned_time_Task;
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
                            //запоминаем первоначальные данные
                            int old_Actual_time_Task = task.Actual_time_Task;
                              //запоминаем первоначальные данные
                            int old_Planned_time_Task = task.Planned_time_Task;

                            task.Name_Task = taskWindow.Task.Name_Task;
                            task.Description_Task = taskWindow.Task.Description_Task;
                            task.Performers_Task = taskWindow.Task.Performers_Task;
                            task.Date_of_registration_Task = taskWindow.Task.Date_of_registration_Task;
                            task.Status_Task = taskWindow.Task.Status_Task;
                            task.Planned_time_Task = taskWindow.Task.Planned_time_Task;
                            task.Actual_time_Task = taskWindow.Task.Actual_time_Task;
                            task.Date_of_completion_Task = taskWindow.Task.Date_of_completion_Task;

                            task.All_Actual_time_Task += task.Actual_time_Task - old_Actual_time_Task;
                            task.All_Planned_time_Task += task.Planned_time_Task - old_Planned_time_Task;

                            bool plannedtime = int.TryParse(task.Planned_time_Task.ToString(), out int planned_time);
                            bool actualtime = int.TryParse(task.Actual_time_Task.ToString(), out int actual_time);
                            bool name = String.IsNullOrEmpty(task.Name_Task);
                            bool description = String.IsNullOrEmpty(task.Description_Task);
                            bool performers = String.IsNullOrEmpty(task.Performers_Task);
                              
                            if (name != true && description != true && performers != true && task.Date_of_registration_Task != null
                             && plannedtime != false && actualtime != false)
                            {

                                  //флаг нужен для работы цикла
                                  bool flag = true;
                                  //получаем нашу подзадачу для инициализации полей его родителя
                                  int change_Actual_time_Task = task.Actual_time_Task - old_Actual_time_Task;
                                  int change_Planned_time_Task = task.Planned_time_Task - old_Planned_time_Task;
                                  Task children_task = task;
                                  // создаем переменную родитель, для того, чтобы изменить у родителя поля Actual_time_Task и All_Planned_time_Task
                                  Task parent_task = new Task();
                                  //бесконечно обращаемся в цикле, чтобы изменить у каждого родители поля Actual_time_Task и All_Planned_time_Task, так как возможно вложенность подзадач
                                  while (flag == true)
                                  {

                                      if (children_task.Ref_Task != 0)
                                      {
                                          // получаем объект родитель
                                          parent_task = db.Tasks.Where(c => c.Id_Task == children_task.Ref_Task).FirstOrDefault();
                                          // изменяем у него вычисляемые поля
                                          parent_task.All_Actual_time_Task += change_Actual_time_Task;
                                          parent_task.All_Planned_time_Task += change_Planned_time_Task;
                                          // необоходимо для того, чтобы узнать есть ли вложенность подзадач, чтобы дальше по дереву изменить у всех вычисляемые поля
                                          children_task = parent_task;
                                      }
                                      // как только родитель и ребенок это один тот же объект, прекращаем цикл.
                                      else if (children_task.Id_Task == parent_task.Id_Task)
                                      {
                                          flag = false;
                                      }
                                      else if(children_task.Ref_Task == 0)
                                      {
                                          flag = false;
                                      }

                                  }

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


                      //флаг нужен для работы цикла
                      bool flag = true;
                      //получаем нашу подзадачу для инициализации полей его родителя
                      Task children_task = taskdelete;
                      // создаем переменную родитель, для того, чтобы изменить у родителя поля Actual_time_Task и All_Planned_time_Task
                      Task parent_task = new Task();
                      //бесконечно обращаемся в цикле, чтобы изменить у каждого родители поля Actual_time_Task и All_Planned_time_Task, так как возможно вложенность подзадач
                      while (flag == true)
                      {

                          if (children_task.Ref_Task != 0)
                          {
                              // получаем объект родитель
                              parent_task = db.Tasks.Where(c => c.Id_Task == children_task.Ref_Task).FirstOrDefault();
                              // изменяем у него вычисляемые поля
                              parent_task.All_Actual_time_Task -= taskdelete.Actual_time_Task;
                              parent_task.All_Planned_time_Task -= taskdelete.Planned_time_Task;
                              // необоходимо для того, чтобы узнать есть ли вложенность подзадач, чтобы дальше по дереву изменить у всех вычисляемые поля
                              children_task = parent_task;
                          }
                          // как только родитель и ребенок это один тот же объект, прекращаем цикл.
                          else if (children_task.Id_Task == parent_task.Id_Task)
                          {
                              flag = false;
                          }

                      }


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

                          subtask.All_Actual_time_Task = subtask.Actual_time_Task;
                          subtask.All_Planned_time_Task = subtask.Planned_time_Task;

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

                              //флаг нужен для работы цикла
                              bool flag = true;
                              //получаем нашу подзадачу для инициализации полей его родителя
                              Task children_task = subtask;
                              // создаем переменную родитель, для того, чтобы изменить у родителя поля Actual_time_Task и All_Planned_time_Task
                              Task parent_task  = new Task();
                              //бесконечно обращаемся в цикле, чтобы изменить у каждого родители поля Actual_time_Task и All_Planned_time_Task, так как возможно вложенность подзадач
                              while (flag == true)
                              {

                                  if (children_task.Ref_Task != 0)
                                  {
                                      // получаем объект родитель
                                      parent_task = db.Tasks.Where(c => c.Id_Task == children_task.Ref_Task).FirstOrDefault();
                                      // изменяем у него вычисляемые поля
                                      parent_task.All_Actual_time_Task += subtask.Actual_time_Task;
                                      parent_task.All_Planned_time_Task += subtask.Planned_time_Task;
                                      // необоходимо для того, чтобы узнать есть ли вложенность подзадач, чтобы дальше по дереву изменить у всех вычисляемые поля
                                      children_task = parent_task;                  
                                  }
                                  // как только родитель и ребенок это один тот же объект, прекращаем цикл.
                                  else if(children_task.Id_Task == parent_task.Id_Task)
                                  {
                                      flag = false;
                                  }

                              }

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
