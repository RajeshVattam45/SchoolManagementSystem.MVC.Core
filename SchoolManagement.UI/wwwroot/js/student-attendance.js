document.addEventListener("DOMContentLoaded", function () {
    const updateMessage = document.getElementById("updateMessage");

    // Handle attendance status update
    function updateAttendance(event) {
        let select = event.target;
        let studentId = select.dataset.student;
        let date = select.dataset.date;
        let status = select.value;
        let studentName = select.closest("tr").querySelector("td").textContent.trim();

        // AJAX request to update attendance
        fetch('/StudentAttendance/UpdateAttendance', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ StudentId: studentId, Status: status, Date: date })
        })
            .then(response => response.json())
            .then(data => {
                updateMessage.textContent = `${studentName}'s attendance updated to "${status}" on ${date}`;
                updateMessage.classList.remove("d-none");
                setTimeout(() => {
                    updateMessage.classList.add("d-none");
                }, 3000);
            })
            .catch(() => {
                alert("Error updating attendance");
            });

        // Change background color
        select.classList.remove("bg-success", "bg-danger", "bg-light", "text-white");
        if (status === "Present") {
            select.classList.add("bg-success", "text-white");
        } else if (status === "Absent") {
            select.classList.add("bg-danger", "text-white");
        } else {
            select.classList.add("bg-light");
        }
    }

    // Attach to each select
    document.querySelectorAll(".attendance-status").forEach(select => {
        select.addEventListener("change", updateAttendance);
    });

    // Class Filter Logic
    let classFilterElement = document.getElementById("classFilter");
    if (classFilterElement) {
        classFilterElement.addEventListener("change", function () {
            let selectedClass = this.value;
            document.querySelectorAll(".attendance-table").forEach(table => {
                table.style.display = (selectedClass === "" || table.dataset.class === selectedClass)
                    ? "block"
                    : "none";
            });
        });
    }
});