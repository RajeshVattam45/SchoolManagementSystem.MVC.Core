$(document).ready(function () {
    $('#ClassId').on('change', function () {
        const classId = $(this).val();
        const studentDropdown = $('#StudentId');
        //const subjectDropdown = $('#SubjectId');

        studentDropdown.empty();
        studentDropdown.append('<option value="">Loading students...</option>');
        //subjectDropdown.empty();
        //subjectDropdown.append('<option value="">Loading subjects...</option>');

        if (classId) {
            $.get('/Marks/GetStudentsByClass', { classId: classId }, function (data) {
                studentDropdown.empty();
                studentDropdown.append('<option value="">-- Select Student --</option>');

                data.forEach(student => {
                    const fullName = student.firstName + ' ' + (student.lastName ?? '');
                    studentDropdown.append('<option value="' + student.id + '">' + fullName + '</option>');
                });
            }).fail(function () {
                studentDropdown.empty();
                studentDropdown.append('<option value="">-- Failed to load students --</option>');
            });

            //// Fetch subjects from the backend instead of directly from the external API
            //$.get('/Marks/GetSubjectsByClass', { classId: classId }, function (data) {
            //    subjectDropdown.empty();
            //    subjectDropdown.append('<option value="">-- Select Subject --</option>');

            //    if (data.success && data.subjects.length > 0) {
            //        data.subjects.forEach(subject => {
            //            subjectDropdown.append('<option value="' + subject + '">' + subject + '</option>');
            //        });
            //    } else {
            //        subjectDropdown.append('<option value="">-- No subjects available --</option>');
            //    }
            //}).fail(function () {
            //    subjectDropdown.empty();
            //    subjectDropdown.append('<option value="">-- Failed to load subjects --</option>');
            //});
        } else {
            studentDropdown.empty();
            studentDropdown.append('<option value="">-- Select Student --</option>');
            //subjectDropdown.empty();
            //subjectDropdown.append('<option value="">-- Select Subject --</option>');
        }
    });
});