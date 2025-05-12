document.addEventListener("DOMContentLoaded", function () {
    const classTab = document.getElementById("class-tab");

    classTab?.addEventListener("click", function (e) {
        e.preventDefault();
        const studentId = classTab.getAttribute("data-id");

        console.log("Student ID:", studentId);

        fetch(`/Class/LoadStudentClassHistory?studentId=${studentId}`)
            .then(res => {
                console.log("Fetch response:", res);
                if (!res.ok) throw new Error("Network response was not ok");
                return res.text();
            })
            .then(html => {
                document.getElementById("student-content").innerHTML = html;

                // Optional: Highlight the active tab
                document.querySelectorAll('.nav-link').forEach(link => link.classList.remove('active'));
                classTab.classList.add('active');
            })
            .catch(err => {
                console.error("Error fetching class history:", err);
                document.getElementById("student-content").innerHTML = `<div class="alert alert-danger">Failed to load class history.</div>`;
            });
    });
});
