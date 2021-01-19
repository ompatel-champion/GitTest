// set global values
var subscriberId = $("#lblSubscriberId").text();
var userId = $("#lblUserId").text();
var eventCategories = $("#companyEventCategories");

$(document).ready(function () {
    new EventCategories().Init();

    $(".color-name").spectrum({
        flat: false,
        showInput: true,
        showPalette: true,
        showPaletteOnly: true,
        palette: [
            ["#000", "#444", "#666", "#999", "#ccc", "#eee", "#f3f3f3", "#fff"],
            ["#f00", "#f90", "#ff0", "#0f0", "#0ff", "#00f", "#90f", "#f0f"]
        ]
    });
});


var EventCategories = function () {
    var self = this;

    this.Init = function () {
        self.BindActions();
    };

    this.BindActions = function () {
        eventCategories.find("tr").each(function () {
            var $tr = $(this);

            // delete event category
            $tr.find("[data-action='delete']").unbind("click").click(function () {
                var id = $(this).closest("tr").attr("data-id");
                self.DeleteCategory(id);
            });

            $tr.find(".category-name").unbind("blur").blur(function () {
                if ($(this).val() !== '') {
                    self.SaveCategory($(this).closest("tr"));
                } else {
                    // reset the value
                    $(this).val($(this).closest("tr").attr("data-current-event-category"));
                }
            });
        });

        // new event category
        $("#txtNewColorCode").unbind("keyup").keyup(function () {
            if ($(this).val !== '' || $(this).val !== '0')
                $(this).removeClass("error");
        });

        $("#txtNewCategory").unbind("keyup").keyup(function () {
            if ($(this).val !== '' || $(this).val !== '0')
                $(this).removeClass("error");
        });

        $("#btnAddCategory").unbind("click").click(function () {
            self.AddNewCategory();
        });
    };

    this.AddNewCategory = function () {
        var isError = false;
        if ($("#ctl10_txtNewCategory").val() === "") {
            $("#ctl10_txtNewCategory").addClass("error");
            isError = true;
        }
        if ($("#ctl10_txtNewColorCode").spectrum('get').toHexString() === "") {
            $("#ctl10_txtNewColorCode").addClass("error");
            isError = true;
        }
        if (!isError)
            self.SaveCategory(null, true);
    };

    this.SaveCategory = function ($tr, newCategory) {
        var category = new Object();
        category.SubscriberId = subscriberId;
        category.UpdateUserId = userId;
        if (newCategory) {
            category.eventCategoryId = 0;
            category.CategoryColor = $("#ctl10_txtNewColorCode").spectrum('get').toHexString();
            category.CategoryName = $("#ctl10_txtNewCategory").val();
        } else {
            category.eventCategoryId = $tr.attr("data-id");
            category.CategoryColor = $tr.find(".color-name").val();
            category.CategoryName = $tr.find(".category-name").val();
        }
        $.ajax({
            type: "POST",
            url: "/api/CalendarEvent/SaveEventCategory",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 50000,
            data: JSON.stringify(category),
            success: function (id) {
                removeSpinner();
                if (parseInt(id) > 0) {
                    $("#txtNewColorCode").val("");
                    $("#txtNewCategory").val("");
                }
                new RunSync();
            }, beforeSend: function () {
                addSpinner();
            }, error: function (request, status, error) {
                removeSpinner();
                alert(JSON.stringify(request));
            }
        });
    };

    this.DeleteCategory = function (id) {
        swal({
            title: translatePhrase("Delete Category!"),
            text: translatePhrase("Are you sure you want to delete this Event Category?"),
            type: "error",
            showCancelButton: true,
            confirmButtonColor: "#f27474",
            confirmButtonText: translatePhrase("Yes, Delete!")
        }).then(function (response) {
            if (response.value) {
                $.ajax({
                    type: "GET",
                    url: "/api/CalendarEvent/DeleteEventCategory/?id=" + id + "&userId=" + userId + "&subscriberId=" + subscriberId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: {},
                    success: function (response) {
                        if (response) {
                            new RunSync();
                            location.reload();
                        }
                    },
                    beforeSend: function () { },
                    error: function (request) { }
                });
            }
        });
    };
};

var RunSync = function () {
    $.ajax({
        type: "GET",
        url: "/api/sync/dosync/?userId=" + userId + "&subscriberId=" + subscriberId,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: {},
        success: function (response) {

        },
        beforeSend: function () {

        },
        error: function (request) { }
    });
};

var fixHelperModified = function (e, tr) {
    var $originals = tr.children();
    var $helper = tr.clone();
    $helper.children().each(function (index) {
        $(this).width($originals.eq(index).outerWidth());
    });
    return $helper;
};
