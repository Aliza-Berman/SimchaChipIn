$(function () {
    $("#new-contributor").on('click', function () {
        $(".new-contrib").modal();
    });
    $(".deposit-button").on('click', function () {
        const name = $(this).data('contributorname');
        const id = $(this).data('contributorid')
        $("#deposit-name").text(name);
        $("#contributor-id").val(id);
        $(".deposit").modal();
    });
    $(".edit-contributor").on('click', function () {
        const id = $(this).data('id');
        const firstName = $(this).data('firstName');
        const lastName = $(this).data('lastName');
        const cell = $(this).data('cell');
        const alwaysInclude = $(this).data('alwaysInclude');
        const date = $(this).data('date');
        const form = $(".new-contrib form")
        form.find("#edit-id").remove();
        const hidden = $(`<input type='hidden' id='edit-id' name='id' value='${id}' />`);
        form.append(hidden);
        $("#initialDepositDiv").hide();

        $("#contributor_first_name").val(firstName);
        $("#contributor_last_name").val(lastName);
        $("#contributor_cell_number").val(cell);
        $("#contributor_always_include").prop('checked', alwaysInclude === "True");
        $("#contributor_created_at").val(date);
        $(".new-contrib").modal();
        form.attr('action', '/contributors/edit');
    })
});