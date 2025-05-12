document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector("form");
    const submitBtn = document.getElementById("submitBtn");
    const studentIdInput = document.getElementById("Student_StudentId");
    const studentIdError = document.getElementById("studentIdError");

    const classSelector = document.getElementById("classSelector");
    const previousSchoolSection = document.getElementById("previousSchoolDetails");

    // === Student ID Validation ===
    studentIdInput.addEventListener("blur", function () {
        const id = this.value.trim();

        if (id === "") {
            studentIdError.innerText = "Student Id is required";
            submitBtn.disabled = true;
            return;
        }

        fetch(`/Student/CheckStudentIdExists?studentId=${id}`)
            .then((res) => res.json())
            .then((data) => {
                if (data.exists) {
                    studentIdError.innerText = "Student ID already exists.";
                    submitBtn.disabled = true;
                } else {
                    studentIdError.innerText = "";
                    submitBtn.disabled = false;
                }
            })
            .catch(() => {
                studentIdError.innerText = "Unable to validate Student ID.";
                submitBtn.disabled = true;
            });
    });

    if (studentIdInput.value.trim() !== "") {
        studentIdInput.dispatchEvent(new Event("blur"));
    }

    //// === Class Selection Logic for Previous School ===
    //function togglePreviousSchoolSection() {
    //    const selectedValue = parseInt(classSelector.value, 10);
    //    if (!isNaN(selectedValue) && selectedValue > 1) {
    //        previousSchoolSection.style.display = "block";
    //    } else {
    //        previousSchoolSection.style.display = "none";
    //    }
    //}

    //classSelector.addEventListener("change", togglePreviousSchoolSection);
    //togglePreviousSchoolSection(); // Run on page load
});