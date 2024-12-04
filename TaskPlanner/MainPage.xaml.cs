using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text;

namespace TaskPlanner
{
    public partial class MainPage : ContentPage
    {
        private const string LocalFileName = "tasks.json";
        private readonly string _localFilePath;

        public ObservableCollection<TaskItem> Tasks { get; set; } = new();

        public MainPage()
        {
            InitializeComponent();
            _localFilePath = Path.Combine(FileSystem.AppDataDirectory, LocalFileName);
            TaskListView.ItemsSource = Tasks;
            LoadTasksFromFile();
        }

        public class TaskItem
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Priority { get; set; }
            public DateTime DueDate { get; set; }
        }

        private void LoadTasksFromFile()
        {
            try
            {
                if (File.Exists(_localFilePath))
                {
                    var json = File.ReadAllText(_localFilePath);
                    var tasks = JsonConvert.DeserializeObject<ObservableCollection<TaskItem>>(json);
                    if (tasks != null)
                    {
                        foreach (var task in tasks)
                            Tasks.Add(task);
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Failed to load tasks: {ex.Message}", "OK").Wait();
            }
        }

        private void SaveTasksToFile()
        {
            try
            {
                var json = JsonConvert.SerializeObject(Tasks, Formatting.Indented);
                File.WriteAllText(_localFilePath, json);
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Failed to save tasks: {ex.Message}", "OK").Wait();
            }
        }

        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TaskNameEntry.Text) || PriorityPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            var newTask = new TaskItem
            {
                Name = TaskNameEntry.Text,
                Description = TaskDescriptionEditor.Text,
                Priority = PriorityPicker.SelectedItem.ToString(),
                DueDate = DueDatePicker.Date
            };

            Tasks.Add(newTask);
            SaveTasksToFile();
        }
    }
}
