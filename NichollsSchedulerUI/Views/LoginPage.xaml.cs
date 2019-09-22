using NichollsSchedulerUI;
using NichollsSchedulerUI.Views;
using NSUSchedulerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NichollsSchedulerUI
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
       
        public LoginPage()
        {
            
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var webScraper = WebScraper.Instance;
            webScraper.loginToBanner(userID.Text, pin.Password);
            if(!webScraper.loggedIn)
            {
                loginFailedMessage.Text = "Login Failed";
                if(System.DateTime.Now.DayOfWeek == DayOfWeek.Thursday && System.DateTime.Now.Hour > 20)
                {
                    loginFailedMessage.Text = "Login Failed; Banner is down for routine Thursday Maintence.";
                    webScraper.shutDown();
                }
            } else
            {
                this.NavigationService.Navigate(new TermSelectionPage());
            }
        }
    }
}
