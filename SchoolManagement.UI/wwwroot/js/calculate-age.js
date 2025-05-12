function togglePreviousSchoolSection() {
    const classSelector = document.getElementById("classSelector");
    const previousSchoolSection = document.getElementById("previousSchoolDetails");
    const selectedValue = parseInt(classSelector.value, 10);

    if (!isNaN(selectedValue) && selectedValue > 1) {
        previousSchoolSection.style.display = "block";
    } else {
        previousSchoolSection.style.display = "none";
    }
}

document.getElementById('dob').addEventListener('input', function () {
    const dob = new Date(this.value);
    const today = new Date();
    let age = today.getFullYear() - dob.getFullYear();
    const m = today.getMonth() - dob.getMonth();

    if (m < 0 || (m === 0 && today.getDate() < dob.getDate())) {
        age--;
    }

    // Set age field
    const ageInput = document.getElementById('age');
    ageInput.value = age;

    // Calculate class
    const calculatedClass = age - 4;
    const classDisplay = document.getElementById('classDisplay');
    const classHidden = document.getElementById('classSelector');

    if (calculatedClass >= 1 && calculatedClass <= 10) {
        classDisplay.value = `Class ${calculatedClass}`;
        classHidden.value = calculatedClass;
    } else {
        classDisplay.value = "Invalid age for class";
        classHidden.value = "";
    }

    // Trigger validation
    ageInput.dispatchEvent(new Event('input'));
    ageInput.dispatchEvent(new Event('blur'));
    classHidden.dispatchEvent(new Event('input'));
    classHidden.dispatchEvent(new Event('blur'));

    // Show/hide previous school section
    togglePreviousSchoolSection();
});

// Set current date as default DOJ
document.getElementById('doj').value = new Date().toISOString().split('T')[0];

// Initial run on page load (in case DOB is already filled)
window.addEventListener('DOMContentLoaded', () => {
    togglePreviousSchoolSection();
});

