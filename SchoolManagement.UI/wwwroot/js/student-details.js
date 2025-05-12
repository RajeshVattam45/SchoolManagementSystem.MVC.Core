$(document).ready(function () {
    // Load the 'About' section by default
    var studentId = $("#about-tab").data("id");
    loadSection(studentId, "StudentAbout");

    // Handle tab clicks
    $(".nav-link").click(function (e) {
        e.preventDefault();
        var section = $(this).attr("id").replace("-tab", "");
        var studentId = $(this).data("id");

        loadSection(studentId, "Student" + section.charAt(0).toUpperCase() + section.slice(1));
    });

    // Function to load sections dynamically
    function loadSection(studentId, action) {
        $.ajax({
            url: "/Student/" + action + "/" + studentId,
            type: "GET",
            success: function (data) {
                $("#student-content").html(data);
            }
        });
    }

    // Handle 'Show Previous Classes' button click
    $(document).on("click", "#show-previous-classes", function () {
        var studentId = $("#student-id").val();

        if (!studentId) {
            console.error("Student ID is missing!");
            return;
        }

        console.log("Fetching previous classes for Student ID:", studentId);

        $.ajax({
            url: "/Student/StudentClassHistory/" + studentId,
            type: "GET",
            success: function (data) {
                $("#previous-classes").html(data);
            }
        });
    });
});

// Subject Classes
let subjectRequest;
$(document).ready(function () {
    $("#subjects-tab").click(function (e) {
        e.preventDefault();

        var studentId = $("#student-id").val();

        $("#student-content").html("<p>Loading...</p>");

        // Abort previous request if still pending
        if (subjectRequest) {
            subjectRequest.abort();
        }

        // Send a new request
        subjectRequest = $.ajax({
            url: "/Student/StudentSubjects/" + studentId,
            type: "GET",
            cache: false,
            success: function (data) {
                $("#student-content").html(data);
            },
            error: function (xhr, status) {
                if (status !== "abort") {
                    $("#student-content").html("<p>Error loading subjects.</p>");
                }
            }
        });
    });
});
