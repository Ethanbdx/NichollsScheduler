using NichollsSchedulerUI;
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

namespace NichollsSchedulerUI.Views
{
    /// <summary>
    /// Interaction logic for TermSelectionPage.xaml
    /// </summary>
    public partial class TermSelectionPage : Page
    {
        
        public TermSelectionPage()
        {
            var webScraper = WebScraper.Instance;
            InitializeComponent();
            var availableTerms = webScraper.getAvailableTerms();
            termComboBox.ItemsSource = availableTerms;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            var webScraper = WebScraper.Instance;
            int index = termComboBox.SelectedIndex;
            webScraper.selectTerm(index);
            this.NavigationService.Navigate(new CurrentSchedulePage());
        }
    }
}
