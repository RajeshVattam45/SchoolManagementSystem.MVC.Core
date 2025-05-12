document.getElementById("add-guardian-btn").addEventListener("click", function () {
    var guardianContainer = document.getElementById("guardian-container");
    var lastGuardianSection = guardianContainer.lastElementChild;
    var newGuardianSection = lastGuardianSection.cloneNode(true);
    var index = guardianContainer.children.length;

    newGuardianSection.id = "guardian-" + index;

    // Update inputs
    var inputs = newGuardianSection.querySelectorAll("input");
    inputs.forEach(function (input) {
        input.name = input.name.replace(/\[\d+\]/, "[" + index + "]");
        input.value = "";
    });

    // Update heading
    var heading = newGuardianSection.querySelector("h5");
    if (heading) heading.textContent = "Guardian " + (index + 1);

    // Update or add remove button
    var removeBtn = newGuardianSection.querySelector(".remove-guardian");
    if (!removeBtn) {
        removeBtn = document.createElement("button");
        removeBtn.className = "btn btn-danger remove-guardian mt-2 mb-2";
        removeBtn.type = "button";
        newGuardianSection.insertBefore(removeBtn, newGuardianSection.children[1]); // insert after heading
    }
    removeBtn.setAttribute("data-guardian-id", index);
    removeBtn.textContent = "Remove Guardian";

    guardianContainer.appendChild(newGuardianSection);
});

document.addEventListener("click", function (event) {
    if (event.target && event.target.classList.contains("remove-guardian")) {
        event.preventDefault();

        var guardianContainer = document.getElementById("guardian-container");
        var guardianSections = guardianContainer.getElementsByClassName("guardian-section");

        if (guardianSections.length <= 1) {
            alert("At least one guardian is required.");
            return;
        }

        var guardianId = event.target.getAttribute("data-guardian-id");
        var guardianSection = document.getElementById("guardian-" + guardianId);
        if (guardianSection) {
            guardianSection.remove();
        }

        // Re-index guardian IDs and names
        Array.from(guardianContainer.children).forEach(function (section, newIndex) {
            section.id = "guardian-" + newIndex;

            // Update heading
            var heading = section.querySelector("h5");
            if (heading) heading.textContent = "Guardian " + (newIndex + 1);

            // Update input names
            section.querySelectorAll("input").forEach(function (input) {
                input.name = input.name.replace(/\[\d+\]/, "[" + newIndex + "]");
            });

            // Update remove button
            var removeBtn = section.querySelector(".remove-guardian");
            if (removeBtn) removeBtn.setAttribute("data-guardian-id", newIndex);
        });
    }
});