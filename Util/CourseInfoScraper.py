from bs4 import BeautifulSoup
import csv
with open("C:\\Users\\Ethanbdx\\Desktop\\coursescatalog.html", 'r', encoding='UTF8') as f:
    contents = f.read()
    soup = BeautifulSoup(contents, 'html.parser')
bodyDiv = soup.find("div", {"class": "pagebodydiv"})
tableBody = bodyDiv.find_all("tbody")
rows = tableBody[1].find_all("tr")
with open('courses.csv', 'w', newline='') as file:
    writer = csv.writer(file)
    
    for x in range(len(rows)):
        if(x % 2 == 0):
            courseTitle = rows[x].find("td")
            formattedTitle = courseTitle.text.split('-')
            subjectAndNumber = formattedTitle[0].split(' ')
            subject = subjectAndNumber[0].strip()
            courseNumber = subjectAndNumber[1].strip()
            courseTitle = formattedTitle[1].strip()
            if(courseNumber.rfind('X') != -1): continue
            if(len(courseNumber) < 3): courseNumber.rjust(3 - len(courseNumber), '0')
            writer.writerow([subject, courseNumber, courseTitle])
