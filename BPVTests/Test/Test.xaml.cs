using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Data.Linq;
using System.Data.Design;
using System.Data.Entity;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace BPVTests
{
    /// <summary>
    /// Логика взаимодействия для Test.xaml
    /// </summary>
    public partial class Test : Window
    {
        private static Random rng = new Random();
        private int cur = 0;
        private double result = 0.0;
        private double max = 0.0;
        private double min = 0.0;
        private bool isAgreed = false;
        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer timer2 = new DispatcherTimer();
        TimeSpan x = new TimeSpan(0,0,0);
        public Test()
        {
            InitializeComponent();
        }
        public Test(int Count)
        {
            try
            {

                InitializeComponent();
                Pause.IsEnabled = false;
                if (Resourses.TimeQ)
                {
                    x = Resourses.TimeP;
                    timer2.Interval = Resourses.TimeP;
                    timer.Interval = new TimeSpan(0, 0, 1);
                    timer.Tick += Tick;
                    timer2.Tick += Tick2;
                    timer.Start();
                    timer2.Start();
                    Pause.IsEnabled = true;
                }

                if (Resourses.CurrentTestUser.Count <= 0 || Resourses.CurrentTestUser == null) throw new Exception(App.Current.Resources["TestProblem0"].ToString());

                Progress.Minimum = 0;
                Progress.Maximum = Count;
                if (Resourses.DifficultQ)
                {
                    for (int i = 0; i < Resourses.CurrentTestUser.Count; i++)
                    {
                        switch (Resourses.CurrentTestUser[i].Difficult)
                        {
                            case 0: max += 0.5; break;
                            case 1: max += 0.75; break;
                            case 2: max += 1; break;
                            case 3: max += 1.5; break;
                            case 4: max += 2; break;
                        }
                    }
                }
                else max = Resourses.CurrentTestUser.Count;

                min = max * ((double)Resourses.Min / 100);

                if (Resourses.FisherQ) {
                    int n = Resourses.CurrentTestUser.Count;
                    while (n > 1)
                    {
                        n--;
                        int k = rng.Next(n + 1);
                        dbQuestion value = Resourses.CurrentTestUser[k];
                        Resourses.CurrentTestUser[k] = Resourses.CurrentTestUser[n];
                        Resourses.CurrentTestUser[n] = value;
                    }
                };
                NextClick();
                if (Resourses.CurrentTestUser.Count == 1)
                {
                    NextQ.Tag = "1";
                    NextQ.Content = App.Current.Resources["TextNext"];
                }

            }
            catch (Exception ex)
            {
                if (MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString()) == MessageBoxResult.OK)
                    this.Close();
            }
        }
        private void NextClick()
        {
            try
            {
                NextQ.IsEnabled = false;
                QuestionCount.Content = (cur + 1).ToString() + "/" + Resourses.CurrentTestUser.Count;
                switch (Resourses.CurrentTestUser[cur].Difficult)
                {
                    case 0: QuestionDifficult.Content = App.Current.Resources["DifficultType0"]; break;
                    case 1: QuestionDifficult.Content = App.Current.Resources["DifficultType1"]; break;
                    case 2: QuestionDifficult.Content = App.Current.Resources["DifficultType2"]; break;
                    case 3: QuestionDifficult.Content = App.Current.Resources["DifficultType3"]; break;
                    case 4: QuestionDifficult.Content = App.Current.Resources["DifficultType4"]; break;
                }
                TestAnswers.Children.Clear();
                QuestionText.Text = Resourses.CurrentTestUser.ElementAt(cur).Text;
                MemoryStream memq;
                ImageQuestion.Width = 0;
                ImageQuestion.Height = 0;
                if (Resourses.CurrentTestUser.ElementAt(cur).Picture != null)
                {
                    memq = new MemoryStream(Resourses.CurrentTestUser.ElementAt(cur).Picture);
                    ImageQuestion.Source = BitmapFrame.Create(memq);
                    ImageQuestion.Height = 256;
                    ImageQuestion.Width = 256;
                    ImageQuestion.Visibility = Visibility.Visible;
                }
                else ImageQuestion.Visibility = Visibility.Collapsed;

                if (Resourses.CurrentTestUser.ElementAt(cur).Type == 2)
                    TestAnswers.AllowDrop = true;
                else
                    TestAnswers.AllowDrop = false;

                DataContext db = new DataContext(Resourses.connectionS);
                IEnumerable<dbAnswer> answers = db.ExecuteQuery<dbAnswer>("Select * from Answers where QuestionID = {0}", Resourses.CurrentTestUser.ElementAt(cur).QuestionID);
                //List<dbAnswer> answ = answers.ToList();
                Resourses.CurrentAnswerstUser = answers.ToList();
                if (Resourses.FisherA)
                {
                    int n = Resourses.CurrentAnswerstUser.Count;
                    while (n > 1)
                    {
                        n--;
                        int k = rng.Next(n + 1);
                        dbAnswer value = Resourses.CurrentAnswerstUser[k];
                        Resourses.CurrentAnswerstUser[k] = Resourses.CurrentAnswerstUser[n];
                        Resourses.CurrentAnswerstUser[n] = value;
                    }
                }
                for (int i = 0; i < Resourses.CurrentTestUser.ElementAt(cur).AnswersCount; i++)
                {
                    FAddAnswer(Resourses.CurrentTestUser.ElementAt(cur).Type);
                    Grid grid = TestAnswers.Children[i] as Grid;
                    switch (Resourses.CurrentTestUser.ElementAt(cur).Type)
                    {
                        case 0:
                            (grid.Children[0] as TextBox).Text = Resourses.CurrentAnswerstUser.ElementAt(i).Text;
                            break;
                        case 1:
                            (grid.Children[0] as TextBox).Text = Resourses.CurrentAnswerstUser.ElementAt(i).Text;
                            break;
                        case 2:
                            (grid.Children[0] as TextBox).Text = Resourses.CurrentAnswerstUser.ElementAt(i).Text;
                            break;
                        case 3:
                            MemoryStream mem = new MemoryStream(Resourses.CurrentAnswerstUser.ElementAt(i).Picture);
                            (grid.Children[0] as Image).Source = BitmapFrame.Create(mem);
                            break;
                        case 4:
                            MemoryStream mem2 = new MemoryStream(Resourses.CurrentAnswerstUser.ElementAt(i).Picture);
                            (grid.Children[0] as Image).Source = BitmapFrame.Create(mem2);
                            break;
                        case 5:
                            break;
                    }
                }
                cur++;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private double getx(int type)
        {
            double x = 0;
            switch (type)
            {
                case 0: x = 0.5; break;
                case 1: x = 0.75; break;
                case 2: x = 1; break;
                case 3: x = 1.5; break;
                case 4: x = 2; break;
                default: break;
            }
            return x;
        }
        void FAddAnswer(int type)
        {
            try
            {

                Grid x = new Grid();
                x.ColumnDefinitions.Add(new ColumnDefinition());
                x.ColumnDefinitions.Add(new ColumnDefinition());
                x.RowDefinitions.Add(new RowDefinition());
                x.RowDefinitions.Add(new RowDefinition());
                x.RowDefinitions.ElementAt(1).MinHeight = 25;
                x.Background = Brushes.White;
                TextBox text = new TextBox();
                text.Height = 50; text.Width = 600;
                text.FontSize = 24;
                text.HorizontalContentAlignment = HorizontalAlignment.Center;
                text.IsReadOnly = true;
                RadioButton radio = new RadioButton();
                radio.HorizontalAlignment = HorizontalAlignment.Center;
                radio.VerticalAlignment = VerticalAlignment.Center;
                radio.GroupName = "Answer" + Resourses.CurrentQuestionNum;
                CheckBox check = new CheckBox();
                check.HorizontalAlignment = HorizontalAlignment.Center;
                check.VerticalAlignment = VerticalAlignment.Center;
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("pack://application:,,,/Icon/add.png", UriKind.Absolute));
                image.Height = 256; image.Width = 256;
                Grid.SetColumn(text, 0); Grid.SetColumn(radio, 1);
                Grid.SetColumn(check, 1); Grid.SetColumn(image, 0);
                switch (type)
                {
                    case 0:
                        x.Children.Add(text); x.Children.Add(radio);
                        TestAnswers.Children.Add(x);
                        break;
                    case 1:
                        x.Children.Add(text); x.Children.Add(check);
                        TestAnswers.Children.Add(x);
                        break;
                    case 2:
                        x.Children.Add(text);
                        TestAnswers.Children.Add(x);
                        break;
                    case 3:
                        x.Children.Add(image); x.Children.Add(radio);
                        TestAnswers.Children.Add(x);
                        break;
                    case 4:
                        x.Children.Add(image); x.Children.Add(check);
                        TestAnswers.Children.Add(x);
                        break;
                    case 5:
                        text.IsReadOnly = false;
                        x.Children.Add(text);
                        TestAnswers.Children.Add(x);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void Tick(object sender, EventArgs e)
        {
            try
            {
                TimeLabel.Content = x.Subtract(timer.Interval);
                x = x.Subtract(timer.Interval);
                if (x == new TimeSpan(0, 0, 0)) timer.Stop();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void Tick2(object sender, EventArgs e)
        {
            try
            {
                timer2.Stop();

                Label resultlabel = new Label();
                resultlabel.HorizontalAlignment = HorizontalAlignment.Center;
                resultlabel.VerticalAlignment = VerticalAlignment.Center;
                resultlabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                resultlabel.VerticalContentAlignment = VerticalAlignment.Center;
                resultlabel.FontSize = 30;
                resultlabel.Content = App.Current.Resources["TestTimeOut"];
                Button exits = new Button();
                exits.Width = 200; exits.Height = 30;
                exits.Click += exit;
                exits.Content = App.Current.Resources["EndTestOK"];
                exits.HorizontalAlignment = HorizontalAlignment.Center;
                exits.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetRow(exits, 2);
                MainGrid.Children.Clear();
                MainGrid.Children.Add(resultlabel);
                MainGrid.Children.Add(exits);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button s = sender as Button;
                switch (s.Tag.ToString()) {
                    case "0": TestAnswers.IsEnabled = false; Exit.IsEnabled = false; Agreed.IsEnabled = false; NextQ.IsEnabled = false; timer2.Stop(); timer.Stop(); (sender as Button).Tag = "1"; (sender as Button).Content = App.Current.Resources["TestContinue"]; break;
                    case "1": if (isAgreed) NextQ.IsEnabled = true; else Agreed.IsEnabled = true; TestAnswers.IsEnabled = true; Exit.IsEnabled = true; timer2.Interval = x; timer2.Start(); timer.Start(); (sender as Button).Tag = "0"; (sender as Button).Content = App.Current.Resources["TestPause"]; break;
                }
            }
            catch (Exception )
            {
                throw;
            }
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception)
            {

            }
        }
        private void NextQ_Click(object sender, RoutedEventArgs e)
        {
            try {
                isAgreed = false;
                Agreed.IsEnabled = true;
                TestAnswers.Children.Clear();
                Progress.Value++;
                switch ((sender as Button).Tag.ToString())
                {
                    case "0":
                        NextClick();
                        if (cur == Resourses.CurrentTestUser.Count)
                        {
                            NextQ.Content = App.Current.Resources["TestEnd"];
                            NextQ.Tag = "1";
                        }
                        
                        break;

                    case "1":
                        if (Resourses.TimeQ)
                        {
                            timer.Stop();
                            timer2.Stop();
                        }
                        string resultst = string.Empty;
                        Label resultlabel = new Label();
                        resultlabel.HorizontalAlignment = HorizontalAlignment.Center;
                        resultlabel.VerticalAlignment = VerticalAlignment.Center;
                        resultlabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                        resultlabel.VerticalContentAlignment = VerticalAlignment.Center;
                        resultlabel.FontSize = 30;
                        MainGrid.Children.Clear();
                        Button exits = new Button();
                        exits.Width = 200; exits.Height = 30;
                        exits.Click += exit;
                        exits.Content = App.Current.Resources["EndTestOK"];
                        exits.HorizontalAlignment = HorizontalAlignment.Center;
                        exits.VerticalAlignment = VerticalAlignment.Center;
                        Grid.SetRow(exits, 2);
                        switch (Resourses.FormatID) {
                            case 0:
                                resultst += App.Current.Resources["TestResult"];
                                if (result >= min) resultst += App.Current.Resources["TestSuccess"];
                                else resultst += App.Current.Resources["TestFail"];
                                break;
                            case 1:
                                double decimalresult = 0.0;
                                decimalresult = Math.Round(result * 10 / max,2);
                                resultst += App.Current.Resources["TestResult"];
                                resultst += (decimalresult.ToString()+"/10"+'\n');
                                if (result >= min) resultst += App.Current.Resources["TestSuccess"];
                                else resultst += (App.Current.Resources["TestFail"]+"\n"+ App.Current.Resources["TestMIN"] + Math.Round(min / max * 10));
                                break;
                            case 2:
                                double perresult = 0.0;
                                perresult = Math.Round(result / max,4);
                                perresult *= 100;
                                resultst += App.Current.Resources["TestResult"];
                                resultst += (perresult.ToString() + "%" + '\n');
                                if (result >= min) resultst += App.Current.Resources["TestSuccess"];
                                else resultst += (App.Current.Resources["TestFail"]+ "\n" + App.Current.Resources["TestMIN"] + Resourses.Min +"%");
                                break;
                        };
                        resultlabel.Content = resultst;
                        MainGrid.Children.Add(resultlabel);
                        MainGrid.Children.Add(exits);
                        //this.Close();
                        break;
                }
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void Agreed_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isAgreed = true;
                (sender as Button).IsEnabled = false;
                switch (Resourses.CurrentTestUser[cur - 1].Type)
                {
                    case 0: case 3:
                        for (int c0 = 0; c0 < Resourses.CurrentTestUser[cur - 1].AnswersCount; c0++)
                        {
                            if (Resourses.CurrentAnswerstUser[c0].Check)
                            {
                                Grid answer0 = TestAnswers.Children[c0] as Grid;
                                if ((answer0.Children[1] as RadioButton).IsChecked.Value)
                                    if (Resourses.DifficultQ)
                                        result += getx(Resourses.CurrentTestUser[cur - 1].Difficult);
                                    else result += 1;
                            }
                        }
                        break;
                    case 1:case 4:
                        if (Resourses.StrongQ)
                        {
                            bool wrong = false;
                            for (int c0 = 0; c0 < Resourses.CurrentTestUser[cur - 1].AnswersCount; c0++)
                            {
                                if (Resourses.CurrentAnswerstUser[c0].Check)
                                {
                                    Grid answer1 = TestAnswers.Children[c0] as Grid;
                                    if (!(answer1.Children[1] as CheckBox).IsChecked.Value)
                                        wrong = true;
                                }
                                else
                                {
                                    Grid answer1 = TestAnswers.Children[c0] as Grid;
                                    if ((answer1.Children[1] as CheckBox).IsChecked.Value)
                                        wrong = true;
                                }
                            }
                            if (Resourses.DifficultQ)
                                if (!wrong) result += getx(Resourses.CurrentTestUser[cur - 1].Difficult);
                                else result += 0;
                            else if (!wrong) result += 1;
                            else result += 0;
                        }
                        else
                        {
                            int trueright = 0, right = 0, truewrong = 0, wrong = 0;
                            for (int c0 = 0; c0 < Resourses.CurrentTestUser[cur - 1].AnswersCount; c0++)
                            {
                                if (Resourses.CurrentAnswerstUser[c0].Check)
                                {
                                    Grid answer2 = TestAnswers.Children[c0] as Grid;
                                    if ((answer2.Children[1] as CheckBox).IsChecked.Value)
                                        right++;
                                    trueright++;
                                }
                                else
                                {
                                    Grid answer2 = TestAnswers.Children[c0] as Grid;
                                    if ((answer2.Children[1] as CheckBox).IsChecked.Value)
                                        wrong++;
                                    truewrong++;
                                }
                            }
                            if (wrong == 0)
                            {
                                if (right == trueright)
                                    if (Resourses.DifficultQ) result += getx(Resourses.CurrentTestUser[cur - 1].Difficult);
                                    else result += 1;
                                else if (Resourses.DifficultQ) result += getx(Resourses.CurrentTestUser[cur - 1].Difficult) * ((double)right / (double)trueright);
                                     else result += ((double)right / (double)trueright);
                            }
                            else result += 0;
                        }
                        break;
                    case 2:
                        string answ = string.Empty;
                        for (int c0 = 0; c0 < Resourses.CurrentTestUser[cur - 1].AnswersCount; c0++)
                        {
                            Grid answer3 = TestAnswers.Children[c0] as Grid;
                            answ += (answer3.Children[0] as TextBox).Text;
                        }
                        if (answ == Resourses.CurrentAnswerstUser[0].DnDAnswer)
                            if (Resourses.DifficultQ) result += getx(Resourses.CurrentTestUser[cur - 1].Difficult);
                            else result += 1;
                        else result += 0;
                        break;
                    case 5:
                        string answ1 = string.Empty;
                        Grid answer4 = TestAnswers.Children[0] as Grid;
                        answ1 = (answer4.Children[0] as TextBox).Text;
                        if (answ1 == Resourses.CurrentAnswerstUser[0].Text)
                            if (Resourses.DifficultQ) result += getx(Resourses.CurrentTestUser[cur - 1].Difficult);
                            else result += 1;
                        else result += 0;
                        break;
                    default:break;
                }
                NextQ.IsEnabled = true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
