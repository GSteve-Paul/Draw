using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Draw
{
    public class App : Application
    {
        public App() { }

        [STAThread]
        public static void Main()
        {
            App app = new App();
            app.Run(new MainWindow());
        }
    }

    public partial class MainWindow : Window
    {
        private DrawAlgo _drawAlgo;

        private List<string> _candidateNames;
        private List<string> _prizeName;
        private List<string> _prizeContent;
        private List<int> _prizeAmount;

        private string _candidateNameFilePath = "./candidate_data.txt";
        private string _prizeNamePath = "./prize_name.txt";
        private string _prizeContentPath = "./prize_content.txt";
        private string _prizeAmountPath = "./prize_amount.txt";

        private int nbCandidates;
        private int nbPrize;

        private int nbButtonClick;


        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void ReadCandidateNames()
        {
            StreamReader streamReader = new StreamReader(_candidateNameFilePath);
            String eachName;
            _candidateNames = new List<string>();
            while (!streamReader.EndOfStream)
            {
                eachName = streamReader.ReadLine();
                _candidateNames.Add(eachName);
            }
            streamReader.Close();
        }

        private void ReadPrizeData()
        {
            StreamReader streamReader = new StreamReader(_prizeNamePath);
            string eachName;
            _prizeName = new List<string>();
            while (!streamReader.EndOfStream)
            {
                eachName = streamReader.ReadLine();
                _prizeName.Add(eachName);
            }
            nbPrize = _prizeName.Count;
            streamReader.Close();

            streamReader = new StreamReader(_prizeContentPath);
            string eachContent;
            _prizeContent = new List<string>();
            while (!streamReader.EndOfStream)
            {
                eachContent = streamReader.ReadLine();
                _prizeContent.Add(eachContent);
            }
            streamReader.Close();
            if (_prizeContent.Count != nbPrize)
            {
                throw new FileFormatException();
            }

            streamReader = new StreamReader(_prizeAmountPath);
            string eachAmount;
            _prizeAmount = new List<int>();
            while (!streamReader.EndOfStream)
            {
                eachAmount = streamReader.ReadLine();
                _prizeAmount.Add(Convert.ToInt32(eachAmount));
            }
            if (_prizeAmount.Count != nbPrize)
            {
                throw new FileFormatException();
            }
            streamReader.Close();
        }

        private void OutputPeopleReceivePrize(Tuple<int, int> range, int prizeLevel)
        {
            string path = "./Output.txt";
            StreamWriter streamWriter = new StreamWriter(path, true);
            int l = range.Item1;
            int r = range.Item2;
            streamWriter.WriteLine("level: " + prizeLevel);
            for (int i = l; i <= r; i++)
            {
                int nb = _drawAlgo.Candidates[i];
                string name = _candidateNames[nb];
                streamWriter.WriteLine(name);
            }
            streamWriter.Close();
        }

        private void Init()
        {
            ReadCandidateNames();
            ReadPrizeData();
            int totalPrize = 0;
            foreach (int eachAmount in _prizeAmount)
            {
                totalPrize += eachAmount;
            }
            nbCandidates = _candidateNames.Count;
            if (totalPrize > nbCandidates)
            {
                throw new InvalidDataException("too many prizes!");
            }
            _drawAlgo = new DrawAlgo(nbCandidates);
        }

        private async void Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            nbButtonClick++;
            int prizeLevel = nbPrize - nbButtonClick;
            int selectedPeople = _prizeAmount[prizeLevel];
            string prizeName = _prizeName[prizeLevel];
            string prizeContent = _prizeContent[prizeLevel];
            int prizeAmount = _prizeAmount[prizeLevel];

            PrizeNameLabel.Content = prizeName + "  ";
            PrizeContentLabel.Content = prizeContent + " " + prizeAmount + "个";

            Tuple<int, int> range = _drawAlgo.SelectSome(selectedPeople);

            Task printer = Task.Run(() => { PrintSelectedPeople(range); });
            Task outputer = Task.Run(() => { OutputPeopleReceivePrize(range, prizeLevel + 1); });
            DrawButton.IsEnabled = false;
            await printer;
            await outputer;

            if (nbButtonClick != nbPrize)
                DrawButton.IsEnabled = true;

        }

        private void PrintSelectedPeople(Tuple<int, int> range)
        {
            int l = range.Item1;
            int r = range.Item2;
            string name;
            int nb;
            var dispatcher = Application.Current.Dispatcher;
            dispatcher.Invoke(new Action(() =>
            {
                SelectedPeopleTextBlock.Inlines.Clear();
                CelebrationLabel.Content = "恭 喜  ";
            }));
            for (int i = l; i <= r; i++)
            {
                Thread.Sleep(1000);
                nb = _drawAlgo.Candidates[i];
                name = _candidateNames[nb];


                dispatcher.Invoke(new Action(() =>
                {
                    Run text = new Run
                    {
                        Text = name + "\n",
                        Foreground = Brushes.Blue
                    };
                    if (SelectedPeopleTextBlock.Inlines.Count != 0)
                        SelectedPeopleTextBlock.Inlines.Last().Foreground = Brushes.Black;
                    SelectedPeopleTextBlock.Inlines.Add(text);
                    TextBlockScroller.ScrollToBottom();
                }));
            }
        }

        Point _pressedPosition;
        bool _isDrag = false;


        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _pressedPosition = e.GetPosition(this);
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_pressedPosition != e.GetPosition(this) && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                _isDrag = true;
                DragMove();
            }
        }

        private void Window_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_isDrag)
            {
                _isDrag = false;
                e.Handled = true;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }

    public class DrawAlgo
    {
        private List<int> _candidates;
        private int _begin;
        private int _size;
        private Random _random;

        public List<int> Candidates
        {
            get { return _candidates; }
        }

        public DrawAlgo(int n)
        {
            if (n < 0)
            {
                throw new ArgumentOutOfRangeException("n must be positive!");
            }
            _random = new Random(Convert.ToInt32(DateTime.Now.Ticks % 114514));
            _candidates = new List<int>(n);
            for (var i = 0; i < n; i++)
            {
                _candidates.Add(i);
            }
            _size = n;
            _begin = 0;
        }

        public Tuple<int, int> SelectSome(int m)
        {
            if (_size - _begin < m)
            {
                throw new ArgumentOutOfRangeException("m is too large!");
            }
            for (int i = 0; i < m; i++)
            {
                int random_idx = _random.Next(_begin, _size);

                int tmp = _candidates[random_idx];
                _candidates[random_idx] = _candidates[_begin];
                _candidates[_begin] = tmp;

                _begin++;
            }
            return Tuple.Create(_begin - m, _begin - 1);
        }

    }
}
