using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TMS_task_management_system
{
    class Commands
    {

        //подключение к базе данных
        ApplicationContext db;

        public Commands()
        {
            db = new ApplicationContext();
            db.Tasks.Load();
            Tasks = db.Tasks.Local.ToBindingList();

        }

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


        public Task AddCommand()
        {
            Task task = new Task();
            TaskWindow taskWindow = new TaskWindow(new Task());
            taskWindow.Task.Status_Task = "Назначена";
            if (taskWindow.ShowDialog() == true)
            {
                task = taskWindow.Task;

                task.All_Actual_time_Task = task.Actual_time_Task;
                task.All_Planned_time_Task = task.Planned_time_Task;
                db.Tasks.Add(task);
                db.SaveChanges();
            }
            return task;
        }

        public Task EditCommand(object selectedItem, ref bool IsErrorTask)
        {
            // получаем выделенный объект
            Task selectedtask = selectedItem as Task;

            Task task = selectedtask;

            Task oldtask = new Task()
            {
                Id_Task = selectedtask.Id_Task,
                Name_Task = selectedtask.Name_Task,
                Description_Task = selectedtask.Description_Task,
                Performers_Task = selectedtask.Performers_Task,
                Date_of_registration_Task = selectedtask.Date_of_registration_Task,
                Status_Task = selectedtask.Status_Task,
                Planned_time_Task = selectedtask.Planned_time_Task,
                Actual_time_Task = selectedtask.Actual_time_Task,
                Date_of_completion_Task = selectedtask.Date_of_completion_Task,
                SubTasks = selectedtask.SubTasks
                
            };


            TaskWindow taskWindow = new TaskWindow(oldtask);
            if (taskWindow.ShowDialog() == true)
            {
                // получаем измененный объект
                task = db.Tasks.Find(taskWindow.Task.Id_Task);

                
                
                //запоминаем первоначальные данные
                int old_Actual_time_Task = task.Actual_time_Task;
                //запоминаем первоначальные данные
                int old_Planned_time_Task = task.Planned_time_Task;

                //запоминаем статус задачи
                string old_status_task = task.Status_Task;


                if (old_status_task != taskWindow.Task.Status_Task)
                {
                    if (old_status_task == "Назначена" && taskWindow.Task.Status_Task == "Завершена")
                    {
                        MessageBox.Show(Resource1.Status_Assigned_СompletedError, Resource1.HeadingError);
                        
                        IsErrorTask = true;
                        return task;
                    }

                    

                    
                    else if(taskWindow.Task.Status_Task == "Завершена")
                    {

                        if (CanChangeStatus(selectedtask))
                            ChangeStatus(selectedtask);

                    }
                }


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
                    else if (children_task.Ref_Task == 0)
                    {
                        flag = false;
                    }

                }

                db.Entry(task).State = EntityState.Modified;
                db.SaveChanges();
                IsErrorTask = false;
                return task;

                    
             }

            IsErrorTask = true;
            return task;
        }


        public Task DeleteCommand(object selectedItem, ref bool IsErrorTask)
        {
            // получаем выделенный объект
            Task task = selectedItem as Task;
            //проверяем есть ли у задачи подзадачи, тем самым запрещая удалять
            if (task.SubTasks.Count != 0)
            {
                MessageBox.Show(Resource1.DeleteTask, Resource1.HeadingError);
                IsErrorTask = true;
                return task;
            }

            //ищем задачу на удаление
            var taskdelete = db.Tasks.Where(c => c.Id_Task == task.Id_Task).First();


            //флаг нужен для работы цикла
            bool flag = true;
            //получаем нашу подзадачу для инициализации полей его родителя
            Task children_task = taskdelete;
            // создаем переменную родитель, для того, чтобы изменить у родителя поля Actual_time_Task и All_Planned_time_Task
            Task parent_task = new Task();
            parent_task = children_task;
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
            IsErrorTask = false;
            return taskdelete;
        }

        public Task AddSubCommand(object selectedItem, ref bool IsErrorTask)
        {
            // получаем выделенный объект
            Task task = selectedItem as Task;
            TaskWindow window = new TaskWindow(new Task());
            window.Task.Status_Task = "Назначена";
            if (window.ShowDialog() == true)
            {
                Task subtask = window.Task;

                subtask.All_Actual_time_Task = subtask.Actual_time_Task;
                subtask.All_Planned_time_Task = subtask.Planned_time_Task;

                subtask.Ref_Task = task.Id_Task;

                task.SubTasks.Add(subtask);

                //флаг нужен для работы цикла
                bool flag = true;
                //получаем нашу подзадачу для инициализации полей его родителя
                Task children_task = subtask;
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
                        parent_task.All_Actual_time_Task += subtask.Actual_time_Task;
                        parent_task.All_Planned_time_Task += subtask.Planned_time_Task;
                        // необоходимо для того, чтобы узнать есть ли вложенность подзадач, чтобы дальше по дереву изменить у всех вычисляемые поля
                        children_task = parent_task;
                    }
                    // как только родитель и ребенок это один тот же объект, прекращаем цикл.
                    else if (children_task.Id_Task == parent_task.Id_Task)
                    {
                        flag = false;
                    }

                }

                db.Tasks.Add(subtask);
                db.SaveChanges();
                IsErrorTask = false;
                return subtask;
            }

            IsErrorTask = true;
            return task;
        }

        private static bool CanChangeStatus(Task task)
        {
            //if (task.SubTasks.Count == 0)
            //    return true;

            foreach(var subtask in task.SubTasks)
            {
                if(subtask.Status_Task == "Назначена")
                    return false;

                if(!CanChangeStatus(subtask))
                    return false;
            }

            return true;
        }

        private static void ChangeStatus(Task task)
        {
            task.Status_Task = "Завершена";

            foreach(var subtask in task.SubTasks)
            {
                //subtask.Status_Task = "Завершена";
                ChangeStatus(subtask);
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
