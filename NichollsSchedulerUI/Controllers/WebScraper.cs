using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSUSchedulerLibrary
{
    public sealed class WebScraper
    {
        private static readonly WebScraper instance = new WebScraper();
        private WebScraper()
        {

        }
        public static WebScraper Instance
        {
            get
            {
                return instance;
            }
        }
        public bool loggedIn { get; set; }
        public ArrayList availableTerms = new ArrayList();
        public ArrayList registeredCourses = new ArrayList();
        public IWebDriver webDriver = new ChromeDriver(options().Item1, options().Item2);
        public static (ChromeDriverService,ChromeOptions) options()
        {
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            var options = new ChromeOptions();
            options.AddArgument("headless");

            return (service, options);
        }
        public void loginToBanner(string username, string pin)
        {
            webDriver.Navigate().GoToUrl("https://banner.nicholls.edu/PROD/twbkwbis.P_WWWLogin");
            IWebElement userBox = webDriver.FindElement(By.Name("sid"));
            IWebElement pinBox = webDriver.FindElement(By.Name("PIN"));
            userBox.SendKeys(username);
            pinBox.SendKeys(pin);
            IWebElement loginButton = webDriver.FindElement(By.XPath("/html/body/div[3]/form/p/input[1]"));
            loginButton.Click();
            if (webDriver.Url == "https://banner.nicholls.edu/PROD/twbkwbis.P_ValLogin")
            {
                Console.WriteLine("Login Failed");
                loggedIn = false;
            }
            else
            {
                Console.WriteLine("Login Successful");
                loggedIn = true;
            }
        }
        public ArrayList getAvailableTerms()
        {
            webDriver.Navigate().GoToUrl("https://banner.nicholls.edu/PROD/bwskfreg.P_AltPin");
            IWebElement termDropDown = webDriver.FindElement(By.Id("term_id"));
            var terms = termDropDown.FindElements(By.TagName("option"));
            foreach (IWebElement i in terms)
            {
                availableTerms.Add(i.Text);
            }
            return availableTerms;
        }
        public void selectTerm(int index)
        {
            IWebElement termDropDown = webDriver.FindElement(By.Id("term_id"));
            var selection = new SelectElement(termDropDown);
            selection.SelectByIndex(index);
            IWebElement submitButton = webDriver.FindElement(By.XPath("/html/body/div[3]/form/input"));
            submitButton.Click();
        }
        public ArrayList getRegisteredCourses()
        {
            IWebElement dataTableBody = webDriver.FindElement(By.XPath("/html/body/div[3]/form/table[1]/tbody"));
            var tableRows = dataTableBody.FindElements(By.TagName("tr"));
            for (int i = 1; i < tableRows.Count; i++)
            {
                var rowContent = tableRows[i].FindElements(By.TagName("td")).ToList();
                rowContent.RemoveAt(1);
                rowContent.RemoveAt(5);
                rowContent.RemoveAt(6);

                string status = rowContent[0].Text;
                string crn = rowContent[1].Text;
                string subject = rowContent[2].Text;
                string courseNumber = rowContent[3].Text;
                string section = rowContent[4].Text;
                string creditHours = rowContent[5].Text;
                string courseTitle = rowContent[6].Text;
                CourseModel model = new CourseModel(status, crn, subject, courseNumber, section, creditHours, courseTitle);
                registeredCourses.Add(model);
            }
            return registeredCourses;
        }
        public ArrayList searchForCourse(string subject, string courseNumber, string desiredSection)
        {
            ArrayList searchResults = new ArrayList();
            var classSearchButton = webDriver.FindElement(By.XPath("/html/body/div[3]/form/input[20]"));
            classSearchButton.Click();
            var advancedSearchButton = webDriver.FindElement(By.XPath("/html/body/div[3]/form/input[109]"));
            advancedSearchButton.Click();
            IWebElement subjectBox = webDriver.FindElement(By.Id("subj_id"));
            SelectElement subjectSelection = new SelectElement(subjectBox);
            subjectSelection.SelectByValue(subject);
            IWebElement courseNumberBox = webDriver.FindElement(By.Id("crse_id"));
            courseNumberBox.SendKeys(courseNumber);
            IWebElement submitButton = webDriver.FindElement(By.Name("sub_btn"));
            submitButton.Click();
            IWebElement dataTableBody = webDriver.FindElement(By.XPath("/html/body/div[3]/form/table/tbody"));
            var tableRows = dataTableBody.FindElements(By.TagName("tr"));
            for (int i = 2; i < tableRows.Count; i++)
            {
                var rowContent = tableRows[i].FindElements(By.TagName("td")).ToList();
                if (desiredSection.Length > 0)
                {


                    if (rowContent[4].Text.Length > 2)
                    {
                        if (rowContent[4].Text.Remove(2) == desiredSection)
                        {
                            searchResults.Add(parseCourseInformation(rowContent));
                        }
                    }
                    else
                    {
                        if (rowContent[4].Text == desiredSection)
                        {
                            searchResults.Add(parseCourseInformation(rowContent));
                        }
                    }
                }
                else
                {
                    searchResults.Add(parseCourseInformation(rowContent));
                }
            }
            return searchResults;
        }
        private CourseModel parseCourseInformation(List<IWebElement> rowContent)
        {
            rowContent.RemoveAt(0);
            rowContent.RemoveAt(4);
            rowContent.RemoveAt(6);
            rowContent.RemoveRange(7, 2);
            rowContent.RemoveRange(8, 6);
            rowContent.RemoveAt(11);

            string crn = rowContent[0].Text;
            string subject = rowContent[1].Text;
            string courseNumber = rowContent[2].Text;
            string section = rowContent[3].Text;
            string creditHours = rowContent[4].Text;
            string courseTitle = rowContent[5].Text;
            string time = rowContent[6].Text;
            string remainingCapacity = rowContent[7].Text;
            string instructor = rowContent[8].Text;
            string dateRange = rowContent[9].Text;
            string location = rowContent[10].Text;
            return new CourseModel(crn, subject, courseNumber, section, creditHours, courseTitle, time, remainingCapacity, instructor, dateRange, location);
        }
    }
}
