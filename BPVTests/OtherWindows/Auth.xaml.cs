using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Data.Linq;
using System.IO;
using System.Globalization;
using BPVTests.DataClasses;
namespace BPVTests
{
    /// <summary>
    /// Логика взаимодействия для Auth.xaml
    /// </summary>
    public partial class Auth : Window
    {
        public Auth()
        {
            InitializeComponent();
            try
            {
                if (CFG.Default.First)
                {
                    string lang = CultureInfo.CurrentCulture.Name;
                    switch (lang)
                    {
                        case "ru-RU":
                            App.Current.Resources.MergedDictionaries[0].Source = new Uri(String.Format("pack://application:,,,/Localisation/Language.ru-RU.xaml"));
                            CFG.Default.Lang = lang;
                            break;
                        case "en-US":
                            App.Current.Resources.MergedDictionaries[0].Source = new Uri(String.Format("pack://application:,,,/Localisation/Language.en-US.xaml"));
                            CFG.Default.Lang = lang;
                            break;
                        default:
                            App.Current.Resources.MergedDictionaries[0].Source = new Uri(String.Format("pack://application:,,,/Localisation/Language.en-US.xaml"));
                            CFG.Default.Lang = "en-US";
                            break;
                    }
                    CFG.Default.First = false;
                    CFG.Default.Save();
                }
                else App.Current.Resources.MergedDictionaries[0].Source = new Uri(String.Format("pack://application:,,,/Localisation/Language.{0}.xaml",CFG.Default.Lang));
                DataContext db = new DataContext(Resourses.connectionS);
                if (!db.DatabaseExists())
                {
                    db = new DataContext(@"Integrated Security=SSPI;Initial Catalog=master");
                    string script = File.ReadAllText(@"../../DataClasses/CreateDB.sql");
                    db.ExecuteCommand(script);
                    string script2 = File.ReadAllText(@"../../DataClasses/CreateDB2.sql");
                    db.ExecuteCommand(script2);
                }
                db.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LoginName.Text.Length == 0) throw new Exception(App.Current.Resources["AuthProblem0"].ToString());
                if (LoginPassword.Password.Length == 0) throw new Exception(App.Current.Resources["AuthProblem1"].ToString());
                DataContext db = new DataContext(Resourses.connectionS);
                if(!db.DatabaseExists()) throw new Exception(App.Current.Resources["AuthProblem2"].ToString());
                IEnumerable<dbUsers> user = db.ExecuteQuery<dbUsers>("SELECT * FROM Users WHERE Username={0} AND Password={1}",LoginName.Text,LoginPassword.Password);
                List<dbUsers> list = user.ToList();
                if (list.Count() > 0)
                {
                    MainWindow x = new MainWindow(list.ElementAt(0).AccType);
                    x.Show();
                    db.Dispose();
                    this.Close();
                }
                else {
                    MessageBox.Show(App.Current.Resources["AuthProblem3"].ToString(), App.Current.Resources["Error"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }

        }
        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RegName.Text.Length == 0) throw new Exception(App.Current.Resources["AuthProblem0"].ToString());
                if (RegPass.Password.Length == 0) throw new Exception(App.Current.Resources["AuthProblem1"].ToString());
                if (RegCreator.IsChecked == false && RegUser.IsChecked == false) throw new Exception(App.Current.Resources["AuthProblem5"].ToString());
                bool type;
                if (RegCreator.IsChecked == true) type = false;
                else type = true;
                DataContext db = new DataContext(Resourses.connectionS);
                if (!db.DatabaseExists()) throw new Exception(App.Current.Resources["AuthProblem2"].ToString());
                IEnumerable<dbUsers> user = db.ExecuteQuery<dbUsers>("SELECT * FROM Users WHERE Username={0}", RegName.Text);
                List<dbUsers> list = user.ToList();
                if (list.Count() > 0) throw new Exception(App.Current.Resources["AuthProblem4"].ToString());
                else {
                    db.ExecuteCommand("Insert Users Values({0},{1},{2})", RegName.Text, RegPass.Password, type.ToString());
                    RegName.Clear();
                    RegPass.Clear();
                    MessageBox.Show(App.Current.Resources["RegSuccess"].ToString());
                    db.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
    }
}
