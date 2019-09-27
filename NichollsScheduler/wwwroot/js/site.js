// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
function addCourse() {
    var table = document.getElementById("courseTable");
    var newRow = table.insertRow(-1);
    var countCell = newRow.insertCell(-1);
    var subjectCell = newRow.insertCell(-1);
    var courseNumberCell = newRow.insertCell(-1);
    var deleteButton = newRow.insertCell(-1);
    countCell.innerText = table.childElementCount;
    let selection = document.getElementById("sel_subj");
    subjectCell.innerText = selection.options[selection.selectedIndex].text;
    let courseNumber = document.getElementById("crseNum").value;
    courseNumberCell.innerText = courseNumber;
}

function removeCourse() {

}