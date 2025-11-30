using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace DeepBattlerPlugin
{
    public partial class AgentOutputWindow : Window
    {
        private FileSystemWatcher _fileWatcher;
        private readonly string _outputFilePath;
        private readonly DispatcherTimer _updateTimer;

        public AgentOutputWindow()
        {
            InitializeComponent();
            
            // Path to agent output file (Python agent will write here)
            _outputFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "DeepBattler",
                "Agent",
                "real_time_caller",
                "agent_output.txt"
            );

            // Ensure directory exists
            var directory = Path.GetDirectoryName(_outputFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Set window position (bottom-left corner - game interface area)
            this.Left = 20;
            this.Top = SystemParameters.PrimaryScreenHeight - this.Height - 100;

            // Setup file watcher
            SetupFileWatcher();

            // Setup update timer (check more frequently for real-time updates)
            _updateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.5)  // Check every 0.5 seconds for faster updates
            };
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();

            // Load initial content
            LoadAgentOutput();

            // Make window draggable
            this.MouseDown += (sender, e) =>
            {
                if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                    this.DragMove();
            };
        }

        private void SetupFileWatcher()
        {
            try
            {
                var directory = Path.GetDirectoryName(_outputFilePath);
                if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
                    return;

                _fileWatcher = new FileSystemWatcher(directory)
                {
                    Filter = Path.GetFileName(_outputFilePath),
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
                };

                _fileWatcher.Changed += FileWatcher_Changed;
                _fileWatcher.Created += FileWatcher_Changed;
                _fileWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            // Use dispatcher to update UI from file watcher thread
            Dispatcher.Invoke(() => LoadAgentOutput());
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            LoadAgentOutput();
        }

        private void LoadAgentOutput()
        {
            try
            {
                if (File.Exists(_outputFilePath))
                {
                    // Read file with retry (in case file is being written)
                    string content = null;
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            content = File.ReadAllText(_outputFilePath);
                            break;
                        }
                        catch (IOException)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                    }

                    if (!string.IsNullOrEmpty(content))
                    {
                        OutputTextBlock.Text = content.Trim();
                        StatusTextBlock.Text = $"Last updated: {DateTime.Now:HH:mm:ss}";
                        StatusTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        OutputTextBlock.Text = "No output from agent yet...";
                        StatusTextBlock.Text = "Status: Waiting";
                        StatusTextBlock.Foreground = new SolidColorBrush(Colors.Gray);
                    }
                }
                else
                {
                    OutputTextBlock.Text = "Waiting for agent to start...\n\nMake sure the Python agent is running and writing to:\n" + _outputFilePath;
                    StatusTextBlock.Text = "Status: No file";
                    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Orange);
                }
            }
            catch (Exception ex)
            {
                OutputTextBlock.Text = $"Error reading output: {ex.Message}";
                StatusTextBlock.Text = "Status: Error";
                StatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide(); // Hide instead of close, so plugin can show it again
        }

        protected override void OnClosed(EventArgs e)
        {
            _fileWatcher?.Dispose();
            _updateTimer?.Stop();
            base.OnClosed(e);
        }
    }
}

